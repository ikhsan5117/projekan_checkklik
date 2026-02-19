document.addEventListener('DOMContentLoaded', () => {
    const today = new Date();
    const filterInput = document.getElementById('filterDate');

    const formatDate = (date) => date.toISOString().split('T')[0];

    // Set default to today
    filterInput.value = formatDate(today);

    // Filter automatically when date is changed
    filterInput.addEventListener('change', filterData);

    document.getElementById('btnResetFilter').addEventListener('click', () => {
        filterInput.value = '';
        activeFilterStatus = null;
        activeFilterCategory = null;
        activeFilterMonth = null;
        filterData();
    });

    // Table Click Interaction
    document.getElementById('tableBody').addEventListener('click', (e) => {
        const badge = e.target.closest('.cat-badge');
        const pill = e.target.closest('.status-pill');

        if (badge) {
            const cat = badge.textContent.trim();
            activeFilterCategory = activeFilterCategory === cat ? null : cat;
            filterData();
        } else if (pill) {
            const status = pill.textContent.trim();
            activeFilterStatus = activeFilterStatus === status ? null : status;
            filterData();
        }
    });

    loadAnalyticsData();
});

let charts = {};
let allAnalyticsData = [];
let activeFilterStatus = null;
let activeFilterCategory = null;
let activeFilterMonth = null;

async function loadAnalyticsData() {
    try {
        const response = await fetch('/Henkaten/GetData');
        if (!response.ok) throw new Error('Network response was not ok');
        const data = await response.json();
        allAnalyticsData = data;

        // Show all data initially (No filter)
        document.getElementById('filterDate').value = '';
        processAnalytics(allAnalyticsData);

    } catch (error) {
        console.error('Error loading analytics data:', error);
    }
}

function filterData() {
    const selectedDateValue = document.getElementById('filterDate').value;

    let filtered = [...allAnalyticsData];

    // 1. Date Filter
    if (selectedDateValue) {
        const [year, month, day] = selectedDateValue.split('-');
        const formattedSelected = `${day}/${month}/${year}`;
        filtered = filtered.filter(item => item.tanggalUpdate === formattedSelected);
    }

    // 2. Status Filter
    if (activeFilterStatus) {
        filtered = filtered.filter(item => getItemStatus(item) === activeFilterStatus);
    }

    // 3. Category Filter
    if (activeFilterCategory) {
        filtered = filtered.filter(item => (item.jenis4M || '').toLowerCase() === activeFilterCategory.toLowerCase());
    }

    // 4. Month Filter
    if (activeFilterMonth !== null) {
        const currentYear = new Date().getFullYear();
        filtered = filtered.filter(item => {
            if (!item.tanggalUpdate) return false;
            const parts = item.tanggalUpdate.split('/');
            const itemMonth = parseInt(parts[1], 10) - 1;
            const itemYear = parseInt(parts[2], 10);
            return itemMonth === activeFilterMonth && itemYear === currentYear;
        });
    }

    processAnalytics(filtered);
}

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
            // Parse dd/MM/yyyy safely
            const day = parseInt(parts[0], 10);
            const month = parseInt(parts[1], 10) - 1;
            const year = parseInt(parts[2], 10);

            const targetDate = new Date(year, month, day);
            const today = new Date();
            today.setHours(0, 0, 0, 0); // Normalize today to midnight

            // If target date is BEFORE today, it's a Delay
            if (targetDate.getTime() < today.getTime()) {
                return 'Delay';
            }
        }
    }

    return 'Pending';
}

function processAnalytics(data) {
    // 0. Update Filter Indicators
    updateFilterChips();

    // 1. Update Stats Overview
    updateStats(data);

    // 2. Prepare Data for Charts
    const statusData = prepareStatusData(data);
    const categoryData = prepareCategoryData(data);
    const trendData = prepareTrendData(allAnalyticsData); // Trend usually uses all data

    renderStatusChart(statusData);
    renderCategoryChart(categoryData);
    renderTrendChart(trendData);

    // 4. Render Table
    renderTable(data);
}

