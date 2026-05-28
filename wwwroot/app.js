const API = '/applications';

// Load all applications on page load
async function loadApplications() {
    const res = await fetch(API);
    const data = await res.json();
    renderTable(data);
}

// Add a new application
async function addApplication() {
    const company = document.getElementById('company').value.trim();
    const role    = document.getElementById('role').value.trim();
    const status  = document.getElementById('status').value;

    if (!company || !role) {
    alert('Please fill in both Company and Role.');
    return;
    }

    await fetch(API, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ company, role, status })
    });

    document.getElementById('company').value = '';
    document.getElementById('role').value = '';
    loadApplications();
}

// Delete an application
async function deleteApplication(id) {
    await fetch(`${API}/${id}`, { method: 'DELETE' });
    loadApplications();
}

// Render the table from data
function renderTable(applications) {
    const tbody = document.getElementById('app-list');

    if (applications.length === 0) {
    tbody.innerHTML = '<tr><td colspan="6" id="empty-msg">No applications yet. Add one above.</td></tr>';
    return;
    }

    tbody.innerHTML = applications.map(a => `
    <tr>
        <td>${escape(a.company)}</td>
        <td>${escape(a.role)}</td>
        <td><span class="status status-${a.status}">${a.status}</span></td>
        <td>${new Date(a.createdAt).toLocaleDateString()}</td>
        <td><button class="delete-btn" onclick="openModal(${a.id}, '${escape(a.company)}', '${escape(a.role)}', '${a.status}')">✏️</button></td>
        <td><button class="delete-btn" onclick="deleteApplication(${a.id})">🗑</button></td>
    </tr>
    `).join('');
}

function openModal(id, company, role, status) {
    document.getElementById('edit-id').value = id;
    document.getElementById('edit-company').value = company;
    document.getElementById('edit-role').value = role;
    document.getElementById('edit-status').value = status;
    document.getElementById('modal').style.display = 'flex';
}

function closeModal() {
    document.getElementById('modal').style.display = 'none';
}

async function saveEdit() {
    const id      = document.getElementById('edit-id').value;
    const company = document.getElementById('edit-company').value.trim();
    const role    = document.getElementById('edit-role').value.trim();
    const status  = document.getElementById('edit-status').value;

    if (!company || !role) {
    alert('Please fill in both Company and Role.');
    return;
    }

    await fetch(`${API}/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ company, role, status })
    });

    closeModal();
    loadApplications();
}

// Basic XSS protection
function escape(str) {
    const d = document.createElement('div');
    d.appendChild(document.createTextNode(str));
    return d.innerHTML;
}

loadApplications();
