using Bookify.Models;
using Bookify.Models.ViewModels;
using Bookify.services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Controllers
{
    public class RoomController : Controller
    {
        private readonly IRoomSerivce _roomService;
        private readonly IAmenityService _amenityService;
        private readonly IRoomTypeService _roomTypeService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public RoomController(IRoomSerivce roomService, IAmenityService amenityService, IRoomTypeService roomTypeService, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _roomService = roomService;
            _amenityService = amenityService;
            _roomTypeService = roomTypeService;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? minPrice, int? maxPrice, int? maxGuests, List<int>? roomTypeIds, List<int>? amenityIds, string? order, int page = 1)
        {
            int pageSize = 6;


            IEnumerable<RoomCardViewModel> rooms;

            bool hasFilters = minPrice.HasValue || maxPrice.HasValue || maxGuests.HasValue ||
                              (roomTypeIds != null && roomTypeIds.Any()) ||
                              (amenityIds != null && amenityIds.Any());

            var allRoomTypes = await _roomTypeService.GetAllAsync();
            var allAmenities = await _amenityService.GetAllAsync();

            if (hasFilters)
            {
                var selectedTypes = allRoomTypes.Where(rt => roomTypeIds != null && roomTypeIds.Contains(rt.Id));
                var selectedAmenities = allAmenities.Where(a => amenityIds != null && amenityIds.Contains(a.Id));

                rooms = await _roomService.SerachWithFilterAsync(
                    minPrice ?? 0,
                    maxPrice ?? 1000000,
                    maxGuests ?? 0,
                    selectedTypes,
                    selectedAmenities
                );
            }
            else
            {
                rooms = await _roomService.AllForCards();
            }

            rooms = order switch
            {
                "price_asc" => rooms.OrderBy(r => r.PricePerNight),
                "price_desc" => rooms.OrderByDescending(r => r.PricePerNight),
                "rating_desc" => rooms.OrderByDescending(r => r.Rating),
                _ => rooms.OrderBy(r => r.PricePerNight)
            };

            var pagedRooms = rooms
                     .Skip((page - 1) * pageSize)
                     .Take(pageSize)
                     .ToList();

            var vm = new RoomIndexViewModel
            {
                Rooms = pagedRooms,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                NumberOfGeusts = maxGuests,
                RoomTypes = allRoomTypes.Select(rt => new SelectListItem { Value = rt.Id.ToString(), Text = rt.Name }),
                Amenities = allAmenities.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name }),
                SelectedRoomTypeIds = roomTypeIds ?? new List<int>(),
                SelectedAmenityIds = amenityIds ?? new List<int>(),
                SelectedOrder = order,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)rooms.Count() / pageSize)
            };
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                Response.Headers.Append("Vary", "X-Requested-With"); 
                Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
                Response.Headers.Append("Pragma", "no-cache");
                Response.Headers.Append("Expires", "0");
                return PartialView("_RoomListPartial", vm);
            }
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var room = await _roomService.GetRoomDetailsAsync(id);

                // Load reviews for this room
                var reviews = await _context.Reviews
                    .Include(r => r.User)
                    .Where(r => r.RoomId == id)
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new ReviewViewModel
                    {
                        UserName = r.User.FullName,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt
                    })
                    .ToListAsync();

                room.Reviews = reviews;

                // Check if user can add a review
                if (User.Identity?.IsAuthenticated == true)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user != null)
                    {
                        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                        var hasReviewed = await _context.Reviews
                            .AnyAsync(r => r.UserId == user.Id && r.RoomId == id);

                        room.CanAddReview = !isAdmin && !hasReviewed;
                        room.HasUserReviewed = hasReviewed;
                    }
                }

                return View(room);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            var model = new AddRoomViewModel();
            await PopulateDropdowns(model);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddRoomViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return View(model);
            }

            await _roomService.AddRoomAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var model = await _roomService.GetRoomForEditAsync(id);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditRoomViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var fullModel = await _roomService.GetRoomForEditAsync(model.Id);

                fullModel.Name = model.Name;
                fullModel.RoomTypeId = model.RoomTypeId;
                fullModel.MaxGuests = model.MaxGuests;
                fullModel.PriceForNight = model.PriceForNight;
                fullModel.Area = model.Area;
                fullModel.NoOfBeds = model.NoOfBeds;
                fullModel.Description = model.Description;
                fullModel.IsAvailable = model.IsAvailable;
                fullModel.HasDiscount = model.HasDiscount;
                fullModel.DiscountPercentage = model.DiscountPercentage;
                fullModel.RoomNumber = model.RoomNumber;
                fullModel.SelectedAmenityIds = model.SelectedAmenityIds;

                return View(fullModel);
            }

            try
            {
                await _roomService.UpdateRoomAsync(model);
                TempData["Success"] = "Room updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                var fullModel = await _roomService.GetRoomForEditAsync(model.Id);
                return View(fullModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPopularRooms()
        {
            var rooms = await _roomService.GetTopRatedRoomsAsync(3);
            return Json(rooms);
        }

        [HttpGet]
        public async Task<IActionResult> GetRecentReviews()
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.Rating == 5)
                .OrderByDescending(r => r.CreatedAt)
                .Take(3)
                .Select(r => new ReviewViewModel
                {
                    UserName = r.User.FullName,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return Json(reviews);
        }

       

        // Public debug endpoint to check room data
        [HttpGet]
        public async Task<IActionResult> CheckRoomData()
        {
            try
            {
                var rooms = await _roomService.AllForCards();
                var result = rooms.Take(5).Select(r => new {
                    Id = r.Id,
                    Name = r.Name,
                    ImageUrl = r.ImageUrl,
                    Description = r.Description?.Substring(0, Math.Min(50, r.Description?.Length ?? 0)) + "...",
                    AmenityCount = r.Amenities?.Count() ?? 0,
                    HasImage = !string.IsNullOrEmpty(r.ImageUrl)
                }).ToList();
                
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // Action to fix room images in database
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> FixRoomImages()
        {
            try
            {
                // Clear existing images
                var existingImages = await _context.RoomImages.ToListAsync();
                _context.RoomImages.RemoveRange(existingImages);
                
                // Get all rooms
                var rooms = await _context.Rooms.OrderBy(r => r.RoomNumber).ToListAsync();
                
                var newImages = new List<RoomImage>();
                
                // Add specific images for each room
                var roomImageMappings = new Dictionary<int, List<string>>
                {
                    // Luxury rooms
                    { 2501, new List<string> { "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800&q=80", "https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=800&q=80" } },
                    { 2502, new List<string> { "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800&q=80", "https://images.unsplash.com/photo-1556020685-ae41abfc9365?w=800&q=80" } },
                    { 2601, new List<string> { "https://images.unsplash.com/photo-1590381105924-c72589b9ef3f?w=800&q=80", "https://images.unsplash.com/photo-1584132915807-fd1f5fbc078f?w=800&q=80" } },
                    { 2602, new List<string> { "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800&q=80", "https://images.unsplash.com/photo-1583847268964-b28dc8f51f92?w=800&q=80" } },
                    { 2701, new List<string> { "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800&q=80", "https://images.unsplash.com/photo-1541971875076-8f970d573be6?w=800&q=80" } },
                    
                    // Family rooms
                    { 1205, new List<string> { "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800&q=80", "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800&q=80" } },
                    { 1206, new List<string> { "https://images.unsplash.com/photo-1629140727571-9b5c6f6267b4?w=800&q=80" } },
                    { 1401, new List<string> { "https://images.unsplash.com/photo-1586611292717-f828b167408c?w=800&q=80" } },
                    { 1402, new List<string> { "https://images.unsplash.com/photo-1584132967334-10e028bd69f7?w=800&q=80" } },
                    { 1501, new List<string> { "https://images.unsplash.com/photo-1578683010236-d716f9a3f461?w=800&q=80" } },
                    
                    // Budget rooms
                    { 310, new List<string> { "https://images.unsplash.com/photo-1568084680786-a84f91d1153c?w=800&q=80" } },
                    { 311, new List<string> { "https://images.unsplash.com/photo-1522771739844-6a9f6d5f14af?w=800&q=80" } },
                    { 405, new List<string> { "https://images.unsplash.com/photo-1586772002994-1c26ab6d7ac7?w=800&q=80" } },
                    { 406, new List<string> { "https://images.unsplash.com/photo-1587985064135-0366536eab42?w=800&q=80" } },
                    { 501, new List<string> { "https://images.unsplash.com/photo-1580977050765-5030877c1b45?w=800&q=80" } },
                    
                    // Romantic suites
                    { 801, new List<string> { "https://images.unsplash.com/photo-1591088398332-8a7791972843?w=800&q=80", "https://images.unsplash.com/photo-1590490360182-c33d57733427?w=800&q=80" } },
                    { 802, new List<string> { "https://images.unsplash.com/photo-1567767292278-a4f21aa2d36e?w=800&q=80" } },
                    { 901, new List<string> { "https://images.unsplash.com/photo-1587985064078-4b1850949efa?w=800&q=80" } },
                    { 902, new List<string> { "https://images.unsplash.com/photo-1598300042247-d088f8ab3a91?w=800&q=80" } },
                    { 1001, new List<string> { "https://images.unsplash.com/photo-1571003123894-1f0594d2b5d9?w=800&q=80" } }
                };

                foreach (var room in rooms)
                {
                    if (roomImageMappings.ContainsKey(room.RoomNumber))
                    {
                        foreach (var imageUrl in roomImageMappings[room.RoomNumber])
                        {
                            newImages.Add(new RoomImage 
                            { 
                                RoomId = room.Id, 
                                ImageUrl = imageUrl 
                            });
                        }
                    }
                    else
                    {
                        // Default fallback image for any room without specific mapping
                        var fallbackImages = new[]
                        {
                            "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800&q=80",
                            "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800&q=80",
                            "https://images.unsplash.com/photo-1590381105924-c72589b9ef3f?w=800&q=80",
                            "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800&q=80",
                            "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800&q=80"
                        };
                        
                        var imageIndex = room.RoomNumber % fallbackImages.Length;
                        newImages.Add(new RoomImage 
                        { 
                            RoomId = room.Id, 
                            ImageUrl = fallbackImages[imageIndex]
                        });
                    }
                }

                _context.RoomImages.AddRange(newImages);
                await _context.SaveChangesAsync();
                
                return Json(new { 
                    success = true, 
                    message = $"Fixed images for {rooms.Count} rooms", 
                    imageCount = newImages.Count,
                    rooms = rooms.Select(r => new { 
                        roomNumber = r.RoomNumber, 
                        name = r.Name,
                        imageCount = newImages.Count(i => i.RoomId == r.Id)
                    })
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // Public endpoint to trigger image refresh without admin requirement (for testing)
        [HttpGet]
        public async Task<IActionResult> RefreshRoomImages()
        {
            try
            {
                // This just returns the current image status and suggests using the admin endpoint
                var rooms = await _roomService.AllForCards();
                var roomsWithIssues = rooms.Where(r => string.IsNullOrEmpty(r.ImageUrl) || r.ImageUrl.Contains("default-room.jpg")).ToList();
                
                return Json(new { 
                    success = true, 
                    message = "Image status check complete",
                    totalRooms = rooms.Count(),
                    roomsWithImageIssues = roomsWithIssues.Count,
                    needsAdminFix = roomsWithIssues.Count > 0,
                    adminFixUrl = "/Room/FixRoomImages",
                    adminFixPageUrl = "/Room/FixImagesPage"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // View for the fix images page
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult FixImagesPage()
        {
            return View("FixImages");
        }

        private async Task PopulateDropdowns(AddRoomViewModel model)
        {
            var types = await _roomTypeService.GetAllAsync();
            var amenities = await _amenityService.GetAllAsync();

            model.RoomTypes = types.Select(t => new SelectListItem { Text = t.Name, Value = t.Id.ToString() }).ToList();
            model.Amenities = amenities.Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString() }).ToList();
        }
    }
}