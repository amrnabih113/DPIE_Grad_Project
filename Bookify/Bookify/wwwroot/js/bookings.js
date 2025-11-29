// ===================================
// BOOKIFY - Booking Management
// ===================================

class BookingManager {
    constructor() {
        this.bookings = [];
        this.init();
    }

    init() {
        const savedBookings = localStorage.getItem('bookify_bookings');
        if (savedBookings) {
            this.bookings = JSON.parse(savedBookings);
        }
    }

    addBooking(bookingData) {
        const user = auth.getCurrentUser();
        if (!user) {
            return { success: false, message: 'Please login to make a booking' };
        }

        const newBooking = {
            ...bookingData,
            id: Date.now().toString(),
            userId: user.id,
            bookingDate: new Date().toISOString(),
            status: 'confirmed'
        };

        this.bookings.push(newBooking);
        localStorage.setItem('bookify_bookings', JSON.stringify(this.bookings));

        return { success: true, booking: newBooking };
    }

    getUserBookings(userId) {
        return this.bookings.filter(b => b.userId === userId);
    }

    getAllBookings() {
        return this.bookings;
    }

    getBookingById(id) {
        return this.bookings.find(b => b.id === id);
    }

    cancelBooking(bookingId) {
        const bookingIndex = this.bookings.findIndex(b => b.id === bookingId);
        if (bookingIndex !== -1) {
            this.bookings[bookingIndex].status = 'cancelled';
            localStorage.setItem('bookify_bookings', JSON.stringify(this.bookings));
            return { success: true };
        }
        return { success: false, message: 'Booking not found' };
    }

    updateBooking(bookingId, updates) {
        const bookingIndex = this.bookings.findIndex(b => b.id === bookingId);
        if (bookingIndex !== -1) {
            this.bookings[bookingIndex] = { ...this.bookings[bookingIndex], ...updates };
            localStorage.setItem('bookify_bookings', JSON.stringify(this.bookings));
            return { success: true };
        }
        return { success: false, message: 'Booking not found' };
    }
}

const bookingManager = new BookingManager();
