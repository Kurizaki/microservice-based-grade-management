// Configuration
const MIN_LOADING_TIME = 700; // Minimum loading time
let testMode = localStorage.getItem("testMode") === "true";

// verify user login state
function checkAuth() {
  if (testMode) return;

  const isAuthenticated = localStorage.getItem("isAuthenticated") === "true";
  const navLinks = document.querySelector(".nav-links");

  if (isAuthenticated) {
    // show authenticated nav
    const gradeLink = navLinks.querySelector('a[href="grade.html"]');
    const calcLink = navLinks.querySelector('a[href="calc.html"]');
    const authLink = navLinks.querySelector('a[href="auth.html"]');

    gradeLink.classList.remove("locked");
    calcLink.classList.remove("locked");
    gradeLink.innerHTML = '<i class="fas fa-book"></i> Grades';
    calcLink.innerHTML = '<i class="fas fa-calculator"></i> Calculator';
    authLink.innerHTML = '<i class="fas fa-sign-out-alt"></i> Logout';

    // Update href for logout
    authLink.href = "#";
    authLink.addEventListener("click", (e) => {
      e.preventDefault();
      logout();
    });
  } else {
    // Redirect if trying to access protected pages
    const currentPage = window.location.pathname;
    if (
      currentPage.includes("grade.html") ||
      currentPage.includes("calc.html")
    ) {
      window.location.href = "auth.html";
    }
  }
}

function logout() {
  localStorage.removeItem("isAuthenticated");
  showToast("Logged out successfully", "success");
  window.location.href = "auth.html";
}

// Error/Success toast notification
function showToast(message, type) {
  // cleanup old toasts
  const existingToasts = document.querySelectorAll('.toast');
  existingToasts.forEach(toast => toast.remove());

  const toast = document.createElement("div");
  toast.className = `toast ${type}`;
  toast.textContent = message;
  document.body.appendChild(toast);

  if (type === 'success') {
    setTimeout(() => {
      toast.remove();
    }, 5000);
  }
  // Error toasts stay until new toast appears
}

// Setup page handlers
// Create spinner element
const spinnerOverlay = document.createElement('div');
spinnerOverlay.className = 'spinner-overlay';
spinnerOverlay.innerHTML = '<div class="spinner"></div>';
const calcContainer = document.querySelector('.calculation-container');
if (calcContainer) {
    calcContainer.appendChild(spinnerOverlay);
} else {
    document.body.appendChild(spinnerOverlay);
}

// Spinner control functions
function showSpinner() {
    spinnerOverlay.classList.add('active');
}

function hideSpinner() {
    spinnerOverlay.classList.remove('active');
}

