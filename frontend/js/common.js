// Configuration
const MIN_LOADING_TIME = 700;
let testMode = localStorage.getItem("testMode") === "true";

// API Base URLs aligned with Nginx routing
const AUTH_API_BASE = "http://localhost:8080/auth-api/";
const GRADE_API_BASE = "http://localhost:8080/grade-api";
const CALC_API_BASE = "http://localhost:8080/calc-api/";

// Toast notifications
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

// Spinner control
const spinnerOverlay = document.createElement("div");
spinnerOverlay.className = "spinner-overlay";
spinnerOverlay.innerHTML = '<div class="spinner"></div>';

document.addEventListener("DOMContentLoaded", () => {
    const calcContainer = document.querySelector(".calculation-container");
    if (calcContainer) {
        calcContainer.appendChild(spinnerOverlay);
    } else {
        document.body.appendChild(spinnerOverlay);
    }

    // debug mode toggle
    const testModeBtn = document.getElementById("toggleTestMode");
    if (testModeBtn) {
        testModeBtn.textContent = testMode ? "Disable Test Mode" : "Enable Test Mode";
        testModeBtn.addEventListener("click", () => {
            testMode = !testMode;
            localStorage.setItem("testMode", testMode);
            testModeBtn.textContent = testMode ? "Disable Test Mode" : "Enable Test Mode";
            showToast(`Test mode ${testMode ? "enabled" : "disabled"}`, "success");
            location.reload();
        });
    }
});

function showSpinner() {
    spinnerOverlay.classList.add("active");
}

function hideSpinner() {
    spinnerOverlay.classList.remove("active");
}

// Auth check
function checkAuth() {
    if (testMode) return;

    const isAuthenticated = localStorage.getItem("isAuthenticated") === "true";
    const navLinks = document.querySelector(".nav-links");

    if (isAuthenticated) {
        const gradeLink = navLinks.querySelector('a[href="grade.html"]');
        const calcLink = navLinks.querySelector('a[href="calc.html"]');
        const authLink = navLinks.querySelector('a[href="auth.html"]');

        gradeLink.classList.remove("locked");
        calcLink.classList.remove("locked");
        gradeLink.innerHTML = '<i class="fas fa-book"></i> Grades';
        calcLink.innerHTML = '<i class="fas fa-calculator"></i> Calculator';
        authLink.innerHTML = '<i class="fas fa-sign-out-alt"></i> Logout';

        authLink.href = "#";
        authLink.addEventListener("click", (e) => {
            e.preventDefault();
            logout();
        });
    } else {
        const currentPage = window.location.pathname;
        if (currentPage.includes("grade.html") || currentPage.includes("calc.html")) {
            window.location.href = "/auth";
        }
    }
}

function logout() {
    localStorage.removeItem("isAuthenticated");
    showToast("Logged out successfully", "success");
    window.location.href = "/auth";
}
