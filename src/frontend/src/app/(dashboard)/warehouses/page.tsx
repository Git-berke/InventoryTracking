"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import {
  Warehouse,
  ChevronLeft,
  ChevronRight,
  MapPin,
  CheckCircle2,
  XCircle,
} from "lucide-react";
import Topbar from "@/components/Topbar";
import api from "@/lib/api";

interface WarehouseItem {
  id: string;
  code: string;
  name: string;
  region: string;
  isActive: boolean;
}

interface PagedResult {
  items: WarehouseItem[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export default function WarehousesPage() {
  const [data, setData] = useState<PagedResult | null>(null);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const PAGE_SIZE = 20;

  useEffect(() => {
    setLoading(true);
    api
      .get<{ data: PagedResult }>(
        `/warehouses?page=${page}&pageSize=${PAGE_SIZE}`
      )
      .then((res) => setData(res.data.data))
      .finally(() => setLoading(false));
  }, [page]);

  const totalPages = data ? Math.ceil(data.totalCount / PAGE_SIZE) : 1;

  return (
    <>
      <Topbar title="Depolar" />
      <main className="flex-1 p-6">
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
          {/* Başlık */}
          <div className="flex items-center gap-2 px-6 py-4 border-b border-slate-100">
            <Warehouse className="w-4 h-4 text-slate-400" />
            <span className="font-semibold text-slate-800 text-sm">
              {data ? `${data.totalCount} depo` : "Yükleniyor..."}
            </span>
          </div>

          {/* Tablo */}
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="text-xs text-slate-400 font-medium bg-slate-50 border-b border-slate-100">
                  <th className="text-left px-6 py-3">Depo Adı</th>
                  <th className="text-left px-4 py-3">Kod</th>
                  <th className="text-left px-4 py-3">Bölge</th>
                  <th className="text-center px-4 py-3">Durum</th>
                  <th className="text-center px-6 py-3">Detay</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-50">
                {loading
                  ? Array.from({ length: 5 }).map((_, i) => (
                      <tr key={i}>
                        {Array.from({ length: 5 }).map((__, j) => (
                          <td key={j} className="px-6 py-4">
                            <div className="h-4 bg-slate-100 rounded animate-pulse" />
                          </td>
                        ))}
                      </tr>
                    ))
                  : data?.items.map((w) => (
                      <tr
                        key={w.id}
                        className="hover:bg-slate-50/70 transition-colors"
                      >
                        {/* Ad */}
                        <td className="px-6 py-3.5 font-medium text-slate-800">
                          {w.name}
                        </td>

                        {/* Kod */}
                        <td className="px-4 py-3.5">
                          <span className="font-mono text-xs bg-slate-100 text-slate-600 px-2 py-0.5 rounded">
                            {w.code}
                          </span>
                        </td>

                        {/* Bölge */}
                        <td className="px-4 py-3.5">
                          <span className="inline-flex items-center gap-1 text-xs text-slate-600">
                            <MapPin className="w-3 h-3 text-slate-400" />
                            {w.region}
                          </span>
                        </td>

                        {/* Durum */}
                        <td className="px-4 py-3.5 text-center">
                          {w.isActive ? (
                            <span className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs font-medium bg-emerald-50 text-emerald-700">
                              <CheckCircle2 className="w-3 h-3" />
                              Aktif
                            </span>
                          ) : (
                            <span className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs font-medium bg-slate-100 text-slate-500">
                              <XCircle className="w-3 h-3" />
                              Pasif
                            </span>
                          )}
                        </td>

                        {/* Detay */}
                        <td className="px-6 py-3.5 text-center">
                          <Link
                            href={`/warehouses/${w.id}`}
                            className="inline-flex items-center gap-1 text-xs text-blue-600 hover:text-blue-800 font-medium transition-colors"
                          >
                            <Warehouse className="w-3.5 h-3.5" />
                            Detay
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
