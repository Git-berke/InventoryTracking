# Database Design

Bu adımda yalnızca domain modeli ve PostgreSQL şeması netleştirildi.

## Tablolar

- `products`: Envanter ürün kartları.
- `warehouses`: Depo tanımları.
- `vehicles`: Araç tanımları.
- `tasks`: Saha görevleri.
- `vehicle_tasks`: Araç-görev atama geçmişi.
- `stock_locations`: Stok tutulabilen fiziksel lokasyonlar. Her depo ve araç için tek kayıt.
- `stock_balances`: Ürünün lokasyon bazlı güncel miktarı.
- `inventory_transactions`: Tüm stok hareketlerinin kronolojik kaydı.

## Tasarım Kararları

- Depo ve araç stokları tek bir `stock_locations` tablosunda normalize edildi.
- Güncel stok ile hareket geçmişi ayrıldı:
  `stock_balances` anlık durumu,
  `inventory_transactions` hareket geçmişini tutar.
- Bir araç göreve çıktıysa, ilgili stok hareketi opsiyonel `task_id` ile göreve bağlanabilir.
- `vehicle_tasks` geçmişi zaman bazlı tutulur; böylece araç birden fazla görevde farklı zamanlarda yer alabilir.

## İlişki Özeti

- `warehouses (1) -> (1) stock_locations`
- `vehicles (1) -> (1) stock_locations`
- `products (1) -> (n) stock_balances`
- `stock_locations (1) -> (n) stock_balances`
- `products (1) -> (n) inventory_transactions`
- `tasks (1) -> (n) vehicle_tasks`
- `vehicles (1) -> (n) vehicle_tasks`
- `tasks (1) -> (n) inventory_transactions`

## Notlar

- `snake_case` isimlendirme için EF naming convention kullanılacak.
- Transfer servislerinde aynı transaction içinde hem `inventory_transactions` hem `stock_balances` güncellenecek.
- Redis ve Elasticsearch bu aşamada tasarıma dahil edilmedi.
