// ============================================
// HENKATEN PROBLEM - JAVASCRIPT
// ============================================

// Global data store for filtering
let allData = [];
let currentEditId = null;

// Helper to determine status
function getItemStatus(item) {
    if (!item) return 'Pending';

    // 1. Case-insensitive check for Selesai
    const dbStatus = (item.status || "").toLowerCase();
    if (dbStatus === 'selesai') return 'Selesai';

    // 2. Determine target date (fallback to update date if target is missing)
    const dateStr = item.tanggalRencanaPerbaikan || item.tanggalUpdate;

    if (dateStr && dateStr.includes('/')) {
        const parts = dateStr.split('/');
        if (parts.length === 3) {
            const day = parseInt(parts[0], 10);
            const month = parseInt(parts[1], 10) - 1;
            const year = parseInt(parts[2], 10);

            const targetDate = new Date(year, month, day);
            const today = new Date();
            today.setHours(0, 0, 0, 0);

            if (targetDate.getTime() < today.getTime()) {
                return 'Delay';
            }
        }
    }
    return 'Pending';
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', function () {
    loadData();
    setupSearchFilter();
    setupStatusFilters();
    setupFormSubmit();
    loadDepartmentOptions();
    initSignalR();
});

// SignalR Real-time Update Logic
let connection;
function initSignalR() {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub")
        .withAutomaticReconnect()
        .build();

    connection.on("HenkatenDataUpdated", function (plantName) {
        console.log(`SignalR: Data Henkaten updated for plant ${plantName}`);

        // Refresh data (filter by current plant if applicable, usually handled in GetData backend)
        loadData();

        // Custom notification (optional but helpful)
        showToast(`Data Henkaten ${plantName} baru saja diperbarui`, 'info');
    });

    connection.start().catch(err => console.error("SignalR Connection Error: ", err));
}

async function loadDepartmentOptions() {
    try {
        const response = await fetch('/Department/GetAll');
        const result = await response.json();

        if (result.success) {
            const select = document.getElementById('department');
            // Biarkan -- Pilih Departemen -- tetap ada

            result.data.forEach(dept => {
                const option = document.createElement('option');
                option.value = dept.name;
                option.textContent = dept.name;
                select.appendChild(option);
            });
        }
    } catch (error) {
        console.error('Error loading departments:', error);
    }
}

