# Panduan File .BAT untuk Publish

## File yang Tersedia

### 1. `publish_iis.bat` (Original)
Script original untuk publish ke IIS.

### 2. `publish_iis_safe.bat` (Recommended - Versi Aman)
Versi yang lebih aman dengan:
- Konfirmasi sebelum menjalankan
- Error handling yang lebih baik
- Tidak akan menghapus dirinya sendiri
- Informasi yang lebih jelas

## Cara Menggunakan

### Opsi 1: Double-click (Paling Mudah)
1. Double-click file `publish_iis_safe.bat`
2. Tekan Enter untuk melanjutkan
3. Tunggu proses publish selesai

### Opsi 2: Dari PowerShell
```powershell
# Pindah ke direktori project
cd "C:\Users\megi.vlt00553\Downloads\CheckingAM"

# Jalankan script
.\publish_iis_safe.bat
```

### Opsi 3: Dari Command Prompt (CMD)
```cmd
cd /d "C:\Users\megi.vlt00553\Downloads\CheckingAM"
publish_iis_safe.bat
```

## Troubleshooting

### File .bat Hilang Setelah Dijalankan
Jika file .bat hilang setelah dijalankan, kemungkinan penyebabnya:

1. **Antivirus/Windows Defender**
   - File .bat mungkin dihapus oleh antivirus
   - Tambahkan folder project ke exclusion list antivirus
   - Atau disable real-time protection sementara

2. **File Terhapus Secara Tidak Sengaja**
   - Cek Recycle Bin
   - Gunakan `publish_iis_safe.bat` yang lebih aman

3. **Permission Issue**
   - Jalankan PowerShell/CMD sebagai Administrator
   - Cek permission folder project

### Cara Mengembalikan File .bat
Jika file hilang, Anda bisa:
1. Copy dari backup (jika ada)
2. Buat ulang menggunakan `publish_iis_safe.bat` yang sudah disediakan
3. Atau copy dari repository Git (jika menggunakan Git)

## Tips Keamanan

1. **Backup Script**
   - Simpan copy script di folder terpisah
   - Atau commit ke Git repository

2. **Gunakan Versi Safe**
   - Gunakan `publish_iis_safe.bat` yang sudah memiliki konfirmasi
   - Lebih aman dan informatif

3. **Cek Antivirus**
   - Pastikan folder project tidak di-block oleh antivirus
   - Tambahkan ke whitelist jika perlu

## Alternatif: Manual Publish

Jika file .bat terus bermasalah, Anda bisa publish manual:

```powershell
# Build dan publish manual
dotnet publish AMRVI.csproj -c Release -o publish
```

File hasil publish akan ada di folder `publish`.
