// ============================================
// MANPOWER - JAVASCRIPT
// ============================================

let currentEditId = null;

// Initialize on page load
document.addEventListener('DOMContentLoaded', function () {
    loadData();
    setupSearchFilter();
    setupFormSubmit();
});

// Load data from server
async function loadData() {
    try {
        const response = await fetch('/ManPower/GetData');
        const data = await response.json();

        renderTable(data);
    } catch (error) {
        console.error('Error loading data:', error);
        showToast('Gagal memuat data', 'error');
    }
}

// Render table
function renderTable(data) {
    const tbody = document.getElementById('tableBody');

    if (!data || data.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="9" style="text-align: center; padding: 3rem; color: #64748b;">
                    <i class="ph-users" style="font-size: 3rem; display: block; margin-bottom: 1rem;"></i>
                    Belum ada data manpower
                </td>
            </tr>
        `;
        return;
    }

    tbody.innerHTML = data.map(item => `
        <tr>
            <td><strong>${item.nik}</strong></td>
            <td>${item.namaLengkap}</td>
            <td>${item.jabatan}</td>
            <td>${item.department}</td>
            <td><span class="badge-shift">${item.shift}</span></td>
            <td>${item.areaLine || '-'}</td>
            <td>${item.noTelepon || '-'}</td>
            <td>
                <span class="status-badge ${item.isActive ? 'status-selesai' : 'status-pending'}">
                    <span class="status-dot"></span>
                    ${item.isActive ? 'Aktif' : 'Tidak Aktif'}
                </span>
            </td>
            <td>
                <div class="action-buttons">
                    <button class="btn-action btn-edit" onclick="editData(${item.id})" title="Edit">
                        <i class="ph-pencil-simple"></i>
                    </button>
                    <button class="btn-action btn-delete" onclick="deleteData(${item.id})" title="Delete">
                        <i class="ph-trash"></i>
                    </button>
                </div>
            </td>
        </tr>
    `).join('');
}

// Search filter
function setupSearchFilter() {
    const searchInput = document.getElementById('searchInput');
    searchInput.addEventListener('input', function (e) {
        const searchTerm = e.target.value.toLowerCase();
        const rows = document.querySelectorAll('#tableBody tr');

        rows.forEach(row => {
            const text = row.textContent.toLowerCase();
            row.style.display = text.includes(searchTerm) ? '' : 'none';
        });
    });
}

// Open create modal
function openCreateModal() {
    currentEditId = null;
    document.getElementById('modalTitle').textContent = 'Tambah Data ManPower';
    document.getElementById('manPowerForm').reset();
    document.getElementById('manPowerId').value = '';
    document.getElementById('isActive').checked = true;

    document.getElementById('formModal').classList.add('active');
}

// Edit data
async function editData(id) {
    try {
        const response = await fetch(`/ManPower/GetById/${id}`);
        const data = await response.json();

        currentEditId = id;
        document.getElementById('modalTitle').textContent = 'Edit Data ManPower';
        document.getElementById('manPowerId').value = data.id;

        // Fill form with data
        document.getElementById('nik').value = data.nik;
        document.getElementById('namaLengkap').value = data.namaLengkap;
        document.getElementById('jabatan').value = data.jabatan;
        document.getElementById('department').value = data.department;
        document.getElementById('shift').value = data.shift;
        document.getElementById('areaLine').value = data.areaLine || '';
        document.getElementById('noTelepon').value = data.noTelepon || '';
        document.getElementById('email').value = data.email || '';
        document.getElementById('isActive').checked = data.isActive;

        document.getElementById('formModal').classList.add('active');
    } catch (error) {
        console.error('Error loading data:', error);
        showToast('Gagal memuat data', 'error');
    }
}

// Delete data
async function deleteData(id) {
    if (!confirm('Apakah Anda yakin ingin menghapus data ini?')) {
        return;
    }

    try {
        const response = await fetch(`/ManPower/${id}`, {
            method: 'DELETE'
        });

        const result = await response.json();

        if (result.success) {
            showToast('Data berhasil dihapus', 'success');
            loadData();
        } else {
            showToast('Gagal menghapus data', 'error');
        }
    } catch (error) {
        console.error('Error deleting data:', error);
        showToast('Gagal menghapus data', 'error');
    }
}

// Setup form submit
function setupFormSubmit() {
    const form = document.getElementById('manPowerForm');
    form.addEventListener('submit', async function (e) {
        e.preventDefault();

        const formData = new FormData(form);
        const data = {
            Id: parseInt(formData.get('Id')) || 0,
            NIK: formData.get('NIK'),
            NamaLengkap: formData.get('NamaLengkap'),
            Jabatan: formData.get('Jabatan'),
            Department: formData.get('Department'),
            Shift: formData.get('Shift'),
            AreaLine: formData.get('AreaLine'),
            NoTelepon: formData.get('NoTelepon'),
            Email: formData.get('Email'),
            IsActive: document.getElementById('isActive').checked
        };

        const url = currentEditId ? '/ManPower/Edit' : '/ManPower/Create';

        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });

            const result = await response.json();

            if (result.success) {
                showToast(result.message, 'success');
                closeModal();
                loadData();
            } else {
                showToast(result.error || 'Gagal menyimpan data', 'error');
            }
        } catch (error) {
            console.error('Error submitting form:', error);
            showToast('Gagal menyimpan data', 'error');
        }
    });
}

// Close modal
function closeModal() {
    document.getElementById('formModal').classList.remove('active');
    currentEditId = null;
}

// Show toast notification
function showToast(message, type = 'info') {
    const container = document.getElementById('toastContainer');
    if (!container) {
        // Create container if it doesn't exist
        const newContainer = document.createElement('div');
        newContainer.id = 'toastContainer';
        newContainer.style.cssText = 'position: fixed; bottom: 2rem; right: 2rem; z-index: 99999; display: flex; flex-direction: column; gap: 0.75rem;';
        document.body.appendChild(newContainer);
    }

    const toast = document.createElement('div');
    toast.className = 'toast-notification';

    const iconClass = type === 'success' ? 'ph-check-circle' : type === 'error' ? 'ph-x-circle' : 'ph-info';
    const bgColor = type === 'success' ? 'rgba(16, 185, 129, 0.2)' : type === 'error' ? 'rgba(239, 68, 68, 0.2)' : 'rgba(59, 130, 246, 0.2)';
    const borderColor = type === 'success' ? 'rgba(16, 185, 129, 0.4)' : type === 'error' ? 'rgba(239, 68, 68, 0.4)' : 'rgba(59, 130, 246, 0.4)';
    const iconColor = type === 'success' ? '#10b981' : type === 'error' ? '#ef4444' : '#3b82f6';

    toast.style.cssText = `
        background: ${bgColor};
        backdrop-filter: blur(12px);
        border: 1px solid ${borderColor};
        padding: 1rem 1.5rem;
        border-radius: 12px;
        color: #fff;
        display: flex;
        align-items: center;
        gap: 0.75rem;
        box-shadow: 0 10px 25px rgba(0,0,0,0.3);
        min-width: 250px;
        animation: slideIn 0.4s ease forwards;
    `;

    toast.innerHTML = `
        <i class="${iconClass}" style="font-size: 1.25rem; color: ${iconColor};"></i>
        <div style="font-size: 0.9rem; font-weight: 500;">${message}</div>
    `;

    document.getElementById('toastContainer').appendChild(toast);

    setTimeout(() => {
        toast.style.animation = 'slideOut 0.4s ease forwards';
        setTimeout(() => toast.remove(), 400);
    }, 3000);
}

// Close modal when clicking outside
document.addEventListener('click', function (e) {
    const formModal = document.getElementById('formModal');

    if (e.target === formModal) {
        closeModal();
    }
});

// Add badge-shift style
const style = document.createElement('style');
style.textContent = `
    .badge-shift {
        display: inline-block;
        padding: 0.4rem 0.8rem;
        border-radius: 6px;
        font-size: 0.8rem;
        font-weight: 600;
        background: rgba(139, 92, 246, 0.2);
        color: #a78bfa;
        border: 1px solid rgba(139, 92, 246, 0.3);
    }
`;
document.head.appendChild(style);
