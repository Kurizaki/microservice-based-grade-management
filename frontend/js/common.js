// Core config
const MIN_LOADING_TIME = 700;
let testMode = localStorage.getItem("testMode") === "true";

// API Endpoints
const AUTH_API_BASE = "/auth-api";
const GRADE_API_BASE = "/grade-api";
const CALC_API_BASE = "/calc-api";
function showToast(message, type) {
  // Cleanup old toast
  document.querySelectorAll(".toast").forEach((toast) => toast.remove());

  const toast = document.createElement("div");
  toast.className = `toast ${type}`;
  toast.textContent = message;
  document.body.appendChild(toast);

  if (type === "success") {
    setTimeout(() => toast.remove(), 5000);
  }
}

// Loading overlay setup
const spinnerOverlay = document.createElement("div");
spinnerOverlay.className = "spinner-overlay";
spinnerOverlay.innerHTML = '<div class="spinner"></div>';

// Init on page load
document.addEventListener("DOMContentLoaded", () => {
  const calcContainer = document.querySelector(".calculation-container");
  if (calcContainer) {
    calcContainer.appendChild(spinnerOverlay);
  } else {
    document.body.appendChild(spinnerOverlay);
  }
});

function showSpinner() {
  spinnerOverlay.classList.add("active");
}

function hideSpinner() {
  spinnerOverlay.classList.remove("active");
}

// Auth check
async function checkAuth() {
  if (testMode) return;

  const isAuthenticated = localStorage.getItem("isAuthenticated") === "true";
  const token = localStorage.getItem("token");
  const navLinks = document.querySelector(".nav-links");
  const currentPage = window.location.pathname.toLowerCase();

  const isAdmin = localStorage.getItem("isAdmin") === "true";
  console.log(isAdmin);
  
  // Update navigation based on authentication status
  const gradeLink = navLinks.querySelector('a[href="grade.html"]');
  const calcLink = navLinks.querySelector('a[href="calc.html"]');
  const authLink = navLinks.querySelector('a[href="auth.html"]');

  if (isAuthenticated) {
    // User is logged in
    gradeLink.classList.remove("locked");
    calcLink.classList.remove("locked");
    gradeLink.innerHTML = '<i class="fas fa-book"></i> Grades';
    calcLink.innerHTML = '<i class="fas fa-calculator"></i> Calculator';

    // Add admin link if user is admin
    if (isAdmin && !navLinks.querySelector('a[href="admin.html"]')) {
      const adminLink = document.createElement('a');
      adminLink.href = 'admin.html';
      adminLink.innerHTML = '<i class="fas fa-cog"></i> Admin';
      navLinks.insertBefore(adminLink, authLink);
    }

    // Change login to logout
    authLink.innerHTML = '<i class="fas fa-sign-out-alt"></i> Logout';
    authLink.href = "#";
    authLink.addEventListener("click", (e) => {
      e.preventDefault();
      logout();
    });

    // Check if we're on admin page
    if (currentPage.includes("admin.html") && !isAdmin) {
      console.warn('User is not an admin. Redirecting to grade page.');
      window.location.replace("grade.html");
      return;
    }
  } else {
    // User is not logged in
    gradeLink.classList.add("locked");
    calcLink.classList.add("locked");
    gradeLink.innerHTML = '<i class="fas fa-lock"></i> Grades';
    calcLink.innerHTML = '<i class="fas fa-lock"></i> Calculator';

    // Remove admin link if it exists
    const adminLink = navLinks.querySelector('a[href="admin.html"]');
    if (adminLink) {
      navLinks.removeChild(adminLink);
    }
    
    // Remove any existing click handlers
    authLink.replaceWith(authLink.cloneNode(true));

    // Redirect if trying to access protected pages
    if (currentPage.includes("grade.html") || 
        currentPage.includes("calc.html") ||
        currentPage.includes("admin.html")) {
      console.warn('User not authenticated. Redirecting to auth page.');
      window.location.replace("auth.html");
      return;
    }
  }
}


function logout() {
  localStorage.removeItem("isAuthenticated");
  localStorage.removeItem("isAdmin");
  localStorage.removeItem("token");
  localStorage.removeItem("username")
  showToast("Logged out successfully", "success");
  window.location.href = "/auth.html";
}
