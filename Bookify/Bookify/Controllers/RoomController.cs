using Bookify.Models;
using Bookify.Models.ViewModels;
using Bookify.services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Controllers
{
    public class RoomController : Controller
    {
        private readonly IRoomSerivce _roomService; 
        private readonly IAmenityService _amenityService;
        private readonly IRoomTypeService _roomTypeService;

        public RoomController(IRoomSerivce roomService, IAmenityService amenityService, IRoomTypeService roomTypeService)
        {
            _roomService = roomService;
            _amenityService = amenityService;
            _roomTypeService = roomTypeService;
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

        private async Task PopulateDropdowns(AddRoomViewModel model)
        {
            var types = await _roomTypeService.GetAllAsync();
            var amenities = await _amenityService.GetAllAsync();

            model.RoomTypes = types.Select(t => new SelectListItem { Text = t.Name, Value = t.Id.ToString() }).ToList();
            model.Amenities = amenities.Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString() }).ToList();
        }
    }
}