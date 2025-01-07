// Core config
const MIN_LOADING_TIME = 700;
let testMode = localStorage.getItem("testMode") === "true";

// API Endpoints
const AUTH_API_BASE = "/auth-api";
const GRADE_API_BASE = "/grade-api";
const CALC_API_BASE = "/calc-api";
const ADMIN_API_BASE = "/admin-api/Admin";
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

  // Check if we're on admin page
  if (currentPage.includes("admin.html")) {
      if (!isAuthenticated || !token) {
          console.warn('User not authenticated or token missing. Redirecting to auth page.');
          window.location.replace("auth.html");
          return;
      }

      try {
          console.log('Starting admin verification check...');
          console.log('Token:', token ? 'Present' : 'Missing');
          console.log('ADMIN_API_BASE:', ADMIN_API_BASE);
          console.log('Request URL:', `${ADMIN_API_BASE}/verify-admin`);
          console.log('Current page path:', window.location.pathname);

          // Perform the fetch request
          const response = await fetch(`${ADMIN_API_BASE}/verify-admin`, {
              method: 'GET',
              headers: {
                  'Authorization': `Bearer ${token}`
              }
          }).catch(error => {
              console.error('Network error during fetch:', error);
              throw error;
          });

          console.log('Admin verification response status:', response.status);
          console.log('Response headers:', [...response.headers.entries()]);
          console.log('Response URL:', response.url);

          // Check if response is not OK
          if (!response.ok) {
              console.warn('Admin verification failed - response not OK');
              if (response.status === 404) {
                  console.error('404 error - Endpoint not found. Check your API routing or Docker setup.');
              }
              window.location.replace("auth.html");
              return;
          }

          const data = await response.json();
          console.log('Admin verification response data:', data);

          if (!data.isAdmin) {
              console.warn('User is not an admin. Redirecting to auth page.');
              window.location.replace("auth.html");
              return;
          }

          console.log('Admin verification successful');
      } catch (error) {
          console.error('Admin verification error:', error);
          console.error('Error details:', {
              name: error.name,
              message: error.message,
              stack: error.stack
          });
          window.location.replace("auth.html");
          return;
      }
  }

  // Regular auth check for other pages
  let isAdmin = false;
  if (isAuthenticated && token) {
      try {
          console.log('Checking admin status...');
          const response = await fetch(`http://auth-service:8080/api/Admin/Admin/verify-admin`, {
              method: 'GET',
              headers: {
                  'Authorization': `Bearer ${token}`
              }
          });

          console.log('Admin status check response status:', response.status);

          if (response.ok) {
              const data = await response.json();
              console.log('Admin status check response data:', data);
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
      if (currentPage.includes("grade.html") || 
          currentPage.includes("calc.html") ||
          currentPage.includes("admin.html")) {
          console.warn('User not authenticated. Redirecting to auth page.');
          window.location.replace("auth.html");
      }
  }
}


function logout() {
  localStorage.removeItem("isAuthenticated");
  showToast("Logged out successfully", "success");
  window.location.href = "/auth.html";
}
