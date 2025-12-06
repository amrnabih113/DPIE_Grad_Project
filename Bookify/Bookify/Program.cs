using Bookify.Repository;
using Bookify.services;
using Bookify.services.IServices;
using Microsoft.Net.Http.Headers;

namespace Bookify
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
            });



            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddTransient<IEmailSender, EmailSender>();

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IAmenityService, AmenityService>();

            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            options.TokenLifespan = TimeSpan.FromHours(3));


            builder.Services.AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
                    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
                    googleOptions.CallbackPath = "/signin-google";
                });
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IRoomRepository, RoomRepository>();
            builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();


            builder.Services.AddScoped<IRoomSerivce, RoomService>();
            builder.Services.AddScoped<IAmenityService, AmenityService>();
            builder.Services.AddScoped<IRoomTypeService, RoomTypeService>();
            builder.Services.AddScoped<IFavoriteService, FavoriteService>();

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Ensure database is created and migrated before seeding
            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    
                    // Ensure database is created
                    await context.Database.EnsureCreatedAsync();
                    
                    // Apply any pending migrations
                    await context.Database.MigrateAsync();
                    
                    // Seed identity data
                    await IdentitySeeder.SeedRolesAsync(scope.ServiceProvider);
                    await IdentitySeeder.SeedAdminAccount(scope.ServiceProvider);

                    // Seed all application data
                    await IdentitySeeder.SeedAllDataAsync(scope.ServiceProvider);
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while creating/migrating the database or seeding data.");
                    
                    // Log more specific details about the error
                    if (ex is Microsoft.EntityFrameworkCore.Storage.RetryLimitExceededException retryEx)
                    {
                        logger.LogError("Database retry limit exceeded. Check your database connection string and ensure SQL Server is running. Inner exception: {InnerException}", retryEx.InnerException?.Message);
                    }
                }
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            //cashing
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 60 * 60 * 24 * 7;
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                        "public,max-age=" + durationInSeconds;
                }
            });
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
