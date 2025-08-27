using Microsoft.AspNetCore.Mvc;
using OMDbMovieSearch.Server.Interfaces;

namespace OMDbMovieSearch.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private static readonly Queue<string> RecentQueries = new();
        private readonly IOmdbService _omdb;

        public MoviesController(IOmdbService omdb)
        {
            _omdb = omdb;
        }

        [HttpGet("getMovies")]
        public async Task<IActionResult> GetMovies([FromQuery] string title, CancellationToken ct)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                if (RecentQueries.Count >= 5)
                    RecentQueries.Dequeue();
                RecentQueries.Enqueue(title);
            }
            else
            {
                return BadRequest("Title is required");
            }

                var result = await _omdb.GetMoviesAsync(title, ct);
            return Ok(result);
        }

        [HttpGet("getRecentMovies")]
        public IActionResult GetRecentMovies()
        {
            // Return the last 5 titles, newest last
            return Ok(RecentQueries.Reverse());
        }

        [HttpGet("getMovieDetails/{imdbId}")]
        public async Task<IActionResult> GetMovieDetails(string imdbId, CancellationToken ct)
        {
            var movie = await _omdb.GetMovieDetailsAsync(imdbId, ct);
            return Ok(movie);
        }
    }
}