"use client";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import Link from "next/link";
import {
  ArrowLeft,
  Truck,
  ClipboardList,
  Package,
  Box,
  AlertCircle,
} from "lucide-react";
import Topbar from "@/components/Topbar";
import api from "@/lib/api";

interface InventoryItem {
  productId: string;
  productCode: string;
  productName: string;
  unit: string;
  quantity: number;
}

interface VehicleInventory {
  vehicleId: string;
  vehicleCode: string;
  licensePlate: string;
  vehicleType: string;
  activeTaskName: string | null;
  inventories: InventoryItem[];
}

export default function VehicleDetailPage() {
  const { id } = useParams<{ id: string }>();
  const [data, setData] = useState<VehicleInventory | null>(null);
  const [loading, setLoading] = useState(true);
  const [notFound, setNotFound] = useState(false);

  useEffect(() => {
    api
      .get<{ data: VehicleInventory }>(`/vehicles/${id}/inventories`)
      .then((res) => setData(res.data.data))
      .catch((err) => {
        if (err.response?.status === 404) setNotFound(true);
      })
      .finally(() => setLoading(false));
  }, [id]);

  const totalItems = data?.inventories.reduce((s, i) => s + i.quantity, 0) ?? 0;

  return (
    <>
      <Topbar
        title={data ? `${data.licensePlate} — Envanter` : "Araç Envanteri"}
      />
      <main className="flex-1 p-6 space-y-5">
        <Link
          href="/vehicles"
          className="inline-flex items-center gap-1.5 text-sm text-slate-500 hover:text-slate-800 transition-colors"
        >
          <ArrowLeft className="w-4 h-4" />
          Araçlar listesine dön
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
            <p className="font-medium text-slate-600">Araç bulunamadı.</p>
          </div>
        )}

        {data && (
          <>
            {/* Araç bilgi kartı */}
            <div className="bg-white rounded-xl border border-slate-200 shadow-sm p-5">
              <div className="flex items-start gap-4">
                <div className="w-12 h-12 bg-emerald-50 rounded-xl flex items-center justify-center shrink-0">
                  <Truck className="w-6 h-6 text-emerald-500" />
                </div>
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2 flex-wrap">
                    <h2 className="text-xl font-bold text-slate-800 font-mono tracking-wide">
                      {data.licensePlate}
                    </h2>
                    <span className="text-xs text-slate-400 font-mono bg-slate-100 px-2 py-0.5 rounded">
                      {data.vehicleCode}
                    </span>
                    <span className="text-xs text-slate-500 bg-slate-100 px-2 py-0.5 rounded">
                      {data.vehicleType}
                    </span>
                  </div>

                  <div className="mt-2">
                    {data.activeTaskName ? (
                      <div className="inline-flex items-center gap-1.5 text-sm font-medium text-blue-700 bg-blue-50 px-3 py-1 rounded-full">
                        <ClipboardList className="w-3.5 h-3.5 shrink-0" />
                        <span>Aktif Görev: {data.activeTaskName}</span>
                      </div>
                    ) : (
                      <span className="text-sm text-slate-400">
                        Şu an aktif görev yok
                      </span>
                    )}
                  </div>
                </div>

                {/* Toplam envanter */}
                <div className="text-right shrink-0">
                  <p className="text-2xl font-bold text-slate-800">{totalItems}</p>
                  <p className="text-xs text-slate-400">toplam ürün</p>
                </div>
              </div>
            </div>

            {/* Envanter tablosu */}
            <div className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
              <div className="flex items-center gap-2 px-6 py-4 border-b border-slate-100">
                <Box className="w-4 h-4 text-slate-400" />
                <span className="font-semibold text-slate-800 text-sm">
                  Araçtaki Envanter
                </span>
                <span className="text-xs text-slate-400">
                  ({data.inventories.length} ürün çeşidi)
                </span>
              </div>

              {data.inventories.length === 0 ? (
                <div className="flex flex-col items-center py-12 text-slate-400">
                  <Package className="w-8 h-8 mb-2" />
                  <p className="text-sm">Bu araçta envanter yok.</p>
                </div>
              ) : (
                <div className="overflow-x-auto">
                  <table className="w-full text-sm">
                    <thead>
                      <tr className="text-xs text-slate-400 font-medium bg-slate-50 border-b border-slate-100">
                        <th className="text-left px-6 py-3">Ürün</th>
                        <th className="text-left px-4 py-3">Kod</th>
                        <th className="text-left px-4 py-3">Birim</th>
                        <th className="text-right px-6 py-3">Miktar</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-50">
                      {data.inventories.map((item) => (
                        <tr
                          key={item.productId}
                          className="hover:bg-slate-50/70 transition-colors"
                        >
                          <td className="px-6 py-3.5">
                            <div className="flex items-center gap-2">
                              <div className="w-7 h-7 bg-blue-50 rounded-lg flex items-center justify-center shrink-0">
                                <Package className="w-3.5 h-3.5 text-blue-500" />
                              </div>
                              <span className="font-medium text-slate-800">
                                {item.productName}
                              </span>
                            </div>
                          </td>
                          <td className="px-4 py-3.5 font-mono text-xs text-slate-500">
                            {item.productCode}
                          </td>
                          <td className="px-4 py-3.5 text-slate-500">
                            {item.unit}
                          </td>
                          <td className="px-6 py-3.5 text-right">
                            <span className="text-lg font-bold text-slate-800">
                              {item.quantity}
                            </span>
                            <span className="text-xs text-slate-400 ml-1">
                              {item.unit}
                            </span>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                    <tfoot>
                      <tr className="bg-slate-50 border-t border-slate-100">
                        <td
                          colSpan={3}
                          className="px-6 py-3 text-xs font-semibold text-slate-500"
                        >
                          Toplam
                        </td>
                        <td className="px-6 py-3 text-right text-base font-bold text-slate-800">
                          {totalItems}
                        </td>
                      </tr>
                    </tfoot>
                  </table>
                </div>
              )}
            </div>
          </>
        )}
      </main>
    </>
  );
}
