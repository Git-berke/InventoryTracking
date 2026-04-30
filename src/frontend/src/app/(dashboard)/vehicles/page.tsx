"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import {
  Truck,
  ChevronLeft,
  ChevronRight,
  CheckCircle2,
  XCircle,
  ClipboardList,
  Box,
} from "lucide-react";
import Topbar from "@/components/Topbar";
import api from "@/lib/api";

interface Vehicle {
  id: string;
  code: string;
  licensePlate: string;
  vehicleType: string;
  isActive: boolean;
  activeTaskName: string | null;
}

interface PagedResult {
  items: Vehicle[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export default function VehiclesPage() {
  const [data, setData] = useState<PagedResult | null>(null);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const PAGE_SIZE = 20;

  useEffect(() => {
    setLoading(true);
    api
      .get<{ data: PagedResult }>(`/vehicles?page=${page}&pageSize=${PAGE_SIZE}`)
      .then((res) => setData(res.data.data))
      .finally(() => setLoading(false));
  }, [page]);

  const totalPages = data ? Math.ceil(data.totalCount / PAGE_SIZE) : 1;

  return (
    <>
      <Topbar title="Araçlar" />
      <main className="flex-1 p-6">
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
          {/* Başlık */}
          <div className="flex items-center gap-2 px-6 py-4 border-b border-slate-100">
            <Truck className="w-4 h-4 text-slate-400" />
            <span className="font-semibold text-slate-800 text-sm">
              {data ? `${data.totalCount} araç` : "Yükleniyor..."}
            </span>
          </div>

          {/* Tablo */}
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="text-xs text-slate-400 font-medium bg-slate-50 border-b border-slate-100">
                  <th className="text-left px-6 py-3">Plaka</th>
                  <th className="text-left px-4 py-3">Kod</th>
                  <th className="text-left px-4 py-3">Tip</th>
                  <th className="text-left px-4 py-3">Aktif Görev</th>
                  <th className="text-center px-4 py-3">Durum</th>
                  <th className="text-center px-6 py-3">Envanter</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-50">
                {loading
                  ? Array.from({ length: 5 }).map((_, i) => (
                      <tr key={i}>
                        {Array.from({ length: 6 }).map((__, j) => (
                          <td key={j} className="px-6 py-4">
                            <div className="h-4 bg-slate-100 rounded animate-pulse" />
                          </td>
                        ))}
                      </tr>
                    ))
                  : data?.items.map((v) => (
                      <tr
                        key={v.id}
                        className="hover:bg-slate-50/70 transition-colors"
                      >
                        {/* Plaka */}
                        <td className="px-6 py-3.5">
                          <div className="flex items-center gap-2">
                            <div className="w-8 h-8 bg-emerald-50 rounded-lg flex items-center justify-center shrink-0">
                              <Truck className="w-4 h-4 text-emerald-500" />
                            </div>
                            <span className="font-semibold text-slate-800 font-mono text-xs tracking-wide">
                              {v.licensePlate}
                            </span>
                          </div>
                        </td>

                        {/* Kod */}
                        <td className="px-4 py-3.5 text-slate-500 font-mono text-xs">
                          {v.code}
                        </td>

                        {/* Tip */}
                        <td className="px-4 py-3.5 text-slate-600 text-sm">
                          {v.vehicleType}
                        </td>

                        {/* Aktif Görev */}
                        <td className="px-4 py-3.5">
                          {v.activeTaskName ? (
                            <span className="inline-flex items-center gap-1 text-xs font-medium text-blue-700 bg-blue-50 px-2 py-0.5 rounded-full max-w-[180px] truncate">
                              <ClipboardList className="w-3 h-3 shrink-0" />
                              {v.activeTaskName}
                            </span>
                          ) : (
                            <span className="text-xs text-slate-400">—</span>
                          )}
                        </td>

                        {/* Durum */}
                        <td className="px-4 py-3.5 text-center">
                          {v.isActive ? (
                            <span className="inline-flex items-center gap-1 text-xs font-medium text-emerald-600 bg-emerald-50 px-2 py-0.5 rounded-full">
                              <CheckCircle2 className="w-3 h-3" />
                              Aktif
                            </span>
                          ) : (
                            <span className="inline-flex items-center gap-1 text-xs font-medium text-slate-500 bg-slate-100 px-2 py-0.5 rounded-full">
                              <XCircle className="w-3 h-3" />
                              Pasif
                            </span>
                          )}
                        </td>

                        {/* Envanter */}
                        <td className="px-6 py-3.5 text-center">
                          <Link
                            href={`/vehicles/${v.id}`}
                            className="inline-flex items-center gap-1 text-xs text-blue-600 hover:text-blue-800 font-medium transition-colors"
                          >
                            <Box className="w-3.5 h-3.5" />
                            Envanter
                          </Link>
                        </td>
                      </tr>
                    ))}
              </tbody>
            </table>
          </div>

          {/* Sayfalama */}
          {totalPages > 1 && (
            <div className="flex items-center justify-between px-6 py-3.5 border-t border-slate-100 text-xs text-slate-500">
              <span>
                Sayfa {page} / {totalPages} · {data?.totalCount} kayıt
              </span>
              <div className="flex items-center gap-1">
                <button
                  onClick={() => setPage((p) => Math.max(1, p - 1))}
                  disabled={page === 1}
                  className="p-1.5 rounded hover:bg-slate-100 disabled:opacity-40 disabled:cursor-not-allowed cursor-pointer"
                >
                  <ChevronLeft className="w-4 h-4" />
                </button>
                <button
                  onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                  disabled={page === totalPages}
                  className="p-1.5 rounded hover:bg-slate-100 disabled:opacity-40 disabled:cursor-not-allowed cursor-pointer"
                >
                  <ChevronRight className="w-4 h-4" />
                </button>
              </div>
            </div>
          )}
        </div>
      </main>
    </>
  );
}
