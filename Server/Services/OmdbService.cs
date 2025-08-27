using Microsoft.Extensions.Caching.Memory;
using OMDbMovieSearch.Server.Interfaces;
using OMDbMovieSearch.Shared.Models;
using System.Text.Json;

namespace OMDbMovieSearch.Server.Services
{
    public class OmdbService : IOmdbService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private readonly IMemoryCache _cache;
        private readonly ILogger<OmdbService> _log;
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public OmdbService(HttpClient httpClient, IConfiguration configuration, IMemoryCache cache, ILogger<OmdbService> log)
        {
            _http = httpClient;
            _apiKey = configuration["Omdb:ApiKey"] ?? throw new InvalidOperationException("OMDb API key not configured.");
            _cache = cache;
            _log = log;
        }

        // Calls the OMDb API to search for movies by title
        public async Task<IReadOnlyList<MovieSearchResult>> SearchMoviesAsync(string query, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query)) return Array.Empty<MovieSearchResult>();

            var cacheKey = $"search::{query.Trim().ToLowerInvariant()}";
            if (_cache.TryGetValue(cacheKey, out IReadOnlyList<MovieSearchResult> cached)) return cached;

            var url = $"?apikey={_apiKey}&s={Uri.EscapeDataString(query)}";
            try
            {
                var resp = await _http.GetAsync(url, ct);
                if (!resp.IsSuccessStatusCode)
                {
                    _log.LogWarning("OMDb returned {StatusCode} for query {Query}", resp.StatusCode, query);
                    return Array.Empty<MovieSearchResult>();
                }

                using var stream = await resp.Content.ReadAsStreamAsync(ct);
                using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
                if (!doc.RootElement.TryGetProperty("Search", out var node)) return Array.Empty<MovieSearchResult>();

                var list = JsonSerializer.Deserialize<List<MovieSearchResult>>(node.GetRawText(), _jsonOptions) ?? new();
                _cache.Set(cacheKey, list, TimeSpan.FromMinutes(5));
                return list;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error while searching OMDb for {Query}", query);
                return Array.Empty<MovieSearchResult>();
            }
        }

        // Calls the OMDb API to retrieve full movie details by IMDb ID
        public async Task<MovieDetails?> GetMovieDetailsAsync(string imdbId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(imdbId)) return null;

            var cacheKey = $"details::{imdbId}";
            if (_cache.TryGetValue(cacheKey, out MovieDetails cached)) return cached;

            var url = $"?apikey={_apiKey}&i={Uri.EscapeDataString(imdbId)}&plot=full";
            try
            {
                var model = await _http.GetFromJsonAsync<MovieDetails>(url, _jsonOptions, ct);
                if (model != null) _cache.Set(cacheKey, model, TimeSpan.FromMinutes(30));
                return model;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error while fetching details for {ImdbId}", imdbId);
                return null;
            }
        }
    }
}
