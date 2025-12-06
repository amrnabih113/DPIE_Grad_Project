namespace Bookify.Data
{
    public class IdentitySeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "Admin", "User" };
            foreach(var rolename in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(rolename);
                if (!roleExists)
                    await roleManager.CreateAsync(new IdentityRole(rolename));
            }
        }

        public static async Task SeedAdminAccount(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var adminEmail = "admin@bookify.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if(adminUser == null)
            {
                var user = new ApplicationUser
                {
                    FullName = adminEmail,
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed =true

                };
                var result = await userManager.CreateAsync(user,"Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }

        public static async Task SeedRoomTypesAsync(ApplicationDbContext context)
        {
            if (!context.RoomTypes.Any())
            {
                var roomTypes = new List<RoomType>
                {
                    new RoomType
                    {
                        Name = "Luxury City View Room",
                        Description = "غرفة فاخرة بإطلالة بانورامية على المدينة، مصممة لتوفير أعلى مستويات الراحة والفخامة."
                    },
                    new RoomType
                    {
                        Name = "Spacious Family Room",
                        Description = "غرفة عائلية واسعة مصممة لتوفير مساحة كافية ومرافق تناسب جميع أفراد العائلة."
                    },
                    new RoomType
                    {
                        Name = "Modern Budget Room",
                        Description = "غرفة اقتصادية وعملية، توفر كل الاحتياجات الأساسية لإقامة مريحة بسعر مناسب."
                    },
                    new RoomType
                    {
                        Name = "Romantic Sea View Suite",
                        Description = "جناح رومانسي فاخر مع شرفة خاصة وإطلالة مباشرة على البحر، مثالي للمناسبات الخاصة."
                    }
                };

                context.RoomTypes.AddRange(roomTypes);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedAmenitiesAsync(ApplicationDbContext context)
        {
            if (!context.Amenities.Any())
            {
                var amenities = new List<Amenity>
                {
                    new Amenity { Name = "Wi-Fi فائق السرعة" },
                    new Amenity { Name = "تكييف هواء بتحكم فردي" },
                    new Amenity { Name = "تلفزيون ذكي بشاشة مسطحة" },
                    new Amenity { Name = "ميني بار مجهز" },
                    new Amenity { Name = "ماكينة صنع القهوة" },
                    new Amenity { Name = "حمام رخامي فاخر" },
                    new Amenity { Name = "خدمة غرف 24/7" },
                    new Amenity { Name = "خزنة إلكترونية" },
                    new Amenity { Name = "ستائر معتمة" },
                    new Amenity { Name = "سريرين مزدوجين" },
                    new Amenity { Name = "أريكة تتحول لسرير" },
                    new Amenity { Name = "ثلاجة صغيرة" },
                    new Amenity { Name = "حمام خاص بدش" },
                    new Amenity { Name = "مكتب عمل" },
                    new Amenity { Name = "شرفة خاصة" },
                    new Amenity { Name = "جاكوزي" },
                    new Amenity { Name = "إطلالة على البحر" }
                };

                context.Amenities.AddRange(amenities);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedRoomsAsync(ApplicationDbContext context)
        {
            if (!context.Rooms.Any())
            {
                // Get room types
                var luxuryType = await context.RoomTypes.FirstAsync(rt => rt.Name == "Luxury City View Room");
                var familyType = await context.RoomTypes.FirstAsync(rt => rt.Name == "Spacious Family Room");
                var budgetType = await context.RoomTypes.FirstAsync(rt => rt.Name == "Modern Budget Room");
                var romanticType = await context.RoomTypes.FirstAsync(rt => rt.Name == "Romantic Sea View Suite");

                var rooms = new List<Room>
                {
                    new Room
                    {
                        Name = "Premium City View",
                        RoomNumber = 2501,
                        RoomTypeId = luxuryType.Id,
                        FloorNumber = 25,
                        PriceForNight = 4500.00m,
                        MaxGuests = 2,
                        Area = 40.0m,
                        NoOfBeds = 1,
                        Description = "غرفة عصرية تقع في طابق مرتفع، مصممة بعناية لتوفير إحساس بالراحة والسكينة. تتميز بنافذة بانورامية تمتد من الأرض إلى السقف، تكشف عن منظر ليلي ساحر لأضواء المدينة المتلألئة. مثالية للاسترخاء بعد يوم حافل.",
                        IsAvailable = true
                    },
                    new Room
                    {
                        Name = "Executive Suite",
                        RoomNumber = 2502,
                        RoomTypeId = luxuryType.Id,
                        FloorNumber = 25,
                        PriceForNight = 4750.00m,
                        MaxGuests = 3,
                        Area = 45.0m,
                        NoOfBeds = 1,
                        Description = "جناح فسيح يجمع بين الفخامة والعملية، يتميز بأرضيات خشبية دافئة تضفي شعوراً بالراحة. يحتوي على منطقة جلوس منفصلة مع أريكة مريحة ومكتب عمل أنيق، مما يجعله الخيار الأمثل للمسافرين من رجال الأعمال.",
                        IsAvailable = true
                    },
                    new Room
                    {
                        Name = "Corner View Room",
                        RoomNumber = 2601,
                        RoomTypeId = luxuryType.Id,
                        FloorNumber = 26,
                        PriceForNight = 4600.00m,
                        MaxGuests = 2,
                        Area = 42.0m,
                        NoOfBeds = 1,
                        Description = "غرفة زاوية فريدة من نوعها توفر إطلالة مزدوجة لا مثيل لها على أفق المدينة، مما يغمر الغرفة بالضوء الطبيعي. تصميمها البسيط والراقي يركز على المساحة والإضاءة لتوفير تجربة إقامة هادئة ومميزة.",
                        IsAvailable = true
                    },
                    new Room
                    {
                        Name = "Royal Luxury Suite",
                        RoomNumber = 2602,
                        RoomTypeId = luxuryType.Id,
                        FloorNumber = 26,
                        PriceForNight = 4800.00m,
                        MaxGuests = 2,
                        Area = 43.0m,
                        NoOfBeds = 1,
                        Description = "تصميم داخلي جريء وفاخر، يغلب عليه استخدام الألوان الداكنة والراقية مثل الأزرق الملكي والرمادي الفحمي، مع لمسات ذهبية براقة تضيف إحساساً بالترف. اللوح الأمامي للسرير مصنوع من المخمل الفاخر لإقامة ملكية.",
                        IsAvailable = true
                    },
                    new Room
                    {
                        Name = "Sky High Penthouse",
                        RoomNumber = 2701,
                        RoomTypeId = luxuryType.Id,
                        FloorNumber = 27,
                        PriceForNight = 5000.00m,
                        MaxGuests = 2,
                        Area = 45.0m,
                        NoOfBeds = 1,
                        Description = "تجربة إقامة فريدة في طابق شاهق يجعلك تشعر وكأنك تطفو فوق المدينة. تتميز الغرفة بتصميم مستقبلي مع إضاءة LED مدمجة ونظام تحكم ذكي، مع الحفاظ على التركيز الكامل على الإطلالة البانورامية المذهلة.",
                        IsAvailable = true
                    },
                    new Room
                    {
                        Name = "Spacious Family Haven",
                        RoomNumber = 1205,
                        RoomTypeId = familyType.Id,
                        FloorNumber = 12,
                        PriceForNight = 2800.00m,
                        MaxGuests = 4,
                        Area = 55.0m,
                        NoOfBeds = 2,
                        Description = "غرفة عائلية واسعة ومشرقة، مصممة خصيصاً لتلبية احتياجات العائلات. تحتوي على سريرين بحجم كوين لضمان نوم مريح للجميع، مع مساحة كافية للعب الأطفال والحركة بحرية. ديكورها بسيط وعملي ومبهج.",
                        IsAvailable = true
                    },
                    new Room
                    {
                        Name = "Family Suite Plus",
                        RoomNumber = 1206,
                        RoomTypeId = familyType.Id,
                        FloorNumber = 12,
                        PriceForNight = 3200.00m,
                        MaxGuests = 5,
                        Area = 60.0m,
                        NoOfBeds = 2,
                        Description = "جناح عائلي مقسم بذكاء ليوفر الخصوصية والراحة. يضم منطقة نوم رئيسية للوالدين، ومنطقة معيشة منفصلة تحتوي على أريكة يمكن تحويلها بسهولة إلى سرير إضافي، مما يجعله مثالياً للعائلات الكبيرة.",
                        IsAvailable = true
                    },
                    new Room
                    {
                        Name = "Kids Paradise Room",
                        RoomNumber = 1401,
                        RoomTypeId = familyType.Id,
                        FloorNumber = 14,
                        PriceForNight = 2900.00m,
                        MaxGuests = 4,
                        Area = 56.0m,
                        NoOfBeds = 2,
                        Description = "غرفة مصممة لتكون جنة للأطفال، حيث تتميز بديكور عصري وألوان زاهية وممتعة. الأثاث متين وآمن، وهناك مساحة مخصصة للعب، مما يضمن إقامة سعيدة ومريحة لجميع أفراد العائلة.",
                        IsAvailable = true
                    },
                    new Room
                    {
                        Name = "Cozy Family Retreat",
                        RoomNumber = 1402,
                        RoomTypeId = familyType.Id,
                        FloorNumber = 14,
                        PriceForNight = 3000.00m,
                        MaxGuests = 4,
                        Area = 58.0m,
                        NoOfBeds = 2,
                        Description = "غرفة عائلية دافئة بأرضيات خشبية وسجاد ناعم، مصممة لتعطي شعوراً بالراحة المنزلية. مثالية للعائلات التي تخطط لإقامة طويلة، حيث توفر كل وسائل الراحة اللازمة لخلق ذكريات جميلة.",
                        IsAvailable = true
                    },
                    new Room
                    {
                        Name = "Grand Family Suite",
                        RoomNumber = 1501,
                        RoomTypeId = familyType.Id,
                        FloorNumber = 15,
                        PriceForNight = 3500.00m,
                        MaxGuests = 6,
                        Area = 65.0m,
                        NoOfBeds = 3,
                        Description = "جناح عائلي ضخم مصمم للعائلات الكبيرة أو المجموعات. يحتوي على أسرّة متعددة وأريكة كبيرة، مع مساحات تخزين واسعة، لضمان أن كل فرد لديه مساحته الخاصة ووسائل راحته الكاملة.",
                        IsAvailable = true
                    },
                    new Room
                    {
                        Name = "Smart Budget Room",
                        RoomNumber = 310,
                        RoomTypeId = budgetType.Id,
                        FloorNumber = 3,
                        PriceForNight = 950.00m,
                        MaxGuests = 2,
                        Area = 22.0m,
                        NoOfBeds = 1,
                        Description = "غرفة مدمجة تتميز بتصميم ذكي لاستغلال كل شبر من المساحة بكفاءة. تحتوي على سرير مزدوج مريح ومكتب عمل مثبت على الحائط، مما يجعلها الخيار الأمثل للمسافر العملي الذي يبحث عن إقامة نظيفة ومريحة.",
                        IsAvailable = true
                    },
                    new Room
                    {
                        Name = "Essential Comfort",
                        RoomNumber = 311,
                        RoomTypeId = budgetType.Id,
                        FloorNumber = 3,
                        PriceForNight = 900.00m,
                        MaxGuests = 2,
                        Area = 20.0m,
                        NoOfBeds = 1,
                        Description = "غرفة اقتصادية أنيقة تتميز ببساطتها الشديدة وتركيزها على الأساسيات. لا يوجد بها أي أثاث غير ضروري، مما يخلق إحساساً بالاتساع والنظافة. مثالية لمن يحتاج إلى مكان هادئ ومريح للراحة والنوم.",
                        IsAvailable = true
                    },
                    new Room
                    {
                        Name = "Artistic Budget Room",
                        RoomNumber = 405,
                        RoomTypeId = budgetType.Id,
                        FloorNumber = 4,
                        PriceForNight = 920.00m,
                        MaxGuests = 2,
                        Area = 21.0m,
                        NoOfBeds = 1,
                        Description = "غرفة اقتصادية لا تتخلى عن الأناقة، حيث تضيف لمسة فنية بسيطة، مثل لوحة جدارية ملونة أو قطعة أثاث مميزة، طابعاً خاصاً على المكان. توفر كل وسائل الراحة الأساسية في بيئة نظيفة ومرحبة.",
                        IsAvailable = true
                    },
                    new Room
                    {
                        Name = "Twin Friends Room",
                        RoomNumber = 406,
                        RoomTypeId = budgetType.Id,
                        FloorNumber = 4,
                        PriceForNight = 1000.00m,
                        MaxGuests = 2,
                        Area = 24.0m,
                        NoOfBeds = 2,
                        Description = "غرفة عملية جداً ومثالية للأصدقاء المسافرين معاً، حيث تحتوي على سريرين منفصلين مريحين. تصميمها بسيط ومباشر، مع توفير كل الاحتياجات الأساسية لإقامة ممتعة واقتصادية.",
                        IsAvailable = true
                    },
                    new Room
                    {
                        Name = "Solo Traveler Pod",
                        RoomNumber = 501,
                        RoomTypeId = budgetType.Id,
                        FloorNumber = 5,
                        PriceForNight = 850.00m,
                        MaxGuests = 1,
                        Area = 18.0m,
                        NoOfBeds = 1,
                        Description = "غرفة فردية صغيرة مصممة بكفاءة عالية لتناسب المسافر المنفرد. كل شيء في متناول اليد، من السرير المريح إلى إضاءة القراءة. هي الخيار الأفضل للإقامات القصيرة ورحلات العمل السريعة التي تتطلب مكاناً آمناً ونظيفة.",
                        IsAvailable = true
                    },
                    new Room
                    {
                        Name = "Ocean Romance Suite",
                        RoomNumber = 801,
                        RoomTypeId = romanticType.Id,
                        FloorNumber = 8,
                        PriceForNight = 6000.00m,
                        MaxGuests = 2,
                        Area = 75.0m,
                        NoOfBeds = 1,
                        Description = "جناح رومانسي مصمم لخلق ذكريات لا تُنسى. يتميز بسرير كينج سايز فاخر وأبواب زجاجية كبيرة تفتح على شرفة خاصة مفروشة، تطل مباشرة على امتداد البحر الأزرق وتوفر خصوصية تامة.",
                        IsAvailable = true
                    },
                    new Room
                    {
                        Name = "Intimate Sea View",
                        RoomNumber = 802,
                        RoomTypeId = romanticType.Id,
                        FloorNumber = 8,
                        PriceForNight = 5800.00m,
                        MaxGuests = 2,
                        Area = 70.0m,
                        NoOfBeds = 1,
                        Description = "جناح دافئ وحميمي يغلب عليه استخدام الخشب الطبيعي والأقمشة الفاخرة، مما يخلق أجواءً رومانسية هادئة. الشرفة الخاصة به توفر إطلالة ساحرة على حديقة الفندق أو منظر طبيعي خلاب، مثالية لتناول القهوة صباحاً.",
                        IsAvailable = true
                    },
                    new Room
                    {
                        Name = "Royal Honeymoon Suite",
                        RoomNumber = 901,
                        RoomTypeId = romanticType.Id,
                        FloorNumber = 9,
                        PriceForNight = 6200.00m,
                        MaxGuests = 3,
                        Area = 80.0m,
                        NoOfBeds = 1,
                        Description = "جناح واسع بتصميم ملكي فاخر، مع فصل واضح بين غرفة النوم الأنيقة ومنطقة الاستقبال الفاخرة التي تحتوي على أرائك مريحة. شرفته الكبيرة والخاصة هي المكان المثالي للاسترخاء والاستمتاع بالمنظر.",
                        IsAvailable = true
                    },
                    new Room
                    {
                        Name = "Plunge Pool Paradise",
                        RoomNumber = 902,
                        RoomTypeId = romanticType.Id,
                        FloorNumber = 9,
                        PriceForNight = 7000.00m,
                        MaxGuests = 2,
                        Area = 85.0m,
                        NoOfBeds = 1,
                        Description = "تجربة فريدة من نوعها في جناح استثنائي، حيث تؤدي شرفته الخاصة مباشرة إلى مسبح صغير خاص (Plunge Pool). يوفر هذا الجناح أقصى درجات الخصوصية والرفاهية، وهو مثالي للاحتفالات الخاصة.",
                        IsAvailable = true
                    },
                    new Room
                    {
                        Name = "Sunset Penthouse",
                        RoomNumber = 1001,
                        RoomTypeId = romanticType.Id,
                        FloorNumber = 10,
                        PriceForNight = 6500.00m,
                        MaxGuests = 2,
                        Area = 78.0m,
                        NoOfBeds = 1,
                        Description = "جناح يقع في أعلى طابق بالفندق، ويتميز بشرفة بانورامية تعتبر المكان المثالي لمشاهدة غروب الشمس الساحر. ديكوره الداخلي فني ورومانسي، ويحتوي على مدفأة كهربائية أنيقة لإضفاء جو من الدفء والحميمية.",
                        IsAvailable = true
                    }
                };

                context.Rooms.AddRange(rooms);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedRoomAmenitiesAsync(ApplicationDbContext context)
        {
            if (!context.RoomAmenities.Any())
            {
                // Get all rooms and amenities from database
                var rooms = await context.Rooms.ToListAsync();
                var amenities = await context.Amenities.ToListAsync();
                
                if (!rooms.Any() || !amenities.Any()) return;

                var roomAmenities = new List<RoomAmenity>();

                // Define amenity sets for different room types
                var basicAmenities = new[] { "Wi-Fi فائق السرعة", "تكييف هواء بتحكم فردي", "تلفزيون ذكي بشاشة مسطحة", "حمام خاص بدش" };
                var standardAmenities = new[] { "Wi-Fi فائق السرعة", "تكييف هواء بتحكم فردي", "تلفزيون ذكي بشاشة مسطحة", "ثلاجة صغيرة", "حمام خاص بدش", "مكتب عمل" };
                var premiumAmenities = new[] { "Wi-Fi فائق السرعة", "تكييف هواء بتحكم فردي", "تلفزيون ذكي بشاشة مسطحة", "ميني بار مجهز", "ماكينة صنع القهوة", "حمام رخامي فاخر", "خزنة إلكترونية", "ستائر معتمة", "ثلاجة صغيرة", "حمام خاص بدش", "مكتب عمل" };
                var luxuryAmenities = new[] { "Wi-Fi فائق السرعة", "تكييف هواء بتحكم فردي", "تلفزيون ذكي بشاشة مسطحة", "ميني بار مجهز", "ماكينة صنع القهوة", "حمام رخامي فاخر", "خدمة غرف 24/7", "خزنة إلكترونية", "ستائر معتمة", "ثلاجة صغيرة", "حمام خاص بدش", "مكتب عمل", "شرفة خاصة" };
                var familyAmenities = new[] { "Wi-Fi فائق السرعة", "تكييف هواء بتحكم فردي", "تلفزيون ذكي بشاشة مسطحة", "سريرين مزدوجين", "أريكة تتحول لسرير", "ثلاجة صغيرة", "حمام خاص بدش" };
                var suiteAmenities = new[] { "Wi-Fi فائق السرعة", "تكييف هواء بتحكم فردي", "تلفزيون ذكي بشاشة مسطحة", "ميني بار مجهز", "ماكينة صنع القهوة", "حمام رخامي فاخر", "خدمة غرف 24/7", "خزنة إلكترونية", "ستائر معتمة", "ثلاجة صغيرة", "حمام خاص بدش", "مكتب عمل", "شرفة خاصة", "جاكوزي", "إطلالة على البحر" };

                // Luxury City View Rooms (rooms with RoomTypeId = 1)
                var luxuryRooms = rooms.Where(r => r.RoomNumber >= 2500 && r.RoomNumber <= 2799).ToList();
                foreach (var room in luxuryRooms)
                {
                    foreach (var amenityName in luxuryAmenities)
                    {
                        var amenity = amenities.FirstOrDefault(a => a.Name == amenityName);
                        if (amenity != null)
                        {
                            roomAmenities.Add(new RoomAmenity { RoomId = room.Id, AmenityId = amenity.Id });
                        }
                    }
                }

                // Family Rooms (rooms with RoomNumber 1200s, 1400s, 1500s)
                var familyRooms = rooms.Where(r => (r.RoomNumber >= 1200 && r.RoomNumber <= 1299) || 
                                                  (r.RoomNumber >= 1400 && r.RoomNumber <= 1499) || 
                                                  (r.RoomNumber >= 1500 && r.RoomNumber <= 1599)).ToList();
                foreach (var room in familyRooms)
                {
                    foreach (var amenityName in familyAmenities)
                    {
                        var amenity = amenities.FirstOrDefault(a => a.Name == amenityName);
                        if (amenity != null)
                        {
                            roomAmenities.Add(new RoomAmenity { RoomId = room.Id, AmenityId = amenity.Id });
                        }
                    }
                    // Add room service for families
                    var roomServiceAmenity = amenities.FirstOrDefault(a => a.Name == "خدمة غرف 24/7");
                    if (roomServiceAmenity != null)
                    {
                        roomAmenities.Add(new RoomAmenity { RoomId = room.Id, AmenityId = roomServiceAmenity.Id });
                    }
                }

                // Budget Rooms (rooms with RoomNumber 300s, 400s, 500s)
                var budgetRooms = rooms.Where(r => (r.RoomNumber >= 300 && r.RoomNumber <= 399) || 
                                                  (r.RoomNumber >= 400 && r.RoomNumber <= 499) || 
                                                  (r.RoomNumber >= 500 && r.RoomNumber <= 599)).ToList();
                foreach (var room in budgetRooms)
                {
                    foreach (var amenityName in standardAmenities)
                    {
                        var amenity = amenities.FirstOrDefault(a => a.Name == amenityName);
                        if (amenity != null)
                        {
                            roomAmenities.Add(new RoomAmenity { RoomId = room.Id, AmenityId = amenity.Id });
                        }
                    }
                }

                // Romantic Sea View Suites (rooms with RoomNumber 800s, 900s, 1000s)
                var romanticRooms = rooms.Where(r => (r.RoomNumber >= 800 && r.RoomNumber <= 899) || 
                                                    (r.RoomNumber >= 900 && r.RoomNumber <= 999) || 
                                                    (r.RoomNumber >= 1000 && r.RoomNumber <= 1099)).ToList();
                foreach (var room in romanticRooms)
                {
                    foreach (var amenityName in suiteAmenities)
                    {
                        var amenity = amenities.FirstOrDefault(a => a.Name == amenityName);
                        if (amenity != null)
                        {
                            roomAmenities.Add(new RoomAmenity { RoomId = room.Id, AmenityId = amenity.Id });
                        }
                    }
                }

                context.RoomAmenities.AddRange(roomAmenities);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedRoomImagesAsync(ApplicationDbContext context)
        {
            if (!context.RoomImages.Any())
            {
                // Get all rooms from database ordered by room number
                var rooms = await context.Rooms.OrderBy(r => r.RoomNumber).ToListAsync();
                if (!rooms.Any()) return;

                var roomImages = new List<RoomImage>();
                
                // High-quality hotel room images from Unsplash
                var hotelImages = new[]
                {
                    // Luxury room images
                    "https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800&q=80", // Luxury bedroom
                    "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800&q=80", // Modern luxury room
                    "https://images.unsplash.com/photo-1590381105924-c72589b9ef3f?w=800&q=80", // Hotel suite
                    
                    // Executive/Business rooms
                    "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800&q=80", // Executive room
                    "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800&q=80", // Business hotel room
                    
                    // Family room images  
                    "https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800&q=80", // Family room
                    "https://images.unsplash.com/photo-1629140727571-9b5c6f6267b4?w=800&q=80", // Spacious family room
                    "https://images.unsplash.com/photo-1586611292717-f828b167408c?w=800&q=80", // Kids-friendly room
                    "https://images.unsplash.com/photo-1584132967334-10e028bd69f7?w=800&q=80", // Cozy family room
                    "https://images.unsplash.com/photo-1578683010236-d716f9a3f461?w=800&q=80", // Grand family suite
                    
                    // Budget room images
                    "https://images.unsplash.com/photo-1568084680786-a84f91d1153c?w=800&q=80", // Simple budget room
                    "https://images.unsplash.com/photo-1522771739844-6a9f6d5f14af?w=800&q=80", // Essential comfort room
                    "https://images.unsplash.com/photo-1586772002994-1c26ab6d7ac7?w=800&q=80", // Modern budget room
                    "https://images.unsplash.com/photo-1587985064135-0366536eab42?w=800&q=80", // Twin beds room
                    "https://images.unsplash.com/photo-1580977050765-5030877c1b45?w=800&q=80", // Solo traveler room
                    
                    // Romantic/Sea view suites
                    "https://images.unsplash.com/photo-1591088398332-8a7791972843?w=800&q=80", // Ocean view suite
                    "https://images.unsplash.com/photo-1567767292278-a4f21aa2d36e?w=800&q=80", // Romantic bedroom
                    "https://images.unsplash.com/photo-1587985064078-4b1850949efa?w=800&q=80", // Honeymoon suite
                    "https://images.unsplash.com/photo-1598300042247-d088f8ab3a91?w=800&q=80", // Suite with pool
                    "https://images.unsplash.com/photo-1571003123894-1f0594d2b5d9?w=800&q=80"  // Penthouse suite
                };

                // Secondary images for variety
                var secondaryImages = new[]
                {
                    "https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=800&q=80", // Bathroom
                    "https://images.unsplash.com/photo-1556020685-ae41abfc9365?w=800&q=80", // Hotel bathroom
                    "https://images.unsplash.com/photo-1584132915807-fd1f5fbc078f?w=800&q=80", // Room view
                    "https://images.unsplash.com/photo-1583847268964-b28dc8f51f92?w=800&q=80", // City view
                    "https://images.unsplash.com/photo-1541971875076-8f970d573be6?w=800&q=80", // Balcony view
                    "https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800&q=80", // Interior
                    "https://images.unsplash.com/photo-1595576508898-0ad5c879a061?w=800&q=80", // Amenities
                    "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800&q=80", // Workspace
                    "https://images.unsplash.com/photo-1590490360182-c33d57733427?w=800&q=80", // Ocean balcony
                    "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800&q=80"  // Room detail
                };

                // Assign 2-3 images per room
                for (int i = 0; i < rooms.Count; i++)
                {
                    var room = rooms[i];
                    
                    // Primary image from room-specific set
                    var primaryImageIndex = i % hotelImages.Length;
                    roomImages.Add(new RoomImage 
                    { 
                        RoomId = room.Id, 
                        ImageUrl = hotelImages[primaryImageIndex] 
                    });

                    // Secondary image for variety
                    var secondaryImageIndex = (i * 2) % secondaryImages.Length;
                    roomImages.Add(new RoomImage 
                    { 
                        RoomId = room.Id, 
                        ImageUrl = secondaryImages[secondaryImageIndex] 
                    });

                    // Third image for premium rooms (luxury and romantic suites)
                    if (room.PriceForNight > 3000)
                    {
                        var tertiaryImageIndex = (i * 3) % secondaryImages.Length;
                        roomImages.Add(new RoomImage 
                        { 
                            RoomId = room.Id, 
                            ImageUrl = secondaryImages[tertiaryImageIndex] 
                        });
                    }
                }

                context.RoomImages.AddRange(roomImages);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedTestimonialsAsync(ApplicationDbContext context)
        {
            if (!context.Testimonials.Any())
            {
                var testimonials = new List<Testimonial>
                {
                    new Testimonial
                    {
                        Name = "أحمد محمد",
                        Message = "تجربة رائعة في هذا الفندق! الخدمة كانت ممتازة والغرفة نظيفة ومريحة. بالتأكيد سأعود مرة أخرى.",
                        Country = "مصر",
                        Rating = 5,
                        Date = DateTime.UtcNow.AddDays(-15)
                    },
                    new Testimonial
                    {
                        Name = "Sarah Johnson",
                        Message = "Amazing hotel with fantastic service. The room was spacious and the view was breathtaking. Highly recommended!",
                        Country = "United States",
                        Rating = 5,
                        Date = DateTime.UtcNow.AddDays(-22)
                    },
                    new Testimonial
                    {
                        Name = "فاطمة العلي",
                        Message = "إقامة هادئة ومريحة، الطاقم ودود ومتعاون. الإفطار كان لذيذاً والموقع ممتاز.",
                        Country = "الإمارات العربية المتحدة",
                        Rating = 4,
                        Date = DateTime.UtcNow.AddDays(-8)
                    },
                    new Testimonial
                    {
                        Name = "Marco Rossi",
                        Message = "Beautiful hotel with excellent facilities. The staff was very helpful and the room was comfortable. Great experience!",
                        Country = "Italy",
                        Rating = 5,
                        Date = DateTime.UtcNow.AddDays(-30)
                    },
                    new Testimonial
                    {
                        Name = "خالد الحسن",
                        Message = "فندق رائع مع مرافق ممتازة. الغرفة كانت واسعة والإطلالة خلابة. خدمة العملاء في المستوى المطلوب.",
                        Country = "السعودية",
                        Rating = 4,
                        Date = DateTime.UtcNow.AddDays(-12)
                    },
                    new Testimonial
                    {
                        Name = "Emma Wilson",
                        Message = "Perfect location and wonderful amenities. The room service was excellent and the hotel exceeded our expectations.",
                        Country = "United Kingdom",
                        Rating = 5,
                        Date = DateTime.UtcNow.AddDays(-5)
                    },
                    new Testimonial
                    {
                        Name = "نور الدين قاسم",
                        Message = "تجربة إقامة مميزة! الفندق نظيف والموظفون محترفون. الغرفة العائلية كانت مثالية لعائلتي.",
                        Country = "الأردن",
                        Rating = 5,
                        Date = DateTime.UtcNow.AddDays(-18)
                    },
                    new Testimonial
                    {
                        Name = "Hans Mueller",
                        Message = "Sehr gutes Hotel mit ausgezeichnetem Service. Das Zimmer war sauber und komfortabel. Ich kann es nur empfehlen!",
                        Country = "Germany",
                        Rating = 4,
                        Date = DateTime.UtcNow.AddDays(-25)
                    }
                };

                context.Testimonials.AddRange(testimonials);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedBookingsAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (!context.Bookings.Any())
            {
                // Get demo users
                var users = await Task.WhenAll(
                    userManager.FindByEmailAsync("john.doe@example.com"),
                    userManager.FindByEmailAsync("jane.smith@example.com"),
                    userManager.FindByEmailAsync("ahmed.hassan@example.com"),
                    userManager.FindByEmailAsync("sara.mohamed@example.com"),
                    userManager.FindByEmailAsync("michael.brown@example.com")
                );

                var validUsers = users.Where(u => u != null).ToList();
                if (!validUsers.Any()) return;

                var bookings = new List<Booking>
                {
                    // Recent completed bookings
                    new Booking
                    {
                        UserId = validUsers[0].Id,
                        RoomId = 1,
                        CheckIn = DateTime.UtcNow.AddDays(-30),
                        CheckOut = DateTime.UtcNow.AddDays(-27),
                        NumberOfGuests = 2,
                        TotalPrice = 13500.00m, // 3 nights * 4500
                        PaymentStatus = "Completed",
                        PaymentMethod = "Credit Card",
                        PaymentDate = DateTime.UtcNow.AddDays(-35),
                        CreatedAt = DateTime.UtcNow.AddDays(-35)
                    },
                    new Booking
                    {
                        UserId = validUsers[1].Id,
                        RoomId = 6,
                        CheckIn = DateTime.UtcNow.AddDays(-20),
                        CheckOut = DateTime.UtcNow.AddDays(-18),
                        NumberOfGuests = 4,
                        TotalPrice = 5600.00m, // 2 nights * 2800
                        PaymentStatus = "Completed",
                        PaymentMethod = "PayPal",
                        PaymentDate = DateTime.UtcNow.AddDays(-25),
                        CreatedAt = DateTime.UtcNow.AddDays(-25)
                    },
                    new Booking
                    {
                        UserId = validUsers[2].Id,
                        RoomId = 16,
                        CheckIn = DateTime.UtcNow.AddDays(-15),
                        CheckOut = DateTime.UtcNow.AddDays(-12),
                        NumberOfGuests = 2,
                        TotalPrice = 18000.00m, // 3 nights * 6000
                        PaymentStatus = "Completed",
                        PaymentMethod = "Credit Card",
                        PaymentDate = DateTime.UtcNow.AddDays(-20),
                        CreatedAt = DateTime.UtcNow.AddDays(-20)
                    },
                    
                    // Current ongoing bookings
                    new Booking
                    {
                        UserId = validUsers[3].Id,
                        RoomId = 12,
                        CheckIn = DateTime.UtcNow.AddDays(-2),
                        CheckOut = DateTime.UtcNow.AddDays(3),
                        NumberOfGuests = 2,
                        TotalPrice = 4500.00m, // 5 nights * 900
                        PaymentStatus = "Completed",
                        PaymentMethod = "Credit Card",
                        PaymentDate = DateTime.UtcNow.AddDays(-7),
                        CreatedAt = DateTime.UtcNow.AddDays(-7)
                    },
                    
                    // Future bookings
                    new Booking
                    {
                        UserId = validUsers[4].Id,
                        RoomId = 20,
                        CheckIn = DateTime.UtcNow.AddDays(15),
                        CheckOut = DateTime.UtcNow.AddDays(19),
                        NumberOfGuests = 2,
                        TotalPrice = 26000.00m, // 4 nights * 6500
                        PaymentStatus = "Pending",
                        PaymentMethod = "Credit Card",
                        PaymentDate = null,
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    },
                    new Booking
                    {
                        UserId = validUsers[0].Id,
                        RoomId = 8,
                        CheckIn = DateTime.UtcNow.AddDays(30),
                        CheckOut = DateTime.UtcNow.AddDays(35),
                        NumberOfGuests = 4,
                        TotalPrice = 14500.00m, // 5 nights * 2900
                        PaymentStatus = "Confirmed",
                        PaymentMethod = "Bank Transfer",
                        PaymentDate = DateTime.UtcNow.AddDays(-1),
                        CreatedAt = DateTime.UtcNow.AddDays(-3)
                    },
                    new Booking
                    {
                        UserId = validUsers[2].Id,
                        RoomId = 3,
                        CheckIn = DateTime.UtcNow.AddDays(45),
                        CheckOut = DateTime.UtcNow.AddDays(48),
                        NumberOfGuests = 2,
                        TotalPrice = 13800.00m, // 3 nights * 4600
                        PaymentStatus = "Pending",
                        PaymentMethod = "Credit Card",
                        PaymentDate = null,
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    }
                };

                context.Bookings.AddRange(bookings);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedReviewsAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (!context.Reviews.Any())
            {
                // Get demo users
                var users = await Task.WhenAll(
                    userManager.FindByEmailAsync("john.doe@example.com"),
                    userManager.FindByEmailAsync("jane.smith@example.com"),
                    userManager.FindByEmailAsync("ahmed.hassan@example.com"),
                    userManager.FindByEmailAsync("sara.mohamed@example.com"),
                    userManager.FindByEmailAsync("michael.brown@example.com"),
                    userManager.FindByEmailAsync("fatima.ali@example.com")
                );

                var validUsers = users.Where(u => u != null).ToList();
                if (!validUsers.Any()) return;

                var reviews = new List<Review>
                {
                    new Review
                    {
                        UserId = validUsers[0].Id,
                        RoomId = 1,
                        Rating = 5,
                        Comment = "Absolutely stunning room with an incredible city view! The service was impeccable and the room was spotlessly clean. Will definitely book again.",
                        CreatedAt = DateTime.UtcNow.AddDays(-25)
                    },
                    new Review
                    {
                        UserId = validUsers[1].Id,
                        RoomId = 6,
                        Rating = 4,
                        Comment = "Perfect for our family vacation. The kids loved the spacious room and the staff was very accommodating. Great value for money.",
                        CreatedAt = DateTime.UtcNow.AddDays(-15)
                    },
                    new Review
                    {
                        UserId = validUsers[2].Id,
                        RoomId = 16,
                        Rating = 5,
                        Comment = "The most romantic getaway we've ever had! The sea view from our private balcony was breathtaking. Perfect for our anniversary.",
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    },
                    new Review
                    {
                        UserId = validUsers[3].Id,
                        RoomId = 12,
                        Rating = 4,
                        Comment = "Clean, comfortable, and affordable. Exactly what I needed for my business trip. The location is convenient and staff is helpful.",
                        CreatedAt = DateTime.UtcNow.AddDays(-5)
                    },
                    new Review
                    {
                        UserId = validUsers[4].Id,
                        RoomId = 2,
                        Rating = 5,
                        Comment = "Luxurious room with beautiful hardwood floors. The separate seating area was perfect for working. Excellent amenities.",
                        CreatedAt = DateTime.UtcNow.AddDays(-20)
                    },
                    new Review
                    {
                        UserId = validUsers[0].Id,
                        RoomId = 8,
                        Rating = 4,
                        Comment = "Great family room with plenty of space for the kids. The colorful decor was a hit with our children. Highly recommend for families.",
                        CreatedAt = DateTime.UtcNow.AddDays(-12)
                    },
                    new Review
                    {
                        UserId = validUsers[5].Id,
                        RoomId = 20,
                        Rating = 5,
                        Comment = "The penthouse suite exceeded all expectations! The panoramic sunset view was magical. Worth every penny for our honeymoon.",
                        CreatedAt = DateTime.UtcNow.AddDays(-8)
                    },
                    new Review
                    {
                        UserId = validUsers[2].Id,
                        RoomId = 3,
                        Rating = 4,
                        Comment = "Corner room with amazing dual city views. The natural light throughout the day was wonderful. Great for photography enthusiasts.",
                        CreatedAt = DateTime.UtcNow.AddDays(-18)
                    },
                    new Review
                    {
                        UserId = validUsers[1].Id,
                        RoomId = 14,
                        Rating = 3,
                        Comment = "Good budget option for friends traveling together. Two separate beds was convenient. Room could use some updates but clean overall.",
                        CreatedAt = DateTime.UtcNow.AddDays(-22)
                    },
                    new Review
                    {
                        UserId = validUsers[4].Id,
                        RoomId = 19,
                        Rating = 5,
                        Comment = "The private plunge pool was incredible! Ultimate luxury and privacy. The staff attention to detail was remarkable.",
                        CreatedAt = DateTime.UtcNow.AddDays(-30)
                    }
                };

                context.Reviews.AddRange(reviews);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedFavoritesAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (!context.Favorites.Any())
            {
                // Get demo users
                var users = await Task.WhenAll(
                    userManager.FindByEmailAsync("john.doe@example.com"),
                    userManager.FindByEmailAsync("jane.smith@example.com"),
                    userManager.FindByEmailAsync("ahmed.hassan@example.com"),
                    userManager.FindByEmailAsync("sara.mohamed@example.com"),
                    userManager.FindByEmailAsync("michael.brown@example.com"),
                    userManager.FindByEmailAsync("fatima.ali@example.com")
                );

                var validUsers = users.Where(u => u != null).ToList();
                if (!validUsers.Any()) return;

                var favorites = new List<Favorite>
                {
                    // John likes luxury rooms
                    new Favorite { UserId = validUsers[0].Id, RoomId = 1 },
                    new Favorite { UserId = validUsers[0].Id, RoomId = 5 },
                    new Favorite { UserId = validUsers[0].Id, RoomId = 20 },
                    
                    // Jane prefers family rooms
                    new Favorite { UserId = validUsers[1].Id, RoomId = 6 },
                    new Favorite { UserId = validUsers[1].Id, RoomId = 7 },
                    new Favorite { UserId = validUsers[1].Id, RoomId = 10 },
                    
                    // Ahmed likes romantic suites
                    new Favorite { UserId = validUsers[2].Id, RoomId = 16 },
                    new Favorite { UserId = validUsers[2].Id, RoomId = 17 },
                    new Favorite { UserId = validUsers[2].Id, RoomId = 19 },
                    
                    // Sara is budget-conscious
                    new Favorite { UserId = validUsers[3].Id, RoomId = 11 },
                    new Favorite { UserId = validUsers[3].Id, RoomId = 12 },
                    new Favorite { UserId = validUsers[3].Id, RoomId = 15 },
                    
                    // Michael likes variety
                    new Favorite { UserId = validUsers[4].Id, RoomId = 2 },
                    new Favorite { UserId = validUsers[4].Id, RoomId = 8 },
                    new Favorite { UserId = validUsers[4].Id, RoomId = 18 },
                    
                    // Fatima prefers luxury
                    new Favorite { UserId = validUsers[5].Id, RoomId = 4 },
                    new Favorite { UserId = validUsers[5].Id, RoomId = 19 },
                    new Favorite { UserId = validUsers[5].Id, RoomId = 20 }
                };

                context.Favorites.AddRange(favorites);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedAllDataAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Seed in dependency order
            await SeedRoomTypesAsync(context);
            await SeedAmenitiesAsync(context);
            await SeedRoomsAsync(context);
            await SeedRoomAmenitiesAsync(context);
            await SeedRoomImagesAsync(context);
            await SeedTestimonialsAsync(context);
            
            // User-dependent data
            await SeedBookingsAsync(context, userManager);
            await SeedReviewsAsync(context, userManager);
            await SeedFavoritesAsync(context, userManager);
        }

        // Method to force reseed room images with real URLs
        public static async Task ForceReseedRoomImagesAsync(ApplicationDbContext context)
        {
            // Clear all existing room images
            var existingImages = await context.RoomImages.ToListAsync();
            context.RoomImages.RemoveRange(existingImages);
            await context.SaveChangesAsync();
            
            // Reseed with new images
            await SeedRoomImagesAsync(context);
        }
    }
}
