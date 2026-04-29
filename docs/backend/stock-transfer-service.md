# Stock Transfer Service

Bu adimda ilk gercek business rule servisi eklendi.

## Kapsam

- `warehouse -> vehicle`
- `vehicle -> warehouse`

## Uygulanan Kurallar

- miktar `0` veya negatif olamaz
- urun aktif degilse transfer yapilamaz
- kaynak lokasyon bulunamazsa islem durur
- kaynak stok miktari yetersizse islem durur
- araca cikis icin gorev zorunludur
- gorev `in_progress` degilse araca yukleme yapilamaz
- arac ilgili goreve aktif atanmis degilse yukleme yapilamaz

## Veri Tutarliligi

Her transfer tek database transaction icinde yapilir:

- `stock_balances` guncellenir
- `inventory_transactions` kaydi eklenir

Bu sayede hareket kaydi ile anlik stok birbirinden kopmaz.
