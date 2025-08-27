using OMDbMovieSearch.Shared.Models;

namespace OMDbMovieSearch.Server.Interfaces
{
    /// <summary>
    /// Defines operations for interacting with the OMDb API.
    /// </summary>
    public interface IOmdbService
    {
        Task<IReadOnlyList<MovieSearchResult>> SearchMoviesAsync(string query, CancellationToken ct);
        Task<MovieDetails?> GetMovieDetailsAsync(string imdbId, CancellationToken ct);
    }
}
