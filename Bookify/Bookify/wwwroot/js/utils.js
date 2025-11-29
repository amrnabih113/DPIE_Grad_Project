// ===================================
// BOOKIFY - Utility Functions
// ===================================

// Toast Notifications
class Toast {
    constructor() {
        this.container = null;
        this.init();
    }

    init() {
        this.container = document.createElement('div');
        this.container.className = 'toast-container';
        document.body.appendChild(this.container);
    }

    show(message, type = 'success') {
        const toast = document.createElement('div');
        toast.className = `toast toast-${type}`;

        const icon = type === 'success' ? '✓' : type === 'error' ? '✕' : 'ℹ';

        toast.innerHTML = `
      <span style="font-size: 1.25rem;">${icon}</span>
      <span>${message}</span>
    `;

        this.container.appendChild(toast);

        setTimeout(() => {
            toast.style.animation = 'slideOut 0.3s ease-out';
            setTimeout(() => {
                this.container.removeChild(toast);
            }, 300);
        }, 3000);
    }

    success(message) {
        this.show(message, 'success');
    }

    error(message) {
        this.show(message, 'error');
    }

    warning(message) {
        this.show(message, 'warning');
    }
}

const toast = new Toast();

// Date Formatting
function formatDate(dateString) {
    const options = { year: 'numeric', month: 'short', day: 'numeric' };
    return new Date(dateString).toLocaleDateString('en-US', options);
}

// Price Formatting
function formatPrice(price) {
    return `$${price.toFixed(2)}`;
}

// Calculate Days Between Dates
function calculateDays(checkIn, checkOut) {
    const start = new Date(checkIn);
    const end = new Date(checkOut);
    const diffTime = Math.abs(end - start);
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays;
}

// Create Star Rating HTML
function createStarRating(rating, size = 'md') {
    const stars = [];
    const sizeClass = size === 'lg' ? '1.25rem' : size === 'sm' ? '0.875rem' : '1rem';

    for (let i = 0; i < 5; i++) {
        const filled = i < rating;
        stars.push(`
      <svg class="star-icon" style="width: ${sizeClass}; height: ${sizeClass}; ${filled ? 'fill: var(--color-accent);' : 'fill: var(--color-gray-300);'}" viewBox="0 0 20 20">
        <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"/>
      </svg>
    `);
    }

    return `<div class="star-rating">${stars.join('')}</div>`;
}

// Debounce Function
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Image With Fallback
function createImageWithFallback(src, alt, className = '') {
    const img = document.createElement('img');
    img.src = src;
    img.alt = alt;
    img.className = className;

    img.onerror = function () {
        this.src = `https://via.placeholder.com/600x400/1E3A5F/D4AF37?text=${encodeURIComponent(alt)}`;
    };

    return img;
}

// Scroll To Top
function scrollToTop(smooth = true) {
    window.scrollTo({
        top: 0,
        behavior: smooth ? 'smooth' : 'auto'
    });
}

// Get Query Parameter
function getQueryParam(param) {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(param);
}

// Set Active Nav Link
function setActiveNavLink() {
    const currentPage = window.location.pathname.split('/').pop() || 'index.html';
    const navLinks = document.querySelectorAll('.nav a, .mobile-nav a');

    navLinks.forEach(link => {
        const href = link.getAttribute('href');
        if (href === currentPage || (currentPage === '' && href === 'index.html')) {
            link.classList.add('active');
        } else {
            link.classList.remove('active');
        }
    });
}

// Validate Email
function isValidEmail(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}

// Validate Form
function validateForm(formData, rules) {
    const errors = {};

    for (const [field, value] of Object.entries(formData)) {
        const fieldRules = rules[field];
        if (!fieldRules) continue;

        if (fieldRules.required && !value.trim()) {
            errors[field] = `${field} is required`;
        }

        if (fieldRules.email && !isValidEmail(value)) {
            errors[field] = 'Invalid email address';
        }

        if (fieldRules.minLength && value.length < fieldRules.minLength) {
            errors[field] = `Must be at least ${fieldRules.minLength} characters`;
        }

        if (fieldRules.match && value !== formData[fieldRules.match]) {
            errors[field] = `${field} must match ${fieldRules.match}`;
        }
    }

    return {
        isValid: Object.keys(errors).length === 0,
        errors
    };
}

// Create Avatar Initials
function createAvatarInitials(firstName, lastName) {
    return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
}

// Intersection Observer for Animations
function observeElements(selector, callback) {
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                callback(entry.target);
                observer.unobserve(entry.target);
            }
        });
    }, {
        threshold: 0.1
    });

    document.querySelectorAll(selector).forEach(el => {
        observer.observe(el);
    });
}

// Add Fade In Animation
function addFadeInAnimation() {
    const style = document.createElement('style');
    style.textContent = `
    .fade-in {
      opacity: 0;
      transform: translateY(20px);
      transition: opacity 0.6s ease-out, transform 0.6s ease-out;
    }
    .fade-in.visible {
      opacity: 1;
      transform: translateY(0);
    }
  `;
    document.head.appendChild(style);

    observeElements('.fade-in', (el) => {
        el.classList.add('visible');
    });
}
