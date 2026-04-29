# Authentication and Authorization

Bu adimda JWT authentication ve role/permission tabanli RBAC altyapisi eklendi.

## Eklenen Tablolar

- `app_users`
- `app_roles`
- `app_permissions`
- `app_user_roles`
- `app_role_permissions`

## JWT Akisi

- `POST /api/v1/auth/login` ile giris yapilir
- basarili giriste `Bearer` token doner
- tum diger endpointler varsayilan olarak authentication ister

## Seed Kullanicilar

- `admin@ptn.local` / `Admin123!`
- `warehouse@ptn.local` / `Warehouse123!`
- `taskmanager@ptn.local` / `Task123!`

## Roller

- `Admin`: tum izinlere sahiptir
- `WarehouseOperator`: depo ve transfer odakli izinlere sahiptir
- `TaskManager`: gorev ve arac odakli izinlere sahiptir

## Notlar

- permission claim tipi `permission` olarak token icine yazilir
- endpointler `Authorize(Policy = "...")` ile korunur
- login sonrasi `GET /api/v1/auth/me` ile mevcut kullanici okunabilir
