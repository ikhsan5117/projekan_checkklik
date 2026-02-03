# 📊 ANALISIS LENGKAP PROYEK AMRVI MULTI-PLANT

## ✅ YANG SUDAH BENAR

### 1. **AccountController.cs** ✓
- ✅ Sudah mendukung multi-plant login (line 34-41)
- ✅ Sudah menyimpan Plant di Claim (line 55)
- ✅ Sudah update LastLogin per plant (line 70-94)
- ⚠️ **WARNING**: Password masih plain text (line 44) - TIDAK AMAN untuk production!

### 2. **PlantService.cs** ✓
- ✅ Sudah membaca Plant dari Claim (line 22)
- ✅ Sudah menyediakan method untuk semua entity (Machines, ChecklistItems, dll.)
- ✅ Default ke RVI jika Plant tidak ditemukan (line 23)

### 3. **PlantModels.cs** ✓
- ✅ Sudah mendefinisikan semua model untuk 5 plant
- ✅ Menggunakan Interface untuk konsistensi (IMachine, IUser, dll.)
- ✅ Sudah ada Table attribute untuk mapping database

### 4. **ApplicationDbContext.cs** ✓
- ✅ Sudah mendefinisikan DbSet untuk semua plant (30 tabel)
- ✅ Sudah ada seed data untuk RVI
- ✅ Sudah konfigurasi indexes dan constraints

### 5. **Login Flow** ✓
- ✅ User bisa memilih plant saat login
- ✅ Plant tersimpan di Claim untuk session
- ✅ Role-based redirect sudah benar

---

## ❌ YANG MASIH SALAH / PERLU DIPERBAIKI

### 1. **HomeController.cs** ❌ CRITICAL
**Lokasi**: `Controllers/HomeController.cs`

**Masalah**: Semua query masih hardcoded ke tabel RVI:
```csharp
Line 27: var totalMachines = _context.MachineNumbers.Count(mn => mn.IsActive);
Line 28: var totalChecklists = _context.ChecklistItems.Count(c => c.IsActive);
Line 30: var inspectionsToday = _context.InspectionSessions...
Line 36: var issuesToday = _context.InspectionResults...
Line 40: var recentInspections = _context.InspectionSessions...
Line 49: var machineStats = _context.Machines...
Line 76: var okCount = _context.InspectionResults...
Line 79: var ngCount = _context.InspectionResults...
Line 90: var thisWeekOkCount = _context.InspectionResults...
Line 93: var thisWeekNgCount = _context.InspectionResults...
Line 108: var totalOkCount = _context.InspectionResults...
Line 109: var totalNgCount = _context.InspectionResults...
```

**Dampak**: 
- User dari plant BTR/HOSE/MOLDED/MIXING akan melihat data RVI
- Dashboard tidak akurat untuk plant selain RVI

**Solusi**: Inject `PlantService` dan gunakan untuk semua query.

---

### 2. **DashboardApiController.cs** ❌ CRITICAL
**Lokasi**: `Controllers/DashboardApiController.cs`

**Masalah**: Semua endpoint API masih hardcoded ke tabel RVI:
```csharp
Line 38-39: var okCount = _context.InspectionResults.Count(...)
Line 41-42: var ngCount = _context.InspectionResults.Count(...)
Line 50-51: var totalOk = _context.InspectionResults.Count(...)
Line 53-54: var totalNg = _context.InspectionResults.Count(...)
Line 56-67: var recentInspections = _context.InspectionSessions...
```

Dan seterusnya untuk endpoint `/weekly` dan `/daily`.

**Dampak**:
- Chart di dashboard selalu menampilkan data RVI
- Drill-down (monthly → weekly → daily) tidak akurat untuk plant lain

**Solusi**: Inject `PlantService` dan gunakan untuk semua query.

---

### 3. **Password Hashing** ⚠️ SECURITY ISSUE
**Lokasi**: 
- `Controllers/AccountController.cs` line 44
- `Data/ApplicationDbContext.cs` line 172-174 (seed data)

**Masalah**: Password disimpan dan dibandingkan dalam plain text.

**Solusi**: 
1. Hash password saat seed data menggunakan BCrypt
2. Gunakan `BCrypt.Net.BCrypt.Verify()` saat login

---

### 4. **Seed Data Hanya RVI** ⚠️
**Lokasi**: `Data/ApplicationDbContext.cs`

