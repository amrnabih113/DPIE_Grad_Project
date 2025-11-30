using Bookify.Data.Models;
using Bookify.Models;
using Bookify.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(AddReviewViewModel model)
        {
            try
            {
                // Get current user
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found. Please login again." });
                }

                // Check if user is Admin
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (isAdmin)
                {
                    return Json(new { success = false, message = "Admins are not allowed to add reviews." });
                }

                // Validate model
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { success = false, message = string.Join(", ", errors) });
                }

                // Check if user has already reviewed this room
                var existingReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.UserId == user.Id && r.RoomId == model.RoomId);

                if (existingReview != null)
                {
                    return Json(new { success = false, message = "You have already reviewed this room." });
                }

                // Check if room exists
                var room = await _context.Rooms.FindAsync(model.RoomId);
                if (room == null)
                {
                    return Json(new { success = false, message = "Room not found." });
                }

                // Create new review
                var review = new Review
                {
                    UserId = user.Id,
                    RoomId = model.RoomId,
                    Rating = model.Rating,
                    Comment = model.Comment,
                    CreatedAt = DateTime.Now
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                // Return success with review data
                return Json(new
                {
                    success = true,
                    message = "Review added successfully!",
                    review = new
                    {
                        userName = user.FullName,
                        rating = review.Rating,
                        comment = review.Comment,
                        createdAt = review.CreatedAt.ToString("MMM dd, yyyy")
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while adding your review. Please try again." });
            }
        }
    }
}
