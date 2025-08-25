namespace OMDbMovieSearch.Tests
{
    public class MoviesControllerTests
    {
        private readonly OmdbService _service;
        private readonly MoviesController _controller;

        public MoviesControllerTests()
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
            _controller = new MoviesController(_service);
        }

        [Fact]
        public async Task Search_WithValidQuery_ReturnsResults()
        {
            // Arrange
            var title = "Amelie";

            // Act
            var result = await _controller.Search(title);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var movies = Assert.IsAssignableFrom<List<OMDbMovieSearch.Shared.Models.MovieSearchResult>>(okResult.Value);
            Assert.NotEmpty(movies);
        }

        [Fact]
        public async Task Search_WithEmptyQuery_ReturnsEmptyList()
        {
            // Arrange
            var title = "";

            // Act
            var result = await _controller.Search(title);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var movies = Assert.IsAssignableFrom<List<OMDbMovieSearch.Shared.Models.MovieSearchResult>>(okResult.Value);
            Assert.Empty(movies);
        }

        [Fact]
        public async Task Details_WithValidImdbId_ReturnsMovieDetails()
        {
            // Arrange
            var imdbId = "tt0211915"; // Amélie

            // Act
            var result = await _controller.Details(imdbId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var movie = Assert.IsType<OMDbMovieSearch.Shared.Models.MovieDetails>(okResult.Value);
            Assert.Equal("Amélie", movie.Title);
            Assert.Equal(imdbId, movie.imdbID);
        }

        [Fact]
        public void Recent_ReturnsQueries()
        {
            // Act
            var result = _controller.Recent();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<IEnumerable<string>>(okResult.Value);
        }
    }
}