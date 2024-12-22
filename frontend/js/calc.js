let currentEditId = null;

// Grade editing
window.editGrade = (id, title, category, mark, weight) => {
  currentEditId = id;

  // Populate the form with existing data
  document.getElementById("edit-title").value = title;
  document.getElementById("edit-category").value = category;
  document.getElementById("edit-mark").value = mark;
  document.getElementById("edit-weight").value = weight;

  // Show the Toast
  document.querySelector(".edit-toast-overlay").classList.add("active");
};

// Toast controls
window.closeEditToast = () => {
  document.querySelector(".edit-toast-overlay").classList.remove("active");
  currentEditId = null;
};
window.submitEditGrade = async () => {
  try {
    const title = document.getElementById("edit-title").value;
    const category = document.getElementById("edit-category").value;
    const mark = document.getElementById("edit-mark").value;
    const weight = document.getElementById("edit-weight").value;

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
      weight: parseFloat(weight),
    };

    showSpinner();
    const response = await fetch(
      `${GRADE_API_BASE}/UpdateGrade/${currentEditId}`,
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(updatedGrade),
      }
    );

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

// Category edit mode
window.toggleEditMode = (category) => {
  const categoryBox = document.getElementById(
    `category-${category.replace(/\s+/g, "-")}`
  );
  const isEditMode = categoryBox.classList.toggle("edit-mode");

  if (isEditMode) {
    showToast(`Edit mode enabled for ${category}`, "warning");

    // Add done and cancel buttons if they don't exist
    if (!categoryBox.querySelector(".edit-mode-controls")) {
      const controls = document.createElement("div");
      controls.className = "edit-mode-controls";
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
  const categoryBox = document.getElementById(
    `category-${category.replace(/\s+/g, "-")}`
  );
  categoryBox.classList.remove("edit-mode");
  showToast(`Changes saved for ${category}`, "success");
};

// Cancel category edit
window.cancelEdit = (category) => {
  const categoryBox = document.getElementById(
    `category-${category.replace(/\s+/g, "-")}`
  );
  categoryBox.classList.remove("edit-mode");
  showToast(`Edit cancelled for ${category}`, "error");
};

let gradeToDelete = null;

// Show delete confirmation toast
window.deleteGrade = (id) => {
  gradeToDelete = id;
  document.querySelector(".delete-toast-overlay").classList.add("active");
};

// Close delete toast
window.closeDeleteToast = () => {
  document.querySelector(".delete-toast-overlay").classList.remove("active");
  gradeToDelete = null;
};

// Confirm and execute delete
window.confirmDelete = async () => {
  if (!gradeToDelete) return;

  try {
    showSpinner();
    const response = await fetch(
      `${GRADE_API_BASE}/DeleteGrade/${gradeToDelete}`,
      {
        method: "DELETE",
      }
    );

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
    closeDeleteToast();
  }
};

document.addEventListener("DOMContentLoaded", () => {
  checkAuth();

  const calcForm = document.querySelector(".calculation-container");
  if (calcForm) {
    (async () => {
      const startTime = Date.now();
      try {
        showSpinner();
        const username = localStorage.getItem("username");
        if (!username) {
          showToast("User not logged in", "error");
          return;
        }

        // Fetch user grades
        const token = localStorage.getItem("token");
        const gradesResponse = await fetch(
          `${GRADE_API_BASE}/GetGradesFromUser`,
          {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${token}`,
            },
          }
        );

        if (!gradesResponse.ok) {
          throw new Error("Failed to fetch grades");
        }

        const grades = await gradesResponse.json();
        const calcResponse = await fetch(`${CALC_API_BASE}/calculate`, {
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
        });

        if (calcResponse.ok) {
          const result = await calcResponse.json();

          // Group grades by category
          const gradesByCategory = grades.reduce((acc, grade) => {
            if (!acc[grade.category]) {
              acc[grade.category] = [];
            }
            acc[grade.category].push(grade);
            return acc;
          }, {});

          // Render grade categories
          const categoryBoxes = document.getElementById("category-boxes");
          categoryBoxes.innerHTML = Object.entries(result.categoryGrades)
            .map(([category, averageGrade]) => {
              const individualGrades = gradesByCategory[category]
                .map(
                  (grade) =>
                    `<div class="grade-item" data-grade-id="${grade.id}">
                                        <div class="grade-info">
                                            <span>${grade.title}</span> - 
                                            <span>${grade.mark}</span>
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
                                <div class="category-box" id="category-${category.replace(
                                  /\s+/g,
                                  "-"
                                )}">
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

          // Show final grade
          document.getElementById("final-grade").value =
            result.finalGrade.toFixed(2);

          showToast("Calculation completed!", "success");
        } else {
          showToast("Calculation failed. Please try again.", "error");
        }
      } catch (error) {
        showToast("Calculation failed. Please try again.", "error");
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
