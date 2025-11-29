// ===================================
// BOOKIFY - Authentication Module
// ===================================

class Auth {
  constructor() {
    this.currentUser = null;
    this.init();
  }

  init() {
    // Load user from localStorage
    const savedUser = localStorage.getItem('bookify_user');
    if (savedUser) {
      this.currentUser = JSON.parse(savedUser);
    }
  }

  async login(email, password) {
    // Check for admin credentials
    if (email === 'admin@test.com' && password === '123') {
      const adminUser = {
        id: 'admin',
        email: 'admin@test.com',
        firstName: 'Admin',
        lastName: 'User',
        isAdmin: true
      };
      this.currentUser = adminUser;
      localStorage.setItem('bookify_user', JSON.stringify(adminUser));
      return { success: true, user: adminUser };
    }

    // Get users from localStorage
    const usersData = localStorage.getItem('bookify_users');
    const users = usersData ? JSON.parse(usersData) : [];

    // Find user with matching email and password
    const foundUser = users.find(u => u.email === email && u.password === password);

    if (foundUser) {
      const { password: _, ...userWithoutPassword } = foundUser;
      this.currentUser = userWithoutPassword;
      localStorage.setItem('bookify_user', JSON.stringify(userWithoutPassword));
      return { success: true, user: userWithoutPassword };
    }

    return { success: false, message: 'Invalid email or password' };
  }

  async signup(email, password, firstName, lastName) {
    // Get existing users
    const usersData = localStorage.getItem('bookify_users');
    const users = usersData ? JSON.parse(usersData) : [];

    // Check if email already exists
    if (users.some(u => u.email === email)) {
      return { success: false, message: 'Email already exists' };
    }

    // Create new user
    const newUser = {
      id: Date.now().toString(),
      email,
      password,
      firstName,
      lastName
    };

    users.push(newUser);
    localStorage.setItem('bookify_users', JSON.stringify(users));

    // Log in the new user
    const { password: _, ...userWithoutPassword } = newUser;
    this.currentUser = userWithoutPassword;
    localStorage.setItem('bookify_user', JSON.stringify(userWithoutPassword));

    return { success: true, user: userWithoutPassword };
  }

  logout() {
    this.currentUser = null;
    localStorage.removeItem('bookify_user');
    window.location.href = 'index.html';
  }

  getCurrentUser() {
    return this.currentUser;
  }

  isAuthenticated() {
    return this.currentUser !== null;
  }

  isAdmin() {
    return this.currentUser && this.currentUser.isAdmin === true;
  }

  requireAuth() {
    if (!this.isAuthenticated()) {
      window.location.href = 'login.html';
      return false;
    }
    return true;
  }

  requireAdmin() {
    if (!this.isAdmin()) {
      window.location.href = 'index.html';
      return false;
    }
    return true;
  }
}

// Create global auth instance
const auth = new Auth();
