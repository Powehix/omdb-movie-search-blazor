using Microsoft.Extensions.Caching.Memory;
using OMDbMovieSearch.Server.Interfaces;
using OMDbMovieSearch.Shared.Models;

namespace OMDbMovieSearch.Server.Services
{
    public class OmdbService : IOmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<OmdbService> _logger;
        private readonly string _apiKey;

        public OmdbService(HttpClient httpClient, IMemoryCache cache, ILogger<OmdbService> logger, IConfiguration config)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
            _apiKey = config["Omdb:ApiKey"] ?? throw new InvalidOperationException("OMDb API key not configured.");
        }

        // Calls the OMDb API to search for movies by title
        public async Task<List<MovieSearchResult>> GetMoviesAsync(string query, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new();

            if (_cache.TryGetValue(query, out List<MovieSearchResult> cached))
                return cached;

            try
            {
                var response = await _httpClient.GetFromJsonAsync<OmdbSearchResponse>(
                    $"?apikey={_apiKey}&s={query}", ct);

                if (response?.Search == null)
                    return new();

                _cache.Set(query, response.Search, TimeSpan.FromMinutes(5));
                return response.Search;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching search results for {Query}", query);
                return new();
            }
        }

        // Calls the OMDb API to retrieve full movie details by IMDb ID
        public async Task<MovieDetails?> GetMovieDetailsAsync(string imdbId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(imdbId))
                return null;

            if (_cache.TryGetValue(imdbId, out MovieDetails cached))
                return cached;

            try
            {
                var movie = await _httpClient.GetFromJsonAsync<MovieDetails>(
                    $"?apikey={_apiKey}&i={imdbId}&plot=full", ct);

                if (movie != null)
                    _cache.Set(imdbId, movie, TimeSpan.FromMinutes(30));

                return movie;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching movie details for {ImdbId}", imdbId);
                return null;
            }
        }

        // Small DTO wrapper for search results
        public class OmdbSearchResponse
        {
            public List<MovieSearchResult>? Search { get; set; }
        }
    }
}