**Masalah**: Hanya plant RVI yang punya seed data (line 134-176).

**Dampak**: Plant lain (BTR, HOSE, MOLDED, MIXING) tidak punya data untuk testing.

**Solusi**: Buat method `SeedDataBTR()`, `SeedDataHOSE()`, dll.

---

### 5. **Program.cs - Database Provider** ℹ️
**Lokasi**: `Program.cs` line 21-22

**Masalah**: Selalu menggunakan SQL Server, tidak ada kondisi untuk Development (SQLite).

**Solusi**: Tambahkan kondisi:
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
    else
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});
```

---

## 🔧 PRIORITAS PERBAIKAN

### **PRIORITAS 1 (CRITICAL - Harus diperbaiki sebelum testing)**

#### 1.1 Fix HomeController
```csharp
// Inject PlantService
private readonly PlantService _plantService;

public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, PlantService plantService)
{
    _logger = logger;
    _context = context;
    _plantService = plantService;
}

// Ubah semua query
var totalMachines = _plantService.GetMachineNumbers().Count(mn => mn.IsActive);
var totalChecklists = _plantService.GetChecklistItems().Count(c => c.IsActive);
var inspectionsToday = _plantService.GetInspectionSessions()...
// dst...
```

#### 1.2 Fix DashboardApiController
```csharp
// Inject PlantService
private readonly PlantService _plantService;

public DashboardApiController(ApplicationDbContext context, PlantService plantService)
{
    _context = context;
    _plantService = plantService;
}

// Ubah semua query
var okCount = _plantService.GetInspectionResults()
    .Count(r => r.CreatedAt >= monthStart && r.CreatedAt <= monthEnd && r.Judgement == "OK");
// dst...
```

#### 1.3 Update Program.cs untuk Development
Tambahkan kondisi SQLite untuk development mode.

---

### **PRIORITAS 2 (HIGH - Penting untuk keamanan)**

#### 2.1 Hash Password di Seed Data
```csharp
using BCrypt.Net;

// Di SeedDataRVI()
new User { 
    Id = 1, 
    Username = "admin", 
    Password = BCrypt.HashPassword("admin123"), // ⬅️ Hash password
    // ...
}
```

#### 2.2 Update Login Logic
```csharp
// Di AccountController.cs line 44
if (user != null && BCrypt.Verify(model.Password, user.Password))
{
    // Login berhasil
}
```

---

### **PRIORITAS 3 (MEDIUM - Untuk testing lengkap)**

#### 3.1 Tambah Seed Data untuk Plant Lain
Buat method di `ApplicationDbContext.cs`:
- `SeedDataBTR()`
- `SeedDataHOSE()`
- `SeedDataMOLDED()`
- `SeedDataMIXING()`

Panggil di `OnModelCreating()`:
```csharp
SeedDataRVI(modelBuilder);
SeedDataBTR(modelBuilder);
SeedDataHOSE(modelBuilder);
SeedDataMOLDED(modelBuilder);
SeedDataMIXING(modelBuilder);
```

---

### **PRIORITAS 4 (LOW - Enhancement)**

#### 4.1 Tambah Plant Indicator di UI
Tampilkan plant yang sedang aktif di navbar/header.

#### 4.2 Tambah Plant Switcher
Buat dropdown untuk switch plant tanpa logout.

#### 4.3 Audit Log
Catat semua aktivitas user per plant.

---

## 📝 CHECKLIST SEBELUM TESTING

- [ ] Update `appsettings.Development.json` dengan SQLite connection string ✅ (DONE)
- [ ] Update `Program.cs` untuk kondisi Development/Production
- [ ] Fix `HomeController.cs` - inject PlantService
- [ ] Fix `DashboardApiController.cs` - inject PlantService
- [ ] Hash password di seed data
- [ ] Update login logic untuk verify password
- [ ] Tambah seed data untuk plant lain (optional untuk testing awal)
- [ ] Jalankan migration: `dotnet ef database update`
- [ ] Test login dengan user admin
- [ ] Test dashboard untuk setiap plant
- [ ] Test drill-down chart (monthly → weekly → daily)

---

## 🚀 QUICK FIX COMMANDS

```powershell
# 1. Backup database lama
Copy-Item app.db app.db.backup -ErrorAction SilentlyContinue

# 2. Hapus database lama
Remove-Item app.db -ErrorAction SilentlyContinue

