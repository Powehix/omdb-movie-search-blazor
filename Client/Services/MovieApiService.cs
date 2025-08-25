using OMDbMovieSearch.Shared.Models;
using System.Net.Http.Json;

namespace OMDbMovieSearch.Client.Services
{
    public class MovieApiService
    {
        private readonly HttpClient _http;

        public MovieApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<MovieSearchResult>> SearchMoviesAsync(string title)
        {
            return await _http.GetFromJsonAsync<List<MovieSearchResult>>($"api/movies/search?title={title}") ?? new();
        }

        public async Task<MovieDetails> GetMovieDetailsAsync(string imdbId)
        {
            return await _http.GetFromJsonAsync<MovieDetails>($"api/movies/details/{imdbId}");
        }

        public async Task<List<string>> GetRecentSearchesAsync()
        {
            return await _http.GetFromJsonAsync<List<string>>("api/movies/recent") ?? new();
        }
    }
}
