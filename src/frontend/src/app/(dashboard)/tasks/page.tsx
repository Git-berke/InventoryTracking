"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import {
  ClipboardList,
  ChevronLeft,
  ChevronRight,
  MapPin,
  Calendar,
} from "lucide-react";
import Topbar from "@/components/Topbar";
import api from "@/lib/api";

interface Task {
  id: string;
  name: string;
  region: string;
  status: string;
  startDate: string;
  endDate: string | null;
}

interface PagedResult {
  items: Task[];
  totalCount: number;
  page: number;
  pageSize: number;
}

const STATUS_CONFIG: Record<string, { label: string; cls: string }> = {
  Draft: {
    label: "Taslak",
    cls: "bg-slate-100 text-slate-600",
  },
  InProgress: {
    label: "Devam Ediyor",
    cls: "bg-blue-100 text-blue-700",
  },
  Completed: {
    label: "Tamamlandı",
    cls: "bg-emerald-100 text-emerald-700",
  },
};

function StatusBadge({ status }: { status: string }) {
  const cfg = STATUS_CONFIG[status] ?? {
    label: status,
    cls: "bg-slate-100 text-slate-600",
  };
  return (
    <span
      className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium ${cfg.cls}`}
    >
      {cfg.label}
    </span>
  );
}

function formatDate(d: string) {
  return new Date(d).toLocaleDateString("tr-TR", {
    day: "2-digit",
    month: "short",
    year: "numeric",
  });
}

export default function TasksPage() {
  const [data, setData] = useState<PagedResult | null>(null);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const PAGE_SIZE = 20;

  useEffect(() => {
    setLoading(true);
    api
      .get<{ data: PagedResult }>(`/tasks?page=${page}&pageSize=${PAGE_SIZE}`)
      .then((res) => setData(res.data.data))
      .finally(() => setLoading(false));
  }, [page]);

  const totalPages = data ? Math.ceil(data.totalCount / PAGE_SIZE) : 1;

  return (
    <>
      <Topbar title="Görevler" />
      <main className="flex-1 p-6">
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
          {/* Başlık */}
          <div className="flex items-center gap-2 px-6 py-4 border-b border-slate-100">
            <ClipboardList className="w-4 h-4 text-slate-400" />
            <span className="font-semibold text-slate-800 text-sm">
              {data ? `${data.totalCount} görev` : "Yükleniyor..."}
            </span>
          </div>

          {/* Tablo */}
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="text-xs text-slate-400 font-medium bg-slate-50 border-b border-slate-100">
                  <th className="text-left px-6 py-3">Görev Adı</th>
                  <th className="text-left px-4 py-3">Bölge</th>
                  <th className="text-left px-4 py-3">Durum</th>
                  <th className="text-left px-4 py-3">Başlangıç</th>
                  <th className="text-left px-4 py-3">Bitiş</th>
                  <th className="text-center px-6 py-3">Detay</th>
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
                  : data?.items.map((t) => (
                      <tr
                        key={t.id}
                        className="hover:bg-slate-50/70 transition-colors"
                      >
                        {/* Ad */}
                        <td className="px-6 py-3.5">
                          <div className="flex items-center gap-2">
                            <div
                              className={`w-2 h-2 rounded-full shrink-0 ${
                                t.status === "InProgress"
                                  ? "bg-blue-500"
                                  : t.status === "Completed"
                                  ? "bg-emerald-500"
                                  : "bg-slate-300"
                              }`}
                            />
                            <span className="font-medium text-slate-800">
                              {t.name}
                            </span>
                          </div>
                        </td>

                        {/* Bölge */}
                        <td className="px-4 py-3.5">
                          <span className="inline-flex items-center gap-1 text-slate-600 text-xs">
                            <MapPin className="w-3 h-3 text-slate-400" />
                            {t.region}
                          </span>
                        </td>

                        {/* Durum */}
                        <td className="px-4 py-3.5">
                          <StatusBadge status={t.status} />
                        </td>

                        {/* Başlangıç */}
                        <td className="px-4 py-3.5">
                          <span className="inline-flex items-center gap-1 text-xs text-slate-500">
                            <Calendar className="w-3 h-3" />
                            {formatDate(t.startDate)}
                          </span>
                        </td>

                        {/* Bitiş */}
                        <td className="px-4 py-3.5 text-xs text-slate-500">
                          {t.endDate ? formatDate(t.endDate) : (
                            <span className="text-slate-300">—</span>
                          )}
                        </td>

                        {/* Detay */}
                        <td className="px-6 py-3.5 text-center">
                          <Link
                            href={`/tasks/${t.id}`}
                            className="inline-flex items-center gap-1 text-xs text-blue-600 hover:text-blue-800 font-medium transition-colors"
                          >
                            <ClipboardList className="w-3.5 h-3.5" />
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
