# Panduan Deploy IIS untuk checklistMechine

Panduan ini akan membantu Anda men-deploy aplikasi **checklistMechine** ke IIS (Internet Information Services) di Windows Server atau Windows 10/11.

## 1. Persiapan Server (IIS)

Pastikan server IIS sudah terinstall komponen berikut:
1.  **IIS (Internet Information Services)**: Pastikan fitur ini aktif di Windows Features.
2.  **.NET Core Hosting Bundle**: Download dan install **.NET Core Hosting Bundle** terbaru (sesuai versi .NET SDK yang dipakai, yaitu .NET 8.0).
    -   Link Download: [Download .NET 8.0 Hosting Bundle](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
    -   **PENTING**: Setelah install, restart IIS dengan menjalankan command `iisreset` di Command Prompt (Run as Administrator).

## 2. Publish Aplikasi

Gunakan script `publish_iis.bat` yang telah dibuat, atau jalankan perintah manual berikut di terminal project:

```sh
dotnet publish -c Release -o ./publish
```

Folder `./publish` akan berisi semua file yang siap untuk di-deploy.

## 3. Konfigurasi IIS

1.  Buka **IIS Manager** (tekan `Win + R`, ketik `inetmgr`, tekan Enter).
2.  Di panel kiri, klik kanan pada **Sites** -> **Add Website**.
3.  Isi form berikut:
    -   **Site name**: `checklistMechine` (atau nama lain yang diinginkan)
    -   **Physical path**: Buat folder baru, misal `C:\inetpub\wwwroot\checklistMechine`, dan pilih folder tersebut.
    -   **Port**: `80` (atau port lain jika bentrok, misal `8080`).
    -   **Host name**: Kosongkan jika untuk lokal, atau isi domain jika ada.
4.  Klik **OK**.

## 4. Konfigurasi Application Pool

1.  Di IIS Manager, klik **Application Pools** di panel kiri.
2.  Cari **checklistMechine** (atau nama Site yang Anda buat tadi).
3.  Klik dua kali atau pilih **Basic Settings**.
4.  Ubah **.NET CLR Version** menjadi **No Managed Code**.
    -   *Kenapa? Karena .NET Core/8.0 menghandle request sendiri via module Kestrel, IIS hanya sebagai reverse proxy.*
5.  Klik **OK**.

## 5. Copy File

1.  Copy **semua file** dari folder `./publish` (hasil langkah no 2).
2.  Paste ke folder Physical Path IIS tadi (misal: `C:\inetpub\wwwroot\checklistMechine`).

## 6. Mengatur Permissions (PENTING)

Agar aplikasi bisa berjalan dan menulis log/file jika perlu, Application Pool butuh akses read/execute.

1.  Buka File Explorer, klik kanan folder `C:\inetpub\wwwroot\checklistMechine` -> **Properties**.
2.  Masuk tab **Security** -> **Edit** -> **Add**.
3.  Di kolom object names, ketik: `IIS AppPool\checklistMechine` (ganti `checklistMechine` dengan nama Application Pool Anda).
    -   *Tips: Jika nama AppPool benar, tombol Check Names akan menggarisbawahi teks tersebut.*
4.  Klik **OK**.
5.  Berikan akses **Read & execute**, **List folder contents**, **Read**.
6.  Klik **OK** -> **OK**.

## 7. Test Aplikasi

Buka browser dan akses `http://localhost` (atau `http://localhost:8080` jika port diubah). Aplikasi seharusnya berjalan.

---

## Troubleshooting

-   **HTTP Error 500.19**: Biasanya karena .NET Core Hosting Bundle belum terinstall atau `web.config` salah. Coba install ulang Hosting Bundle dan restart IIS.
-   **Database Error**: Pastikan connection string di `appsettings.json` bisa diakses dari server IIS. Jika menggunakan SQL Server LocalDB, mungkin perlu diganti ke SQL Server Express/Full atau izinkan user AppPool mengakses database.