function renderTable(data) {
    const tbody = document.getElementById('tableBody');
    tbody.innerHTML = '';

    // Show all data (scrollbar will handle overflow)
    const displayData = [...data].reverse();

    displayData.forEach((item, index) => {
        const row = document.createElement('tr');
        const currentStatus = getItemStatus(item);
        const statusClass = currentStatus === 'Selesai' ? 'status-selesai' :
            currentStatus === 'Delay' ? 'status-delay' : 'status-pending';

        // 4M Category Styling
        const cat = (item.jenis4M || 'Man').toLowerCase();
        const catClass = `cat-${cat}`;
        const catColors = { man: '#38bdf8', machine: '#c084fc', material: '#a5b4fc', method: '#fda4af' };
        const accentColor = catColors[cat] || '#64748b';

        const problemText = item.keteranganProblem || item.problem || 'N/A';
        const targetText = item.rencanaPerbaikan || item.target || '-';

        row.innerHTML = `
            <td style="font-family: 'monospace'; opacity: 0.5; font-size: 0.75rem;">${String(index + 1).padStart(2, '0')}</td>
            <td style="font-weight: 600; color: #f8fafc;">${item.tanggalUpdate || '-'}</td>
            <td>
                <span style="background: rgba(255,255,255,0.05); padding: 2px 8px; border-radius: 4px; font-size: 0.7rem; font-weight: 700; color: #94a3b8; border: 1px solid rgba(255,255,255,0.05);">
                    ${item.shift || '-'}
                </span>
            </td>
            <td style="max-width: 300px;" title="${problemText}">
                <div style="display: flex; align-items: flex-start; gap: 10px;">
                    <div style="width: 3px; height: 18px; background: ${accentColor}; border-radius: 10px; margin-top: 2px; box-shadow: 0 0 10px ${accentColor}44;"></div>
                    <span style="color: #e2e8f0; line-height: 1.4;">${problemText}</span>
                </div>
            </td>
            <td>
                <div style="color: #cbd5e1; margin-bottom: 2px;">${targetText}</div>
                <div style="display: flex; align-items: center; gap: 4px; color: #64748b; font-size: 0.65rem; font-weight: 600; text-transform: uppercase;">
                    <i class="ph-calendar"></i> Target: ${item.tanggalRencanaPerbaikan || '-'}
                </div>
            </td>
            <td><span class="cat-badge ${catClass}">${item.jenis4M || '-'}</span></td>
            <td><span class="status-pill ${statusClass}">${currentStatus}</span></td>
        `;
        tbody.appendChild(row);
    });
}

function updateStats(filteredData) {
    // Now using filteredData to respond to date filters
    const total = filteredData.length;
    const itemsWithStatus = filteredData.map(item => ({ ...item, currentStatus: getItemStatus(item) }));

    const pending = itemsWithStatus.filter(item => item.currentStatus === 'Pending').length;
    const selesai = itemsWithStatus.filter(item => item.currentStatus === 'Selesai').length;
    const delay = itemsWithStatus.filter(item => item.currentStatus === 'Delay').length;

    document.getElementById('totalTemuan').textContent = total;
    document.getElementById('pendingTemuan').textContent = pending;
    document.getElementById('selesaiTemuan').textContent = selesai;
    document.getElementById('delayTemuan').textContent = delay;

    // Dynamic descriptions based on filtered context
    if (total > 0) {
        const pPerc = Math.round((pending / total) * 100);
        const sPerc = Math.round((selesai / total) * 100);
        const dPerc = Math.round((delay / total) * 100);

        document.getElementById('pendingDesc').innerHTML = `<i class="ph-activity"></i> ${pPerc}% DARI TOTAL`;
        document.getElementById('selesaiDesc').innerHTML = `<i class="ph-trend-up"></i> ${sPerc}% BERHASIL`;
        document.getElementById('delayDesc').innerHTML = `<i class="ph-warning-octagon"></i> ${dPerc}% OVERDUE`;
        document.getElementById('totalDesc').innerHTML = `<i class="ph-chart-line"></i> HISTORI TEMUAN`;
    }
}

