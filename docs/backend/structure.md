# Backend Structure

Bu aşamada yalnızca backend iskeleti oluşturuldu.

## Solution

- `PTN.InventoryTracking.slnx`: Tüm backend ve test projelerini toplar.

## Source

- `src/backend/PTN.InventoryTracking.Api`: REST API katmanı, controller'lar, middleware, contract'lar.
- `src/backend/PTN.InventoryTracking.Application`: Use-case'ler, servis sözleşmeleri, DTO'lar, davranışlar.
- `src/backend/PTN.InventoryTracking.Domain`: Entity, enum ve domain seviyesindeki ortak tipler.
- `src/backend/PTN.InventoryTracking.Infrastructure`: JWT, dış servis adaptörleri ve altyapı servisleri.
- `src/backend/PTN.InventoryTracking.Persistence`: DbContext, entity configuration, repository ve seed alanı.

## Tests

- `tests/backend/PTN.InventoryTracking.UnitTests`: Domain ve application odaklı birim testleri.
- `tests/backend/PTN.InventoryTracking.IntegrationTests`: API ve veri erişim akışları için entegrasyon testleri.

## Notes

- Redis ve Elasticsearch bu yapıya bilinçli olarak eklenmedi.
- Sonraki adımda domain modelini ve veritabanı tasarımını netleştirebiliriz.

