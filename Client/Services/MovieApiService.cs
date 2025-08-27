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

        public async Task<List<MovieSearchResult>> GetMoviesAsync(string title)
        {
            return await _http.GetFromJsonAsync<List<MovieSearchResult>>($"api/movies/getMovies?title={title}") ?? new();
        }

        public async Task<MovieDetails?> GetMovieDetailsAsync(string imdbId)
        {
            return await _http.GetFromJsonAsync<MovieDetails>($"api/movies/getMovieDetails/{imdbId}");
        }

        public async Task<List<string>> GetRecentMoviesAsync()
        {
            return await _http.GetFromJsonAsync<List<string>>("api/movies/getRecentMovies") ?? new();
        }
    }
}
