document.addEventListener("DOMContentLoaded", async () => {
    // Check if user is admin, redirect if not
    const isAdmin = localStorage.getItem("isAdmin");
    if (!isAdmin || isAdmin === "false") {
        console.warn('User is not an admin. Redirecting to grade page.');
        window.location.replace("grade.html");
        return;
    }
    
    loadUsers();
    await loadDashboards();
});

async function loadDashboards() {
    try {
        const token = localStorage.getItem("token");
        const response = await fetch(`${AUTH_API_BASE}/dashboards`, {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (response.ok) {
            const dashboards = await response.json();
            console.log(dashboards)
            const dashboardIframe = document.querySelector('.dashboard-iframe[data-dashboard="prometheus"]');
            const kibanaIframe = document.querySelector('.dashboard-iframe[data-dashboard="kibana"]');
            
            if (dashboardIframe) {
                dashboardIframe.src = "http://sprudello.ch:19999";
            }
            if (kibanaIframe) {
                kibanaIframe.src = dashboards.kibana;
            }
        }
    } catch (error) {
        console.error("Error loading dashboards:", error);
    }
}


async function loadUsers() {
    try {
        const token = localStorage.getItem("token");
        const response = await fetch(`${AUTH_API_BASE}/users`, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            }
        });

        if (response.ok) {
            const users = await response.json();
            renderUsers(users);
        } else {
            showToast("Failed to load users", "error");
        }
    } catch (error) {
        console.error("Error loading users:", error);
        showToast("Error loading users", "error");
    }
}

function renderUsers(users) {
    const userList = document.getElementById("user-list-content");
    userList.innerHTML = users.map(user => `
        <div class="user-item" data-username="${user.username}">
            <span>${user.username}</span>
            <span>${user.isAdmin ? "Admin" : "User"}</span>
            <div class="user-actions">
                ${!user.isAdmin ? `
                    <button class="make-admin-btn" onclick="toggleAdmin('${user.username}', true)">
                        Make Admin
                    </button>
                ` : `
                    <button class="remove-admin-btn" onclick="toggleAdmin('${user.username}', false)">
                        Remove Admin
                    </button>
                `}
                ${!user.isAdmin ? `
                    <button class="delete-user-btn" onclick="deleteUser('${user.username}')">
                        Delete
                    </button>
                ` : ''}
            </div>
        </div>
    `).join("");
}

async function toggleAdmin(username, makeAdmin) {
    try {
        const token = localStorage.getItem("token");
        const response = await fetch(`${AUTH_API_BASE}/users/${username}/admin`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: makeAdmin
        });

        if (response.ok) {
            showToast(`User ${username} ${makeAdmin ? "promoted to" : "demoted from"} admin`, "success");
            loadUsers();
        } else {
            const errorData = await response.json();
            if (errorData.code === "LAST_ADMIN") {
                showToast(errorData.message, "error");
            } else {
                showToast("Failed to update user role", "error");
            }
        }
    } catch (error) {
        console.error("Error updating user role:", error);
        showToast("Error updating user role", "error");
    }
}

async function deleteUser(username) {
    if (confirm(`Are you sure you want to delete user ${username}?`)) {
        try {
            const token = localStorage.getItem("token");
            const response = await fetch(`${AUTH_API_BASE}/users/${username}`, {
                method: "DELETE",
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            });

            if (response.ok) {
                showToast(`User ${username} deleted`, "success");
                loadUsers();
            } else {
                showToast("Failed to delete user", "error");
            }
        } catch (error) {
            console.error("Error deleting user:", error);
            showToast("Error deleting user", "error");
        }
    }
}
