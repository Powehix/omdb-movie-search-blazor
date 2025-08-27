using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OMDbMovieSearch.Shared.Models;

namespace OMDbMovieSearch.Tests
{
    public class MoviesControllerTests
    {
        private readonly MoviesController _controller;

        public MoviesControllerTests()
        {
            var httpClient = new HttpClient { BaseAddress = new Uri("http://www.omdbapi.com/") };
            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            var logger = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            }).CreateLogger<OmdbService>();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Omdb:ApiKey", "api_key" } // test API key
                })
                .Build();

            var omdbService = new OmdbService(httpClient, memoryCache, logger, config);
            _controller = new MoviesController(omdbService);
        }

        [Fact]
        public async Task GetMovies_WithValidQuery_ReturnsResults()
        {
            // Arrange
            var title = "Amelie";
            var ct = CancellationToken.None;

            // Act
            var result = await _controller.GetMovies(title, ct);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var movies = Assert.IsAssignableFrom<List<MovieSearchResult>>(okResult.Value);
            Assert.NotEmpty(movies);
        }

        [Fact]
        public async Task GetMovies_WithEmptyQuery_ReturnsEmptyList()
        {
            // Arrange
            var title = "";
            var ct = CancellationToken.None;

            // Act
            var result = await _controller.GetMovies(title, ct);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var movies = Assert.IsAssignableFrom<List<MovieSearchResult>>(okResult.Value);
            Assert.Empty(movies);
        }

        [Fact]
        public async Task GetMovieDetails_WithValidImdbId_ReturnsMovieDetails()
        {
            // Arrange
            var imdbId = "tt0211915"; // Amélie
            var ct = CancellationToken.None;

            // Act
            var result = await _controller.GetMovieDetails(imdbId, ct);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var movie = Assert.IsType<MovieDetails>(okResult.Value);
            Assert.Equal("Amélie", movie.Title);
            Assert.Equal(imdbId, movie.imdbID);
        }

        [Fact]
        public void GetRecentMovies_ReturnsQueries()
        {
            // Act
            var result = _controller.GetRecentMovies();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<IEnumerable<string>>(okResult.Value);
        }
    }
}