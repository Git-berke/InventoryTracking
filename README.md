# Inventory Tracking

`Inventory Tracking`, depo, arac ve gorev bazli envanter takibini hedefleyen full-stack bir uygulamadir. Proje `.NET 10` backend, `Next.js 16` frontend ve `PostgreSQL` veritabani ile calisir.

## Mimari

### Genel Kurgu

- `src/backend/PTN.InventoryTracking.Api`
  HTTP giris noktasi, controllerlar, middleware ve API response contractlari
- `src/backend/PTN.InventoryTracking.Application`
  use-case handler'lar, DTO'lar, abstraction'lar ve security sabitleri
- `src/backend/PTN.InventoryTracking.Domain`
  entity modelleri ve enum'lar
- `src/backend/PTN.InventoryTracking.Infrastructure`
  JWT authentication, authorization, cache ve SignalR entegrasyonu
- `src/backend/PTN.InventoryTracking.Persistence`
  EF Core context, migration'lar, entity configuration'lari, query/service implementasyonlari ve seed data
- `src/frontend`
  Next.js tabanli dashboard arayuzu

### Veritabani Yaklasimi

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

Temel tasarim kararlari:

- depo ve arac stoklari ayri tablolar yerine `stock_locations` ile tek soyutlama altinda tutulur
- anlik stok icin `stock_balances`, hareket gecmisi icin `inventory_transactions` kullanilir
- backend katmanli yapidadir; controller dogrudan EF Core ile konusmaz

## Setup

### Gereksinimler

- `.NET 10 SDK`
- `Node.js 20+`
- `PostgreSQL 15+`

### Environment Variables ve Konfigurasyon

Backend baglantisi iki sekilde verilebilir:

1. `src/backend/PTN.InventoryTracking.Api/appsettings.Development.json` icinde:

```json
{
  "ConnectionStrings": {
    "InventoryTracking": "Host=localhost;Port=5432;Database=ptn_inventory_tracking;Username=postgres;Password=SENIN_SIFREN"
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

2. Environment variable ile:

```powershell
$env:PTN_INVENTORY_DB_CONNECTION="Host=localhost;Port=5432;Database=ptn_inventory_tracking;Username=postgres;Password=SENIN_SIFREN"
```

Frontend icin:

```env
NEXT_PUBLIC_API_URL=http://localhost:5227/api/v1
```

### Projeyi Lokalde Ayaga Kaldirma

```powershell
git clone <repo-url>
cd piton-proje
dotnet tool restore
dotnet restore
```

Veritabanini olustur ve migration'lari uygula:

```powershell
dotnet ef database update --project src/backend/PTN.InventoryTracking.Persistence --startup-project src/backend/PTN.InventoryTracking.Api
```

Backend'i baslat:

```powershell
dotnet run --project src/backend/PTN.InventoryTracking.Api --launch-profile http
```

Frontend'i baslat:

```powershell
cd src/frontend
copy .env.local.example .env.local
npm install
npm run dev
```

Varsayilan adresler:

- Backend: `http://localhost:5227`
- Frontend: `http://localhost:3000`
- Swagger: `http://localhost:5227/swagger`

### Seed Data

Migration sonrasinda demo ve senaryo testi icin hazir veri gelir:

- urunler: `Telsiz`, `Jenerator`, `El Feneri`
- depolar: `Ana Depo`, `Ankara Destek Deposu`
- araclar: `34 ABC 123`, `06 XYZ 789`, `26 ES 001`
- gorevler: `Izmir Saha Destek Gorevi`, `Ankara Acil Enerji Destegi`, `Bursa Hazirlik Gorevi`
- depo ve araclara dagitilmis stok bakiyeleri
- bu stok dagilimina ait ornek hareket gecmisi

Hazir kullanicilar:

| Rol | E-posta | Sifre |
| --- | --- | --- |
| Admin | `admin@ptn.local` | `Admin123!` |
| WarehouseOperator | `warehouse@ptn.local` | `Warehouse123!` |
| TaskManager | `taskmanager@ptn.local` | `Task123!` |

## Usage

### Temel Kullanim Senaryolari

1. `admin@ptn.local` ile giris yap.
2. Urun detayindan stok dagilimini incele.
3. Depo detayindan depo envanterini incele.
4. Arac detayindan arac envanterini incele.
5. Gorev detayindan goreve atali araclari ve gorev envanterini gor.
6. `warehouse -> vehicle` transferi yap ve stok degisimini gozlemle.
7. `vehicle -> warehouse` iadesi yap ve hareket gecmisini kontrol et.
8. Dashboard ve detay ekranlarinda SignalR canli guncellemelerini izle.

### API Uc Noktalari

#### Auth

- `POST /api/v1/auth/login`
- `GET /api/v1/auth/me`

#### Products

