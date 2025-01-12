document.addEventListener("DOMContentLoaded", () => {
  // Signup handler
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
        const response = await fetch(`${AUTH_API_BASE}/register`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(signupData),
        });

        const responseData = await response.json();

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
        showToast("Signup failed. Please try again.", "error");
      }
    });
  }

  // Login handler
  const loginForm = document.querySelector(".login-container");
  if (loginForm) {
    loginForm.querySelector("button").addEventListener("click", async (e) => {
      e.preventDefault();
      const username = document.getElementById("username").value;
      const password = document.getElementById("password").value;

      try {
        const response = await fetch(`${AUTH_API_BASE}/login`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ username, password }),
        });

        const responseData = await response.json();

        if (response.ok) {
          localStorage.setItem("isAuthenticated", "true");
          localStorage.setItem("username", username);
          localStorage.setItem("token", responseData.token);

          showToast("Login successful!", "success");

          // Check if user is admin and redirect accordingly
          const isAdmin = data.isAdmin || false;
          if (isAdmin) {
            localStorage.setItem("isAdmin", "true");
            setTimeout(() => {
              window.location.href = "admin.html";
            }, 300);
          } else {
            setTimeout(() => {
              window.location.href = "grade.html";
            }, 300);
          }
        } else {
          const responseData = await response.json();
          showToast(
            responseData.message || "Login failed. Please try again.",
            "error"
          );
        }
      } catch (error) {
        showToast("Login failed. Please try again.", "error");
      }
    });
  }
});
