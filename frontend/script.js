// TEST MODE - DELETE THIS SECTION WHEN DEPLOYING
let testMode = localStorage.getItem("testMode") === "true";

// Check authentication status
function checkAuth() {
  // TEST MODE - DELETE THIS LINE WHEN DEPLOYING
  if (testMode) return;

  const isAuthenticated = localStorage.getItem("isAuthenticated") === "true";
  const navLinks = document.querySelector(".nav-links");

  if (isAuthenticated) {
    // Update navigation for authenticated users
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
  showNotification("Logged out successfully", "success");
  window.location.href = "auth.html";
}

// Utility function for showing notifications
function showNotification(message, type) {
  const notification = document.createElement("div");
  notification.className = `notification ${type}`;
  notification.textContent = message;
  document.body.appendChild(notification);

  setTimeout(() => {
    notification.remove();
  }, 3000);
}

// Login functionality
document.addEventListener("DOMContentLoaded", () => {
  // Check authentication status on every page load
  checkAuth();

  // TEST MODE - DELETE THIS SECTION WHEN DEPLOYING
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
      showNotification(
        `Test mode ${testMode ? "enabled" : "disabled"}`,
        "success"
      );
      location.reload();
    });
  }

  // Signup functionality
  const signupForm = document.querySelector(".signup-container");
  if (signupForm) {
    signupForm.querySelector("button").addEventListener("click", async (e) => {
      e.preventDefault();
      const password = document.getElementById("password").value;
      const confirmPassword = document.getElementById("confirm-password").value;

      if (password !== confirmPassword) {
        showNotification("Passwords do not match", "error");
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
          showNotification("Account created successfully!", "success");
          setTimeout(() => {
            window.location.href = "auth.html";
          }, 1500);
        } else {
          showNotification(
            responseData.message || "Signup failed. Please try again.",
            "error"
          );
        }
      } catch (error) {
        console.error("Signup error:", error);
        showNotification("An error occurred. Please try again.", "error");
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
          showNotification("Login successful!", "success");
          window.location.href = "grade.html";
        } else {
          showNotification("Login failed. Please try again.", "error");
        }
      } catch (error) {
        console.error("Login error:", error);
        showNotification("An error occurred. Please try again.", "error");
      }
    });
  }

  // Grade form functionality
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
          showNotification("Grade added successfully!", "success");
          // Clear form
          document.getElementById("title").value = "";
          document.getElementById("category").value = "";
          document.getElementById("mark").value = "";
          document.getElementById("weight").value = "";
        } else {
          showNotification("Failed to add grade. Please try again.", "error");
        }
      } catch (error) {
        console.error("Grade submission error:", error);
        showNotification("An error occurred. Please try again.", "error");
      }
    });
  }

  // Calculation functionality
  const calcForm = document.querySelector(".calculation-container");
  if (calcForm) {
    (async () => {
      try {
        console.log("Fetching grades for calculation");
        const username = localStorage.getItem("username");

        if (!username) {
          showNotification("User not logged in", "error");
          return;
        }

        // Fetch grades by sending the username in the request body
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

          // Group grades by category
          const gradesByCategory = grades.reduce((acc, grade) => {
            if (!acc[grade.category]) {
              acc[grade.category] = [];
            }
            acc[grade.category].push(grade);
            return acc;
          }, {});

          // Populate category grades in category-boxes
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

          // Display final grade
          document.getElementById("final-grade").value =
            result.finalGrade.toFixed(2);

          showNotification("Calculation completed!", "success");
        } else {
          showNotification("Calculation failed. Please try again.", "error");
        }
      } catch (error) {
        console.error("Calculation error:", error);
        showNotification("An error occurred. Please try again.", "error");
      }
    })();
  }
});
