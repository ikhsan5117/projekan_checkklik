# Panduan Fitur Import Data Mesin

## Tinjauan Umum
Fitur ini memungkinkan pengguna untuk memasukkan data mesin secara massal (termasuk nama, deskripsi, dan nomor unit/lokasi) ke dalam sistem menggunakan file Excel. Fitur ini berguna untuk pengaturan awal sistem atau pembaruan data dalam jumlah banyak.

## Cara Penggunaan

### 1. Akses Fitur
1. Masuk ke halaman **"Kelola Data Mesin"** melalui sidebar atau tautan `http://localhost:5028/MachineData`.
2. Temukan tombol hijau **"📥 IMPORT DATA"** di pojok kanan atas.

### 2. Unduh Template
1. Klik tombol **"📥 IMPORT DATA"**.
2. Pada modal yang muncul, klik tombol **"📄 Download Template.xlsx"**.
3. File bernama `MachineTemplate.xlsx` akan terunduh ke komputer Anda.

### 3. Persiapkan Data
1. Buka file `MachineTemplate.xlsx` yang telah diunduh.
2. Isi kolom-kolom berikut:
   - **Machine Name**: Nama tipe mesin (contoh: "Mesin Injection A").
   - **Description**: (Opsional) Deskripsi mesin.
   - **Unit Number**: ID unik untuk unit spesifik (contoh: "M-001").
   - **Location**: (Opsional) Lokasi unit berada (contoh: "Line 1").
3. **Catatan Penting**: Jika Anda memiliki beberapa unit untuk satu tipe mesin yang sama, ulangi "Machine Name" tetapi ubah "Unit Number"-nya.
   - *Contoh*:
     - Baris 2: Mesin A | Desk A | Unit 1 | Loc A
     - Baris 3: Mesin A | Desk A | Unit 2 | Loc B
4. Simpan file tersebut.

### 4. Import Data
1. Kembali ke modal aplikasi, klik input **"Choose File"**.
2. Pilih file Excel yang telah Anda siapkan.
3. Klik tombol **"📤 UPLOAD & IMPORT"**.
4. Sistem akan memproses file:
   - Membuat Mesin baru jika belum ada.
   - Menambahkan Nomor Unit baru ke Mesin yang sudah ada.
   - Melewati data duplikat untuk mencegah error.
5. Pesan sukses akan muncul menunjukkan berapa banyak mesin dan unit yang berhasil ditambahkan.

## Pemecahan Masalah (Troubleshooting)
- **Error: "File tidak valid"**: Pastikan Anda mengunggah file `.xlsx`, bukan CSV atau PDF.
- **Data tidak muncul**: Refresh halaman jika data tidak langsung muncul otomatis.
- **Data Duplikat**: Sistem otomatis mengecek duplikat. Jika nomor unit sudah ada untuk mesin tersebut, sistem akan melewatinya (skip).

---
*Dibuat oleh Antigravity*
