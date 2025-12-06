// Universal Room Image Fix Script
// Run this in browser console on any page (Home, Room Details, Room List)

console.log('?? Universal Room Image Fix Starting...');

// High-quality hotel room fallback images
const fallbackImages = [
    'https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=800&q=80',
    'https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800&q=80',
    'https://images.unsplash.com/photo-1590381105924-c72589b9ef3f?w=800&q=80',
    'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800&q=80',
    'https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800&q=80',
    'https://images.unsplash.com/photo-1551882547-ff40c63fe5fa?w=800&q=80',
    'https://images.unsplash.com/photo-1568084680786-a84f91d1153c?w=800&q=80',
    'https://images.unsplash.com/photo-1591088398332-8a7791972843?w=800&q=80'
];

// Function to fix a single image
function fixImage(img, index) {
    const currentSrc = img.src;
    
    // Check if image needs fixing
    if (currentSrc.includes('default-room.jpg') || 
        currentSrc.includes('placeholder') || 
        currentSrc.includes('via.placeholder.com') ||
        !currentSrc || 
        currentSrc === window.location.origin + '/') {
        
        const newImageUrl = fallbackImages[index % fallbackImages.length];
        console.log(`?? Fixing image ${index + 1}: ${currentSrc} ? ${newImageUrl}`);
        
        img.src = newImageUrl;
        img.style.objectFit = 'cover';
        img.style.width = '100%';
        img.style.display = 'block';
        
        // Add a subtle animation to show the change
        img.style.transition = 'opacity 0.3s ease-in-out';
        img.style.opacity = '0';
        setTimeout(() => {
            img.style.opacity = '1';
        }, 100);
        
        return true;
    }
    return false;
}

// Fix all images on the page
function fixAllImages() {
    console.log('?? Searching for images to fix...');
    
    // Common selectors for room images across different pages
    const selectors = [
        '.room-card-image',        // Room list page
        '.gallery-image',          // Room details page
        '.room-image',             // General room image
        'img[src*="room"]',        // Any image with 'room' in src
        'img[alt*="room" i]',      // Any image with 'room' in alt text
        'img[src*="default"]',     // Default placeholder images
        'img[src*="placeholder"]'  // Placeholder images
    ];
    
    let totalFixed = 0;
    let totalFound = 0;
    
    selectors.forEach(selector => {
        const images = document.querySelectorAll(selector);
        images.forEach((img, index) => {
            totalFound++;
            if (fixImage(img, totalFound - 1)) {
                totalFixed++;
            }
        });
    });
    
    console.log(`? Image fix complete: ${totalFixed} images fixed out of ${totalFound} found`);
    return { totalFound, totalFixed };
}

// Check if admin fix is needed
async function checkAdminFix() {
    try {
        console.log('?? Checking if admin database fix is needed...');
        const response = await fetch('/Room/RefreshRoomImages');
        const data = await response.json();
        
        if (data.success) {
            console.log(`?? Database Status:`);
            console.log(`  - Total Rooms: ${data.totalRooms}`);
            console.log(`  - Rooms with Image Issues: ${data.roomsWithImageIssues}`);
            
            if (data.needsAdminFix) {
                console.log(`??  Database needs fixing! Admin can visit: ${data.adminFixPageUrl}`);
                console.log(`   Or call API directly: ${data.adminFixUrl}`);
            } else {
                console.log(`? Database images look good!`);
            }
        }
    } catch (error) {
        console.log(`??  Could not check admin fix status: ${error.message}`);
    }
}

// Run the fixes
const result = fixAllImages();
checkAdminFix();

// Show completion message
if (result.totalFixed > 0) {
    console.log('?? Room images have been improved!');
    
    // Show a temporary notification
    const notification = document.createElement('div');
    notification.innerHTML = `
        <div style="
            position: fixed; 
            top: 20px; 
            right: 20px; 
            background: linear-gradient(135deg, #10b981 0%, #059669 100%); 
            color: white; 
            padding: 1rem 1.5rem; 
            border-radius: 10px; 
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            z-index: 9999;
            font-family: system-ui, sans-serif;
            font-weight: 600;
        ">
            ? Fixed ${result.totalFixed} room image(s)!
        </div>
    `;
    
    document.body.appendChild(notification);
    
    setTimeout(() => {
        notification.style.transition = 'all 0.3s ease-out';
        notification.style.transform = 'translateX(100%)';
        notification.style.opacity = '0';
        setTimeout(() => notification.remove(), 300);
    }, 3000);
} else {
    console.log('??  No images needed fixing on this page.');
}

console.log('?? Universal Room Image Fix Complete!');
console.log('?? For permanent fix, admin should visit: /Room/FixImagesPage');