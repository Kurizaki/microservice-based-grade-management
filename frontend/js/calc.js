let currentEditId = null;

// Edit grade function
window.editGrade = (id, title, category, mark, weight) => {
  currentEditId = id;

  // Populate the form with existing data
  document.getElementById("edit-title").value = title;
  document.getElementById("edit-category").value = category;
  document.getElementById("edit-mark").value = mark;
  document.getElementById("edit-weight").value = weight;

  // Show the popup
  document.querySelector(".edit-popup-overlay").classList.add("active");
};

// Close edit popup
window.closeEditPopup = () => {
  document.querySelector(".edit-popup-overlay").classList.remove("active");
  currentEditId = null;
};

// Submit edit grade
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

// Toggle edit mode for a category
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

// Show delete confirmation popup
window.deleteGrade = (id) => {
  gradeToDelete = id;
  document.querySelector(".delete-popup-overlay").classList.add("active");
};

// Close delete popup
window.closeDeletePopup = () => {
  document.querySelector(".delete-popup-overlay").classList.remove("active");
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
    closeDeletePopup();
  }
};

document.addEventListener("DOMContentLoaded", () => {
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

        console.log("Fetch grades response status:", gradesResponse.status);
        if (!gradesResponse.ok) {
          throw new Error("Failed to fetch grades");
        }

        const grades = await gradesResponse.json();
        console.log("Fetched grades:", grades);

        console.log("Sending grades for calculation");
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
