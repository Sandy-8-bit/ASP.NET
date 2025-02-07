using Microsoft.AspNetCore.Mvc;
using BlazorWasmApp.Models;
using WeatherApp.Services;
using Microsoft.AspNetCore.Cors;

namespace WeatherApp.Controllers
{
    [EnableCors("AllowBlazorWasm")]
    [ApiController]
    [Route("api/favorites")]
    public class FavoritesController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;
        private readonly SupabaseAuthService _authService;

        public FavoritesController(MongoDBService mongoDBService, SupabaseAuthService authService)
        {
            _mongoDBService = mongoDBService;
            _authService = authService;
        }

        // Add a city to the favorites list
        [HttpPost("{city}")]
        public async Task<ActionResult> AddFavoriteCityAsync(string city)
        {
            var currentUser = _authService.GetCurrentUser();
            if (currentUser == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var userId = currentUser.Id; // Assuming `Id` is the unique identifier for the user
            if (string.IsNullOrEmpty(city))
            {
                return BadRequest("City name cannot be empty.");
            }

            try
            {

                var isAlreadyFavourite = await _mongoDBService.isFavouritecityAsync(userId!, city);
                if (isAlreadyFavourite)
                {
                    return Conflict($"{city} is Already in Your Favourtite");
                }
                await _mongoDBService.AddFavoriteCityAsync(userId!, city);
                return Ok($"{city} added to favorites.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the city: {ex.Message}");
            }
        }

        // Retrieve all favorite cities for the current user
        [HttpGet]
        public async Task<ActionResult<List<FavoriteCity>>> GetFavorites()
        {
            var currentUser = _authService.GetCurrentUser();
            if (currentUser == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var userId = currentUser.Id;
            try
            {
                var favorites = await _mongoDBService.GetFavoriteCitiesAsync(userId!);
                return Ok(favorites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving favorites: {ex.Message}");
            }
        }

        // Remove a city from the favorites list
        [HttpDelete("{city}")]
        public async Task<ActionResult> RemoveFavoriteCity(string city)
        {
            var currentUser = _authService.GetCurrentUser();
            if (currentUser == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var userId = currentUser.Id;
            try
            {
                await _mongoDBService.RemoveFavoriteCityAsync(userId!, city);

                return Ok($"{city} removed from favorites.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while removing the city: {ex.Message}");
            }
        }

        // Set a city as the home city
        [HttpPost("{city}/home")]
        public async Task<ActionResult> SetHomeCity(string city)
        {
            var currentUser = _authService.GetCurrentUser();
            if (currentUser == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var userId = currentUser.Id;
            if (string.IsNullOrEmpty(city))
            {
                return BadRequest("City name cannot be empty.");
            }

            try
            {
                await _mongoDBService.SetHomeCityAsync(userId!, city);
                return Ok($"{city} is now your home city.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while setting the home city: {ex.Message}");
            }
        }

        // Get the current home city
        [HttpGet("home")]
        public async Task<ActionResult<FavoriteCity>> GetHomeCity()
        {
            var currentUser = _authService.GetCurrentUser();
            if (currentUser == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var userId = currentUser.Id;
            try
            {
                var homeCity = await _mongoDBService.GetHomeCityAsync(userId!);
                if (homeCity == null)
                {
                    return NotFound("No home city set.");
                }

                return Ok(homeCity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the home city: {ex.Message}");
            }
        }

        // Remove the current home city
        [HttpDelete("home")]
        public async Task<ActionResult> RemoveHomeCity()
        {
            var currentUser = _authService.GetCurrentUser();
            if (currentUser == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var userId = currentUser.Id;
            try
            {
                await _mongoDBService.RemoveHomeCityAsync(userId!);
                return Ok("Home city has been removed.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while removing the home city: {ex.Message}");
            }
        }
    }
}
