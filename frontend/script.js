// Configuration
const MIN_LOADING_TIME = 700; // Minimum loading time
let testMode = localStorage.getItem("testMode") === "true";

// API Base URLs aligned with Nginx routing
const AUTH_API_BASE = "/auth/api";
const GRADE_API_BASE = "/grade/api";
const CALC_API_BASE = "/calc/api";


// Verify user login state
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

// Logout function
function logout() {
  localStorage.removeItem("isAuthenticated");
  showToast("Logged out successfully", "success");
  window.location.href = "/auth";
}

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

// Setup page handlers
// Create spinner element
const spinnerOverlay = document.createElement("div");
spinnerOverlay.className = "spinner-overlay";
spinnerOverlay.innerHTML = '<div class="spinner"></div>';
const calcContainer = document.querySelector(".calculation-container");
if (calcContainer) {
  calcContainer.appendChild(spinnerOverlay);
} else {
  document.body.appendChild(spinnerOverlay);
}

// Spinner control functions
function showSpinner() {
  spinnerOverlay.classList.add("active");
}

function hideSpinner() {
  spinnerOverlay.classList.remove("active");
}

document.addEventListener("DOMContentLoaded", () => {
  checkAuth();

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
          `${AUTH_API_BASE}/register`,
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
        const response = await fetch(`${AUTH_API_BASE}/login`, {
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
    // Random data fill functionality
    const fillRandomBtn = document.getElementById("fillRandom");
    if (fillRandomBtn) {
      fillRandomBtn.addEventListener("click", () => {
        const subjects = [
          "Mathematics",
          "Physics",
          "Chemistry",
          "Biology",
          "History",
          "English",
        ];
        const categories = [
          "Homework",
          "Quiz",
          "Exam",
          "Project",
          "Presentation",
        ];

        document.getElementById("title").value = `${
          categories[Math.floor(Math.random() * categories.length)]
        } - ${subjects[Math.floor(Math.random() * subjects.length)]}`;
        document.getElementById("category").value =
          categories[Math.floor(Math.random() * categories.length)];
        document.getElementById("mark").value =
          Math.floor(Math.random() * 7) + 1;
        document.getElementById("weight").value =
          (Math.floor(Math.random() * 20) + 1) / 10;
      });
    }
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
          `${GRADE_API_BASE}/AddGrade`,
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

  let currentEditId = null;

  // Edit grade function
  window.editGrade = (id, title, category, mark, weight) => {
    currentEditId = id;
    
    // Populate the form with existing data
    document.getElementById('edit-title').value = title;
    document.getElementById('edit-category').value = category;
    document.getElementById('edit-mark').value = mark;
    document.getElementById('edit-weight').value = weight;
    
    // Show the popup
    document.querySelector('.edit-popup-overlay').classList.add('active');
  };

  // Close edit popup
  window.closeEditPopup = () => {
    document.querySelector('.edit-popup-overlay').classList.remove('active');
    currentEditId = null;
  };

  // Submit edit grade
  window.submitEditGrade = async () => {
    try {
      const title = document.getElementById('edit-title').value;
      const category = document.getElementById('edit-category').value;
      const mark = document.getElementById('edit-mark').value;
      const weight = document.getElementById('edit-weight').value;

      if (!title || !category || !mark || !weight) {
        showToast("Please fill all fields", "error");
        return;
      }

      const updatedGrade = {
        id: parseInt(currentEditId),
        username: localStorage.getItem("username"),
        title: title,
        category: category,
        mark: parseFloat(mark),
        weight: parseFloat(weight)
      };

      showSpinner();
      const response = await fetch(`${GRADE_API_BASE}/UpdateGrade/${currentEditId}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify(updatedGrade)
      });

      if (response.ok) {
        showToast("Grade updated successfully!", "success");
        location.reload();
      } else {
        showToast("Failed to update grade", "error");
      }
    } catch (error) {
      console.error("Edit grade error:", error);
      showToast("An error occurred while editing", "error");
    } finally {
      hideSpinner();
    }
  };

  // Toggle edit mode for a category
  window.toggleEditMode = (category) => {
    const categoryBox = document.getElementById(`category-${category.replace(/\s+/g, '-')}`);
    const isEditMode = categoryBox.classList.toggle('edit-mode');
    
    if (isEditMode) {
      showToast(`Edit mode enabled for ${category}`, "warning");
      
      // Add done and cancel buttons if they don't exist
      if (!categoryBox.querySelector('.edit-mode-controls')) {
        const controls = document.createElement('div');
        controls.className = 'edit-mode-controls';
        controls.innerHTML = `
          <button class="done-btn" onclick="saveCategory('${category}')">Done</button>
          <button class="cancel-btn" onclick="cancelEdit('${category}')">Cancel</button>
        `;
        categoryBox.appendChild(controls);
      }
    }
  };

  // Save category changes
  window.saveCategory = (category) => {
    const categoryBox = document.getElementById(`category-${category.replace(/\s+/g, '-')}`);
    categoryBox.classList.remove('edit-mode');
    showToast(`Changes saved for ${category}`, "success");
  };

  // Cancel category edit
  window.cancelEdit = (category) => {
    const categoryBox = document.getElementById(`category-${category.replace(/\s+/g, '-')}`);
    categoryBox.classList.remove('edit-mode');
    showToast(`Edit cancelled for ${category}`, "error");
  };

  let gradeToDelete = null;

  // Show delete confirmation popup
  window.deleteGrade = (id) => {
    gradeToDelete = id;
    document.querySelector('.delete-popup-overlay').classList.add('active');
  };

  // Close delete popup
  window.closeDeletePopup = () => {
    document.querySelector('.delete-popup-overlay').classList.remove('active');
    gradeToDelete = null;
  };

  // Confirm and execute delete
  window.confirmDelete = async () => {
    if (!gradeToDelete) return;

    try {
      showSpinner();
      const response = await fetch(`${GRADE_API_BASE}/DeleteGrade/${gradeToDelete}`, {
        method: "DELETE"
      });

      if (response.ok) {
        showToast("Grade deleted successfully!", "success");
        location.reload();
      } else {
        showToast("Failed to delete grade", "error");
      }
    } catch (error) {
      console.error("Delete grade error:", error);
      showToast("An error occurred while deleting", "error");
    } finally {
      hideSpinner();
      closeDeletePopup();
    }
  };

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
          `${GRADE_API_BASE}/GetGradesFromUser`,
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
          `${CALC_API_BASE}/calculate`,
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
                    `<div class="grade-item" data-grade-id="${grade.id}">
                        <div class="grade-info">
                            <span>${grade.title}</span> - 
                            <span>Score: ${grade.mark}</span>
                        </div>
                        <div class="grade-actions">
                            <button class="edit-btn" onclick="editGrade(${grade.id}, '${grade.title}', '${grade.category}', ${grade.mark}, ${grade.weight})">
                                <i class="fas fa-edit"></i>
                            </button>
                            <button class="delete-btn" onclick="deleteGrade(${grade.id})">
                                <i class="fas fa-trash"></i>
                            </button>
                        </div>
                    </div>`
                )
                .join("");

              return `
                            <div class="category-box" id="category-${category.replace(/\s+/g, '-')}">
                                <h3>
                                    ${category}
                                    <span class="edit-mode-toggle" onclick="toggleEditMode('${category}')">
                                        <i class="fas fa-pen-to-square"></i>
                                    </span>
                                </h3>
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