// Prepare Data Helpers
function prepareStatusData(data) {
    const itemsWithStatus = data.map(item => getItemStatus(item));
    const selesai = itemsWithStatus.filter(s => s === 'Selesai').length;
    const pending = itemsWithStatus.filter(s => s === 'Pending').length;
    const delay = itemsWithStatus.filter(s => s === 'Delay').length;
    return [selesai, pending, delay];
}

function prepareCategoryData(data) {
    const categories = ['Man', 'Machine', 'Material', 'Method'];
    return categories.map(cat => data.filter(item => item.jenis4M === cat).length);
}

function prepareTrendData(data) {
    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'Mei', 'Jun', 'Jul', 'Agu', 'Sep', 'Okt', 'Nov', 'Des'];
    const currentYear = new Date().getFullYear();

    const counts = months.map((month, index) => {
        return data.filter(item => {
            if (!item.tanggalUpdate) return false;
            const parts = item.tanggalUpdate.split('/');
            const itemMonth = parseInt(parts[1], 10) - 1;
            const itemYear = parseInt(parts[2], 10);
            return itemMonth === index && itemYear === currentYear;
        }).length;
    });

    return { labels: months, counts: counts };
}

// Rendering Functions
// Helper to create gradient
function createGradient(ctx, colorStart, colorEnd) {
    const gradient = ctx.createLinearGradient(0, 0, 0, 400);
    gradient.addColorStop(0, colorStart);
    gradient.addColorStop(1, colorEnd);
    return gradient;
}