# 3. Hapus migrations lama (optional jika ada masalah)
# Remove-Item -Recurse Migrations

# 4. Buat migration baru (jika perlu)
# dotnet ef migrations add MultiPlantSetup

# 5. Update database
dotnet ef database update

# 6. Jalankan aplikasi
dotnet run

# 7. Buka browser
start https://localhost:5001
```

---

## 🧪 TEST SCENARIOS

### Test 1: Login RVI Plant
1. Buka `/Account/Login`
2. Pilih Plant: **RVI**
3. Username: `admin`
4. Password: `admin123`
5. ✅ Expected: Redirect ke `/Home/Index` dengan data RVI

### Test 2: Login BTR Plant (Setelah seed data BTR dibuat)
1. Buka `/Account/Login`
2. Pilih Plant: **BTR**
3. Username: `admin_btr` (harus dibuat di seed data)
4. Password: `admin123`
5. ✅ Expected: Redirect ke `/Home/Index` dengan data BTR

### Test 3: Dashboard Chart Drill-Down
1. Login sebagai admin RVI
2. Buka dashboard
3. Klik salah satu bulan di chart
4. ✅ Expected: Chart berubah ke weekly view
5. Klik salah satu week
6. ✅ Expected: Chart berubah ke daily view
7. Klik tombol "Back"
8. ✅ Expected: Kembali ke weekly view

### Test 4: API Endpoint
1. Login sebagai admin RVI
2. Buka browser console
3. Fetch: `fetch('/api/dashboard/monthly?year=2026').then(r => r.json()).then(console.log)`
4. ✅ Expected: Response berisi data RVI

---

## 📊 DATABASE SCHEMA VERIFICATION

Setelah migration, verifikasi tabel berikut ada di database:

### RVI Tables (6)
- [x] Machines_RVI
- [x] MachineNumbers_RVI
- [x] ChecklistItems_RVI
- [x] InspectionSessions_RVI
- [x] InspectionResults_RVI
- [x] Users_RVI

### BTR Tables (6)
- [x] Machines_BTR
- [x] MachineNumbers_BTR
- [x] ChecklistItems_BTR
- [x] InspectionSessions_BTR
- [x] InspectionResults_BTR
- [x] Users_BTR

### HOSE Tables (6)
- [x] Machines_HOSE
- [x] MachineNumbers_HOSE
- [x] ChecklistItems_HOSE
- [x] InspectionSessions_HOSE
- [x] InspectionResults_HOSE
- [x] Users_HOSE

### MOLDED Tables (6)
- [x] Machines_MOLDED
- [x] MachineNumbers_MOLDED
- [x] ChecklistItems_MOLDED
- [x] InspectionSessions_MOLDED
- [x] InspectionResults_MOLDED
- [x] Users_MOLDED

### MIXING Tables (6)
- [x] Machines_MIXING
- [x] MachineNumbers_MIXING
- [x] ChecklistItems_MIXING
- [x] InspectionSessions_MIXING
- [x] InspectionResults_MIXING
- [x] Users_MIXING

**Total: 30 Tables**

---

## 🔍 TOOLS UNTUK DEBUGGING

### 1. DB Browser for SQLite
Download: https://sqlitebrowser.org/
Gunakan untuk inspect `app.db` dan verify data.

### 2. Browser DevTools
- **Console**: Untuk test API endpoints
- **Network**: Untuk monitor AJAX calls
- **Application > Cookies**: Untuk verify authentication cookie

### 3. Visual Studio / VS Code Extensions
- **SQLite Viewer**: Untuk view database langsung di editor
- **C# Dev Kit**: Untuk debugging breakpoints

---

## 📞 SUPPORT & NEXT STEPS

Jika ada error saat testing:

1. **Check Migration Status**
   ```powershell
   dotnet ef migrations list
   ```

2. **Check Database Connection**
   ```powershell
   # Untuk SQLite
   Test-Path app.db
   ```

3. **Check Logs**
   Lihat console output saat `dotnet run` untuk error messages.

4. **Reset Database**
   ```powershell
   Remove-Item app.db
   dotnet ef database update
   ```

---

**Status**: ⚠️ **NEEDS FIXES BEFORE PRODUCTION**

**Estimated Fix Time**: 
- Priority 1: ~2 hours
- Priority 2: ~1 hour
- Priority 3: ~3 hours
- Priority 4: ~2 hours

**Total**: ~8 hours untuk full implementation
