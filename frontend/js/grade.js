document.addEventListener("DOMContentLoaded", () => {
    checkAuth();
    
    const gradeForm = document.querySelector(".grade-container form");
    if (gradeForm) {
        // Demo data generator
        const fillRandomBtn = document.getElementById("fillRandom");
        if (fillRandomBtn) {
            fillRandomBtn.addEventListener("click", () => {
                const categories = ["Mathematics", "Physics", "Chemistry", "Biology", "History", "English", "French",];
                const subjects = ["Homework", "Quiz", "Exam", "Project", "Presentation"];

                document.getElementById("title").value = 
                    `${subjects[Math.floor(Math.random() * subjects.length)]}`;
                document.getElementById("category").value = categories[Math.floor(Math.random() * categories.length)];
                document.getElementById("mark").value = Math.floor(Math.random() * 7) + 1;
                document.getElementById("weight").value = (Math.floor(Math.random() * 20) + 1) / 10;
            });
        }

        gradeForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            const username = localStorage.getItem("username");
            if (!username) {
                showToast("Please log in first", "error");
                return;
            }

            const gradeData = {
                username: username,
                category: document.getElementById("category").value.trim(),
                title: document.getElementById("title").value.trim(),
                mark: parseFloat(document.getElementById("mark").value),
                weight: parseFloat(document.getElementById("weight").value) || 1,
            };

            // Validate data
            if (!gradeData.category || !gradeData.title || isNaN(gradeData.mark) || isNaN(gradeData.weight)) {
                showToast("Please fill in all fields correctly", "error");
                return;
            }

            try {
                showSpinner();
                const requestBody = JSON.stringify(gradeData);
                const response = await fetch(`${GRADE_API_BASE}/AddGrade`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify(gradeData),
                });

                let responseData;
                const contentType = response.headers.get("content-type");
                if (contentType && contentType.includes("application/json")) {
                    responseData = await response.json();
                } else {
                    responseData = await response.text();
                }
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
});
