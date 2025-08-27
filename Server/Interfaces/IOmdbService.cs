using OMDbMovieSearch.Shared.Models;

namespace OMDbMovieSearch.Server.Interfaces
{
    /// <summary>
    /// Defines operations for interacting with the OMDb API.
    /// </summary>
    public interface IOmdbService
    {
        Task<List<MovieSearchResult>> GetMoviesAsync(string query, CancellationToken ct);
        Task<MovieDetails?> GetMovieDetailsAsync(string imdbId, CancellationToken ct);
    }
}
