// Quick JavaScript fix to test room images loading
// Run this in browser console on the rooms page

console.log('?? Testing room image fixes...');

// Check if we can access the debug endpoint
fetch('/Room/CheckRoomData')
    .then(response => response.json())
    .then(data => {
        console.log('?? Room data:', data);
        
        if (data.success && data.data) {
            console.log(`Found ${data.data.length} rooms`);
            
            // Test loading images for each room
            data.data.forEach((room, index) => {
                console.log(`Room ${index + 1}: ${room.name}`);
                console.log(`  Image URL: ${room.imageUrl}`);
                console.log(`  Has Image: ${room.hasImage}`);
                
                // Test if the image actually loads
                if (room.imageUrl) {
                    const img = new Image();
                    img.onload = () => console.log(`? Image loads: ${room.name}`);
                    img.onerror = () => console.log(`? Image failed: ${room.name} - ${room.imageUrl}`);
                    img.src = room.imageUrl;
                }
            });
        }
    })
    .catch(error => {
        console.error('? Failed to fetch room data:', error);
    });

// Fix images on the current page immediately
document.querySelectorAll('.room-card-image').forEach((img, index) => {
    console.log(`??? Found image ${index + 1}: ${img.src}`);
    
    if (img.src.includes('default-room.jpg')) {
        console.log(`?? Replacing default image ${index + 1}...`);
        
        // Replace with a high-quality hotel room image
        const hotelImages = [
            'https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800&q=80',
            'https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800&q=80',
            'https://images.unsplash.com/photo-1590381105924-c72589b9ef3f?w=800&q=80',
            'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800&q=80',
            'https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800&q=80',
            'https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800&q=80',
            'https://images.unsplash.com/photo-1568084680786-a84f91d1153c?w=800&q=80',
            'https://images.unsplash.com/photo-1591088398332-8a7791972843?w=800&q=80'
        ];
        
        const newImageUrl = hotelImages[index % hotelImages.length];
        img.src = newImageUrl;
        console.log(`? Replaced with: ${newImageUrl}`);
    }
});

console.log('?? Room image fix complete!');

// Instructions for permanent fix
console.log('?? To permanently fix this:');
console.log('1. Run the SQL script: SQL_Scripts/FixRoomImagesComplete.sql');
console.log('2. Or visit /Room/FixRoomImages (admin only) if that endpoint is available');
console.log('3. Or update the IdentitySeeder.cs and run seeding again');