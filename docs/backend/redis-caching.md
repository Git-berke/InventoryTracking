# Redis Caching

Bu adimda sik erisilen `product stock summary` sorgulari icin Redis destekli cache omurgasi eklendi.

## Kapsam

- `GET /api/v1/products/{id}/stock-summary`

## Davranis

- once cache kontrol edilir
- cache varsa veri Redis'ten doner
- cache yoksa veri veritabanindan okunur ve cache'e yazilir

## Invalidation

Su aksiyonlardan sonra ilgili urunun stok ozeti cache'i silinir:

- urun guncelleme
- urun silme
- depodan araca transfer
- aractan depoya iade

## Konfigurasyon

`appsettings.json` altinda:

- `Cache:UseRedis`
- `Cache:RedisConnectionString`
- `Cache:StockSummaryTtlMinutes`

Redis ayari yoksa veya `UseRedis=false` ise sistem `DistributedMemoryCache` fallback ile calismaya devam eder.