document.addEventListener("DOMContentLoaded", () => {
  checkAuth();

  // debug mode toggle
  const testModeBtn = document.getElementById("toggleTestMode");
  if (testModeBtn) {
    testModeBtn.textContent = testMode
      ? "Disable Test Mode"
      : "Enable Test Mode";
    testModeBtn.addEventListener("click", () => {
      testMode = !testMode;
      localStorage.setItem("testMode", testMode);
      testModeBtn.textContent = testMode
        ? "Disable Test Mode"
        : "Enable Test Mode";
      showToast(
        `Test mode ${testMode ? "enabled" : "disabled"}`,
        "success"
      );
      location.reload();
    });
  }

  // handle signup form
  const signupForm = document.querySelector(".signup-container");
  if (signupForm) {
    signupForm.querySelector("button").addEventListener("click", async (e) => {
      e.preventDefault();
      const password = document.getElementById("password").value;
      const confirmPassword = document.getElementById("confirm-password").value;

      if (password !== confirmPassword) {
        showToast("Passwords do not match", "error");
        return;
      }

      const signupData = {
        username: document.getElementById("username").value,
        password: password,
      };

      try {
        console.log("Attempting signup with data:", signupData);
        const response = await fetch(
          "http://localhost:5201/api/Auth/register",
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify(signupData),
          }
        );

        console.log("Signup response status:", response.status);
        const responseData = await response.json();
        console.log("Signup response data:", responseData);

        if (response.ok) {
          showToast("Account created successfully!", "success");
          setTimeout(() => {
            window.location.href = "auth.html";
          }, 1500);
        } else {
          showToast(
            responseData.message || "Signup failed. Please try again.",
            "error"
          );
        }
      } catch (error) {
        console.error("Signup error:", error);
        showToast("An error occurred. Please try again.", "error");
      }
    });
  }
  const loginForm = document.querySelector(".login-container");
  if (loginForm) {
    loginForm.querySelector("button").addEventListener("click", async (e) => {
      e.preventDefault();
      const username = document.getElementById("username").value;
      const password = document.getElementById("password").value;

      try {
        console.log("Attempting login with username:", username);
        const response = await fetch("http://localhost:5201/api/Auth/login", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ username, password }),
        });

        console.log("Login response status:", response.status);
        if (response.ok) {
          localStorage.setItem("isAuthenticated", "true");
          localStorage.setItem("username", username);
          showToast("Login successful!", "success");
          window.location.href = "grade.html";
        } else {
          showToast("Login failed. Please try again.", "error");
        }
      } catch (error) {
        console.error("Login error:", error);
        showToast("An error occurred. Please try again.", "error");
      }
    });
  }

  // handle grade submission
  const gradeForm = document.querySelector(".grade-container");
  if (gradeForm) {
    gradeForm.querySelector("button").addEventListener("click", async (e) => {
      e.preventDefault();
      const gradeData = {
        username: localStorage.getItem("username") || "",
        category: document.getElementById("category").value,
        title: document.getElementById("title").value,
        mark: parseFloat(document.getElementById("mark").value),
        weight: parseFloat(document.getElementById("weight").value) || 1,
      };

      try {
        showSpinner();
        console.log("Submitting grade data:", gradeData);
        const response = await fetch(
          "http://localhost:5035/api/Grade/AddGrade",
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify(gradeData),
          }
        );

        console.log("Add Grade response status:", response.status);
        if (response.ok) {
          showToast("Grade added successfully!", "success");
          // Clear form
          document.getElementById("title").value = "";
          document.getElementById("category").value = "";
          document.getElementById("mark").value = "";
          document.getElementById("weight").value = "";
        } else {
          showToast("Failed to add grade. Please try again.", "error");
        }
      } catch (error) {
        console.error("Grade submission error:", error);
        showToast("An error occurred. Please try again.", "error");
      } finally {
        hideSpinner();
      }
    });
  }

  // grade calculations
  const calcForm = document.querySelector(".calculation-container");
  if (calcForm) {
    (async () => {
      const startTime = Date.now();
      try {
        showSpinner();
        console.log("Fetching grades for calculation");
        const username = localStorage.getItem("username");

        if (!username) {
          showToast("User not logged in", "error");
          return;
        }

        // Fetch grades by username
        const gradesResponse = await fetch(
          "http://localhost:5035/api/Grade/GetGradesFromUser",
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify({ username }),
          }
        );

        console.log("Fetch grades response status:", gradesResponse.status);
        if (!gradesResponse.ok) {
          throw new Error("Failed to fetch grades");
        }

        const grades = await gradesResponse.json();
        console.log("Fetched grades:", grades);

        console.log("Sending grades for calculation");
        const calcResponse = await fetch(
          "http://localhost:5210/api/Grade/calculate",
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify(
              grades.map((grade) => ({
                category: grade.category,
                score: grade.mark,
                weight: grade.weight,
              }))
            ),
          }
        );

        console.log("Calculation response status:", calcResponse.status);
        if (calcResponse.ok) {
          const result = await calcResponse.json();
          console.log("Calculation result:", result);

          // sort group by category
          const gradesByCategory = grades.reduce((acc, grade) => {
            if (!acc[grade.category]) {
              acc[grade.category] = [];
            }
            acc[grade.category].push(grade);
            return acc;
          }, {});

          // render category boxes
          const categoryBoxes = document.getElementById("category-boxes");
          categoryBoxes.innerHTML = Object.entries(result.categoryGrades)
            .map(([category, averageGrade]) => {
              // Generate list of individual grades for this category
              const individualGrades = gradesByCategory[category]
                .map(
                  (grade) =>
                    `<div class="grade-item">
                                        <span>Title: ${grade.title}</span> - 
                                        <span>Score: ${grade.mark}</span>
                                    </div>`
                )
                .join("");

              return `
                            <div class="category-box">
                                <h3>${category}</h3>
                                ${individualGrades}
                                <p class="category-average">Average: ${averageGrade.toFixed(
                                  2
                                )}</p>
                            </div>
                        `;
            })
            .join("");

          // show final grade
          document.getElementById("final-grade").value =
            result.finalGrade.toFixed(2);

          showToast("Calculation completed!", "success");
        } else {
          showToast("Calculation failed. Please try again.", "error");
        }
      } catch (error) {
        console.error("Calculation error:", error);
        showToast("An error occurred. Please try again.", "error");
      } finally {
        const elapsedTime = Date.now() - startTime;
        if (elapsedTime < MIN_LOADING_TIME) {
          setTimeout(() => {
            hideSpinner();
          }, MIN_LOADING_TIME - elapsedTime);
        } else {
          hideSpinner();
        }
      }
    })();
  }
});