- `GET /api/v1/products`
- `GET /api/v1/products/{id}`
- `GET /api/v1/products/{id}/stock-summary`
- `POST /api/v1/products`
- `PUT /api/v1/products/{id}`
- `DELETE /api/v1/products/{id}`

#### Warehouses

- `GET /api/v1/warehouses`
- `GET /api/v1/warehouses/{id}`
- `GET /api/v1/warehouses/{id}/inventories`
- `POST /api/v1/warehouses`
- `PUT /api/v1/warehouses/{id}`
- `DELETE /api/v1/warehouses/{id}`

#### Vehicles

- `GET /api/v1/vehicles`
- `GET /api/v1/vehicles/{id}`
- `GET /api/v1/vehicles/{id}/inventories`
- `POST /api/v1/vehicles`
- `PUT /api/v1/vehicles/{id}`
- `DELETE /api/v1/vehicles/{id}`

#### Tasks

- `GET /api/v1/tasks`
- `GET /api/v1/tasks/{id}`
- `GET /api/v1/tasks/{id}/vehicles`
- `GET /api/v1/tasks/{id}/inventory`
- `POST /api/v1/tasks`
- `PUT /api/v1/tasks/{id}`
- `DELETE /api/v1/tasks/{id}`

#### Inventory Transactions

- `GET /api/v1/inventory-transactions`

#### Stock Transfers

- `POST /api/v1/stock-transfers/warehouse-to-vehicle`
- `POST /api/v1/stock-transfers/vehicle-to-warehouse`

#### Realtime

- SignalR hub: `/hubs/inventory-events`

### Stok Transfer Akisi

#### Depodan Araca Transfer

- urun aktif olmali
- miktar sifirdan buyuk olmali
- kaynak depoda yeterli stok olmali
- hedef arac lokasyonu bulunmali
- gorev `in_progress` olmali
- arac ilgili goreve aktif atanmis olmali

Islem sonunda:

- kaynak `stock_balance` azalir
- hedef `stock_balance` artar
- `inventory_transactions` kaydi olusur
- ilgili urun cache'i temizlenir
- SignalR uzerinden canli event yayini yapilir

#### Aractan Depoya Iade

- miktar sifirdan buyuk olmali
- aracta yeterli stok olmali
- hedef depo lokasyonu bulunmali

Islem sonunda ayni sekilde bakiye, hareket gecmisi, cache invalidation ve canli bildirim guncellenir.

## Teknik Tercihler

### Kullanilan Kutuphaneler ve Teknolojiler

- `ASP.NET Core`
- `Entity Framework Core`
- `Npgsql`
- `Next.js 16`
- `React 19`
- `Axios`
- `Tailwind CSS`
- `JWT Bearer Authentication`
- `SignalR`
- `IDistributedCache` / `Redis`

### Mimari Kararlar ve Gerekceleri

- `stock_locations` kullanildi
  Depo ve arac stoklarini tek modelde toplamak, sorgulari ve domain mantigini sade tuttu.

- `stock_balances` ve `inventory_transactions` ayrildi
  Hem hizli anlik stok sorgusu hem de geriye donuk audit izi birlikte saglandi.

- Katmanli backend mimarisi tercih edildi
  API, application, infrastructure ve persistence sorumluluklari ayrilarak bakim kolaylasti.

- JWT + RBAC kurgusu kullanildi
  Endpoint bazli yetki kontrolu role/permission claim'leri ile netlestirildi.

- Cache-aside deseni secildi
  Sik okunan `product stock summary` sorgularinda veritabani yukunu azaltmak icin.

- SignalR eklendi
  Transfer ve gorev degisikliklerinin frontend'de canli guncellenmesi icin.

- Seed data eklendi
  Kurulumdan hemen sonra bos ekran yerine gercek senaryo testine uygun veri gormek icin.

Bu dosyalar gelistirme surecinde alinan kararlarin kisa teknik ozetlerini icerir.






ekran görüntüleri:


<img width="735" height="821" alt="image" src="https://github.com/user-attachments/assets/0f6d8b8e-64b5-4286-ba14-8d567c7add22" />

<img width="1910" height="870" alt="image" src="https://github.com/user-attachments/assets/126822c2-15b2-4580-b9cf-da8dbf73a9d9" />

<img width="1905" height="544" alt="image" src="https://github.com/user-attachments/assets/7684d8de-bcd4-4822-a436-959f34b37e1e" />


<img width="1719" height="312" alt="image" src="https://github.com/user-attachments/assets/b4365c59-4be6-4212-989e-b5522507b4e2" />

<img width="1919" height="706" alt="image" src="https://github.com/user-attachments/assets/2a91c83f-2a27-438c-97d8-031d1c53d4f3" />

<img width="1919" height="878" alt="image" src="https://github.com/user-attachments/assets/1f0ccbdd-906f-4973-81aa-5af6a4320c4a" />
<img width="751" height="718" alt="image" src="https://github.com/user-attachments/assets/dbae7961-2bb0-41ae-86dc-4f8576049f19" />
