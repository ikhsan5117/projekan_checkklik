// ============================================
// HENKATEN PROBLEM - JAVASCRIPT
// ============================================

// Global data store for filtering
let allData = [];

// Initialize on page load
document.addEventListener('DOMContentLoaded', function () {
    loadData();
    setupSearchFilter();
    setupStatusFilters();
    setupFormSubmit();
});

// Load data from server
async function loadData() {
    try {
        const response = await fetch('/Henkaten/GetData');
        const data = await response.json();

        allData = data;
        renderTable(data);
    } catch (error) {
        console.error('Error loading data:', error);
        showToast('Gagal memuat data', 'error');
    }
}

// Status Filter Logic
function setupStatusFilters() {
    const filterBtns = document.querySelectorAll('.btn-filter');
    filterBtns.forEach(btn => {
        btn.addEventListener('click', function () {
            // UI Update
            filterBtns.forEach(b => b.classList.remove('active'));
            this.classList.add('active');

            const filterValue = this.getAttribute('data-filter');
            applyFilters(filterValue);
        });
    });
}

function applyFilters(status) {
    let filtered = allData;
    if (status !== 'all') {
        filtered = allData.filter(item => item.status.toLowerCase() === status);
    }
    renderTable(filtered);
}

// Render table
function renderTable(data) {
    const tbody = document.getElementById('tableBody');

    if (!data || data.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="10" style="text-align: center; padding: 3rem; color: #64748b;">
                    <i class="ph-database" style="font-size: 3rem; display: block; margin-bottom: 1rem;"></i>
                    Belum ada data temuan
                </td>
            </tr>
        `;
        return;
    }

    tbody.innerHTML = data.map(item => `
        <tr>
            <td>${item.tanggalUpdate}</td>
            <td><span class="badge-shift ${getShiftClass(item.shift)}">${item.shift}</span></td>
            <td>${item.picLeader}</td>
            <td>${item.namaAreaLine}</td>
            <td>${item.namaOperator}</td>
            <td><span class="badge-4m badge-${item.jenis4M.toLowerCase()}">${item.jenis4M}</span></td>
            <td>
                ${item.fotoTemuan ? `
                    <div class="media-icon" onclick="viewImage('${item.fotoTemuan}')">
                        <i class="ph-image"></i>
                    </div>
                ` : '-'}
            </td>
            <td>${item.tanggalRencanaPerbaikan}</td>
            <td>
                <span class="status-badge status-${item.status.toLowerCase()}">
                    <span class="status-dot"></span>
                    ${item.status}
                </span>
            </td>
            <td>
                <div class="action-buttons">
                    <button class="btn-action btn-edit" onclick="editData(${item.id})" title="Edit">
                        <i class="ph-pencil-simple"></i>
                    </button>
                    <button class="btn-action btn-view" onclick="viewDetail(${item.id})" title="View Detail">
                        <i class="ph-eye"></i>
                    </button>
                    <button class="btn-action btn-delete" onclick="deleteData(${item.id})" title="Delete">
                        <i class="ph-trash"></i>
                    </button>
                </div>
            </td>
        </tr>
    `).join('');
}

// View Detail Problem
async function viewDetail(id) {
    try {
        const response = await fetch(`/Henkaten/GetById/${id}`);
        const data = await response.json();

        const modal = document.getElementById('detailModal');
        const content = document.getElementById('detailContent');

        content.innerHTML = `
            <div class="detail-grid">
                <div class="detail-section full-width">
                    <div class="detail-header-info">
                        <span class="badge-4m badge-${data.jenis4M.toLowerCase()}">${data.jenis4M}</span>
                        <span class="badge-shift ${getShiftClass(data.shift)}">${data.shift}</span>
                        <span class="status-badge status-${data.status.toLowerCase()}">
                            <span class="status-dot"></span> ${data.status}
                        </span>
                    </div>
                </div>

                <div class="detail-col">
                    <h4 class="detail-label">Tanggal Update</h4>
                    <p class="detail-value">${data.tanggalUpdate}</p>
                </div>
                <div class="detail-col">
                    <h4 class="detail-label">PIC Leader</h4>
                    <p class="detail-value">${data.picLeader}</p>
                </div>
                <div class="detail-col">
                    <h4 class="detail-label">Area / Line</h4>
                    <p class="detail-value">${data.namaAreaLine}</p>
                </div>
                <div class="detail-col">
                    <h4 class="detail-label">Operator</h4>
                    <p class="detail-value">${data.namaOperator}</p>
                </div>

                <div class="detail-section full-width">
                    <h4 class="detail-label">Keterangan Problem</h4>
                    <div class="detail-box problem-msg">${data.keteranganProblem}</div>
                </div>

                <div class="detail-section">
                    <h4 class="detail-label">Rencana Perbaikan</h4>
                    <div class="detail-box">${data.rencanaPerbaikan}</div>
                    <div class="detail-sub-info">
                        <span>Target: ${data.tanggalRencanaPerbaikan}</span>
                    </div>
                </div>

                <div class="detail-section">
                    <h4 class="detail-label">Aktual Perbaikan</h4>
                    <div class="detail-box">${data.aktualPerbaikan || 'Belum ada data aktual'}</div>
                    <div class="detail-sub-info">
                        <span>Selesai: ${data.tanggalAktualPerbaikan || '-'}</span>
                    </div>
                </div>

                <div class="detail-section">
                    <h4 class="detail-label">Foto Temuan</h4>
                    <div class="detail-photo-container">
                        ${data.fotoTemuan ? `<img src="${data.fotoTemuan}" alt="Temuan">` : '<div class="no-photo">Tidak ada foto</div>'}
                    </div>
                </div>

                <div class="detail-section">
                    <h4 class="detail-label">Foto Aktual</h4>
                    <div class="detail-photo-container">
                        ${data.fotoAktual ? `<img src="${data.fotoAktual}" alt="Aktual">` : '<div class="no-photo">Tidak ada foto</div>'}
                    </div>
                </div>
            </div>
        `;

        modal.classList.add('active');
    } catch (error) {
        console.error('Error loading detail:', error);
        showToast('Gagal memuat detail data', 'error');
    }
}

// Helper to get shift class
function getShiftClass(shiftText) {
    if (!shiftText) return 'shift-1';
    if (shiftText.includes('1')) return 'shift-1';
    if (shiftText.includes('2')) return 'shift-2';
    if (shiftText.includes('3')) return 'shift-3';
    return 'shift-1';
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
    document.getElementById('modalTitle').textContent = 'Input Temuan Problem Henkaten';
    document.getElementById('henkatenForm').reset();
    document.getElementById('problemId').value = '';
    document.getElementById('previewTemuan').innerHTML = '';
    document.getElementById('previewAktual').innerHTML = '';
    document.getElementById('previewTemuan').classList.remove('active');
    document.getElementById('previewAktual').classList.remove('active');

    // Set default date to today
    const today = new Date().toISOString().split('T')[0];
    document.getElementById('tanggalUpdate').value = today;

    document.getElementById('formModal').classList.add('active');
}

// Edit data
async function editData(id) {
    try {
        const response = await fetch(`/Henkaten/GetById/${id}`);
        const data = await response.json();

        currentEditId = id;
        document.getElementById('modalTitle').textContent = 'Edit Temuan Problem Henkaten';
        document.getElementById('problemId').value = data.id;

        // Fill form with data
        document.getElementById('tanggalUpdate').value = formatDateForInput(data.tanggalUpdate);
        document.getElementById('shift').value = data.shift;
        document.getElementById('picLeader').value = data.picLeader;
        document.getElementById('namaAreaLine').value = data.namaAreaLine;
        document.getElementById('namaOperator').value = data.namaOperator;
        document.getElementById('jenis4M').value = data.jenis4M;
        document.getElementById('keteranganProblem').value = data.keteranganProblem;
        document.getElementById('rencanaPerbaikan').value = data.rencanaPerbaikan;
        document.getElementById('tanggalRencanaPerbaikan').value = formatDateForInput(data.tanggalRencanaPerbaikan);
        document.getElementById('aktualPerbaikan').value = data.aktualPerbaikan || '';
        document.getElementById('tanggalAktualPerbaikan').value = data.tanggalAktualPerbaikan ? formatDateForInput(data.tanggalAktualPerbaikan) : '';

        // Show existing images
        if (data.fotoTemuan) {
            const previewTemuan = document.getElementById('previewTemuan');
            previewTemuan.innerHTML = `
                <img src="${data.fotoTemuan}" alt="Foto Temuan">
                <button type="button" class="btn-remove-image" onclick="removeImage(event, 'fotoTemuan', 'previewTemuan')">
                    <i class="ph-trash"></i>
                </button>
            `;
            previewTemuan.classList.add('active');
        }

        if (data.fotoAktual) {
            const previewAktual = document.getElementById('previewAktual');
            previewAktual.innerHTML = `
                <img src="${data.fotoAktual}" alt="Foto Aktual">
                <button type="button" class="btn-remove-image" onclick="removeImage(event, 'fotoAktual', 'previewAktual')">
                    <i class="ph-trash"></i>
                </button>
            `;
            previewAktual.classList.add('active');
        }

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
        const response = await fetch(`/Henkaten/${id}`, {
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
    const form = document.getElementById('henkatenForm');
    form.addEventListener('submit', async function (e) {
        e.preventDefault();

        const formData = new FormData(form);
        const url = currentEditId ? '/Henkaten/Edit' : '/Henkaten/Create';

        try {
            const response = await fetch(url, {
                method: 'POST',
                body: formData
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

// Close detail modal
function closeDetailModal() {
    document.getElementById('detailModal').classList.remove('active');
}

// Preview image
function previewImage(input, previewId) {
    const preview = document.getElementById(previewId);

    if (input.files && input.files[0]) {
        const reader = new FileReader();

        reader.onload = function (e) {
            preview.innerHTML = `
                <img src="${e.target.result}" alt="Preview">
                <button type="button" class="btn-remove-image" onclick="removeImage(event, '${input.id}', '${previewId}')">
                    <i class="ph-trash"></i>
                </button>
            `;
            preview.classList.add('active');
        };

        reader.readAsDataURL(input.files[0]);
    }
}

// Remove image
function removeImage(event, inputId, previewId) {
    event.stopPropagation(); // Prevent triggering the container click
    const input = document.getElementById(inputId);
    const preview = document.getElementById(previewId);

    input.value = '';
    preview.innerHTML = '';
    preview.classList.remove('active');
}

// View image in modal
function viewImage(imagePath) {
    const modal = document.getElementById('detailModal');
    const content = document.getElementById('detailContent');

    content.innerHTML = `
        <div style="text-align: center;">
            <img src="${imagePath}" alt="Foto" style="max-width: 100%; max-height: 70vh; border-radius: 12px;">
        </div>
    `;

    modal.classList.add('active');
}

// Format date for input field (YYYY-MM-DD)
function formatDateForInput(dateString) {
    if (!dateString) return '';

    // If it's an ISO string (contains 'T'), take only the date part
    if (dateString.includes('T')) {
        return dateString.split('T')[0];
    }

    // If in DD/MM/YYYY format
    const parts = dateString.split('/');
    if (parts.length === 3) {
        return `${parts[2]}-${parts[1]}-${parts[0]}`;
    }

    // If already in YYYY-MM-DD format but has extra characters
    if (dateString.length > 10) {
        return dateString.substring(0, 10);
    }

    return dateString;
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
    const detailModal = document.getElementById('detailModal');

    if (e.target === formModal) {
        closeModal();
    }

    if (e.target === detailModal) {
        closeDetailModal();
    }
});