// Load data from server
async function loadData() {
    try {
        const response = await fetch('/Henkaten/GetData');

        // Check if response is OK before parsing
        if (!response.ok) {
            const errorData = await response.json().catch(() => ({ error: 'Unknown error' }));
            console.error('Error loading data:', errorData);
            showToast(errorData.error || 'Gagal memuat data', 'error');
            allData = [];
            renderTable([]);
            return;
        }

        const data = await response.json();

        // Ensure data is an array
        if (!Array.isArray(data)) {
            console.error('Invalid data format:', data);
            showToast('Format data tidak valid', 'error');
            allData = [];
            renderTable([]);
            return;
        }

        allData = data;
        renderTable(data);
    } catch (error) {
        console.error('Error loading data:', error);
        showToast('Gagal memuat data', 'error');
        allData = [];
        renderTable([]);
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

function applyFilters(filterValue) {
    let filtered = allData;
    if (filterValue !== 'all') {
        filtered = allData.filter(item => {
            const calculatedStatus = getItemStatus(item).toLowerCase();
            return calculatedStatus === filterValue;
        });
    }
    renderTable(filtered);
}

// Render table
function renderTable(data) {
    const tbody = document.getElementById('tableBody');

    // Ensure data is an array
    if (!Array.isArray(data)) {
        console.error('renderTable: data is not an array', data);
        tbody.innerHTML = `
            <tr>
                <td colspan="12" class="empty-state" style="text-align: center; padding: 3rem; color: #64748b;">
                    <i class="ph-database" style="font-size: 3rem; display: block; margin-bottom: 1rem;"></i>
                    Belum ada data temuan
                </td>
            </tr>
        `;
        return;
    }

    if (data.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="12" class="empty-state" style="text-align: center; padding: 3rem; color: #64748b;">
                    <i class="ph-database" style="font-size: 3rem; display: block; margin-bottom: 1rem;"></i>
                    Belum ada data temuan
                </td>
            </tr>
        `;
        return;
    }

    tbody.innerHTML = data.map(item => {
        const currentStatus = getItemStatus(item);
        const statusClass = currentStatus.toLowerCase();

        return `
        <tr>
            <td data-label="TANGGAL">${item.tanggalUpdate}</td>
            <td data-label="SHIFT"><span class="badge-shift ${getShiftClass(item.shift)}">${item.shift}</span></td>
            <td data-label="JENIS HENKATEN (4M)"><span class="badge-4m badge-${item.jenis4M.toLowerCase()}">${item.jenis4M}</span></td>
            <td data-label="4M STANDARD">${item.standard4M || '-'}</td>
            <td data-label="4M ACTUAL">${item.actual4M || '-'}</td>
            <td data-label="ALASAN HENKATEN">${item.keteranganProblem}</td>
            <td data-label="TEMPORARY ACTION">${item.temporaryAction || '-'}</td>
            <td data-label="PERMANENT ACTION">${item.rencanaPerbaikan}</td>
            <td data-label="PIC (LEADER)">${item.picLeader}</td>
            <td data-label="DUE DATE">${item.tanggalRencanaPerbaikan}</td>
            <td data-label="STATUS">
                <span class="status-badge status-${statusClass}">
                    <span class="status-dot"></span>
                    ${currentStatus}
                </span>
            </td>
            <td data-label="AKSI">
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
    `;
    }).join('');
}

// View Detail Problem
async function viewDetail(id) {
    try {
        const response = await fetch(`/Henkaten/GetById/${id}`);

        // Check if response is OK before parsing
        if (!response.ok) {
            const errorData = await response.json().catch(() => ({ error: 'Unknown error' }));
            console.error('Error loading detail:', errorData);
            showToast(errorData.error || 'Gagal memuat detail data', 'error');
            return;
        }

        const data = await response.json();

        const modal = document.getElementById('detailModal');
        const content = document.getElementById('detailContent');

        content.innerHTML = `
            <div class="detail-grid">
                <div class="detail-section full-width">
                    <div class="detail-header-info">
                        <span class="badge-4m badge-${data.jenis4M.toLowerCase()}">${data.jenis4M}</span>
                        <span class="badge-shift ${getShiftClass(data.shift)}">${data.shift}</span>
                        <span class="status-badge status-${getItemStatus(data).toLowerCase()}">
                            <span class="status-dot"></span> ${getItemStatus(data)}
                        </span>
                    </div>
                </div>

                <div class="detail-col">
                    <h4 class="detail-label">Tanggal</h4>
                    <p class="detail-value">${data.tanggalUpdate ? new Date(data.tanggalUpdate).toLocaleDateString('id-ID') : '-'}</p>
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

                <div class="detail-section">
                    <h4 class="detail-label">4M Standard</h4>
                    <div class="detail-box">${data.standard4M || '-'}</div>
                </div>
                <div class="detail-section">
                    <h4 class="detail-label">4M Actual</h4>
                    <div class="detail-box">${data.actual4M || '-'}</div>
                </div>

                <div class="detail-section full-width">
                    <h4 class="detail-label">Alasan Henkaten (Problem)</h4>
                    <div class="detail-box problem-msg">${data.keteranganProblem}</div>
                </div>

                <div class="detail-section">
                    <h4 class="detail-label">Temporary Action</h4>
                    <div class="detail-box">${data.temporaryAction || '-'}</div>
                </div>

                <div class="detail-section">
                    <h4 class="detail-label">Permanent Action</h4>
                    <div class="detail-box">${data.rencanaPerbaikan}</div>
                    <div class="detail-sub-info">
                        <span>Due Date: ${data.tanggalRencanaPerbaikan ? new Date(data.tanggalRencanaPerbaikan).toLocaleDateString('id-ID') : '-'}</span>
                    </div>
                </div>

                <div class="detail-section">
                    <h4 class="detail-label">Foto Temuan</h4>
                    <div class="detail-photo-container">
                        ${data.fotoTemuan ? `<img src="${fixImagePath(data.fotoTemuan)}" alt="Temuan" onerror="this.src='/images/no-image.png'; this.classList.add('broken');">` : '<div class="no-photo">Tidak ada foto</div>'}
                    </div>
                </div>

                <div class="detail-section">
                    <h4 class="detail-label">Foto Aktual</h4>
                    <div class="detail-photo-container">
                        ${data.fotoAktual ? `<img src="${fixImagePath(data.fotoAktual)}" alt="Aktual" onerror="this.src='/images/no-image.png'; this.classList.add('broken');">` : '<div class="no-photo">Tidak ada foto</div>'}
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

    // Remove Id field for create (or set to empty)
    const problemIdField = document.getElementById('problemId');
    if (problemIdField) {
        problemIdField.remove(); // Remove the field completely for create
    }

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

        // Check if response is OK before parsing
        if (!response.ok) {
            const errorData = await response.json().catch(() => ({ error: 'Unknown error' }));
            console.error('Error loading data:', errorData);
            showToast(errorData.error || 'Gagal memuat data', 'error');
            return;
        }

        const data = await response.json();

        currentEditId = id;
        document.getElementById('modalTitle').textContent = 'Edit Temuan Problem Henkaten';

        // Ensure Id field exists for edit
        let problemIdField = document.getElementById('problemId');
        if (!problemIdField) {
            // Create the hidden field if it doesn't exist
            const form = document.getElementById('henkatenForm');
            problemIdField = document.createElement('input');
            problemIdField.type = 'hidden';
            problemIdField.id = 'problemId';
            problemIdField.name = 'Id';
            form.insertBefore(problemIdField, form.firstChild);
        }
        problemIdField.value = data.id;

        // Fill form with data
        document.getElementById('tanggalUpdate').value = formatDateForInput(data.tanggalUpdate);
        document.getElementById('department').value = data.department || '';
        document.getElementById('shift').value = data.shift;
        document.getElementById('picLeader').value = data.picLeader;
        document.getElementById('namaAreaLine').value = data.namaAreaLine;
        document.getElementById('namaOperator').value = data.namaOperator;
        document.getElementById('jenis4M').value = data.jenis4M;
        document.getElementById('standard4M').value = data.standard4M || '';
        document.getElementById('actual4M').value = data.actual4M || '';
        document.getElementById('keteranganProblem').value = data.keteranganProblem;
        document.getElementById('temporaryAction').value = data.temporaryAction || '';
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

        // Check if response is OK before parsing
        if (!response.ok) {
            const errorData = await response.json().catch(() => ({ error: 'Unknown error' }));
            console.error('Error deleting data:', errorData);
            showToast(errorData.error || 'Gagal menghapus data', 'error');
            return;
        }

        const result = await response.json();

        if (result.success) {
            showToast('Data berhasil dihapus', 'success');
            loadData();
        } else {
            showToast(result.error || 'Gagal menghapus data', 'error');
        }
    } catch (error) {
        console.error('Error deleting data:', error);
        showToast('Gagal menghapus data: ' + error.message, 'error');
    }
}

// Setup form submit
function setupFormSubmit() {
    const form = document.getElementById('henkatenForm');
    form.addEventListener('submit', async function (e) {
        e.preventDefault();

        // Client-side validation
        const requiredFields = {
            'TanggalUpdate': 'Tanggal Update',
            'Shift': 'Shift',
            'PicLeader': 'PIC Leader',
            'NamaAreaLine': 'Nama Area/Line',
            'NamaOperator': 'Nama Operator',
            'Jenis4M': 'Jenis 4M',
            'KeteranganProblem': 'Keterangan Problem',
            'RencanaPerbaikan': 'Rencana Perbaikan',
            'TanggalRencanaPerbaikan': 'Tanggal Rencana Perbaikan'
        };

        const missingFields = [];
        for (const [fieldName, fieldLabel] of Object.entries(requiredFields)) {
            const field = form.querySelector(`[name="${fieldName}"]`);
            if (field && !field.value.trim()) {
                missingFields.push(fieldLabel);
            }
        }

        if (missingFields.length > 0) {
            showToast(`Mohon lengkapi field: ${missingFields.join(', ')}`, 'error');
            return;
        }

        const formData = new FormData(form);
        const url = currentEditId ? '/Henkaten/Edit' : '/Henkaten/Create';

        try {
            // Log form data for debugging
            console.log('Submitting form data...');
            const formDataEntries = {};
            for (let pair of formData.entries()) {
                formDataEntries[pair[0]] = pair[1];
            }
            console.log('Form data:', formDataEntries);

            const response = await fetch(url, {
                method: 'POST',
                body: formData
            });

            // Check if response is OK before parsing
            if (!response.ok) {
                let errorData;
                try {
                    const responseText = await response.text();
                    console.error('Raw response text:', responseText);
                    errorData = JSON.parse(responseText);
                } catch (e) {
                    console.error('Failed to parse error response:', e);
                    errorData = { error: `Server error: ${response.status} ${response.statusText}` };
                }
                console.error('Error submitting form:', errorData);
                console.error('Response status:', response.status);
                console.error('Response statusText:', response.statusText);

                // Build error message
                let errorMessage = errorData.error || `Gagal menyimpan data (${response.status})`;
                if (errorData.details) {
                    if (Array.isArray(errorData.details)) {
                        const fieldErrors = errorData.details.map(d => {
                            if (d.Errors && Array.isArray(d.Errors)) {
                                return `${d.Field || d.field || 'Unknown'}: ${d.Errors.join(', ')}`;
                            } else if (d.Errors) {
                                return `${d.Field || d.field || 'Unknown'}: ${d.Errors}`;
                            } else {
                                return `${d.Field || d.field || 'Unknown'}: ${JSON.stringify(d)}`;
                            }
                        }).join('; ');
                        errorMessage += ' - ' + fieldErrors;
                    } else {
                        errorMessage += ' - ' + (typeof errorData.details === 'string' ? errorData.details : JSON.stringify(errorData.details));
                    }
                }

                showToast(errorMessage, 'error');
                return;
            }

            const result = await response.json();

            if (result.success) {
                showToast(result.message, 'success');
                closeModal();
                loadData();
            } else {
                let errorMessage = result.error || 'Gagal menyimpan data';
                if (result.details) {
                    if (Array.isArray(result.details)) {
                        const fieldErrors = result.details.map(d => {
                            if (d.Errors && Array.isArray(d.Errors)) {
                                return `${d.Field || d.field || 'Unknown'}: ${d.Errors.join(', ')}`;
                            } else if (d.Errors) {
                                return `${d.Field || d.field || 'Unknown'}: ${d.Errors}`;
                            } else {
                                return `${d.Field || d.field || 'Unknown'}: ${JSON.stringify(d)}`;
                            }
                        }).join('; ');
                        errorMessage += ' - ' + fieldErrors;
                    } else {
                        errorMessage += ' - ' + (typeof result.details === 'string' ? result.details : JSON.stringify(result.details));
                    }
                }
                showToast(errorMessage, 'error');
            }
        } catch (error) {
            console.error('Error submitting form:', error);
            showToast('Gagal menyimpan data: ' + error.message, 'error');
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

    const fullPath = fixImagePath(imagePath);

    content.innerHTML = `
        <div style="text-align: center;">
            <img src="${fullPath}" alt="Foto" style="max-width: 100%; max-height: 70vh; border-radius: 12px;" onerror="this.src='/images/no-image.png';">
            <div style="margin-top: 10px; color: #94a3b8; font-size: 0.8rem;">${imagePath}</div>
        </div>
    `;

    modal.classList.add('active');
}

function fixImagePath(path) {
    if (!path) return '';
    if (path.startsWith('http') || path.startsWith('/') || path.startsWith('data:')) return path;
    return '/uploads/henkaten/' + path;
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
        box-shadow: 0 10px 25px rgba(0, 0, 0, 0.3);
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

// Export to Excel
function exportToExcel() {
    try {
        // Show loading indicator
        showToast('Mengekspor data ke Excel...', 'info');

        // Create a link to download the file
        const link = document.createElement('a');
        link.href = '/Henkaten/ExportToExcel';
        link.download = 'Henkaten_Export.xlsx';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);

        // Show success message after a short delay
        setTimeout(() => {
            showToast('Data berhasil diekspor', 'success');
        }, 1000);
    } catch (error) {
        console.error('Error exporting to Excel:', error);
        showToast('Gagal mengekspor data', 'error');
    }
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
