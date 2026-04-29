# Application Layer

Bu adimda yalnizca `Application` katmaninin iskeleti kuruldu.

## Amac

`Application` katmani, API ile veri erisim katmani arasinda use-case seviyesinde bir sozlesme olusturur.

Bu katmanda:

- DTO'lar bulunur
- use-case query kayitlari bulunur
- handler siniflari bulunur
- persistence ve servis abstraction'lari tanimlanir

## Mevcut Yapi

- `DTOs/Common`: ortak sonuc modelleri
- `DTOs/Products`: urun ve stok ozeti cevaplari
- `DTOs/Vehicles`: arac liste ve envanter cevaplari
- `DTOs/Tasks`: gorev liste, gorev-arac ve gorev-envanter cevaplari
- `DTOs/InventoryTransactions`: hareket gecmisi cevaplari
- `Abstractions/Persistence`: db context sozlesmesi
- `Abstractions/Services`: query ve stok transfer servis sozlesmeleri
- `Features/*`: her endpoint/use-case icin query ve handler siniflari

## Neden Simdi

Bu yapi sayesinde:

- API controller'lari dogrudan EF yazmaz
- query mantigi persistence tarafina tasinabilir
- business kurallari servis seviyesinde ayrisabilir
- test yazmak daha kolay hale gelir

## Sonraki Adim

Bir sonraki adimda bu abstraction'larin `Persistence` icindeki gercek implementasyonlarini yazabiliriz.
