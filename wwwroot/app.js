const API = '/applications';
let allApplications = [];

// Load all applications on page load
async function loadApplications() {
    const res = await fetch(API);
    allApplications = await res.json();
    applyFilters();
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
        <td><button class="edit-btn" onclick="openModal(${a.id}, '${escape(a.company)}', '${escape(a.role)}', '${a.status}', '${a.createdAt}')"><i class="fa-solid fa-pen"></i></button></td>
        <td><button class="delete-btn" onclick="deleteApplication(${a.id})"><i class="fa-solid fa-trash"></i></button></td>
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
    const date    = document.getElementById('edit-date').value;

    if (!company || !role) {
        alert('Please fill in both Company and Role.');
        return;
    }

    await fetch(`${API}/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ company, role, status, createdAt: new Date(date).toISOString() })
    });

    closeModal();
    loadApplications();
}

function applyFilters() {
    const text   = document.getElementById('filter-text').value.toLowerCase();
    const status = document.getElementById('filter-status').value;
    const sortBy = document.getElementById('sort-by').value;

    let filtered = [...allApplications];

    // Filter by search text
    if (text) {
        filtered = filtered.filter(a =>
        a.company.toLowerCase().includes(text) ||
        a.role.toLowerCase().includes(text)
        );
    }

    // Filter by status
    if (status) {
        filtered = filtered.filter(a => a.status === status);
    }

    // Sort
    filtered.sort((a, b) => {
        switch (sortBy) {
        case 'date-asc':     return new Date(a.createdAt) - new Date(b.createdAt);
        case 'date-desc':    return new Date(b.createdAt) - new Date(a.createdAt);
        case 'company-asc':  return a.company.localeCompare(b.company);
        case 'company-desc': return b.company.localeCompare(a.company);
        default:             return 0;
        }
    });

    renderTable(filtered);
}

async function analyseJob() {
    const jobDescription = document.getElementById('job-description').value.trim();

    if (!jobDescription) {
        alert('Please paste a job description first.');
        return;
    }

    const resultDiv    = document.getElementById('analysis-result');
    const contentDiv   = document.getElementById('analysis-content');
    const button       = document.querySelector('.analyse-btn');

    button.disabled    = true;
    button.innerHTML   = '<i class="fa-solid fa-spinner fa-spin"></i> Analysing...';
    resultDiv.style.display = 'none';

    try {
        const res = await fetch('/analyse', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ jobDescription })
        });

        const data = await res.json();
        contentDiv.innerText = data.analysis;
        resultDiv.style.display = 'block';
    } catch (err) {
        alert('Something went wrong. Please try again.');
    } finally {
        button.disabled  = false;
        button.innerHTML = '<i class="fa-solid fa-magnifying-glass"></i> Analyse';
    }
}

// Basic XSS protection
function escape(str) {
    const d = document.createElement('div');
    d.appendChild(document.createTextNode(str));
    return d.innerHTML;
}

loadApplications();