// Rendering Functions
// Rendering Functions
function renderStatusChart(data) {
    const ctx = document.getElementById('statusChart').getContext('2d');
    if (charts.status) charts.status.destroy();

    // Bright Crystal Glass Gradients (Vibrant & Translucent)
    const gradientSelesai = ctx.createLinearGradient(0, 0, 0, 300);
    gradientSelesai.addColorStop(0, 'rgba(52, 211, 153, 0.95)'); // Emerald Cerah
    gradientSelesai.addColorStop(1, 'rgba(16, 185, 129, 0.8)');

    const gradientPending = ctx.createLinearGradient(0, 0, 0, 300);
    gradientPending.addColorStop(0, 'rgba(251, 191, 36, 0.95)'); // Amber Cerah
    gradientPending.addColorStop(1, 'rgba(245, 158, 11, 0.8)');

    const gradientDelay = ctx.createLinearGradient(0, 0, 0, 300);
    gradientDelay.addColorStop(0, 'rgba(248, 113, 113, 0.95)'); // Red Cerah
    gradientDelay.addColorStop(1, 'rgba(239, 68, 68, 0.8)');

    charts.status = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: ['Selesai', 'Pending', 'Delay'],
            datasets: [{
                data: data,
                backgroundColor: [
                    gradientSelesai,
                    gradientPending,
                    gradientDelay
                ].map((g, i) => {
                    const status = ['Selesai', 'Pending', 'Delay'][i];
                    return (activeFilterStatus === null || activeFilterStatus === status) ? g : 'rgba(255,255,255,0.05)';
                }),
                hoverBackgroundColor: ['rgba(52, 211, 153, 1)', 'rgba(251, 191, 36, 1)', 'rgba(248, 113, 113, 1)'],
                borderWidth: 2,
                borderColor: 'rgba(255, 255, 255, 0.45)', // Sisi putih tipis (glass-edge)
                borderRadius: 0,
                spacing: 12,
                hoverOffset: 15
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            cutout: '72%',
            layout: {
                padding: {
                    top: 35,
                    bottom: 35,
                    left: 65,
                    right: 65
                }
            },
            plugins: {
                legend: { display: false },
                tooltip: {
                    backgroundColor: 'rgba(15, 23, 42, 0.95)',
                    padding: 12,
                    cornerRadius: 10,
                    borderColor: 'rgba(255, 255, 255, 0.1)',
                    borderWidth: 1
                }
            },
            animation: {
                animateScale: true,
                animateRotate: true
            },
            onClick: (event, elements) => {
                if (elements.length > 0) {
                    const index = elements[0].index;
                    const status = ['Selesai', 'Pending', 'Delay'][index];
                    activeFilterStatus = activeFilterStatus === status ? null : status;
                    filterData();
                }
            }
        },
        plugins: [{
            id: 'backdropRing',
            beforeDraw: (chart) => {
                const { ctx, chartArea: { width, height } } = chart;
                const meta = chart.getDatasetMeta(0);
                const x = chart.width / 2;
                const y = chart.height / 2;
                const outerRadius = meta.data[0]?.outerRadius || 0;
                const innerRadius = meta.data[0]?.innerRadius || 0;

                if (outerRadius > 0) {
                    ctx.save();
                    ctx.beginPath();
                    ctx.arc(x, y, (outerRadius + innerRadius) / 2, 0, Math.PI * 2);
                    ctx.strokeStyle = 'rgba(255, 255, 255, 0.03)';
                    ctx.lineWidth = outerRadius - innerRadius;
                    ctx.stroke();
                    ctx.restore();
                }
            }
        }, {
            id: 'centerText',
            beforeDraw: function (chart) {
                var width = chart.width,
                    height = chart.height,
                    ctx = chart.ctx;

                ctx.restore();

                var total = chart.data.datasets[0].data.reduce((a, b) => a + b, 0);
                var completed = chart.data.datasets[0].data[0];
                var percentage = total > 0 ? Math.round((completed / total) * 100) + "%" : "0%";

                // Percentage Glow Text
                var fontSize = (height / 120).toFixed(2);
                ctx.font = "900 " + fontSize + "em 'Inter', sans-serif";
                ctx.textBaseline = "middle";
                ctx.fillStyle = "#fff";
                ctx.shadowBlur = 20;
                ctx.shadowColor = "rgba(255, 255, 255, 0.5)";

                var text = percentage,
                    textX = Math.round((width - ctx.measureText(text).width) / 2),
                    textY = height / 2 - (height / 30);

                ctx.fillText(text, textX, textY);
                ctx.shadowBlur = 0;

                // Label Underneath
                ctx.font = "700 " + (height / 320).toFixed(2) + "em 'Inter', sans-serif";
                ctx.fillStyle = "rgba(255, 255, 255, 0.4)";
                ctx.letterSpacing = "2px";
                var label = "COMPLETED",
                    labelX = Math.round((width - ctx.measureText(label).width) / 2),
                    labelY = height / 2 + (height / 10);

                ctx.fillText(label, labelX, labelY);
                ctx.save();
            }
        }, {
            id: 'calloutLines',
            afterDraw: (chart) => {
                const { ctx, data } = chart;
                const meta = chart.getDatasetMeta(0);
                const colors = ['#10b981', '#f59e0b', '#ef4444'];

                meta.data.forEach((element, index) => {
                    const value = data.datasets[0].data[index];
                    if (value === 0) return;

                    const { x, y, outerRadius } = element;
                    const midAngle = element.startAngle + (element.endAngle - element.startAngle) / 2;

                    // Line points
                    const x1 = x + Math.cos(midAngle) * (outerRadius + 2);
                    const y1 = y + Math.sin(midAngle) * (outerRadius + 2);
                    const x2 = x + Math.cos(midAngle) * (outerRadius + 20);
                    const y2 = y + Math.sin(midAngle) * (outerRadius + 20);

                    const isRight = x2 > x;
                    const x3 = x2 + (isRight ? 15 : -15);

                    // HUD Point at start
                    ctx.beginPath();
                    ctx.arc(x1, y1, 2.5, 0, Math.PI * 2);
                    ctx.fillStyle = colors[index];
                    ctx.fill();

                    // Draw Line with glow
                    ctx.beginPath();
                    ctx.moveTo(x1, y1);
                    ctx.lineTo(x2, y2);
                    ctx.lineTo(x3, y2);
                    ctx.strokeStyle = colors[index];
                    ctx.lineWidth = 2;
                    ctx.lineCap = 'round';
                    ctx.shadowColor = colors[index];
                    ctx.shadowBlur = 8;
                    ctx.stroke();
                    ctx.shadowBlur = 0;

                    // HUD Style Label
                    const statusLabel = data.labels[index].toUpperCase();
                    ctx.font = '800 11px "Inter"';
                    ctx.fillStyle = colors[index];
                    ctx.textAlign = isRight ? 'left' : 'right';
                    ctx.fillText(statusLabel, x3 + (isRight ? 8 : -8), y2 - 8);

                    ctx.font = '800 18px "Inter"';
                    ctx.fillStyle = '#fff';
                    ctx.fillText(value, x3 + (isRight ? 8 : -8), y2 + 10);
                });
            }
        }]
    });
}

