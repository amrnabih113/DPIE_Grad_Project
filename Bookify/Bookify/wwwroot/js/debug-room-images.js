// Debug script to test room images loading
// Add this to the browser console or create a temporary test page

console.log("?? Testing room images...");

// Test the CheckRoomData endpoint
fetch('/Room/CheckRoomData')
    .then(response => response.json())
    .then(data => {
        console.log("? Room data response:", data);
        
        if (data.success) {
            console.log("?? Room data details:");
            data.data.forEach((room, index) => {
                console.log(`?? Room ${index + 1}:`, {
                    ID: room.id,
                    Name: room.name,
                    ImageURL: room.imageUrl,
                    HasImage: room.hasImage,
                    Description: room.description,
                    AmenityCount: room.amenityCount
                });
                
                // Test if image loads
                if (room.imageUrl) {
                    const img = new Image();
                    img.onload = function() {
                        console.log(`? Image loads successfully: ${room.name} (${room.imageUrl})`);
                    };
                    img.onerror = function() {
                        console.log(`? Image failed to load: ${room.name} (${room.imageUrl})`);
                    };
                    img.src = room.imageUrl;
                }
            });
        } else {
            console.error("? Error fetching room data:", data.error);
        }
    })
    .catch(error => {
        console.error("? Network error:", error);
    });

// Test image loading in the current page
console.log("?? Checking images on current page...");
const images = document.querySelectorAll('.room-card-image, .gallery-image');
console.log(`Found ${images.length} room images on page`);

images.forEach((img, index) => {
    console.log(`Image ${index + 1}:`, {
        src: img.src,
        alt: img.alt,
        naturalWidth: img.naturalWidth,
        naturalHeight: img.naturalHeight,
        complete: img.complete
    });
    
    if (!img.complete || img.naturalWidth === 0) {
        console.log(`?? Image may not be loading: ${img.src}`);
    }
});

// Check for CSS issues
const roomCards = document.querySelectorAll('.room-card');
console.log(`Found ${roomCards.length} room cards`);

roomCards.forEach((card, index) => {
    const img = card.querySelector('img');
    if (img) {
        const computedStyle = window.getComputedStyle(img);
        console.log(`Card ${index + 1} image styles:`, {
            display: computedStyle.display,
            visibility: computedStyle.visibility,
            width: computedStyle.width,
            height: computedStyle.height,
            objectFit: computedStyle.objectFit
        });
    }
});

console.log("?? Room image debugging complete!");