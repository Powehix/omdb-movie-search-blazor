using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace OMDbMovieSearch.Tests
{
    public class OmdbServiceTests
    {
        private readonly OmdbService _service;

        public OmdbServiceTests()
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

            _service = new OmdbService(httpClient, memoryCache, logger, config);
        }

        [Fact]
        public async Task GetMoviesAsync_WithValidTitle_ReturnsResults()
        {
            // Arrange
            var title = "Amelie";
            var ct = CancellationToken.None;

            // Act
            var results = await _service.GetMoviesAsync(title, ct);

            // Assert
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Contains(results, m => m.Title.Contains("Amélie", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task GetMoviesAsync_WithEmptyQuery_ReturnsEmptyList()
        {
            // Arrange
            var title = "";
            var ct = CancellationToken.None;

            // Act
            var results = await _service.GetMoviesAsync(title, ct);

            // Assert
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        [Fact]
        public async Task GetMovieDetailsAsync_WithValidId_ReturnsMovieDetails()
        {
            // Arrange
            var imdbId = "tt0211915"; // Amelie
            var ct = CancellationToken.None;

            // Act
            var movie = await _service.GetMovieDetailsAsync(imdbId, ct);

            // Assert
            Assert.NotNull(movie);
            Assert.Equal("Amélie", movie.Title);
        }

        [Fact]
        public async Task GetMovieDetailsAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var imdbId = "invalid_id";
            var ct = CancellationToken.None;

            // Act
            var movie = await _service.GetMovieDetailsAsync(imdbId, ct);

            // Assert
            Assert.Null(movie);
        }
    }
}