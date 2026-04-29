# Seed Data

Bu adimda veritabanina anlamli ornek veri eklenmesi icin temel seed senaryosu hazirlandi.

## Senaryo

- `Ana Depo` icinde 10 adet `Telsiz` bulunur.
- `34 ABC 123` aracinda 5 adet `Telsiz` ve 1 adet `Jenerator` bulunur.
- `06 XYZ 789` aracinda 5 adet `Telsiz` bulunur.
- `Ankara Destek Deposu` icinde 2 adet `Jenerator` bulunur.
- `Ana Depo` icinde 12 adet `El Feneri` bulunur.
- `26 ES 001` aracinda 4 adet `El Feneri` bulunur.

## Gorevler

- `Izmir Saha Destek Gorevi`: aktif gorev
- `Ankara Acil Enerji Destegi`: tamamlanmis gorev
- `Bursa Hazirlik Gorevi`: taslak gorev

## Arac Gorev Iliskisi

- `34 ABC 123` ve `06 XYZ 789` aktif olarak `Izmir Saha Destek Gorevi`ne bagli
- `26 ES 001` tamamlanmis `Ankara Acil Enerji Destegi` kaydina bagli

## Hareket Gecmisi

Seed icinde yalnizca anlik stok degil, bu stok dagilimina yol acan ornek hareketler de bulunur:

- depoya ilk giris hareketleri
- depodan araca transfer hareketleri
- gorev baglantili transfer kayitlari

Bu sayede `stock summary`, `vehicle inventory`, `task inventory` ve `inventory transactions` sorgulari bos donmez.
