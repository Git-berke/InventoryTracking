# Inventory Tracking

`Inventory Tracking`, bir kurumun envanterlerini depo, arac ve gorev bazli takip etmeyi hedefleyen moduler bir backend servisidir. Bu repo, PITON technical case dokumanindaki senaryoya gore hazirlanmis backend omurgasini icerir.

Bu surumda odak tamamen backend tarafindadir. Redis, Elasticsearch, SignalR ve frontend taraflari bilerek kapsam disinda birakildi; buna karsin veritabani modeli, stok transfer kurallari, JWT authentication, RBAC ve temel REST API omurgasi calisir durumdadir.

## Icerik

- [Proje Ozeti](#proje-ozeti)
- [Mimari](#mimari)
- [Veritabani Tasarimi](#veritabani-tasarimi)
- [Ozellikler](#ozellikler)
- [Kullanilan Teknolojiler](#kullanilan-teknolojiler)
- [Kurulum](#kurulum)
- [Calistirma](#calistirma)
- [Migration ve Seed Data](#migration-ve-seed-data)
- [Kimlik Dogrulama ve Yetkilendirme](#kimlik-dogrulama-ve-yetkilendirme)
- [API Uc Noktalari](#api-uc-noktalari)
- [Stok Transfer Akisi](#stok-transfer-akisi)
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

## Kullanilan Teknolojiler

- `.NET 10`
- `ASP.NET Core`
- `Entity Framework Core`
- `PostgreSQL`
- `Npgsql`
- `JWT Bearer Authentication`
- `GitHub Actions`

Not:
- `Redis` su an dahil degil
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

API'yi lokalde calistirmak icin:

```powershell
dotnet run --project src/backend/PTN.InventoryTracking.Api
```

Varsayilan development HTTP adresi:

```text
http://localhost:5227
```

OpenAPI tanimi development ortaminda acilir.

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

- frontend uygulamasi
- GitHub Actions disinda daha ileri deployment CI/CD
- merkezi log sink entegrasyonu
- Redis cache
- Elasticsearch
- daha kapsamli unit/integration testler
- daha detayli authorization yonetim ekranlari veya kullanici yonetimi

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
