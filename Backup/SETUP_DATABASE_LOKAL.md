# Setup Database Lokal - ChecklistAM_DB

## Cara 1: Menggunakan EF Core Migrations (Recommended)

### Langkah-langkah:

1. **Install SQL Server di Laptop/PC Rumah**
   - Download SQL Server Express (gratis): https://www.microsoft.com/en-us/sql-server/sql-server-downloads
   - Install SQL Server Management Studio (SSMS) untuk management

2. **Update Connection String**
   
   Edit file `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=ChecklistAM_DB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
     }
   }
   ```
   
   > **Note**: `Trusted_Connection=True` menggunakan Windows Authentication (tidak perlu username/password)

3. **Jalankan Migration**
   
   Buka terminal di folder project (`e:\AMRVI`), lalu jalankan:
   ```powershell
   dotnet ef database update
   ```
   
   Ini akan:
   - Membuat database `ChecklistAM_DB`
   - Membuat semua tabel (RVI, BTR, HOSE, MOLDED, MIXING)
   - Insert seed data RVI (3 machines, 6 machine numbers, 13 checklist items, 3 users)

4. **Tambah Data BTR (Opsional)**
   
   Jika ingin test login BTR, jalankan SQL ini di SSMS:
   ```sql
   USE ChecklistAM_DB;
   
   INSERT INTO Users_BTR (Username, FullName, Email, Password, Role, Department, IsActive, CreatedAt)
   VALUES ('admin', 'Admin BTR', 'admin@btr.com', 'admin123', 'Admin', 'Production', 1, GETDATE());
   ```

5. **Test Aplikasi**
   ```powershell
   dotnet watch
   ```
   
   Buka browser: `http://localhost:5028`
   
   Login:
   - **RVI**: username `admin`, password `admin123`
   - **BTR**: username `admin`, password `admin123` (jika sudah insert data BTR)

---

## Cara 2: Restore dari Backup File (Jika Punya Akses ke Server)

Jika Bos bisa akses file server di `10.14.149.34`, file backup ada di:
```
\\10.14.149.34\[SQL Server Backup Folder]\ChecklistAM_DB.bak
```

Cara restore:
1. Copy file `.bak` ke komputer lokal
2. Buka SSMS
3. Klik kanan `Databases` > `Restore Database`
4. Pilih `Device` > Browse ke file `.bak`
5. Klik OK

---

## Troubleshooting

### Error: "Login failed for user"
Ganti connection string pakai SQL Server Authentication:
```json
"Server=localhost;Database=ChecklistAM_DB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

### Error: "A network-related or instance-specific error"
- Pastikan SQL Server service sudah running
- Cek di SQL Server Configuration Manager

### Database sudah ada tapi mau reset
```powershell
dotnet ef database drop
dotnet ef database update
```

---

## Data Seed yang Tersedia (RVI)

Setelah migration, data berikut otomatis tersedia di tabel RVI:

**Users_RVI**:
- `admin` / `admin123` (Admin)
- `supervisor` / `super123` (Supervisor)
- `operator1` / `user123` (User)

**Machines_RVI**:
- MESIN INJECTION
- MESIN PRESS
- MESIN CNC

**ChecklistItems_RVI**:
- 13 item checklist untuk MESIN INJECTION

---

## Kontak
Jika ada masalah, hubungi tim IT atau cek dokumentasi EF Core:
https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/
