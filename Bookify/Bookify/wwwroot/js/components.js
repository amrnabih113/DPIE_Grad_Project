// ===================================
// BOOKIFY - Shared Components
// ===================================

// Header Component
function renderHeader() {
    const user = auth.getCurrentUser();
    const currentPage = window.location.pathname.split('/').pop() || 'index.html';

    return `
    <header class="header">
      <div class="container header-content">
        <a href="index.html" class="logo">
          <div class="logo-icon">B</div>
          <span>Bookify</span>
        </a>
        
        <nav class="nav">
          <a href="index.html" class="${currentPage === 'index.html' || currentPage === '' ? 'active' : ''}">Home</a>
          <a href="rooms.html" class="${currentPage === 'rooms.html' ? 'active' : ''}">Rooms</a>
          <a href="about.html" class="${currentPage === 'about.html' ? 'active' : ''}">About</a>
          <a href="contact.html" class="${currentPage === 'contact.html' ? 'active' : ''}">Contact</a>
        </nav>
        
        <div class="header-actions">
          ${user ? `
            <div class="user-menu">
              <div class="user-avatar" onclick="toggleUserDropdown()">
                ${createAvatarInitials(user.firstName, user.lastName)}
              </div>
              <div class="user-dropdown" id="userDropdown">
                <div style="padding: var(--spacing-md); border-bottom: 1px solid var(--color-gray-200);">
                  <div style="font-weight: var(--font-weight-semibold);">${user.firstName} ${user.lastName}</div>
                  <div style="font-size: var(--font-size-sm); color: var(--color-gray-500);">${user.email}</div>
                </div>
                ${user.isAdmin ? `
                  <a href="admin.html" class="user-dropdown-item">
                    <svg width="16" height="16" viewBox="0 0 20 20" fill="currentColor">
                      <path fill-rule="evenodd" d="M2.166 4.999A11.954 11.954 0 0010 1.944 11.954 11.954 0 0017.834 5c.11.65.166 1.32.166 2.001 0 5.225-3.34 9.67-8 11.317C5.34 16.67 2 12.225 2 7c0-.682.057-1.35.166-2.001zm11.541 3.708a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"/>
                    </svg>
                    Admin Dashboard
                  </a>
                  <div class="user-dropdown-separator"></div>
                ` : ''}
                <a href="my-bookings.html" class="user-dropdown-item">
                  <svg width="16" height="16" viewBox="0 0 20 20" fill="currentColor">
                    <path fill-rule="evenodd" d="M6 2a1 1 0 00-1 1v1H4a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V6a2 2 0 00-2-2h-1V3a1 1 0 10-2 0v1H7V3a1 1 0 00-1-1zm0 5a1 1 0 000 2h8a1 1 0 100-2H6z" clip-rule="evenodd"/>
                  </svg>
                  My Bookings
                </a>
                <a href="settings.html" class="user-dropdown-item">
                  <svg width="16" height="16" viewBox="0 0 20 20" fill="currentColor">
                    <path fill-rule="evenodd" d="M11.49 3.17c-.38-1.56-2.6-1.56-2.98 0a1.532 1.532 0 01-2.286.948c-1.372-.836-2.942.734-2.106 2.106.54.886.061 2.042-.947 2.287-1.561.379-1.561 2.6 0 2.978a1.532 1.532 0 01.947 2.287c-.836 1.372.734 2.942 2.106 2.106a1.532 1.532 0 012.287.947c.379 1.561 2.6 1.561 2.978 0a1.533 1.533 0 012.287-.947c1.372.836 2.942-.734 2.106-2.106a1.533 1.533 0 01.947-2.287c1.561-.379 1.561-2.6 0-2.978a1.532 1.532 0 01-.947-2.287c.836-1.372-.734-2.942-2.106-2.106a1.532 1.532 0 01-2.287-.947zM10 13a3 3 0 100-6 3 3 0 000 6z" clip-rule="evenodd"/>
                  </svg>
                  Settings
                </a>
                <div class="user-dropdown-separator"></div>
                <div class="user-dropdown-item" onclick="handleLogout()">
                  <svg width="16" height="16" viewBox="0 0 20 20" fill="currentColor">
                    <path fill-rule="evenodd" d="M3 3a1 1 0 00-1 1v12a1 1 0 102 0V4a1 1 0 00-1-1zm10.293 9.293a1 1 0 001.414 1.414l3-3a1 1 0 000-1.414l-3-3a1 1 0 10-1.414 1.414L14.586 9H7a1 1 0 100 2h7.586l-1.293 1.293z" clip-rule="evenodd"/>
                  </svg>
                  Logout
                </div>
              </div>
            </div>
          ` : `
            <a href="login.html" class="btn btn-primary">
              <svg width="16" height="16" viewBox="0 0 20 20" fill="currentColor">
                <path fill-rule="evenodd" d="M3 3a1 1 0 011 1v12a1 1 0 11-2 0V4a1 1 0 011-1zm7.707 3.293a1 1 0 010 1.414L9.414 9H17a1 1 0 110 2H9.414l1.293 1.293a1 1 0 01-1.414 1.414l-3-3a1 1 0 010-1.414l3-3a1 1 0 011.414 0z" clip-rule="evenodd"/>
              </svg>
              Login
            </a>
          `}
          
          <button class="mobile-menu-btn" onclick="toggleMobileMenu()">
            <svg width="24" height="24" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd" d="M3 5a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM3 10a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM3 15a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z" clip-rule="evenodd"/>
            </svg>
          </button>
        </div>
      </div>
    </header>
    
    <!-- Mobile Menu -->
    <div class="mobile-menu-overlay" id="mobileMenuOverlay" onclick="toggleMobileMenu()"></div>
    <div class="mobile-menu" id="mobileMenu">
      <h2>Menu</h2>
      <nav class="mobile-nav">
        <a href="index.html">Home</a>
        <a href="rooms.html">Rooms</a>
        <a href="about.html">About</a>
        <a href="contact.html">Contact</a>
        ${user ? `
          <div style="border-top: 1px solid var(--color-gray-200); margin-top: var(--spacing-lg); padding-top: var(--spacing-lg);">
            <div style="margin-bottom: var(--spacing-md); padding-bottom: var(--spacing-md); border-bottom: 1px solid var(--color-gray-200);">
              <div style="font-weight: var(--font-weight-semibold); color: var(--color-primary);">${user.firstName} ${user.lastName}</div>
              <div style="font-size: var(--font-size-sm); color: var(--color-gray-500);">${user.email}</div>
            </div>
            ${user.isAdmin ? '<a href="admin.html">Admin Dashboard</a>' : ''}
            <a href="my-bookings.html">My Bookings</a>
            <a href="settings.html">Settings</a>
            <a href="#" onclick="handleLogout(); return false;">Logout</a>
          </div>
        ` : `
          <a href="login.html">Login</a>
        `}
      </nav>
    </div>
  `;
}

