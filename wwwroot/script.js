const API_URL = '/api/users';
const TOKEN = 'Bearer mysecrettoken';

const userTableBody = document.getElementById('userTableBody');
const userForm = document.getElementById('userForm');
const refreshBtn = document.getElementById('refreshBtn');
const tokenStatus = document.getElementById('tokenStatus');

// Initialize
document.addEventListener('DOMContentLoaded', () => {
    fetchUsers();
    updateTokenDisplay();
});

function updateTokenDisplay() {
    tokenStatus.innerHTML = `Token Status: <span class="badge badge-success">Bearer ...token</span>`;
}

// Fetch Users
async function fetchUsers() {
    try {
        const response = await fetch(API_URL, {
            headers: { 'Authorization': TOKEN }
        });

        if (response.status === 401) {
            showNotification('Error: Unauthorized access (Invalid token)', 'error');
            return;
        }

        const users = await response.json();
        renderTable(users);
    } catch (error) {
        showNotification('Failed to fetch users: ' + error.message, 'error');
    }
}

// Render Table
function renderTable(users) {
    if (users.length === 0) {
        userTableBody.innerHTML = `<tr><td colspan="4" style="text-align: center; color: var(--text-dim); padding: 40px;">No users found in database.</td></tr>`;
        return;
    }

    userTableBody.innerHTML = users.map(user => `
        <tr id="user-${user.id}" class="fade-in">
            <td class="user-id"># ${user.id}</td>
            <td class="user-name">
                <i class="fa-solid fa-user-circle" style="color: var(--primary); margin-right: 8px;"></i> ${user.name}
            </td>
            <td class="user-email">
                <i class="fa-regular fa-envelope" style="margin-right: 8px;"></i> ${user.email}
            </td>
            <td style="text-align: right;">
                <button onclick="deleteUser(${user.id})" class="btn btn-delete">
                    <i class="fa-solid fa-trash-can"></i>
                </button>
            </td>
        </tr>
    `).join('');
}

// Create User
userForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    const name = document.getElementById('name').value;
    const email = document.getElementById('email').value;

    const newUser = { name, email };

    try {
        const response = await fetch(API_URL, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': TOKEN
            },
            body: JSON.stringify(newUser)
        });

        if (response.ok) {
            showNotification('User added successfully!', 'success');
            userForm.reset();
            fetchUsers();
        } else {
            const errorData = await response.json();
            showNotification('Error: ' + JSON.stringify(errorData), 'error');
        }
    } catch (error) {
        showNotification('An error occurred while creating the user.', 'error');
    }
});

// Delete User
async function deleteUser(id) {
    if (!confirm('Are you sure you want to delete this user?')) return;

    try {
        const response = await fetch(`${API_URL}/${id}`, {
            method: 'DELETE',
            headers: { 'Authorization': TOKEN }
        });

        if (response.ok) {
            showNotification('User deleted!', 'success');
            fetchUsers();
        } else {
            showNotification('Delete failed.', 'error');
        }
    } catch (error) {
        showNotification('Error deleting user.', 'error');
    }
}

refreshBtn.addEventListener('click', fetchUsers);

// Notifications
function showNotification(message, type = 'info') {
    const container = document.getElementById('notifications');
    const note = document.createElement('div');
    note.className = 'notification';
    note.textContent = message;

    if (type === 'error') note.style.borderLeftColor = '#ef4444';
    if (type === 'success') note.style.borderLeftColor = '#10b981';

    container.appendChild(note);

    setTimeout(() => {
        note.style.opacity = '0';
        note.style.transform = 'translateX(20px)';
        setTimeout(() => note.remove(), 300);
    }, 3000);
}