function renderCategoryChart(data) {
    const canvas = document.getElementById('categoryChart');
    const ctx = canvas.getContext('2d');
    if (charts.category) charts.category.destroy();

    const h = canvas.height || 300;

    // SKY BLUE - MAN
    const gradMan = ctx.createLinearGradient(0, 0, 0, h);
    gradMan.addColorStop(0, 'rgba(14, 165, 233, 0.8)');
    gradMan.addColorStop(0.5, 'rgba(2, 132, 199, 0.6)');
    gradMan.addColorStop(1, 'rgba(12, 74, 110, 0.7)');

    // PURPLE - MACHINE
    const gradMachine = ctx.createLinearGradient(0, 0, 0, h);
    gradMachine.addColorStop(0, 'rgba(168, 85, 247, 0.8)');
    gradMachine.addColorStop(0.5, 'rgba(147, 51, 234, 0.6)');
    gradMachine.addColorStop(1, 'rgba(88, 28, 135, 0.7)');

    // INDIGO - MATERIAL
    const gradMaterial = ctx.createLinearGradient(0, 0, 0, h);
    gradMaterial.addColorStop(0, 'rgba(129, 140, 248, 0.8)');
    gradMaterial.addColorStop(0.5, 'rgba(79, 70, 229, 0.6)');
    gradMaterial.addColorStop(1, 'rgba(49, 46, 129, 0.7)');

    // ROSE - METHOD
    const gradMethod = ctx.createLinearGradient(0, 0, 0, h);
    gradMethod.addColorStop(0, 'rgba(251, 113, 133, 0.8)');
    gradMethod.addColorStop(0.5, 'rgba(244, 63, 94, 0.6)');
    gradMethod.addColorStop(1, 'rgba(136, 19, 55, 0.7)');

    charts.category = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['MAN', 'MACHINE', 'MATERIAL', 'METHOD'],
            datasets: [{
                data: data,
                backgroundColor: [gradMan, gradMachine, gradMaterial, gradMethod].map((g, i) => {
                    const cat = ['Man', 'Machine', 'Material', 'Method'][i];
                    return (activeFilterCategory === null || activeFilterCategory === cat) ? g : 'rgba(255,255,255,0.05)';
                }),
                borderColor: [
                    'rgba(14, 165, 233, 0.8)',
                    'rgba(168, 85, 247, 0.8)',
                    'rgba(129, 140, 248, 0.8)',
                    'rgba(251, 113, 133, 0.8)'
                ],
                borderWidth: 1,
                borderRadius: 0,
                barPercentage: 0.5,
                categoryPercentage: 0.8
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: {
                    backgroundColor: 'rgba(15, 23, 42, 0.98)',
                    padding: 12,
                    displayColors: false
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    grid: { color: 'rgba(255, 255, 255, 0.03)', drawBorder: false },
                    ticks: { color: '#64748b', font: { size: 10 } },
                    border: { display: false }
                },
                x: {
                    grid: { display: false },
                    ticks: {
                        color: '#f8fafc',
                        font: { family: "'Inter', sans-serif", weight: '900', size: 12 },
                        padding: 15
                    },
                    border: { display: false }
                }
            },
            onClick: (event, elements) => {
                if (elements.length > 0) {
                    const index = elements[0].index;
                    const categories = ['Man', 'Machine', 'Material', 'Method'];
                    const cat = categories[index];
                    activeFilterCategory = activeFilterCategory === cat ? null : cat;
                    filterData();
                }
            }
        },
        plugins: [{
            id: 'softCrystalGlass',
            afterDatasetsDraw(chart) {
                const { ctx } = chart;
                const colors = ['#60a5fa', '#facc15', '#22c55e', '#ef4444'];

                chart.getDatasetMeta(0).data.forEach((bar, index) => {
                    const { x, y, base, width } = bar;

                    // 1. Subtle Reflection (Sangat Tipis)
                    ctx.save();
                    ctx.beginPath();
                    ctx.rect(x - width / 2, y, width, base - y);
                    ctx.clip();

                    const reflection = ctx.createLinearGradient(x - width, y, x + width, base);
                    reflection.addColorStop(0, 'rgba(255, 255, 255, 0)');
                    reflection.addColorStop(0.5, 'rgba(255, 255, 255, 0.03)'); // Jauh lebih tipis
                    reflection.addColorStop(0.55, 'rgba(255, 255, 255, 0)');
                    ctx.fillStyle = reflection;
                    ctx.fillRect(x - width / 2, y, width, base - y);
                    ctx.restore();

                    // 2. Value INSIDE the bar
                    const value = chart.data.datasets[0].data[index];
                    if (value > 0) {
                        ctx.save();
                        ctx.textAlign = 'center';
                        ctx.textBaseline = 'top';
                        ctx.font = '900 16px "Inter", sans-serif';
                        ctx.fillStyle = '#fff';

                        const textY = (base - y) > 30 ? y + 8 : y - 22;

                        if ((base - y) <= 30) {
                            ctx.textBaseline = 'bottom';
                        } else {
                            ctx.shadowBlur = 10;
                            ctx.shadowColor = colors[index];
                        }

                        ctx.fillText(value, x, textY);
                        ctx.restore();
                    }
                });
            }
        }]
    });
}

