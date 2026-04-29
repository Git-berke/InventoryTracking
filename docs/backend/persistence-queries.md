# Persistence Queries

Bu adimda `Application` katmaninda tanimlanan query sozlesmelerinin gercek `Persistence` implementasyonlari eklendi.

## Eklenen Servisler

- `ProductQueries`
- `VehicleQueries`
- `TaskQueries`
- `InventoryTransactionQueries`

## Sorumluluk

Bu servisler:

- EF Core uzerinden veriyi okur
- domain entity donmek yerine `Application DTO` dondurur
- sayfalama ve temel filtreleme uygular
- controller ya da API katmaninin SQL/EF detayi bilmesini engeller

## DI

`AddPersistence(...)` extension'i ile:

- `InventoryTrackingDbContext`
- query servisleri
- application persistence sozlesmesi

tek noktadan kaydedilebilir hale getirildi.
