namespace OMDbMovieSearch.Tests
{
    public class OmdbServiceTests
    {
        private readonly OmdbService _service;

        public OmdbServiceTests()
        {
            var httpClient = new HttpClient();
            var configValues = new Dictionary<string, string>
            {
                { "Omdb:ApiKey", "api_key" } // Insert real API key here
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();

            _service = new OmdbService(httpClient, configuration);
        }

        [Fact]
        public async Task SearchMovies_WithValidQuery_ReturnsResults()
        {
            // Arrange
            var query = "Amelie";

            // Act
            var results = await _service.SearchMovies(query);

            // Assert
            Assert.NotNull(results);
            Assert.NotEmpty(results);
        }

        [Fact]
        public async Task SearchMovies_WithEmptyQuery_ReturnsEmptyList()
        {
            // Arrange
            var query = "";

            // Act
            var results = await _service.SearchMovies(query);

            // Assert
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        [Fact]
        public async Task GetMovieDetails_WithValidId_ReturnsMovieDetails()
        {
            // Arrange
            var imdbId = "tt0211915"; // Amelie

            // Act
            var movie = await _service.GetMovieDetails(imdbId);

            // Assert
            Assert.NotNull(movie);
            Assert.Equal("Amélie", movie.Title);
            Assert.Equal(imdbId, movie.imdbID);
        }

        [Fact]
        public async Task GetMovieDetails_WithInvalidId_ThrowsHttpRequestException()
        {
            // Arrange
            var invalidId = "invalid_id";

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                var result = await _service.GetMovieDetails(invalidId);
            });
        }
    }
}