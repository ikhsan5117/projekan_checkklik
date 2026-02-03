# 🧪 PANDUAN TESTING LOKAL - AMRVI Multi-Plant System

## 📋 RINGKASAN PROYEK

Proyek ini adalah **Audit Machine Receiving Visual Inspection System** yang mendukung **5 Plant**:
- **BTR** (Bridgestone)
- **HOSE**
- **MOLDED**
- **MIXING**
- **RVI** (Default)

Setiap plant memiliki tabel database terpisah dan data independen.

---

## ⚠️ MASALAH YANG DITEMUKAN

### 1. **DashboardApiController Tidak Multi-Plant Aware**
**Lokasi**: `Controllers/DashboardApiController.cs`

**Masalah**: Semua endpoint (`/api/dashboard/monthly`, `/weekly`, `/daily`) hanya mengambil data dari tabel RVI:
```csharp
var okCount = _context.InspectionResults  // ❌ Hanya RVI
    .Count(r => r.CreatedAt >= monthStart && r.CreatedAt <= monthEnd && r.Judgement == "OK");
```

**Solusi**: Harus menggunakan `PlantService` untuk mendapatkan data sesuai plant user yang login.

### 2. **Authentication Claim "Plant" Belum Diset**
**Lokasi**: `Controllers/AccountController.cs`

**Masalah**: Saat login, sistem belum menambahkan Claim `"Plant"` yang dibutuhkan oleh `PlantService`.

**Solusi**: Tambahkan claim saat membuat `ClaimsIdentity` di `AccountController`.

### 3. **Seed Data Hanya untuk RVI**
**Lokasi**: `Data/ApplicationDbContext.cs`

**Masalah**: Hanya plant RVI yang memiliki seed data. Plant lain (BTR, HOSE, MOLDED, MIXING) tidak memiliki data awal.

**Solusi**: Tambahkan seed data untuk semua plant atau buat fitur import data.

---

## 🔧 CARA TESTING TANPA KONEKSI SERVER

### **Opsi 1: Menggunakan SQLite Lokal (RECOMMENDED)**

#### Step 1: Update `appsettings.Development.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

#### Step 2: Update `Program.cs` untuk Development
Ubah baris 21-22 menjadi:
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

#### Step 3: Jalankan Migration
```powershell
# Hapus database lama jika ada
Remove-Item app.db -ErrorAction SilentlyContinue

# Buat database baru dengan semua tabel
dotnet ef database update

# Atau jika migration belum ada:
dotnet ef migrations add InitialCreate
dotnet ef database update
```

#### Step 4: Jalankan Aplikasi
```powershell
dotnet run
```

Aplikasi akan berjalan di `https://localhost:5001` atau `http://localhost:5000`

---

### **Opsi 2: Menggunakan SQL Server LocalDB**

#### Step 1: Install SQL Server LocalDB
Download dari: https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb

#### Step 2: Update `appsettings.Development.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ChecklistAM_Local;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

#### Step 3: Jalankan Migration
```powershell
dotnet ef database update
```

---

## 🐛 DEBUGGING CHECKLIST

### 1. **Verifikasi Database Terbuat**
```powershell
# Untuk SQLite
Get-Item app.db

# Untuk SQL Server LocalDB
sqllocaldb info mssqllocaldb
```

### 2. **Cek Tabel yang Terbuat**
Gunakan DB Browser for SQLite atau SQL Server Management Studio untuk memverifikasi tabel:
- `Machines_RVI`, `Machines_BTR`, `Machines_HOSE`, dll.
- `Users_RVI`, `Users_BTR`, dll.
- `InspectionSessions_*`
- `InspectionResults_*`

### 3. **Test Login**
- Username: `admin`
- Password: `admin123`

**CATATAN**: Password di seed data belum di-hash! Perlu update `SeedDataRVI()` untuk menggunakan BCrypt.

### 4. **Verifikasi Plant Selection**
Setelah login, cek apakah user bisa memilih plant (BTR/HOSE/MOLDED/MIXING/RVI).

---

## 🔨 PERBAIKAN YANG HARUS DILAKUKAN

### **PRIORITAS TINGGI**

#### 1. Fix DashboardApiController
File: `Controllers/DashboardApiController.cs`

Inject `PlantService` dan gunakan untuk query:
```csharp
private readonly PlantService _plantService;

public DashboardApiController(ApplicationDbContext context, PlantService plantService)
{
    _context = context;
    _plantService = plantService;
}

[HttpGet("monthly")]
public IActionResult GetMonthlyData(int year)
{
    var inspectionResults = _plantService.GetInspectionResults();
    var inspectionSessions = _plantService.GetInspectionSessions();
    
    // ... rest of the code using inspectionResults and inspectionSessions
}
```

#### 2. Fix AccountController - Add Plant Claim
File: `Controllers/AccountController.cs`

