document.addEventListener('DOMContentLoaded', () => {
    // Initial data load
    loadAnalyticsData();
});

let charts = {};

async function loadAnalyticsData() {
    try {
        const response = await fetch('/Henkaten/GetData');
        if (!response.ok) throw new Error('Network response was not ok');
        const data = await response.json();

        processAnalytics(data);
    } catch (error) {
        console.error('Error loading analytics data:', error);
    }
}

function processAnalytics(data) {
    // 1. Update Stats Overview
    updateStats(data);

    // 2. Prepare Data for Charts
    const statusData = prepareStatusData(data);
    const categoryData = prepareCategoryData(data);
    const shiftData = prepareShiftData(data);
    const trendData = prepareTrendData(data);

    // 3. Render Charts
    renderStatusChart(statusData);
    renderCategoryChart(categoryData);
    renderShiftChart(shiftData);
    renderTrendChart(trendData);
}

function updateStats(data) {
    const total = data.length;
    const pending = data.filter(item => item.status === 'Pending').length;
    const selesai = data.filter(item => item.status === 'Selesai').length;
    const ratio = total > 0 ? Math.round((selesai / total) * 100) : 0;

    document.getElementById('totalTemuan').textContent = total;
    document.getElementById('pendingTemuan').textContent = pending;
    document.getElementById('selesaiTemuan').textContent = selesai;
    document.getElementById('ratioSelesai').textContent = `${ratio}%`;
}

// Prepare Data Helpers
function prepareStatusData(data) {
    const pending = data.filter(item => item.status === 'Pending').length;
    const selesai = data.filter(item => item.status === 'Selesai').length;
    return [selesai, pending];
}

function prepareCategoryData(data) {
    const categories = ['Man', 'Machine', 'Material', 'Method'];
    return categories.map(cat => data.filter(item => item.jenis4M === cat).length);
}

function prepareShiftData(data) {
    const shifts = ['Shift 1', 'Shift 2', 'Shift 3'];
    return shifts.map(s => data.filter(item => item.shift === s).length);
}

function prepareTrendData(data) {
    // Group by month/year
    const months = {};
    data.forEach(item => {
        // Assume format dd/MM/yyyy
        const parts = item.tanggalUpdate.split('/');
        if (parts.length === 3) {
            const monthYear = `${parts[1]}/${parts[2]}`; // MM/YYYY
            months[monthYear] = (months[monthYear] || 0) + 1;
        }
    });

    // Sort months effectively (basic sort for now)
    const sortedKeys = Object.keys(months).sort();
    return {
        labels: sortedKeys,
        values: sortedKeys.map(key => months[key])
    };
}

// Rendering Functions
function renderStatusChart(data) {
    const ctx = document.getElementById('statusChart').getContext('2d');
    if (charts.status) charts.status.destroy();

    charts.status = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: ['Selesai', 'Pending'],
            datasets: [{
                data: data,
                backgroundColor: [
                    'rgba(16, 185, 129, 0.7)', // Green
                    'rgba(245, 158, 11, 0.7)'  // Orange
                ],
                borderColor: [
                    'rgba(16, 185, 129, 1)',
                    'rgba(245, 158, 11, 1)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: { color: '#94a3b8', font: { weight: 'bold' } }
                }
            }
        }
    });
}

function renderCategoryChart(data) {
    const ctx = document.getElementById('categoryChart').getContext('2d');
    if (charts.category) charts.category.destroy();

    charts.category = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['Man', 'Machine', 'Material', 'Method'],
            datasets: [{
                label: 'Jumlah Temuan',
                data: data,
                backgroundColor: [
                    'rgba(59, 130, 246, 0.7)',
                    'rgba(139, 92, 246, 0.7)',
                    'rgba(236, 72, 153, 0.7)',
                    'rgba(245, 158, 11, 0.7)'
                ],
                borderRadius: 8
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    grid: { color: 'rgba(255, 255, 255, 0.05)' },
                    ticks: { color: '#64748b' }
                },
                x: {
                    grid: { display: false },
                    ticks: { color: '#64748b', font: { weight: 'bold' } }
                }
            }
        }
    });
}

function renderShiftChart(data) {
    const ctx = document.getElementById('shiftChart').getContext('2d');
    if (charts.shift) charts.shift.destroy();

    charts.shift = new Chart(ctx, {
        type: 'polarArea',
        data: {
            labels: ['Shift 1', 'Shift 2', 'Shift 3'],
            datasets: [{
                data: data,
                backgroundColor: [
                    'rgba(59, 130, 246, 0.6)',
                    'rgba(16, 185, 129, 0.6)',
                    'rgba(139, 92, 246, 0.6)'
                ],
                borderColor: 'rgba(255, 255, 255, 0.1)',
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                r: {
                    grid: { color: 'rgba(255, 255, 255, 0.05)' },
                    angleLines: { color: 'rgba(255, 255, 255, 0.05)' },
                    ticks: { display: false }
                }
            },
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: { color: '#94a3b8' }
                }
            }
        }
    });
}

function renderTrendChart(data) {
    const ctx = document.getElementById('trendChart').getContext('2d');
    if (charts.trend) charts.trend.destroy();

    const gradient = ctx.createLinearGradient(0, 0, 0, 400);
    gradient.addColorStop(0, 'rgba(245, 158, 11, 0.4)');
    gradient.addColorStop(1, 'rgba(245, 158, 11, 0)');

    charts.trend = new Chart(ctx, {
        type: 'line',
        data: {
            labels: data.labels,
            datasets: [{
                label: 'Total Temuan',
                data: data.values,
                borderColor: '#f59e0b',
                backgroundColor: gradient,
                fill: true,
                tension: 0.4,
                pointBackgroundColor: '#fff',
                pointBorderColor: '#f59e0b',
                pointHoverRadius: 6
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    grid: { color: 'rgba(255, 255, 255, 0.05)' },
                    ticks: { color: '#64748b' }
                },
                x: {
                    grid: { color: 'rgba(255, 255, 255, 0.05)' },
                    ticks: { color: '#64748b' }
                }
            }
        }
    });
}
