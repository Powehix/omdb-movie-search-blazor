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

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string title, CancellationToken ct)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                if (RecentQueries.Count >= 5)
                    RecentQueries.Dequeue();
                RecentQueries.Enqueue(title);
            }

            var result = await _omdb.SearchMoviesAsync(title, ct);
            return Ok(result);
        }

        [HttpGet("recent")]
        public IActionResult Recent()
        {
            return Ok(RecentQueries.Reverse());
        }

        [HttpGet("details/{imdbId}")]
        public async Task<IActionResult> Details(string imdbId, CancellationToken ct)
        {
            var movie = await _omdb.GetMovieDetailsAsync(imdbId, ct);
            return Ok(movie);
        }
    }
}