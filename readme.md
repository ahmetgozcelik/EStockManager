# EStockManager API

Stok ve Sipariþ Yönetimi üzerine kurulu, .NET Core ve Katmanlý Mimari prensipleriyle geliþtirilmiþ bir Web API projesidir. Proje, veri bütünlüðünü ve güvenliðini öncelikli hale getirerek temel iþ süreçlerini otomatikleþtirmeyi amaçlamaktadýr.

---

## 1. Mimarî ve Teknik Özellikler

| Kategori | Görev Adý | Uygulanan Kavram | Açýklama |
| :--- | :--- | :--- | :--- |
| **Mimari** | Proje Baþlatma | **Katmanlý Mimari** | Controller (Sunum), Service (Ýþ Mantýðý) ve Repository (Veri Eriþimi) katmanlarýndan oluþan modüler yapý (G1.1). |
| **Veri Eriþim** | Repository Pattern | **Generic Repository** | Tüm Entity'ler için ortak CRUD (Create, Read, Update, Delete) metotlarýný tanýmlayan esnek bir depo yapýsý (G1.5). |
| **Veri Modelleri** | Temel Veri Modelleri | **EF Core Ýliþkileri** | **User**, **Product** ve **Order** Entity'lerinin bire-çok iliþkilerle tanýmlanmasý (G1.3). |
| **Veritabaný** | EF Core Migrations | **Veritabaný Þemasý Yönetimi** | Modelleri PostgreSQL veritabanýna yansýtmak için EF Core Migrations kullanýmý (G1.4). |
| **Hata Yönetimi**| Global Hata Yönetimi | **Custom Middleware** | Tüm API hatalarýnýn (400, 404, 500) standart, tutarlý bir JSON formatýnda döndürülmesi (G2.5). |

---

## 2. Güvenlik ve Ýþ Mantýðý

| Modül | Uç Nokta | Uygulanan Kural | Odak Noktasý |
| :--- | :--- | :--- | :--- |
| **Yetkilendirme** | Kayýt/Giriþ | **JWT Token** | Baþarýlý giriþ sonrasý eriþim için JSON Web Token (JWT) üretimi (G2.2) ve **BCrypt** ile parola hashing (G2.1). |
| **Yetkilendirme** | Hassas Ýþlemler | **`[Authorize]` Nitelikleri** | Ürün ekleme (`POST /products`) ve Sipariþ verme (`POST /orders`) gibi tüm hassas uç noktalarýn geçerli bir JWT ile korunmasý (G2.3). |
| **Ýþ Mantýðý** | Ürün CRUD | **CRUD API** | Ürünler için tam iþlevli (GET, POST, PUT, DELETE) CRUD uç noktalarýnýn Service katmaný üzerinden saðlanmasý (G2.4). |
| **Sipariþ** | Sipariþ Ýþleme | **Stok Kontrolü** | Sipariþ sýrasýnda ürün stok miktarý kontrolü ve yetersiz stok durumunda iþlem reddi (G3.1). |
| **Veri Bütünlüðü** | Sipariþ Ýþlemi | **Veritabaný Transaction** | Stok düþürme ve sipariþ kaydýnýn atomik (ya hep ya hiç) olmasý için Explicit Transaction kullanýmý (G3.2). |
| **Test** | Unit Test (xUnit) | **Moq ve Ýþ Kuralý Testi** | Projenin en kritik mantýðý olan Sipariþ ve Stok Düþürme iþ akýþýnýn xUnit/Moq ile test edilmesi (G3.3). |

---

## 3. Kurulum ve Çalýþtýrma Talimatlarý

### 3.1 Gereksinimler

* **.NET 8 SDK** veya daha yenisi.
* **PostgreSQL** veritabaný sunucusu.

### 3.2 Yapýlandýrma

1.  **Baðlantý Dizesi:** `EStockManager/appsettings.json` dosyasýný açýn ve baðlantý dizesini yerel PostgreSQL sunucunuza göre güncelleyin:

    ```json
    "ConnectionStrings": {
      "PostgreSqlConnection": "Host=localhost;Port=5432;Database=proje_db;Username=postgres;Password=your_password"
    }
    ```

2.  **Veritabaný Güncelleme:** Proje kökünden (Solution seviyesinden) `EStockManager` klasörüne geçerek tüm migration'larý uygulayýn:

    ```bash
    cd EStockManager
    dotnet ef database update
    cd ..
    ```

### 3.3 API'yi Baþlatma

API'yi Solution kökünden çalýþtýrýn:

```bash
dotnet run --project EStockManager/EStockManager.csproj