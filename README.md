# Inventory Tracking

`Inventory Tracking`, bir kurumun envanterlerini depo, arac ve gorev bazli takip etmeyi hedefleyen moduler bir sistemdir. .NET 10 backend + Next.js 16 frontend kombinasyonuyla calisir.

---

## Hizli Baslangic (Klonlayanlar Icin)

### Gereksinimler
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org)
- [PostgreSQL 15+](https://www.postgresql.org/download/)

### 1. Repoyu klonla
```bash
git clone <repo-url>
cd piton-proje
```

### 2. Backend — veritabani baglantisini ayarla

`src/backend/PTN.InventoryTracking.Api/appsettings.Development.json` dosyasini olustur (bu dosya `.gitignore`'da oldugu icin gelmez):

```json
{
  "ConnectionStrings": {
    "InventoryTracking": "Host=localhost;Port=5432;Database=ptn_inventory_tracking;Username=postgres;Password=SENIN_SIFREN"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Jwt": {
    "Issuer": "PTN.InventoryTracking",
    "Audience": "PTN.InventoryTracking.Client",
    "SigningKey": "PTN.InventoryTracking.Development.Signing.Key.2026.04.29",
    "ExpirationMinutes": 120
  },
  "Cache": {
    "UseRedis": false,
    "RedisConnectionString": "localhost:6379",
    "StockSummaryTtlMinutes": 10
  }
}
```

Ya da `appsettings.json` icindeki `YOUR_POSTGRES_PASSWORD` degerini kendi sifrenle degistir.

### 3. Veritabani migration'ini calistir
```bash
cd src/backend/PTN.InventoryTracking.Api
dotnet ef database update
```

### 4. Backend'i baslat

Visual Studio'da `http` profilini sec ve F5.  
Ya da terminalde:
```bash
dotnet run --launch-profile http
```
Backend `http://localhost:5227` adresinde baslar.

### 5. Frontend kurulumu
```bash
cd src/frontend
cp .env.local.example .env.local   # Windows: copy .env.local.example .env.local
npm install
npm run dev
```
Frontend `http://localhost:3000` adresinde baslar.

> **Not:** Backend `localhost:5000`'de baslarsa (Production modu) `.env.local` icindeki URL'yi `http://localhost:5000/api/v1` olarak guncelle.

### 6. Tarayicide ac

`http://localhost:3000` adresine git:

| Rol | E-posta | Sifre |
|-----|---------|-------|
| Admin | `admin@ptn.local` | `Admin123!` |
| Depo Sorumlusu | `warehouse@ptn.local` | `Warehouse123!` |
| Arac Takip | `vehicle@ptn.local` | `Vehicle123!` |

---

## Icerik

- [Proje Ozeti](#proje-ozeti)
- [Mimari](#mimari)
- [Veritabani Tasarimi](#veritabani-tasarimi)
- [Ozellikler](#ozellikler)
- [Kullanilan Teknolojiler](#kullanilan-teknolojiler)
- [Kurulum](#kurulum)
- [Calistirma](#calistirma)
- [Migration ve Seed Data](#migration-ve-seed-data)
- [Swagger ile API Testi](#swagger-ile-api-testi)
- [Kimlik Dogrulama ve Yetkilendirme](#kimlik-dogrulama-ve-yetkilendirme)
- [API Uc Noktalari](#api-uc-noktalari)
- [Stok Transfer Akisi](#stok-transfer-akisi)
- [Redis Cache](#redis-cache)
- [Gercek Zamanli Bildirim](#gercek-zamanli-bildirim)
- [CI/CD](#cicd)
- [Teknik Tercihler](#teknik-tercihler)
- [Mevcut Durum ve Sonraki Adimlar](#mevcut-durum-ve-sonraki-adimlar)

## Proje Ozeti

Dokumandaki ana senaryo su ihtiyaclara odaklaniyor:

- envanterin hangi depoda oldugunu gormek
- bir urunun araca ne zaman yuklendigini izlemek
- aracin hangi gorevde oldugunu gormek
- lokasyon bazli stok dagilimini listelemek
- stok hareketlerinin gecmisini kaydetmek
- gorev tamamlendiginda envanteri tekrar depoya alabilmek

Bu repo icinde bu ihtiyaclara cevap verecek backend altyapisi kuruldu:

- normalize PostgreSQL semasi
- EF Core migration ve seed data
- query servisleri
- CRUD servisleri
- stok transfer business kurallari
- JWT authentication
- role/permission tabanli RBAC
- standart API response ve global error handling
- urun stok ozeti icin Redis tabanli distributed cache (in-memory fallback ile)

## Mimari

Proje katmanli bir yapida kurgulandi:

- `src/backend/PTN.InventoryTracking.Api`
  REST API, controllerlar, middleware, response contractlari
- `src/backend/PTN.InventoryTracking.Application`
  DTO'lar, use-case handler'lar, abstraction'lar, security constant'lari
- `src/backend/PTN.InventoryTracking.Domain`
  entity modelleri ve enum'lar
- `src/backend/PTN.InventoryTracking.Infrastructure`
  JWT token uretimi, authentication ve authorization konfigurasyonu
- `src/backend/PTN.InventoryTracking.Persistence`
  EF Core context, entity configuration'lari, repository'ler, query service'ler, migration'lar, seed data
- `tests/backend/PTN.InventoryTracking.UnitTests`
  unit test projesi
- `tests/backend/PTN.InventoryTracking.IntegrationTests`
  integration test projesi

Katmanlarin sorumluluklari ayrildi:

- `Domain` sadece is modelini bilir
- `Application` is akislarinin sozlesmelerini tanimlar
- `Persistence` veriye nasil ulasilacagini bilir
- `Infrastructure` cross-cutting guvenlik altyapisini saglar
- `Api` sadece HTTP giris/cikis katmanidir

## Veritabani Tasarimi

PDF'te istenen normalize yapi temel alindi. `snake_case` isimlendirme kullanildi ve iliskiler foreign key ile kuruldu.

Ana tablolar:

- `products`
- `warehouses`
- `vehicles`
- `tasks`
- `vehicle_tasks`
- `stock_locations`
- `stock_balances`
- `inventory_transactions`
- `app_users`
- `app_roles`
- `app_permissions`
- `app_user_roles`
- `app_role_permissions`

Tasarim kararlarindan biri `warehouse_stocks` ve `vehicle_stocks` gibi ayri tablolar yerine tek bir `stock_locations` modeli kullanmaktir. Boylece depo ve arac lokasyonlari ayni soyutlama altinda izlenebilir.

Stok durumu iki farkli seviyede tutulur:

- `stock_balances`: anlik stok miktari
- `inventory_transactions`: hareket gecmisi

Bu ayrim sayesinde hem hizli sorgu hem de audit izi korunur.

## Ozellikler

Su anki backend kapsaminda tamamlanan basliklar:

- urun, depo, arac ve gorev yonetimi icin CRUD servisleri
- urun stok ozeti sorgulari
- arac uzerindeki envanteri gorme
- gorev bazli arac ve envanter listesi
- stok hareket gecmisi sorgusu
- `warehouse -> vehicle` transferi
- `vehicle -> warehouse` iadesi
- transferlerde DB transaction ile veri tutarliligi
- JWT login
- permission claim tabanli endpoint korumasi
- global exception middleware
- standart API response zarfi
- request validation
- SignalR tabanli gercek zamanli bildirim omurgasi
- urun stok ozeti uzerinde cache-aside deseni ile distributed cache

## Kullanilan Teknolojiler

- `.NET 10`
- `ASP.NET Core`
- `Entity Framework Core`
- `PostgreSQL`
- `Npgsql`
- `JWT Bearer Authentication`
- `Redis (StackExchange.Redis / IDistributedCache)` - opsiyonel, varsayilan in-memory fallback ile
- `SignalR`
- `GitHub Actions`

Not:
- `Elasticsearch` su an dahil degil
- `Next.js` frontend su an bu repo kapsaminda yok

## Kurulum

Gereksinimler:

- `.NET SDK 10`
- `PostgreSQL 14+`
- opsiyonel olarak `pgAdmin`

Repoyu klonladiktan sonra:

```powershell
git clone https://github.com/Git-berke/InventoryTracking.git
cd InventoryTracking
dotnet tool restore
dotnet restore
```

## Calistirma

Backend uygulamasini ayaga kaldirmak icin asagidaki adimlari izleyin.

### 1. PostgreSQL Hazir Oldugundan Emin Olun

Yerel makinenizde PostgreSQL servisinin calistigindan ve `5432` portunu dinledigi nden emin olun. Eger sifre `postgres` disinda bir deger ise (ornegin `123`), sonraki adimda bunu connection string'e yansitin.

### 2. Connection String'i Ayarlayin

Iki secenek var:

Secenek A - `appsettings.Development.json` icine ekleyin (onerilen):

```json
{
  "ConnectionStrings": {
    "InventoryTracking": "Host=localhost;Port=5432;Database=ptn_inventory_tracking;Username=postgres;Password=123"
  }
}
```

Secenek B - environment variable kullanin:

```powershell
$env:PTN_INVENTORY_DB_CONNECTION="Host=localhost;Port=5432;Database=ptn_inventory_tracking;Username=postgres;Password=123"
```

### 3. Veritabanini Olusturun

Migration uygulamak icin (detay icin [Migration ve Seed Data](#migration-ve-seed-data)):

```powershell
dotnet ef database update --project src/backend/PTN.InventoryTracking.Persistence --startup-project src/backend/PTN.InventoryTracking.Api
```

### 4. API'yi Baslatin

#### Komut satiri ile

```powershell
dotnet run --project src/backend/PTN.InventoryTracking.Api --launch-profile http
```

veya HTTPS icin:

```powershell
dotnet run --project src/backend/PTN.InventoryTracking.Api --launch-profile https
```

#### Visual Studio ile

1. Solution Explorer'da `PTN.InventoryTracking.Api` projesine sag tiklayin -> **Set as Startup Project**
2. Yesil play butonunun yanindaki dropdown'dan **`http`** veya **`https`** profilini secin (`PTN.InventoryTracking.Api` yani `.exe` profilini secmeyin, aksi halde Production modda calisir ve Swagger acilmaz)
3. **F5**'e basin

### 5. Calismakta Olan Adresler

| Profil  | Adres                       |
| ------- | --------------------------- |
| `http`  | http://localhost:5227       |
| `https` | https://localhost:7180      |

Uygulama basariyla baslayinca konsolda su goruntulenir:

```text
Now listening on: http://localhost:5227
Hosting environment: Development
```

`Hosting environment: Development` yazisi onemlidir; aksi halde Swagger ve OpenAPI uc noktalari devre disi kalir.

## Migration ve Seed Data

Veritabanini hazirlamak icin once connection string'i ayarlayabilirsin:

```powershell
$env:PTN_INVENTORY_DB_CONNECTION="Host=localhost;Port=5432;Database=ptn_inventory_tracking;Username=postgres;Password=123"
```

Migration uygulamak icin:

```powershell
dotnet ef database update --project src/backend/PTN.InventoryTracking.Persistence --startup-project src/backend/PTN.InventoryTracking.Api
```

Seed data ile birlikte su senaryolar gelir:

- `Telsiz`, `Jenerator`, `El Feneri`
- `Ana Depo`, `Ankara Destek Deposu`
- `34 ABC 123`, `06 XYZ 789`, `26 ES 001`
- `Izmir Saha Destek Gorevi`, `Ankara Acil Enerji Destegi`, `Bursa Hazirlik Gorevi`
- depoda ve araclarda dagitilmis ornek stoklar
- bu stoklara ait ornek hareket gecmisi

Bu sayede veritabani kurulduktan sonra endpointler bos donmez.

## Swagger ile API Testi

Uygulama Development modunda calisirken Swagger UI uzerinden tum endpointleri tarayicidan test edebilirsiniz.

### 1. Swagger UI'i Acin

API ayaga kalktiktan sonra tarayicinizdan acin:

```text
http://localhost:5227/swagger
```

(HTTPS profili kullaniyorsaniz `https://localhost:7180/swagger`.)

Sayfa yuklendiginde sol tarafta endpoint gruplari (`Auth`, `Products`, `Warehouses`, `Vehicles`, `Tasks`, `InventoryTransactions`, `StockTransfers`) listelenir.

### 2. Login Olun ve Token Alin

Stok ve gorev endpointleri yetki ister. Token almak icin:

1. `Auth` grubunu acin -> `POST /api/v1/auth/login`
2. **Try it out** butonuna basin
3. Request body'yi su sekilde doldurun:

```json
{
  "email": "admin@ptn.local",
  "password": "Admin123!"
}
```

4. **Execute** butonuna basin
5. Response icindeki `data.accessToken` degerini kopyalayin (uzun bir JWT karakter dizisi)

Seed kullanicilar:

| Kullanici             | Sifre            | Rol                |
| --------------------- | ---------------- | ------------------ |
| `admin@ptn.local`     | `Admin123!`      | Admin (tum yetki)  |
| `warehouse@ptn.local` | `Warehouse123!`  | WarehouseOperator  |
| `taskmanager@ptn.local` | `Task123!`     | TaskManager        |

### 3. Token'i Authorize Butonuyla Ekleyin

1. Sayfanin sag ust kosesindeki **Authorize** butonuna tiklayin (kilit ikonu)
2. Acilan kutuya kopyaladiginiz token'i **oldugu gibi** yapistirin (basina `Bearer ` eklemeyin, otomatik eklenir)
3. **Authorize** -> **Close**

Token tum endpoint isteklerine `Authorization: Bearer <token>` header'i olarak otomatik eklenir.

### 4. Endpointleri Deneyin

Ornek senaryolar:

- `GET /api/v1/products` ile urun listesini alin
- `GET /api/v1/products/{id}/stock-summary` ile bir urunun lokasyon bazli stok dagilimini gorun
- `POST /api/v1/stock-transfers/warehouse-to-vehicle` ile depodan araca transfer yapin
- `GET /api/v1/inventory-transactions` ile hareket gecmisini gorun
- `GET /api/v1/auth/me` ile token icindeki kullanici bilgilerini ve permission'lari gorun

### 5. Rol Bazli Davranisi Gozlemleyin

Authorize butonuna tekrar basip **Logout** dedikten sonra `warehouse@ptn.local` ile login olup ayni endpointleri deneyebilirsiniz. Yetkisiz oldugunuz endpointler `403 Forbidden` doner. Ornegin:

- `POST /api/v1/products` -> WarehouseOperator icin **403 Forbidden** (bu rolde `products.create` izni yok)
- `GET /api/v1/warehouses/{id}` -> WarehouseOperator icin **200 OK**

### Sik Yasanan Sorunlar

| Belirti | Sebep | Cozum |
| --- | --- | --- |
| `/swagger` 404 doner | Production modda calisiyor | Launch profile olarak `http` veya `https` secin, `ASPNETCORE_ENVIRONMENT=Development` olmalidir |
| Login `500 Internal Server Error` | PostgreSQL bag lantisi basarisiz | Connection string ve `postgres` sifresini kontrol edin |
| `401 Unauthorized` | Token suresi dolmus veya eklenmemis | Tekrar login olup Authorize butonu uzerinden token'i yenileyin |
| `403 Forbidden` | Mevcut rolde gerekli permission yok | Admin hesabi ile login olun veya RBAC tablosunu inceleyin |

## Kimlik Dogrulama ve Yetkilendirme

Sistem JWT Bearer kullanir. Login endpointi:

```text
POST /api/v1/auth/login
```

Ornek seed kullanicilar:

- `admin@ptn.local` / `Admin123!`
- `warehouse@ptn.local` / `Warehouse123!`
- `taskmanager@ptn.local` / `Task123!`

Giris basarili oldugunda `Bearer` token doner. Sonraki isteklerde:

```text
Authorization: Bearer <token>
```

kullanilmalidir.

### RBAC Kurgusu

Roller:

- `Admin`
- `WarehouseOperator`
- `TaskManager`

Permission mantigi endpoint policy'leri ile calisir. Ornek permission'lar:

- `products.read`
- `products.create`
- `vehicles.update`
- `tasks.create`
- `inventory-transactions.read`
- `stock-transfers.create`

Yaklasim su sekildedir:

- rol kullaniciya atanir
- permission role atanir
- login sirasinda permission claim'leri tokene yazilir
- endpointler ilgili policy ile korunur

## API Uc Noktalari

### Auth

- `POST /api/v1/auth/login`
- `GET /api/v1/auth/me`

### Products

- `GET /api/v1/products`
- `GET /api/v1/products/{id}`
- `GET /api/v1/products/{id}/stock-summary`
- `POST /api/v1/products`
- `PUT /api/v1/products/{id}`
- `DELETE /api/v1/products/{id}`

### Warehouses

- `GET /api/v1/warehouses/{id}`
- `POST /api/v1/warehouses`
- `PUT /api/v1/warehouses/{id}`
- `DELETE /api/v1/warehouses/{id}`

### Vehicles

- `GET /api/v1/vehicles`
- `GET /api/v1/vehicles/{id}`
- `GET /api/v1/vehicles/{id}/inventories`
- `POST /api/v1/vehicles`
- `PUT /api/v1/vehicles/{id}`
- `DELETE /api/v1/vehicles/{id}`

### Tasks

- `GET /api/v1/tasks`
- `GET /api/v1/tasks/{id}`
- `GET /api/v1/tasks/{id}/vehicles`
- `GET /api/v1/tasks/{id}/inventory`
- `POST /api/v1/tasks`
- `PUT /api/v1/tasks/{id}`
- `DELETE /api/v1/tasks/{id}`

### Inventory Transactions

- `GET /api/v1/inventory-transactions`

### Stock Transfers

- `POST /api/v1/stock-transfers/warehouse-to-vehicle`
- `POST /api/v1/stock-transfers/vehicle-to-warehouse`

### SignalR Hub

- `/hubs/inventory-events`

## Stok Transfer Akisi

### Depodan Araca Transfer

Kurallar:

- miktar sifirdan buyuk olmali
- urun aktif olmali
- kaynak depo lokasyonu bulunmali
- kaynak stok yeterli olmali
- hedef arac lokasyonu bulunmali
- transfer bir gorev ile iliskili olmali
- gorev `in_progress` olmali
- arac o goreve aktif atanmis olmali

Transfer sirasinda ayni transaction icinde:

- kaynak `stock_balance` azaltilir
- hedef `stock_balance` arttirilir
- `inventory_transactions` kaydi eklenir

### Aractan Depoya Iade

Kurallar:

- miktar sifirdan buyuk olmali
- arac uzerinde yeterli stok olmali
- hedef depo lokasyonu bulunmali

Bu islem de tek DB transaction icinde tamamlanir.

## Redis Cache

Urun stok ozeti uzerinde **cache-aside** deseni uygulandi. Hedef, sik sorgulanan ancak nadir degisen `GET /api/v1/products/{id}/stock-summary` endpointinin DB yukunu azaltmak ve yanit suresini dusurmektir.

### Mimari

`IDistributedCache` abstraction'i kullanildi, somut implementasyon `appsettings` ile konfigure edilir:

- `UseRedis: true` -> StackExchange.Redis tabanli `AddStackExchangeRedisCache`
- `UseRedis: false` -> `AddDistributedMemoryCache` (in-memory fallback)

Bu sayede development sirasinda Redis kurulu olmasa bile cache mantigi calisir, production'da sadece konfigurasyon degisikligi ile dagitik bir Redis sunucusuna gecilir.

### Konfigurasyon

`appsettings.json` icindeki `Cache` sekmesi:

```json
{
  "Cache": {
    "UseRedis": false,
    "RedisConnectionString": "localhost:6379",
    "StockSummaryTtlMinutes": 10
  }
}
```

| Anahtar | Aciklama |
| --- | --- |
| `UseRedis` | `true` ise Redis, `false` ise in-memory cache kullanilir |
| `RedisConnectionString` | StackExchange.Redis baglanti string'i |
| `StockSummaryTtlMinutes` | Cache entry'sinin yasam suresi (dakika) |

Kayitlar cache icine `ptn-inventory:product-stock-summary:{productId}` formatinda yazilir (`InstanceName` prefix'i ile).

### Cache-Aside Akisi

`GetProductStockSummaryAsync` icindeki akis:

1. Cache'e bakilir; entry varsa dogrudan donulur.
2. Yoksa DB'den hesaplanir.
3. Hesaplanan ozet cache'e yazilir (`AbsoluteExpirationRelativeToNow = TtlMinutes`).
4. Caller'a donulur.

Cache miss durumunda 1 sorgu, hit durumunda 0 sorgu olusur.

### Cache Invalidation

Stok degeri degisebilecek aksiyonlardan sonra ilgili `productId` icin cache temizlenir:

- `POST /api/v1/stock-transfers/warehouse-to-vehicle`
- `POST /api/v1/stock-transfers/vehicle-to-warehouse`
- `PUT /api/v1/products/{id}`
- `DELETE /api/v1/products/{id}`

Bu sayede stale veri donulmesi onlenir.

### Dosya Konumlari

- `src/backend/PTN.InventoryTracking.Infrastructure/Caching/CacheOptions.cs`
- `src/backend/PTN.InventoryTracking.Infrastructure/Caching/RedisProductStockSummaryCacheService.cs`
- `src/backend/PTN.InventoryTracking.Infrastructure/Extensions/DependencyInjection.cs` (DI kayitlari)
- `src/backend/PTN.InventoryTracking.Persistence/QueryServices/ProductQueries.cs` (cache-aside akisi)

### Hizla Test Etme

Authorize olduktan sonra Postman'de veya Swagger'da:

```text
GET http://localhost:5227/api/v1/products/11111111-1111-1111-1111-111111111111/stock-summary
```

isteklerini art arda yollayin. Ilk istek DB'ye gider (~50-150 ms), sonraki istekler cache'ten doner (~5-15 ms). Bu fark cache'in etkin oldugunun gostergesidir.

Redis kuruluysa redis-cli ile dogrudan da inceleyebilirsiniz:

```bash
redis-cli
> KEYS ptn-inventory:*
> TTL "ptn-inventory:product-stock-summary:11111111-1111-1111-1111-111111111111"
```

## Gercek Zamanli Bildirim

PDF'teki arti deger maddesi icin SignalR tabanli temel bir bildirim omurgasi eklendi.

Hub:

```text
/hubs/inventory-events
```

JWT ile korunur. Baglanan client'lar `inventory-event` method adi uzerinden event alir.

Yayinlanan temel eventler:

- `stock.transfer.completed`
- `stock.return.completed`
- `task.created`
- `task.updated`

Bu sayede kritik aksiyonlardan sonra istemci tarafina anlik bildirim akisi saglanabilir.

## CI/CD

Repo icine temel bir GitHub Actions workflow eklendi.

Amaç:

- `dotnet restore`
- `dotnet build`
- `dotnet test`

Workflow dosyasi:

```text
.github/workflows/ci.yml
```

Bu pipeline her `push` ve `pull_request` olayinda solution'i derler ve testleri kosar.

## Teknik Tercihler

Bu projede bazi bilincli teknik kararlar alindi:

- `stock_locations` ile depo ve arac stoklari tek modelde toplandi
- `stock_balances` ve `inventory_transactions` ayri tutularak hem performans hem audit izi korundu
- controller'larin EF Core bilmemesi icin `Application` ve `Persistence` ayrildi
- JWT ve RBAC `Infrastructure` katmanina alindi
- standart response zarfi ile API davranisi tek tip hale getirildi
- validation icin request DTO seviyesinde data annotation kullanildi
- beklenmeyen hatalar middleware katmaninda ele alindi

## Mevcut Durum ve Sonraki Adimlar

Backend tarafinda guclu bir temel hazir durumda. Ancak PDF'in tum beklentileri acisindan henuz yapilmamis basliklar da var:

-

## Dizin Notlari

Projeyi adim adim ilerletirken birkac yardimci teknik not da `docs/backend` altinda tutuldu:

- `structure.md`
- `database-design.md`
- `seed-data.md`
- `application-layer.md`
- `crud-layers.md`
- `stock-transfer-service.md`
- `authentication.md`
- `realtime-notifications.md`

Bu dosyalar gelistirme surecinde alinan kararlarin kisa teknik ozetlerini icerir.






ekran görüntüleri:


<img width="735" height="821" alt="image" src="https://github.com/user-attachments/assets/0f6d8b8e-64b5-4286-ba14-8d567c7add22" />

<img width="1910" height="870" alt="image" src="https://github.com/user-attachments/assets/126822c2-15b2-4580-b9cf-da8dbf73a9d9" />

<img width="1905" height="544" alt="image" src="https://github.com/user-attachments/assets/7684d8de-bcd4-4822-a436-959f34b37e1e" />


<img width="1719" height="312" alt="image" src="https://github.com/user-attachments/assets/b4365c59-4be6-4212-989e-b5522507b4e2" />

<img width="1919" height="706" alt="image" src="https://github.com/user-attachments/assets/2a91c83f-2a27-438c-97d8-031d1c53d4f3" />

<img width="1919" height="878" alt="image" src="https://github.com/user-attachments/assets/1f0ccbdd-906f-4973-81aa-5af6a4320c4a" />
<img width="751" height="718" alt="image" src="https://github.com/user-attachments/assets/dbae7961-2bb0-41ae-86dc-4f8576049f19" />