// Footer Component
function renderFooter() {
    return `
    <footer class="footer">
      <div class="container">
        <div class="footer-content">
          <div class="footer-section">
            <h3>Bookify</h3>
            <p style="color: rgba(255, 255, 255, 0.8); margin-top: var(--spacing-md);">
              Experience luxury and comfort in our premium hotel rooms. Your perfect stay awaits.
            </p>
          </div>
          
          <div class="footer-section">
            <h3>Quick Links</h3>
            <div class="footer-links">
              <a href="index.html">Home</a>
              <a href="rooms.html">Rooms</a>
              <a href="about.html">About</a>
              <a href="contact.html">Contact</a>
            </div>
          </div>
          
          <div class="footer-section">
            <h3>Account</h3>
            <div class="footer-links">
              <a href="login.html">Login</a>
              <a href="my-bookings.html">My Bookings</a>
              <a href="settings.html">Settings</a>
            </div>
          </div>
          
          <div class="footer-section">
            <h3>Contact</h3>
            <div class="footer-links">
              <p style="color: rgba(255, 255, 255, 0.8);">123 Luxury Ave, Paradise City</p>
              <p style="color: rgba(255, 255, 255, 0.8);">Phone: +1 (555) 123-4567</p>
              <p style="color: rgba(255, 255, 255, 0.8);">Email: info@bookify.com</p>
            </div>
          </div>
        </div>
        
        <div class="footer-bottom">
          <p>&copy; ${new Date().getFullYear()} Bookify. All rights reserved.</p>
        </div>
      </div>
    </footer>
  `;
}

// Initialize Header and Footer
function initLayout() {
    const headerContainer = document.getElementById('header');
    const footerContainer = document.getElementById('footer');

    if (headerContainer) {
        headerContainer.innerHTML = renderHeader();
    }

    if (footerContainer) {
        footerContainer.innerHTML = renderFooter();
    }
}

// Toggle User Dropdown
function toggleUserDropdown() {
    const dropdown = document.getElementById('userDropdown');
    if (dropdown) {
        dropdown.classList.toggle('active');
    }
}

// Close dropdown when clicking outside
document.addEventListener('click', (e) => {
    const dropdown = document.getElementById('userDropdown');
    const avatar = document.querySelector('.user-avatar');
    if (dropdown && avatar && !dropdown.contains(e.target) && !avatar.contains(e.target)) {
        dropdown.classList.remove('active');
    }
});

// Toggle Mobile Menu
function toggleMobileMenu() {
    const menu = document.getElementById('mobileMenu');
    const overlay = document.getElementById('mobileMenuOverlay');
    if (menu && overlay) {
        menu.classList.toggle('active');
        overlay.classList.toggle('active');
    }
}

// Handle Logout
function handleLogout() {
    auth.logout();
    toast.success('Logged out successfully');
    setTimeout(() => {
        window.location.href = 'index.html';
    }, 500);
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', () => {
    initLayout();
    setActiveNavLink();
});

