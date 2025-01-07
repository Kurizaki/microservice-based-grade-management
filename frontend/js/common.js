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

  let isAdmin = false;
  if (isAuthenticated && token) {
    try {
      const response = await fetch(`${AUTH_API_BASE}/verify-admin`, {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      if (response.ok) {
        const data = await response.json();
        isAdmin = data.isAdmin;
      }
    } catch (error) {
      console.error('Error checking admin status:', error);
    }
  }

  if (isAuthenticated) {
    const gradeLink = navLinks.querySelector('a[href="grade.html"]');
    const calcLink = navLinks.querySelector('a[href="calc.html"]');
    const authLink = navLinks.querySelector('a[href="auth.html"]');

    gradeLink.classList.remove("locked");
    calcLink.classList.remove("locked");
    gradeLink.innerHTML = '<i class="fas fa-book"></i> Grades';
    calcLink.innerHTML = '<i class="fas fa-calculator"></i> Calculator';
    
    // Add admin link if user is admin
    if (isAdmin) {
      const adminLink = document.createElement('a');
      adminLink.href = 'admin.html';
      adminLink.innerHTML = '<i class="fas fa-cog"></i> Admin';
      navLinks.insertBefore(adminLink, authLink);
    }
    
    authLink.innerHTML = '<i class="fas fa-sign-out-alt"></i> Logout';

    authLink.href = "#";
    authLink.addEventListener("click", (e) => {
      e.preventDefault();
      logout();
    });
  } else {
    const currentPage = window.location.pathname.toLowerCase();
    if (
      currentPage.includes("grade.html") ||
      currentPage.includes("calc.html")
    ) {
      window.location.replace("auth.html");
    }
  }
}

function logout() {
  localStorage.removeItem("isAuthenticated");
  showToast("Logged out successfully", "success");
  window.location.href = "/auth.html";
}
