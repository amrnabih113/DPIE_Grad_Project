using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoomImagesWithRealUrls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clear existing default room images
            migrationBuilder.Sql("DELETE FROM RoomImages WHERE ImageUrl = '/images/rooms/default-room.jpg'");
            
            // Add real hotel images for each room
            // Luxury rooms (2501-2701)
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 2501
            ");
            
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 2501
            ");

            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 2502
            ");
            
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1556020685-ae41abfc9365?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 2502
            ");

            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1590381105924-c72589b9ef3f?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 2601
            ");
            
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1584132915807-fd1f5fbc078f?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 2601
            ");

            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 2602
            ");
            
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1583847268964-b28dc8f51f92?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 2602
            ");

            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 2701
            ");
            
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1541971875076-8f970d573be6?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 2701
            ");

            // Family rooms (1205, 1206, 1401, 1402, 1501)
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 1205
            ");
            
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1629140727571-9b5c6f6267b4?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 1206
            ");
            
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1586611292717-f828b167408c?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 1401
            ");
            
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1584132967334-10e028bd69f7?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 1402
            ");
            
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1578683010236-d716f9a3f461?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 1501
            ");

            // Budget rooms (310, 311, 405, 406, 501)
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1568084680786-a84f91d1153c?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 310
            ");
            
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1522771739844-6a9f6d5f14af?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 311
            ");
            
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1586772002994-1c26ab6d7ac7?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 405
            ");
            
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1587985064135-0366536eab42?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 406
            ");
            
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1580977050765-5030877c1b45?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 501
            ");

            // Romantic suites (801, 802, 901, 902, 1001)
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1591088398332-8a7791972843?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 801
            ");
            
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1567767292278-a4f21aa2d36e?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 802
            ");
            
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1587985064078-4b1850949efa?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 901
            ");
            
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1598300042247-d088f8ab3a91?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 902
            ");
            
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, 'https://images.unsplash.com/photo-1571003123894-1f0594d2b5d9?w=800&q=80' 
                FROM Rooms WHERE RoomNumber = 1001
            ");

            // Add second images for all rooms
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT r.Id, 
                    CASE 
                        WHEN r.RoomNumber BETWEEN 2500 AND 2799 THEN 'https://images.unsplash.com/photo-1595576508898-0ad5c879a061?w=800&q=80'
                        WHEN r.RoomNumber BETWEEN 1200 AND 1599 THEN 'https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800&q=80'
                        WHEN r.RoomNumber BETWEEN 300 AND 599 THEN 'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800&q=80'
                        WHEN r.RoomNumber BETWEEN 800 AND 1099 THEN 'https://images.unsplash.com/photo-1590490360182-c33d57733427?w=800&q=80'
                        ELSE 'https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800&q=80'
                    END
                FROM Rooms r
                WHERE NOT EXISTS (
                    SELECT 1 FROM RoomImages ri 
                    WHERE ri.RoomId = r.Id 
                    AND ri.ImageUrl LIKE '%unsplash%'
                )
                OR (
                    SELECT COUNT(*) FROM RoomImages ri WHERE ri.RoomId = r.Id
                ) = 1
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove all Unsplash images and restore default
            migrationBuilder.Sql("DELETE FROM RoomImages WHERE ImageUrl LIKE '%unsplash%'");
            
            migrationBuilder.Sql(@"
                INSERT INTO RoomImages (RoomId, ImageUrl) 
                SELECT Id, '/images/rooms/default-room.jpg' 
                FROM Rooms 
                WHERE NOT EXISTS (SELECT 1 FROM RoomImages WHERE RoomId = Rooms.Id)
            ");
        }
    }
}