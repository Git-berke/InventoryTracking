# CRUD Layers

Bu adimda ana yonetim kayitlari icin CRUD omurgasi eklendi.

## Kapsam

- `products`
- `warehouses`
- `vehicles`
- `tasks`

## Eklenen Katmanlar

- request/response DTO'lari
- repository interface'leri
- repository implementasyonlari
- management service interface'leri
- management service implementasyonlari

## Notlar

- `products`, `warehouses`, `vehicles` silme islemi su an soft delete mantigiyla `is_active = false` yapar
- `tasks` silme islemi iliskili kayit varsa engellenir
- benzersiz alan kontrolleri servis seviyesinde dogrulanir
- tarih ve temel veri dogrulamalari servis seviyesinde uygulanir
