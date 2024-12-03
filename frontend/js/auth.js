document.addEventListener("DOMContentLoaded", () => {
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
                const response = await fetch(`${AUTH_API_BASE}/register`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify(signupData),
                });

                console.log("Signup response status:", response.status);
                const responseData = await response.json();
                console.log("Signup response data:", responseData);

                if (response.ok) {
                    showToast("Account created successfully!", "success");
                    setTimeout(() => {
                        window.location.href = "auth.html";
                    }, 1500);
                } else {
                    showToast(responseData.message || "Signup failed. Please try again.", "error");
                }
            } catch (error) {
                console.error("Signup error:", error);
                showToast("An error occurred. Please try again.", "error");
            }
        });
    }

    // handle login form
    const loginForm = document.querySelector(".login-container");
    if (loginForm) {
        loginForm.querySelector("button").addEventListener("click", async (e) => {
            e.preventDefault();
            const username = document.getElementById("username").value;
            const password = document.getElementById("password").value;

            try {
                console.log("Attempting login with username:", username);
                const response = await fetch(`${AUTH_API_BASE}login`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify({ username, password }),
                });

                console.log("Login response status:", response.status);
                if (response.ok) {
                    const responseData = await response.json();
                    localStorage.setItem("isAuthenticated", "true");
                    localStorage.setItem("username", username);
                    localStorage.setItem("token", responseData.token);
                    showToast("Login successful!", "success");
                    window.location.href = "grade.html";
                } else {
                    const responseData = await response.json();
                    showToast(responseData.message || "Login failed. Please try again.", "error");
                }
            } catch (error) {
                console.error("Login error:", error);
                showToast("An error occurred. Please try again.", "error");
            }
        });
    }
});
