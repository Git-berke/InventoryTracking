"use client";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import Link from "next/link";
import {
  ArrowLeft,
  Warehouse,
  Truck,
  Package,
  AlertCircle,
} from "lucide-react";
import Topbar from "@/components/Topbar";
import api from "@/lib/api";

interface StockLocation {
  stockLocationId: string;
  stockLocationName: string;
  locationType: "warehouse" | "vehicle";
  warehouseName: string | null;
  vehicleLicensePlate: string | null;
  quantity: number;
}

interface StockSummary {
  productId: string;
  productCode: string;
  productName: string;
  unit: string;
  totalQuantity: number;
  warehouseQuantity: number;
  vehicleQuantity: number;
  stockDistribution: StockLocation[];
}

export default function ProductStockPage() {
  const { id } = useParams<{ id: string }>();
  const [data, setData] = useState<StockSummary | null>(null);
  const [loading, setLoading] = useState(true);
  const [notFound, setNotFound] = useState(false);

  useEffect(() => {
    api
      .get<{ data: StockSummary }>(`/products/${id}/stock-summary`)
      .then((res) => setData(res.data.data))
      .catch((err) => {
        if (err.response?.status === 404) setNotFound(true);
      })
      .finally(() => setLoading(false));
  }, [id]);

  return (
    <>
      <Topbar title={data ? `${data.productName} — Stok Dağılımı` : "Stok Dağılımı"} />
      <main className="flex-1 p-6 space-y-5">
        <Link
          href="/products"
          className="inline-flex items-center gap-1.5 text-sm text-slate-500 hover:text-slate-800 transition-colors"
        >
          <ArrowLeft className="w-4 h-4" />
          Ürünler listesine dön
        </Link>

        {loading && (
          <div className="space-y-4">
            <div className="h-28 bg-white rounded-xl border border-slate-200 animate-pulse" />
            <div className="h-48 bg-white rounded-xl border border-slate-200 animate-pulse" />
          </div>
        )}

        {notFound && (
          <div className="flex flex-col items-center justify-center py-16 text-slate-400">
            <AlertCircle className="w-10 h-10 mb-3" />
            <p className="font-medium text-slate-600">Ürün bulunamadı.</p>
          </div>
        )}

        {data && (
          <>
            {/* Özet kartlar */}
            <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
              <div className="bg-white rounded-xl border border-slate-200 shadow-sm p-5">
                <div className="flex items-center gap-3 mb-3">
                  <div className="w-9 h-9 bg-blue-50 rounded-lg flex items-center justify-center">
                    <Package className="w-5 h-5 text-blue-500" />
                  </div>
                  <span className="text-sm font-medium text-slate-500">
                    Toplam Stok
                  </span>
                </div>
                <p className="text-3xl font-bold text-slate-800">
                  {data.totalQuantity}
                </p>
                <p className="text-xs text-slate-400 mt-1">{data.unit}</p>
              </div>

              <div className="bg-white rounded-xl border border-slate-200 shadow-sm p-5">
                <div className="flex items-center gap-3 mb-3">
                  <div className="w-9 h-9 bg-amber-50 rounded-lg flex items-center justify-center">
                    <Warehouse className="w-5 h-5 text-amber-500" />
                  </div>
                  <span className="text-sm font-medium text-slate-500">
                    Depoda
                  </span>
                </div>
                <p className="text-3xl font-bold text-slate-800">
                  {data.warehouseQuantity}
                </p>
                <p className="text-xs text-slate-400 mt-1">
                  {data.totalQuantity > 0
                    ? `Toplam stokun %${Math.round((data.warehouseQuantity / data.totalQuantity) * 100)}'i`
                    : "—"}
                </p>
              </div>

              <div className="bg-white rounded-xl border border-slate-200 shadow-sm p-5">
                <div className="flex items-center gap-3 mb-3">
                  <div className="w-9 h-9 bg-emerald-50 rounded-lg flex items-center justify-center">
                    <Truck className="w-5 h-5 text-emerald-500" />
                  </div>
                  <span className="text-sm font-medium text-slate-500">
                    Araçlarda
                  </span>
                </div>
                <p className="text-3xl font-bold text-slate-800">
                  {data.vehicleQuantity}
                </p>
                <p className="text-xs text-slate-400 mt-1">
                  {data.totalQuantity > 0
                    ? `Toplam stokun %${Math.round((data.vehicleQuantity / data.totalQuantity) * 100)}'i`
                    : "—"}
                </p>
              </div>
            </div>

            {/* Stok dağılım tablosu */}
            <div className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
              <div className="px-6 py-4 border-b border-slate-100 flex items-center gap-2">
                <span className="font-semibold text-slate-800 text-sm">
                  Lokasyon Bazlı Dağılım
                </span>
                <span className="text-xs text-slate-400">
                  ({data.stockDistribution.length} lokasyon)
                </span>
              </div>

              {data.stockDistribution.length === 0 ? (
                <div className="flex flex-col items-center py-12 text-slate-400">
                  <AlertCircle className="w-7 h-7 mb-2" />
                  <p className="text-sm">Stok kaydı yok.</p>
                </div>
              ) : (
                <div className="divide-y divide-slate-50">
                  {data.stockDistribution.map((loc) => {
                    const isWarehouse = loc.locationType === "warehouse";
                    const pct =
                      data.totalQuantity > 0
                        ? Math.round((loc.quantity / data.totalQuantity) * 100)
                        : 0;

                    return (
                      <div
                        key={loc.stockLocationId}
                        className="flex items-center gap-4 px-6 py-4 hover:bg-slate-50/50 transition-colors"
                      >
                        {/* İkon */}
                        <div
                          className={`w-9 h-9 rounded-lg flex items-center justify-center shrink-0 ${
                            isWarehouse ? "bg-amber-50" : "bg-emerald-50"
                          }`}
                        >
                          {isWarehouse ? (
                            <Warehouse className="w-4 h-4 text-amber-500" />
                          ) : (
                            <Truck className="w-4 h-4 text-emerald-500" />
                          )}
                        </div>

                        {/* Bilgi */}
                        <div className="flex-1 min-w-0">
                          <p className="text-sm font-medium text-slate-800 truncate">
                            {isWarehouse
                              ? loc.warehouseName
                              : loc.vehicleLicensePlate}
                          </p>
                          <p className="text-xs text-slate-400 truncate">
                            {loc.stockLocationName}
                          </p>
                        </div>

                        {/* Progress bar */}
                        <div className="hidden sm:flex flex-col items-end w-40">
                          <div className="w-full bg-slate-100 rounded-full h-1.5 mb-1">
                            <div
                              className={`h-1.5 rounded-full ${
                                isWarehouse ? "bg-amber-400" : "bg-emerald-400"
                              }`}
                              style={{ width: `${pct}%` }}
                            />
                          </div>
                          <p className="text-xs text-slate-400">
                            %{pct}
                          </p>
                        </div>

                        {/* Miktar */}
                        <div className="text-right shrink-0">
                          <p className="text-lg font-bold text-slate-800">
                            {loc.quantity}
                          </p>
                          <p className="text-xs text-slate-400">{data.unit}</p>
                        </div>
                      </div>
                    );
                  })}
                </div>
              )}
            </div>
          </>
        )}
      </main>
    </>
  );
}