Saat membuat claims setelah login berhasil, tambahkan:
```csharp
var claims = new List<Claim>
{
    new Claim(ClaimTypes.Name, user.Username),
    new Claim(ClaimTypes.Email, user.Email),
    new Claim(ClaimTypes.Role, user.Role),
    new Claim("Plant", "RVI") // ⬅️ TAMBAHKAN INI (atau dari user.Plant jika ada)
};
```

#### 3. Hash Password di Seed Data
File: `Data/ApplicationDbContext.cs`

Update seed data untuk menggunakan BCrypt:
```csharp
// Di bagian atas file, tambahkan using
using BCrypt.Net;

// Di SeedDataRVI(), ubah password:
modelBuilder.Entity<User>().HasData(
    new User { 
        Id = 1, 
        Username = "admin", 
        FullName = "Administrator", 
        Email = "admin@amrvi.com", 
        Password = BCrypt.Net.BCrypt.HashPassword("admin123"), // ⬅️ HASH PASSWORD
        Role = "Admin", 
        Department = "IT", 
        IsActive = true, 
        CreatedAt = new DateTime(2025, 1, 1) 
    },
    // ... dst
);
```

### **PRIORITAS SEDANG**

#### 4. Tambah Seed Data untuk Plant Lain
Buat method `SeedDataBTR()`, `SeedDataHOSE()`, dll. di `ApplicationDbContext.cs`

#### 5. Tambah Plant Selector di UI
Buat dropdown/menu untuk user memilih plant yang ingin diakses.

#### 6. Update HomeController
Pastikan `HomeController` juga menggunakan `PlantService` untuk dashboard utama.

---

## 📊 STRUKTUR DATABASE YANG SEHARUSNYA

Setelah migration, Anda harus punya **30 tabel**:

### RVI Plant (6 tabel)
- Machines_RVI
- MachineNumbers_RVI
- ChecklistItems_RVI
- InspectionSessions_RVI
- InspectionResults_RVI
- Users_RVI

### BTR Plant (6 tabel)
- Machines_BTR
- MachineNumbers_BTR
- ChecklistItems_BTR
- InspectionSessions_BTR
- InspectionResults_BTR
- Users_BTR

### HOSE Plant (6 tabel)
- Machines_HOSE
- MachineNumbers_HOSE
- ChecklistItems_HOSE
- InspectionSessions_HOSE
- InspectionResults_HOSE
- Users_HOSE

### MOLDED Plant (6 tabel)
- Machines_MOLDED
- MachineNumbers_MOLDED
- ChecklistItems_MOLDED
- InspectionSessions_MOLDED
- InspectionResults_MOLDED
- Users_MOLDED

### MIXING Plant (6 tabel)
- Machines_MIXING
- MachineNumbers_MIXING
- ChecklistItems_MIXING
- InspectionSessions_MIXING
- InspectionResults_MIXING
- Users_MIXING

---

## 🚀 QUICK START (TL;DR)

```powershell
# 1. Update appsettings.Development.json (gunakan SQLite)
# 2. Update Program.cs (tambahkan kondisi Development/Production)
# 3. Jalankan migration
dotnet ef database update

# 4. Jalankan aplikasi
dotnet run

# 5. Buka browser
# https://localhost:5001

# 6. Login
# Username: admin
# Password: admin123
```

---

## 📝 CATATAN PENTING

1. **File `app.db` sudah ada** di root project Anda, tapi mungkin outdated. Hapus dan buat ulang dengan migration terbaru.

2. **Password belum di-hash** di seed data. Untuk testing bisa pakai plain text dulu, tapi HARUS di-hash sebelum production.

3. **PlantService default ke RVI** jika user belum login atau claim "Plant" tidak ada.

4. **Dashboard saat ini hanya menampilkan data RVI** karena bug di `DashboardApiController`.

5. **Migrations sudah ada** (`20260203090924_InitialMultiPlantSetup`), tapi perlu dicek apakah sudah sesuai dengan model terbaru.

---

## 🆘 TROUBLESHOOTING

### Error: "No connection string named 'DefaultConnection' found"
**Solusi**: Pastikan `appsettings.Development.json` memiliki ConnectionStrings.

### Error: "Unable to create an object of type 'ApplicationDbContext'"
**Solusi**: Jalankan `dotnet ef database update` dari root project.

### Error: "Login failed for user"
**Solusi**: 
1. Cek apakah database sudah dibuat
2. Cek apakah seed data sudah dijalankan
3. Cek apakah password di-hash atau plain text

### Dashboard kosong / tidak ada data
**Solusi**:
1. Cek apakah ada data di tabel `InspectionResults_RVI`
2. Cek apakah user sudah login dan memiliki claim "Plant"
3. Cek console browser untuk error JavaScript

---

## 📞 NEXT STEPS

Setelah berhasil testing lokal:

1. ✅ Fix semua bug yang ditemukan
2. ✅ Tambahkan seed data untuk semua plant
3. ✅ Implementasi plant selector di UI
4. ✅ Test semua fitur untuk setiap plant
5. ✅ Deploy ke server production dengan SQL Server
6. ✅ Update connection string di `appsettings.json` untuk production

---

**Good luck with testing! 🚀**
