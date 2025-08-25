using OMDbMovieSearch.Shared.Models;
using System.Text.Json;

namespace OMDbMovieSearch.Server.Services
{
    public class OmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OmdbService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Omdb:ApiKey"] ?? throw new InvalidOperationException("OMDB API key not configured.");
        }

        // Calls the OMDb API to search for movies by title
        public async Task<List<MovieSearchResult>> SearchMovies(string query)
        {
            var response = await _httpClient.GetFromJsonAsync<Dictionary<string, object>>($"http://www.omdbapi.com/?apikey={_apiKey}&s={query}");
            if (response is null || !response.TryGetValue("Search", out var results)) return new();
            var list = JsonSerializer.Deserialize<List<MovieSearchResult>>(results.ToString());
            return list ?? new();
        }

        // Calls the OMDb API to retrieve full movie details by IMDb ID
        public async Task<MovieDetails> GetMovieDetails(string imdbId)
        {
            return await _httpClient.GetFromJsonAsync<MovieDetails>($"http://www.omdbapi.com/?apikey={_apiKey}&i={imdbId}&plot=full");
        }
    }
}
