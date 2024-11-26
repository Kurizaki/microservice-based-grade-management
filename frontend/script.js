// Replace with your actual auth-service URL
const AUTH_SERVICE_URL = "http://localhost/api/auth";

// Handle registration
document.getElementById("register-form").addEventListener("submit", async (event) => {
    event.preventDefault();

    const username = document.getElementById("reg-username").value;
    const password = document.getElementById("reg-password").value;

    try {
        const response = await fetch(`${AUTH_SERVICE_URL}/register/`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, password })
        });

        if (response.ok) {
            document.getElementById("register-message").textContent = "Registration successful!";
        } else {
            const error = await response.json();
            document.getElementById("register-message").textContent = `Error: ${error.message}`;
        }
    } catch (err) {
        document.getElementById("register-message").textContent = `Error: ${err.message}`;
    }
});

// Handle login
document.getElementById("login-form").addEventListener("submit", async (event) => {
    event.preventDefault();

    const username = document.getElementById("login-username").value;
    const password = document.getElementById("login-password").value;

    try {
        const response = await fetch(`${AUTH_SERVICE_URL}/login/`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, password })
        });

        if (response.ok) {
            const token = await response.text(); // AuthController returns the token as plain text
            document.getElementById("login-message").textContent = "Login successful!";
            localStorage.setItem("jwt", token); // Save JWT token for future use
        } else {
            const error = await response.json();
            document.getElementById("login-message").textContent = `Error: ${error.message}`;
        }
    } catch (err) {
        document.getElementById("login-message").textContent = `Error: ${err.message}`;
    }
});
