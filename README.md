# OMDb Movie Search App

A simple Blazor WebAssembly application for searching movies using the [OMDb API](http://www.omdbapi.com).  

## 🛠 Tech Stack

- **.NET 7**
- **Blazor WebAssembly** (Frontend)
- **ASP.NET Core Web API** (Backend)
- **OMDb API** (Movie data)
- **xUnit** (Unit Testing)

## 📦 Features

- 🔍 Movie search by title
- 📜 View extended movie details (poster, plot, rating, etc.)
- 🕓 Stores and displays 5 latest search queries
- ✅ Includes unit tests for `OmdbService` and `MoviesController`

## ⚙️ Setup Instructions

1. **Clone the repository**:

   ```bash
   git clone https://github.com/Powehix/omdb-movie-search-blazor.git

2. **Obtain OMDB API key**:

   - Go to [OMDb API](http://www.omdbapi.com). 
   - Click the API key in the upper menu.
   - Enter you e-mail and click Submit.

2. **Add OMDb API Key using User Secrets**:

    Run this in the terminal from the Server project folder:

   ```dotnet user-secrets init
    dotnet user-secrets set "Omdb:ApiKey" "api_key"
   ```
    Your secrets file will look like this:
    
    ```{
      "Omdb": {
        "ApiKey": "your_key"
      }
3. **Set the Server project as Startup Project in Visual Studio**:

    Right-click `OMDbMovieSearch.Server` → Set as Startup Project.

4. **Run the application (`F5` or `dotnet run`)**.

    The Blazor client will be served through the backend.
