# Panduan Deploy Daily_Checklist_Machine ke IIS (Remote Desktop / Server Windows)

Proyek ini .NET 8, pakai SQL Server. Ikuti langkah berikut.

---

## 1. Persiapan di Server (lewat Remote Desktop)

### A. Install .NET 8 Runtime + Hosting Bundle

Agar IIS bisa menjalankan aplikasi .NET 8, harus ada **ASP.NET Core Hosting Bundle** di server.

1. Download: https://dotnet.microsoft.com/download/dotnet/8.0  
   - Pilih **Hosting Bundle** (bukan hanya Runtime).
2. Install di server (Restart jika diminta).
3. Setelah itu, IIS bisa menjalankan aplikasi ASP.NET Core.

### B. Pastikan IIS dan modul yang dipakai sudah aktif

1. Buka **Server Manager** → **Add Roles and Features**.
2. Pilih **Web Server (IIS)** → **Application Development**:
   - Centang **ASP.NET 4.8** (kalau ada).
   - Centang **WebSocket Protocol** (penting untuk SignalR).
3. Selesai wizard.

---

## 2. Publish aplikasi dari komputer development

Di komputer kamu (tempat source code), buka **PowerShell** di folder proyek:

```powershell
cd "c:\Users\dede.vlt00046\Downloads\CheckingAM"

# Publish untuk Production (folder hasil: Daily_Checklist_Machine)
dotnet publish -c Release -o .\Daily_Checklist_Machine
```

Atau jalankan script: `.\publish-for-iis.ps1`

Folder **Daily_Checklist_Machine** akan berisi semua file yang perlu di-copy ke server.

---

## 3. Copy ke server

1. Copy seluruh isi folder **Daily_Checklist_Machine** (hasil dari `publish-for-iis.ps1`) ke server.
2. Taruh di folder yang dipakai IIS, misalnya:  
   `C:\inetpub\wwwroot\Daily_Checklist_Machine`  
   (boleh path lain, yang penting konsisten.)

Pastikan di dalam folder itu ada:
- `AMRVI.exe`
- `web.config`
- `appsettings.json`
- folder `wwwroot`, `Views`, DLL-dll, dll.

---

## 4. Konfigurasi di IIS (di server)

### A. Buat Application Pool

1. Buka **IIS Manager** (inetmgr).
2. Klik **Application Pools** → **Add Application Pool**.
3. Isi:
   - **Name:** `Daily_Checklist_Machine`
   - **.NET CLR version:** **No Managed Code**
   - **Managed pipeline mode:** Integrated
4. Klik **OK**.
5. Klik kanan pool **Daily_Checklist_Machine** → **Advanced Settings**:
   - **Start Mode:** AlwaysRunning (opsional).
   - **Identity:** ApplicationPoolIdentity (default) atau akun domain jika butuh akses DB khusus.

### B. Buat Website

1. Klik kanan **Sites** → **Add Website**.
2. Isi:
   - **Site name:** `Daily_Checklist_Machine` (atau nama lain).
   - **Application pool:** pilih **Daily_Checklist_Machine**.
   - **Physical path:** `C:\inetpub\wwwroot\Daily_Checklist_Machine` (sesuaikan dengan lokasi copy tadi).
   - **Binding:**
     - Type: **http** (atau https kalau sudah ada sertifikat).
     - Port: **80** (atau port lain, misal 8080).
     - Host name: kosongkan atau isi nama/domain jika pakai.
3. Klik **OK**.

### C. Hak akses folder

1. Di Explorer, buka folder aplikasi (misal `C:\inetpub\wwwroot\Daily_Checklist_Machine`).
2. Klik kanan folder → **Properties** → **Security** → **Edit** → **Add**.
3. Ketik: `IIS_IUSRS` → **Check Names** → **OK**.
4. Beri hak **Read & Execute** (dan Read, List folder contents). Kalau ada upload/simpan file, bisa tambah **Modify** untuk folder yang dipakai simpan data.
5. **OK** semua.

---

## 5. Connection string & appsettings (penting)

Aplikasi pakai SQL Server. Di server, pastikan:

1. **SQL Server bisa diakses** dari server (firewall, user/password).
2. **Connection string** di server benar (bisa beda IP/user dari development).

Cara mengatur:

- **Opsi A – Langsung edit di server**  
  Edit file `appsettings.json` di folder deploy (di server), sesuaikan `ConnectionStrings:DefaultConnection` (Server, Database, User Id, Password).

- **Opsi B – Pakai appsettings.Production.json**  
  Buat file `appsettings.Production.json` di folder **publish** (sebelum copy), isi misalnya:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SERVER_IP_ATAU_NAMA;Database=ChecklistAM_DB;User Id=user;Password=pass;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

Saat jalan di IIS, environment biasanya **Production**, jadi `appsettings.Production.json` akan dipakai dan override `appsettings.json`.

---

## 6. Database di server

- Pastikan database **ChecklistAM_DB** (atau nama di connection string) sudah ada di SQL Server yang dipakai server.
- Kalau pakai migration, jalankan sekali di server (lewat RDP) dari folder deploy:

```powershell
cd C:\inetpub\wwwroot\Daily_Checklist_Machine
dotnet AMRVI.dll
# (Ini hanya contoh; biasanya migration dijalankan dengan: dotnet ef database update)
```

Lebih aman: jalankan migration dari komputer dev yang bisa akses DB production, atau pakai script SQL yang sudah di-export. Yang penting skema DB sudah siap sebelum akses aplikasi.

---

## 7. Cek dan restart

1. Di IIS Manager, klik website **Daily_Checklist_Machine** → **Manage Website** → **Start** (atau Restart).
2. Buka browser di server (atau dari PC lain):  
   `http://IP_SERVER` atau `http://IP_SERVER:PORT`  
   (sesuai binding yang dipakai).
3. Kalau error:
   - Cek **Event Viewer** (Windows Logs → Application) untuk error .NET/IIS.
   - Cek **C:\inetpub\logs\LogFiles** untuk log HTTP.
   - Cek hak akses folder dan connection string lagi.

---

## Ringkasan singkat

| Langkah | Yang dilakukan |
|--------|-----------------|
| 1 | Di server: Install .NET 8 Hosting Bundle + IIS + WebSocket |
| 2 | Di PC dev: jalankan `.\publish-for-iis.ps1` (atau `dotnet publish -c Release -o .\Daily_Checklist_Machine`) |
| 3 | Copy isi folder **Daily_Checklist_Machine** ke server (misal `C:\inetpub\wwwroot\Daily_Checklist_Machine`) |
| 4 | Buat Application Pool **Daily_Checklist_Machine** (No Managed Code), lalu Add Website ke folder tadi |
| 5 | Beri hak **IIS_IUSRS** ke folder aplikasi |
| 6 | Sesuaikan connection string (appsettings.json atau appsettings.Production.json) |
| 7 | Pastikan DB sudah ada, lalu Start/Restart website di IIS |

Kalau ada pesan error saat akses pertama (500, 503, atau blank page), kirimkan teks errornya atau isi Event Viewer supaya bisa dibantu cek lebih lanjut.
