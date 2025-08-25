using OMDbMovieSearch.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace OMDbMovieSearch.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private static readonly Queue<string> RecentQueries = new();
        private readonly OmdbService _omdb;

        public MoviesController(OmdbService omdb)
        {
            _omdb = omdb;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                if (RecentQueries.Count >= 5)
                    RecentQueries.Dequeue();
                RecentQueries.Enqueue(title);
            }

            var result = await _omdb.SearchMovies(title);
            return Ok(result);
        }

        [HttpGet("recent")]
        public IActionResult Recent()
        {
            return Ok(RecentQueries.Reverse());
        }

        [HttpGet("details/{imdbId}")]
        public async Task<IActionResult> Details(string imdbId)
        {
            var movie = await _omdb.GetMovieDetails(imdbId);
            return Ok(movie);
        }
    }
}