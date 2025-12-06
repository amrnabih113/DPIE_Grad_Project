using Bookify.Models;
using Bookify.Models.ViewModels;
using Bookify.Repository.IRepository;
using Bookify.services.IServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bookify.services
{
    public class RoomService : IRoomSerivce
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IAmenityService _amenityService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFavoriteService _favoriteService;
        private readonly IWebHostEnvironment _env;
        private readonly IGenericRepository<RoomType> _roomTypesRepo;
        private readonly IGenericRepository<Booking> _bookingRepository;

        public RoomService(IRoomRepository roomRepository,
                           IAmenityService amenityService,
                           IHttpContextAccessor httpContextAccessor,
                           IFavoriteService favoriteService,
                           IWebHostEnvironment env,
                           IGenericRepository<RoomType> roomTypesRepo,
                           IGenericRepository<Booking> bookingRepository)
        {
            _roomRepository = roomRepository;
            _amenityService = amenityService;
            _httpContextAccessor = httpContextAccessor;
            _favoriteService = favoriteService;
            _env = env;
            _roomTypesRepo = roomTypesRepo;
            _bookingRepository = bookingRepository;
        }

        public async Task<IEnumerable<Room>> GetAllAsync() => await _roomRepository.GetAllAsync();
        public async Task<Room> GetByIdAsync(int id) => await _roomRepository.GetByIdAsync(id);

        public async Task<Room> AddAsync(Room entity)
        {
            await _roomRepository.AddAsync(entity);
            await _roomRepository.SaveChangesAsync();
            return entity;
        }

        public async Task<Room> UpdateAsync(Room entity)
        {
            _roomRepository.Update(entity);
            await _roomRepository.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return false;

            _roomRepository.Delete(room);
            await _roomRepository.SaveChangesAsync();
            return true;
        }

        public async Task AddRoomAsync(AddRoomViewModel model)
        {
            var room = new Room
            {
                Name = model.Name,
                RoomTypeId = model.RoomTypeId,
                MaxGuests = model.MaxGuests,
                PriceForNight = model.PriceForNight,
                Area = (decimal)model.Area,
                NoOfBeds = model.NoOfBeds,
                Description = model.Description,
                IsAvailable = model.IsAvailable,
                HasDiscount = model.HasDiscount,
                DiscountPercent = model.HasDiscount ? model.DiscountPercentage : null,
                RoomNumber = model.RoomNumber
            };

            room.RoomImages = await ProcessRoomImagesAsync(model.RoomImages);

            if (model.SelectedAmenityIds != null && model.SelectedAmenityIds.Any())
            {
                room.RoomAmenities = model.SelectedAmenityIds.Select(id => new RoomAmenity { AmenityId = id }).ToList();
            }

            await _roomRepository.AddAsync(room);
            await _roomRepository.SaveChangesAsync();
        }

        public async Task UpdateRoomAsync(EditRoomViewModel model)
        {
            var room = await _roomRepository.GetByIdWithAllFeaturesAsync(model.Id);
            if (room == null) throw new Exception($"Room with id {model.Id} not found.");

            room.Name = model.Name;
            room.RoomTypeId = model.RoomTypeId;
            room.MaxGuests = model.MaxGuests;
            room.PriceForNight = model.PriceForNight;
            room.Area = (decimal)model.Area;
            room.NoOfBeds = model.NoOfBeds;
            room.Description = model.Description;
            room.IsAvailable = model.IsAvailable;
            room.HasDiscount = model.HasDiscount;
            room.DiscountPercent = model.HasDiscount ? model.DiscountPercentage : null;
            room.RoomNumber = model.RoomNumber;

            room.RoomAmenities.Clear();
            if (model.SelectedAmenityIds != null && model.SelectedAmenityIds.Any())
            {
                foreach (var id in model.SelectedAmenityIds)
                {
                    room.RoomAmenities.Add(new RoomAmenity { AmenityId = id, RoomId = room.Id });
                }
            }

            if (room.RoomImages != null && room.RoomImages.Any())
            {
                var imagesToDelete = room.RoomImages
                    .Where(img => !model.KeepImageIds.Contains(img.Id))
                    .ToList();

                foreach (var img in imagesToDelete)
                {
                    var imagePath = Path.Combine(_env.WebRootPath, img.ImageUrl.TrimStart('/').Replace("/", "\\"));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    room.RoomImages.Remove(img);
                }
            }

            if (model.RoomImages != null && model.RoomImages.Any())
            {
                var newImages = await ProcessRoomImagesAsync(model.RoomImages);
                foreach (var image in newImages)
                {
                    room.RoomImages.Add(image);
                }
            }

            _roomRepository.Update(room);
            await _roomRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<RoomCardViewModel>> AllForCards()
        {
            var allRooms = await _roomRepository.GetAllWithReviewsAsync();
            return await MapRoomsToCardsAsync(allRooms);
        }

        public async Task<IEnumerable<RoomCardViewModel>> SerachWithFilterAsync(int minPrice, int maxPrice, int maxGuests, IEnumerable<RoomType> roomTypes, IEnumerable<Amenity> amenities)
        {
            var allRooms = await _roomRepository.SearchWithFiltersAsync(minPrice, maxPrice, maxGuests, roomTypes, amenities);
            return await MapRoomsToCardsAsync(allRooms);
        }

        public async Task<RoomDetailsViewModel> GetRoomDetailsAsync(int id)
        {
            var room = await _roomRepository.GetByIdWithReviewsAsync(id);

            if (room == null) throw new Exception($"Room with id {id} not found.");
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var currentDate = DateTime.Now.Date;
            var allBookings = await _bookingRepository.GetAllAsync();
            var activeBooking = allBookings
                .Where(b => b.RoomId == id &&
                            b.CheckOut.Date > currentDate &&
                            (b.PaymentStatus == "Pending" || b.PaymentStatus == "Confirmed"))
                .OrderBy(b => b.CheckIn)
                .FirstOrDefault();

            bool isBooked = activeBooking != null;
            DateTime? checkIn = activeBooking?.CheckIn;
            DateTime? checkOut = activeBooking?.CheckOut;
            int? daysUntilAvailable = checkOut.HasValue ? (checkOut.Value.Date - currentDate).Days : null;

            return new RoomDetailsViewModel
            {
                Id = room.Id,
                Name = room.Name ?? $"Room {room.RoomNumber}",
                Description = room.Description ?? "Comfortable and well-appointed room with modern amenities.",
                NumberOfGusts = room.MaxGuests,
                Area = (int)room.Area,
                IsFavorite = userId != null && await _favoriteService.IsFavoriteAsync(userId, room.Id),
                PricePerNight = (int)room.PriceForNight,
                DiscountPercentage = room.HasDiscount ? room.DiscountPercent : null,
                FinalPrice = CalculateFinalPrice(room.PriceForNight, room.HasDiscount, room.DiscountPercent),
                Rating = CalculateRating(room.Reviews),
                NumberOfReviews = room.Reviews?.Count ?? 0,
                Amenities = await _amenityService.GetAmenitiesForRoomAsync(room.Id) ?? new List<Amenity>(),
                RoomImages = GetRoomImagesWithFallback(room.RoomImages, room.Id),
                IsBooked = isBooked,
                BookingCheckIn = checkIn,
                BookingCheckOut = checkOut,
                DaysUntilAvailable = daysUntilAvailable
            };
        }

        public async Task<IEnumerable<TopRoomViewModel>> GetTopRatedRoomsAsync(int top = 3)
        {
            var rooms = await _roomRepository.GetAllWithReviewsAsync();
            var allBookings = await _bookingRepository.GetAllAsync();
            var currentDate = DateTime.Now.Date;

            return rooms
                .OrderByDescending(r => CalculateRating(r.Reviews))
                .Take(top)
                .Select(r => {
                    var activeBooking = allBookings
                        .Where(b => b.RoomId == r.Id && 
                                    b.CheckOut.Date > currentDate &&
                                    (b.PaymentStatus == "Pending" || b.PaymentStatus == "Confirmed"))
                        .OrderBy(b => b.CheckIn)
                        .FirstOrDefault();

                    return new TopRoomViewModel
                    {
                        Id = r.Id,
                        Name = r.Name ?? $"Room {r.RoomNumber}",
                        PricePerNight = (int)r.PriceForNight,
                        Rating = CalculateRating(r.Reviews),
                        FirstImageUrl = GetFirstImageUrlWithFallback(r.RoomImages, r.Id),
                        BookingStatus = activeBooking?.PaymentStatus
                    };
                })
                .ToList();
        }

        public async Task<EditRoomViewModel> GetRoomForEditAsync(int id)
        {
            var room = await _roomRepository.GetByIdWithAllFeaturesAsync(id);
            if (room == null) throw new Exception($"Room with id {id} not found.");

            var allRoomTypes = await _roomTypesRepo.GetAllAsync();
            var allAmenities = await _amenityService.GetAllAsync();

            return new EditRoomViewModel
            {
                Id = room.Id,
                Name = room.Name,
                RoomTypeId = room.RoomTypeId,
                MaxGuests = room.MaxGuests,
                PriceForNight = room.PriceForNight,
                Area = (double)room.Area,
                NoOfBeds = room.NoOfBeds,
                Description = room.Description,
                IsAvailable = room.IsAvailable,
                HasDiscount = room.HasDiscount,
                DiscountPercentage = room.DiscountPercent,
                RoomNumber = room.RoomNumber,
                SelectedAmenityIds = room.RoomAmenities?.Select(ra => ra.AmenityId).ToList() ?? new List<int>(),
                ExistingRoomImages = room.RoomImages ?? new List<RoomImage>(),
                RoomTypes = allRoomTypes.Select(rt => new SelectListItem { Text = rt.Name, Value = rt.Id.ToString() }).ToList(),
                Amenities = allAmenities.Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString() }).ToList()
            };
        }


        public async Task<IEnumerable<Room>> GetAllForAdminAsync()
        {
            return await _roomRepository.GetAllIncludingAsync(r => r.RoomType, r => r.RoomImages);
        }

        // Debug method to check room images
        public async Task<object> GetRoomImagesDebugInfoAsync()
        {
            var rooms = await _roomRepository.GetAllWithReviewsAsync();
            return rooms.Select(r => new 
            {
                RoomId = r.Id,
                RoomNumber = r.RoomNumber,
                RoomName = r.Name,
                ImageCount = r.RoomImages?.Count ?? 0,
                Images = r.RoomImages?.Select(img => img.ImageUrl).ToList() ?? new List<string>(),
                FirstImageUrl = r.RoomImages?.FirstOrDefault()?.ImageUrl ?? "No images"
            }).ToList();
        }

        // Some helper Methods
        private decimal CalculateFinalPrice(decimal price, bool hasDiscount, int? discountPercent)
        {
            if (!hasDiscount || discountPercent == null) return price;
            return price - (price * discountPercent.Value / 100);
        }

        private int CalculateRating(IEnumerable<Review>? reviews)
        {
            if (reviews == null || !reviews.Any()) return 0;
            return (int)reviews.Average(r => r.Rating);
        }

        private async Task<IEnumerable<Room>> FilterAvailableRoomsAsync(IEnumerable<Room> rooms)
        {
            var allBookings = await _bookingRepository.GetAllAsync();
            
            var currentDate = DateTime.Now.Date;
            
            var bookedRoomIds = allBookings
                .Where(b => (b.PaymentStatus == "Pending" || b.PaymentStatus == "Confirmed") 
                            && b.CheckOut.Date > currentDate)
                .Select(b => b.RoomId)
                .Distinct()
                .ToList();

            return rooms.Where(r => !bookedRoomIds.Contains(r.Id));
        }

        private async Task<List<RoomCardViewModel>> MapRoomsToCardsAsync(IEnumerable<Room> rooms)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cardList = new List<RoomCardViewModel>();

            List<int> userFavoriteRoomIds = new List<int>();
            if (userId != null)
            {
                var favorites = await _favoriteService.GetFavoritesForUser(userId);
                userFavoriteRoomIds = favorites.Select(f => f.RoomId).ToList();
            }

            var allBookings = await _bookingRepository.GetAllAsync();
            var currentDate = DateTime.Now.Date;

            foreach (var room in rooms)
            {
                var amenities = await _amenityService.GetAmenitiesForRoomAsync(room.Id);

                var activeBooking = allBookings
                    .Where(b => b.RoomId == room.Id &&
                                b.CheckOut.Date > currentDate &&
                                (b.PaymentStatus == "Pending" || b.PaymentStatus == "Confirmed"))
                    .OrderBy(b => b.CheckIn)
                    .FirstOrDefault();

                string imageUrl = "";
                if (room.RoomImages != null && room.RoomImages.Any())
                {
                    // Use the first available image
                    imageUrl = room.RoomImages.First().ImageUrl;
                }
                
                // If no image or still a placeholder, assign a high-quality fallback based on room type/number
                if (string.IsNullOrEmpty(imageUrl) || imageUrl.Contains("default-room.jpg"))
                {
                    var fallbackImages = new[]
                    {
                        "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800&q=80", // Luxury bedroom
                        "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800&q=80", // Modern luxury room
                        "https://images.unsplash.com/photo-1590381105924-c72589b9ef3f?w=800&q=80", // Hotel suite
                        "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800&q=80", // Executive room
                        "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800&q=80", // Business hotel room
                        "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800&q=80", // Family room
                        "https://images.unsplash.com/photo-1568084680786-a84f91d1153c?w=800&q=80", // Budget room
                        "https://images.unsplash.com/photo-1591088398332-8a7791972843?w=800&q=80"  // Ocean view suite
                    };
                    
                    // Use room ID to get consistent image selection
                    var imageIndex = room.Id % fallbackImages.Length;
                    imageUrl = fallbackImages[imageIndex];
                }

                cardList.Add(new RoomCardViewModel
                {
                    Id = room.Id,
                    Name = room.Name ?? $"Room {room.RoomNumber}",
                    Rating = CalculateRating(room.Reviews),
                    NumberOfReviews = room.Reviews?.Count() ?? 0,
                    Description = room.Description ?? "Comfortable and well-appointed room",
                    NumberOfGusts = room.MaxGuests,
                    Area = (int)room.Area,
                    DiscountPercentage = room.HasDiscount ? room.DiscountPercent : null,
                    PricePerNight = (int)room.PriceForNight,
                    FinalPrice = CalculateFinalPrice(room.PriceForNight, room.HasDiscount, room.DiscountPercent),
                    Amenities = amenities,
                    IsFavorite = userId != null && userFavoriteRoomIds.Contains(room.Id),
                    ImageUrl = imageUrl,
                    BookingStatus = activeBooking?.PaymentStatus
                });
            }
            return cardList;
        }

        private async Task<List<RoomImage>> ProcessRoomImagesAsync(List<IFormFile> files)
        {
            var roomImages = new List<RoomImage>();
            if (files == null || !files.Any()) return roomImages;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "rooms");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    roomImages.Add(new RoomImage { ImageUrl = "/images/rooms/" + fileName });
                }
            }
            return roomImages;
        }

        // Helper method to get room images with fallback
        private IEnumerable<RoomImage> GetRoomImagesWithFallback(ICollection<RoomImage>? roomImages, int roomId)
        {
            if (roomImages != null && roomImages.Any())
            {
                // Check if any images are placeholders and replace them
                var validImages = roomImages.Where(img => !string.IsNullOrEmpty(img.ImageUrl) && 
                                                         !img.ImageUrl.Contains("default-room.jpg")).ToList();
                
                if (validImages.Any())
                {
                    return validImages;
                }
            }

            // Return fallback images
            var fallbackImages = GetFallbackImagesForRoom(roomId);
            return fallbackImages.Select(url => new RoomImage { ImageUrl = url }).ToList();
        }

        // Helper method to get first image URL with fallback
        private string GetFirstImageUrlWithFallback(ICollection<RoomImage>? roomImages, int roomId)
        {
            if (roomImages != null && roomImages.Any())
            {
                var firstValidImage = roomImages.FirstOrDefault(img => !string.IsNullOrEmpty(img.ImageUrl) && 
                                                                      !img.ImageUrl.Contains("default-room.jpg"));
                if (firstValidImage != null)
                {
                    return firstValidImage.ImageUrl;
                }
            }

            // Return fallback image
            var fallbackImages = GetFallbackImagesForRoom(roomId);
            return fallbackImages.First();
        }

        // Helper method to get fallback images for a specific room
        private List<string> GetFallbackImagesForRoom(int roomId)
        {
            var fallbackImages = new[]
            {
                "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800&q=80", // Luxury bedroom
                "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800&q=80", // Modern luxury room
                "https://images.unsplash.com/photo-1590381105924-c72589b9ef3f?w=800&q=80", // Hotel suite
                "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800&q=80", // Executive room
                "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800&q=80", // Business hotel room
                "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800&q=80", // Family room
                "https://images.unsplash.com/photo-1568084680786-a84f91d1153c?w=800&q=80", // Budget room
                "https://images.unsplash.com/photo-1591088398332-8a7791972843?w=800&q=80"  // Ocean view suite
            };
            
            // Use room ID to get consistent image selection
            var imageIndex = roomId % fallbackImages.Length;
            return new List<string> { fallbackImages[imageIndex] };
        }
    }
}