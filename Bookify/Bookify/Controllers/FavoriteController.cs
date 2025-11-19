using Bookify.services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bookify.Controllers
{
    [Authorize]
    public class FavoriteController : Controller
    {
        private readonly IFavoriteService _favoriteService;
        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }
        [HttpPost]
        public async Task<IActionResult> Toggle(int roomId)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();
            var isfav = await _favoriteService.ToggleFavoriteAsync(userId, roomId);

            return Json(new { isFavorite = isfav });

        }
        public async Task<IActionResult> MyFavorites()
        {
            var userId = GetUserId();
            if(userId == null) 
                return Unauthorized();
            var favorites = await _favoriteService.GetFavoritesForUser(userId);
            return View(favorites);
        }
    }
}