// Filter UI Helpers
function updateFilterChips() {
    const container = document.getElementById('activeFiltersContainer');
    if (!container) return;
    container.innerHTML = '';

    const addChip = (text, type) => {
        const chip = document.createElement('div');
        chip.className = 'filter-chip';
        const label = type.charAt(0).toUpperCase() + type.slice(1);
        chip.innerHTML = `<span><strong style="opacity: 0.7">${label}:</strong> ${text}</span> <i class="ph-x-circle" onclick="clearSpecificFilter('${type}')"></i>`;
        container.appendChild(chip);
    };

    if (activeFilterStatus) addChip(activeFilterStatus, 'status');
    if (activeFilterCategory) addChip(activeFilterCategory, 'category');
    if (activeFilterMonth !== null) {
        const months = ['Januari', 'Februari', 'Maret', 'April', 'Mei', 'Juni', 'Juli', 'Agustus', 'September', 'Oktober', 'November', 'Desember'];
        addChip(months[activeFilterMonth], 'month');
    }
}

window.clearSpecificFilter = (type) => {
    if (type === 'status') activeFilterStatus = null;
    if (type === 'category') activeFilterCategory = null;
    if (type === 'month') activeFilterMonth = null;
    filterData();
};

function renderTrendChart(trendData) {
    const canvas = document.getElementById('trendChart');
    const ctx = canvas.getContext('2d');
    if (charts.trend) charts.trend.destroy();

    const h = canvas.height || 400;

    // Premium Blue Glass Gradient
    const glassBlue = ctx.createLinearGradient(0, 0, 0, h);
    glassBlue.addColorStop(0, 'rgba(59, 130, 246, 0.8)');   // Bright Blue
    glassBlue.addColorStop(0.5, 'rgba(37, 99, 235, 0.6)'); // Core Blue
    glassBlue.addColorStop(1, 'rgba(30, 58, 138, 0.7)');   // Deep Blue

    charts.trend = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: trendData.labels,
            datasets: [{
                label: 'Jumlah Temuan',
                data: trendData.counts,
                backgroundColor: trendData.counts.map((_, i) =>
                    (activeFilterMonth === null || activeFilterMonth === i) ? glassBlue : 'rgba(59, 130, 246, 0.1)'
                ),
                borderColor: 'rgba(96, 165, 250, 0.5)',
                borderWidth: 1,
                borderRadius: 0,
                barPercentage: 0.6,
                categoryPercentage: 0.8
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: {
                    backgroundColor: 'rgba(15, 23, 42, 0.98)',
                    padding: 12,
                    borderColor: 'rgba(255, 255, 255, 0.1)',
                    borderWidth: 1,
                    displayColors: false
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    grid: { color: 'rgba(255, 255, 255, 0.03)', drawBorder: false },
                    ticks: { color: '#64748b', font: { size: 10 }, stepSize: 1 },
                    border: { display: false }
                },
                x: {
                    grid: { display: false },
                    ticks: {
                        color: '#94a3b8',
                        font: { family: "'Inter', sans-serif", weight: '600', size: 11 },
                        padding: 10
                    },
                    border: { display: false }
                }
            },
            onClick: (event, elements) => {
                if (elements.length > 0) {
                    const index = elements[0].index;
                    activeFilterMonth = activeFilterMonth === index ? null : index;
                    filterData();
                }
            }
        },
        plugins: [{
            id: 'trendGlassAesthetics',
            afterDatasetsDraw(chart) {
                const { ctx } = chart;
                chart.getDatasetMeta(0).data.forEach((bar, index) => {
                    const { x, y, base, width } = bar;

                    // 1. Diagonal Glass Reflection
                    ctx.save();
                    ctx.beginPath();
                    ctx.rect(x - width / 2, y, width, base - y);
                    ctx.clip();

                    const reflection = ctx.createLinearGradient(x - width, y, x + width, base);
                    reflection.addColorStop(0, 'rgba(255, 255, 255, 0)');
                    reflection.addColorStop(0.5, 'rgba(255, 255, 255, 0.05)');
                    reflection.addColorStop(0.55, 'rgba(255, 255, 255, 0)');
                    ctx.fillStyle = reflection;
                    ctx.fillRect(x - width / 2, y, width, base - y);
                    ctx.restore();

                    // 2. Integrated Data Label (Inside Bar)
                    const value = chart.data.datasets[0].data[index];
                    if (value > 0) {
                        ctx.save();
                        ctx.textAlign = 'center';
                        ctx.textBaseline = 'top';
                        ctx.font = '900 13px "Inter", sans-serif';
                        ctx.fillStyle = '#fff';

                        // Posisi: Sedikit di bawah puncak batang
                        const textY = (base - y) > 25 ? y + 6 : y - 18;

                        // Fallback ke atas jika batang terlalu pendek
                        if ((base - y) <= 25) {
                            ctx.textBaseLine = 'bottom';
                        } else {
                            ctx.shadowBlur = 8;
                            ctx.shadowColor = 'rgba(59, 130, 246, 0.8)';
                        }

                        ctx.fillText(value, x, textY);
                        ctx.restore();
                    }
                });
            }
        }]
    });
}
