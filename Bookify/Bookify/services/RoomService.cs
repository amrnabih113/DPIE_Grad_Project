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

        public RoomService(IRoomRepository roomRepository,
                           IAmenityService amenityService,
                           IHttpContextAccessor httpContextAccessor,
                           IFavoriteService favoriteService,
                           IWebHostEnvironment env,
                           IGenericRepository<RoomType> roomTypesRepo)
        {
            _roomRepository = roomRepository;
            _amenityService = amenityService;
            _httpContextAccessor = httpContextAccessor;
            _favoriteService = favoriteService;
            _env = env;
            _roomTypesRepo = roomTypesRepo;
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

            return new RoomDetailsViewModel
            {
                Id = room.Id,
                Name = room.Name ?? "",
                Description = room.Description ?? "",
                NumberOfGusts = room.MaxGuests,
                Area = (int)room.Area,
                IsFavorite = userId != null && await _favoriteService.IsFavoriteAsync(userId, room.Id),
                PricePerNight = (int)room.PriceForNight,
                DiscountPercentage = room.HasDiscount ? room.DiscountPercent : null,
                FinalPrice = CalculateFinalPrice(room.PriceForNight, room.HasDiscount, room.DiscountPercent),
                Rating = CalculateRating(room.Reviews),
                NumberOfReviews = room.Reviews?.Count ?? 0,
                Amenities = await _amenityService.GetAmenitiesForRoomAsync(room.Id) ?? new List<Amenity>(),
                RoomImages = room.RoomImages ?? new List<RoomImage>()
            };
        }

        public async Task<IEnumerable<TopRoomViewModel>> GetTopRatedRoomsAsync(int top = 3)
        {
            var rooms = await _roomRepository.GetAllWithReviewsAsync();

            return rooms
                .OrderByDescending(r => CalculateRating(r.Reviews))
                .Take(top)
                .Select(r => new TopRoomViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    PricePerNight = (int)r.PriceForNight,
                    Rating = CalculateRating(r.Reviews),
                    FirstImageUrl = r.RoomImages != null && r.RoomImages.Any() ? r.RoomImages.First().ImageUrl : ""
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

            foreach (var room in rooms)
            {
                var amenities = await _amenityService.GetAmenitiesForRoomAsync(room.Id);

                cardList.Add(new RoomCardViewModel
                {
                    Id = room.Id,
                    Name = room.Name,
                    Rating = CalculateRating(room.Reviews),
                    NumberOfReviews = room.Reviews?.Count() ?? 0,
                    Description = room.Description,
                    NumberOfGusts = room.MaxGuests,
                    Area = (int)room.Area,
                    DiscountPercentage = room.HasDiscount ? room.DiscountPercent : null,
                    PricePerNight = (int)room.PriceForNight,
                    FinalPrice = CalculateFinalPrice(room.PriceForNight, room.HasDiscount, room.DiscountPercent),
                    Amenities = amenities,
                    IsFavorite = userId != null && userFavoriteRoomIds.Contains(room.Id),
                    ImageUrl = room.RoomImages != null && room.RoomImages.Any() ? room.RoomImages.First().ImageUrl : "/images/rooms/default-room.jpg"
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
    }
}