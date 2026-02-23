# Panduan Deploy IIS - AMRVI Andon System

## ✅ Checklist Sebelum Deploy

- [ ] .NET 8.0 Hosting Bundle terinstall di server IIS
- [ ] Fitur **WebSocket Protocol** aktif di IIS (wajib untuk SignalR / auto-update)
- [ ] SQL Server bisa diakses dari server IIS (cek koneksi ke `10.14.149.34`)
- [ ] Application Pool sudah dikonfigurasi **No Managed Code**

---

## 1. Persiapan Server IIS

### A. Install .NET 8.0 Hosting Bundle
Download dan install dari: [https://dotnet.microsoft.com/en-us/download/dotnet/8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

Setelah install, wajib jalankan di Command Prompt (Administrator):
```cmd
iisreset
```

### B. Aktifkan WebSocket Protocol (WAJIB untuk SignalR)
> ⚠️ Jika ini tidak diaktifkan, fitur **auto-update realtime** di dashboard Andon tidak akan berfungsi!

1. Buka **Control Panel** → **Programs** → **Turn Windows features on or off**
2. Expand: `Internet Information Services` → `World Wide Web Services` → `Application Development Features`
3. Centang ✅ **WebSocket Protocol**
4. Klik OK, tunggu proses selesai
5. Jalankan `iisreset` di Command Prompt (Administrator)

---

## 2. Publish Aplikasi

Jalankan file `publish_iis.bat` (double-click atau via CMD):
```cmd
publish_iis.bat
```

Atau manual via terminal di folder project:
```cmd
dotnet publish "AMRVI.csproj" -c Release -o "checklist"
```

Folder `checklist` akan berisi semua file siap deploy.

---

## 3. Konfigurasi IIS Manager

1. Buka **IIS Manager** (`Win + R` → ketik `inetmgr` → Enter)
2. Di panel kiri, klik kanan **Sites** → **Add Website**
3. Isi form:
   - **Site name**: `AMRVI` (atau nama lain)
   - **Physical path**: `C:\inetpub\wwwroot\AMRVI` (buat folder ini dulu)
   - **Port**: `80` atau port lain yang tersedia
4. Klik **OK**

---

## 4. Konfigurasi Application Pool

1. Di IIS Manager → **Application Pools**
2. Cari Application Pool dengan nama site yang dibuat
3. Klik kanan → **Basic Settings**
4. Ubah **.NET CLR Version** → **No Managed Code**
5. Klik **OK**

---

## 5. Copy File ke IIS

1. Copy **semua file** dari folder `checklist` (hasil publish)
2. Paste ke folder Physical Path IIS: `C:\inetpub\wwwroot\AMRVI`

> ⚠️ Pastikan file `web.config` ikut ter-copy. File ini sudah dikonfigurasi dengan WebSocket support.

---

## 6. Permissions (PENTING)

1. Klik kanan folder `C:\inetpub\wwwroot\AMRVI` → **Properties** → tab **Security**
2. Klik **Edit** → **Add**
3. Ketik: `IIS AppPool\AMRVI` (sesuaikan dengan nama AppPool)
4. Klik **Check Names** → pastikan ter-underline → **OK**
5. Berikan akses: ✅ **Read & Execute**, ✅ **List folder contents**, ✅ **Read**
6. Klik **OK** → **OK**

---

## 7. Buat Folder Logs

Buat folder `logs` di dalam folder Physical Path agar stdout logging berfungsi:
```
C:\inetpub\wwwroot\AMRVI\logs\
```

---

## 8. Test Aplikasi

Buka browser → akses `http://localhost` (atau sesuai port yang dipilih)

---

## 🔧 Troubleshooting

| Error | Kemungkinan Penyebab | Solusi |
|---|---|---|
| **HTTP 500.19** | Hosting Bundle belum install / `web.config` salah | Install ulang Hosting Bundle, jalankan `iisreset` |
| **HTTP 500.30** | App gagal start | Aktifkan stdout log di `web.config`, cek file `logs\stdout_*.log` |
| **Auto-update tidak jalan** | WebSocket Protocol belum aktif | Aktifkan WebSocket Protocol di Windows Features |
| **Data tidak muncul / 500** | Koneksi ke database gagal | Cek koneksi ke `10.14.149.34` dari server IIS, pastikan firewall tidak memblokir |
| **Halaman kosong / redirect loop** | Port konflik | Ganti port di IIS ke yang tidak terpakai (8080, 8081, dll) |
