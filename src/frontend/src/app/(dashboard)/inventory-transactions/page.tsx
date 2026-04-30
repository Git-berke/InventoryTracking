"use client";

import { useEffect, useState, useCallback } from "react";
import {
  History,
  ChevronLeft,
  ChevronRight,
  ArrowRight,
  ArrowLeft,
  RefreshCw,
  Filter,
  X,
} from "lucide-react";
import Topbar from "@/components/Topbar";
import api from "@/lib/api";

/* ─── Tipler ───────────────────────────────────────────── */
interface Transaction {
  id: string;
  productId: string;
  productCode: string;
  productName: string;
  transactionType: string;
  quantity: number;
  performedAtUtc: string;
  sourceLocationName: string | null;
  destinationLocationName: string | null;
  taskName: string | null;
  referenceNote: string | null;
}

interface PagedResult {
  items: Transaction[];
  totalCount: number;
  page: number;
  pageSize: number;
}

interface Product {
  id: string;
  code: string;
  name: string;
}
interface TaskItem {
  id: string;
  name: string;
}
interface PagedList<T> {
  items: T[];
}

/* ─── İşlem tipi konfigürasyonu ───────────────────────── */
type TxCfg = { label: string; cls: string; icon: React.ReactNode };

const TX_CONFIG: Record<string, TxCfg> = {
  InitialLoad: {
    label: "İlk Yükleme",
    cls: "bg-slate-100 text-slate-600",
    icon: null,
  },
  WarehouseToVehicle: {
    label: "Depo → Araç",
    cls: "bg-blue-50 text-blue-700",
    icon: <ArrowRight className="w-3 h-3" />,
  },
  VehicleToWarehouse: {
    label: "Araç → Depo",
    cls: "bg-violet-50 text-violet-700",
    icon: <ArrowLeft className="w-3 h-3" />,
  },
  WarehouseToWarehouse: {
    label: "Depo → Depo",
    cls: "bg-indigo-50 text-indigo-700",
    icon: <RefreshCw className="w-3 h-3" />,
  },
  VehicleToVehicle: {
    label: "Araç → Araç",
    cls: "bg-cyan-50 text-cyan-700",
    icon: <RefreshCw className="w-3 h-3" />,
  },
  AdjustmentIn: {
    label: "Düzeltme (+)",
    cls: "bg-emerald-50 text-emerald-700",
    icon: null,
  },
  AdjustmentOut: {
    label: "Düzeltme (-)",
    cls: "bg-red-50 text-red-600",
    icon: null,
  },
};

function TxBadge({ type }: { type: string }) {
  const cfg = TX_CONFIG[type] ?? {
    label: type,
    cls: "bg-slate-100 text-slate-600",
    icon: null,
  };
  return (
    <span
      className={`inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs font-medium ${cfg.cls}`}
    >
      {cfg.icon}
      {cfg.label}
    </span>
  );
}

function formatDateTime(d: string) {
  return new Date(d).toLocaleString("tr-TR", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

/* ─── Ana Sayfa ─────────────────────────────────────────── */
export default function InventoryTransactionsPage() {
  const [data, setData] = useState<PagedResult | null>(null);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);

  /* Filtreler */
  const [products, setProducts] = useState<Product[]>([]);
  const [tasks, setTasks] = useState<TaskItem[]>([]);
  const [filterProductId, setFilterProductId] = useState("");
  const [filterTaskId, setFilterTaskId] = useState("");
  const [showFilters, setShowFilters] = useState(false);

  const PAGE_SIZE = 25;

  /* Dropdown verisi */
  useEffect(() => {
    Promise.all([
      api.get<{ data: PagedList<Product> }>("/products?page=1&pageSize=200"),
      api.get<{ data: PagedList<TaskItem> }>("/tasks?page=1&pageSize=200"),
    ]).then(([p, t]) => {
      setProducts(p.data.data.items);
      setTasks(t.data.data.items);
    });
  }, []);

  /* Veri yükleme */
  const loadData = useCallback(() => {
    setLoading(true);
    const params = new URLSearchParams({
      page: String(page),
      pageSize: String(PAGE_SIZE),
    });
    if (filterProductId) params.set("productId", filterProductId);
    if (filterTaskId) params.set("taskId", filterTaskId);

    api
      .get<{ data: PagedResult }>(`/inventory-transactions?${params}`)
      .then((res) => setData(res.data.data))
      .finally(() => setLoading(false));
  }, [page, filterProductId, filterTaskId]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  /* Filtre sıfırla */
  function clearFilters() {
    setFilterProductId("");
    setFilterTaskId("");
    setPage(1);
  }

  const hasFilters = !!filterProductId || !!filterTaskId;
  const totalPages = data ? Math.ceil(data.totalCount / PAGE_SIZE) : 1;

  return (
    <>
      <Topbar title="Hareket Geçmişi" />
      <main className="flex-1 p-6">
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">

          {/* Toolbar */}
          <div className="flex items-center justify-between px-6 py-4 border-b border-slate-100">
            <div className="flex items-center gap-2">
              <History className="w-4 h-4 text-slate-400" />
              <span className="font-semibold text-slate-800 text-sm">
                {data ? `${data.totalCount} hareket` : "Yükleniyor..."}
              </span>
              {hasFilters && (
                <span className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs font-medium bg-blue-50 text-blue-700">
                  Filtre aktif
                </span>
              )}
            </div>
            <div className="flex items-center gap-2">
              {hasFilters && (
                <button
                  onClick={clearFilters}
                  className="flex items-center gap-1 text-xs text-slate-500 hover:text-slate-800 transition-colors cursor-pointer"
                >
                  <X className="w-3.5 h-3.5" />
                  Temizle
                </button>
              )}
              <button
                onClick={() => setShowFilters((s) => !s)}
                className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-medium border transition-colors cursor-pointer ${
                  showFilters
                    ? "bg-slate-800 text-white border-slate-800"
                    : "bg-white text-slate-600 border-slate-200 hover:bg-slate-50"
                }`}
              >
                <Filter className="w-3.5 h-3.5" />
                Filtrele
              </button>
            </div>
          </div>

          {/* Filtre paneli */}
          {showFilters && (
            <div className="px-6 py-4 bg-slate-50 border-b border-slate-100 flex flex-wrap gap-4">
              <div className="flex-1 min-w-48">
                <label className="block text-xs font-medium text-slate-500 mb-1">
                  Ürüne Göre
                </label>
                <select
                  value={filterProductId}
                  onChange={(e) => {
                    setFilterProductId(e.target.value);
                    setPage(1);
                  }}
                  className="w-full border border-slate-200 rounded-lg px-3 py-2 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  <option value="">Tüm ürünler</option>
                  {products.map((p) => (
                    <option key={p.id} value={p.id}>
                      {p.name} ({p.code})
                    </option>
                  ))}
                </select>
              </div>
              <div className="flex-1 min-w-48">
                <label className="block text-xs font-medium text-slate-500 mb-1">
                  Göreve Göre
                </label>
                <select
                  value={filterTaskId}
                  onChange={(e) => {
                    setFilterTaskId(e.target.value);
                    setPage(1);
                  }}
                  className="w-full border border-slate-200 rounded-lg px-3 py-2 text-sm bg-white focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  <option value="">Tüm görevler</option>
                  {tasks.map((t) => (
                    <option key={t.id} value={t.id}>
                      {t.name}
                    </option>
                  ))}
                </select>
              </div>
            </div>
          )}

          {/* Tablo */}
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="text-xs text-slate-400 font-medium bg-slate-50 border-b border-slate-100">
                  <th className="text-left px-6 py-3">Tarih / Saat</th>
                  <th className="text-left px-4 py-3">Ürün</th>
                  <th className="text-left px-4 py-3">İşlem Tipi</th>
                  <th className="text-right px-4 py-3">Miktar</th>
                  <th className="text-left px-4 py-3">Kaynak</th>
                  <th className="text-left px-4 py-3">Hedef</th>
                  <th className="text-left px-6 py-3">Görev / Not</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-50">
                {loading
                  ? Array.from({ length: 8 }).map((_, i) => (
                      <tr key={i}>
                        {Array.from({ length: 7 }).map((__, j) => (
                          <td key={j} className="px-6 py-4">
                            <div className="h-4 bg-slate-100 rounded animate-pulse" />
                          </td>
                        ))}
                      </tr>
                    ))
                  : data?.items.length === 0 ? (
                    <tr>
                      <td
                        colSpan={7}
                        className="px-6 py-12 text-center text-slate-400 text-sm"
                      >
                        Bu kriterlere uygun hareket kaydı bulunamadı.
                      </td>
                    </tr>
                  ) : (
                    data?.items.map((tx) => (
                      <tr
                        key={tx.id}
                        className="hover:bg-slate-50/70 transition-colors"
                      >
                        {/* Tarih */}
                        <td className="px-6 py-3.5 text-xs text-slate-500 whitespace-nowrap">
                          {formatDateTime(tx.performedAtUtc)}
                        </td>

                        {/* Ürün */}
                        <td className="px-4 py-3.5">
                          <div className="font-medium text-slate-800 text-xs leading-tight">
                            {tx.productName}
                          </div>
                          <div className="text-slate-400 text-xs font-mono">
                            {tx.productCode}
                          </div>
                        </td>

                        {/* Tip */}
                        <td className="px-4 py-3.5">
                          <TxBadge type={tx.transactionType} />
                        </td>

                        {/* Miktar */}
                        <td className="px-4 py-3.5 text-right">
                          <span
                            className={`font-semibold text-sm ${
                              tx.transactionType === "AdjustmentOut"
                                ? "text-red-600"
                                : "text-slate-800"
                            }`}
                          >
                            {tx.transactionType === "AdjustmentOut" ? "−" : "+"}
                            {tx.quantity}
                          </span>
                        </td>

                        {/* Kaynak */}
                        <td className="px-4 py-3.5 text-xs text-slate-600 max-w-32 truncate">
                          {tx.sourceLocationName ?? (
                            <span className="text-slate-300">—</span>
                          )}
                        </td>

                        {/* Hedef */}
                        <td className="px-4 py-3.5 text-xs text-slate-600 max-w-32 truncate">
                          {tx.destinationLocationName ?? (
                            <span className="text-slate-300">—</span>
                          )}
                        </td>

                        {/* Görev / Not */}
                        <td className="px-6 py-3.5 text-xs text-slate-500 max-w-40">
                          {tx.taskName && (
                            <div className="font-medium text-slate-700 truncate">
                              {tx.taskName}
                            </div>
                          )}
                          {tx.referenceNote && (
                            <div className="text-slate-400 truncate">
                              {tx.referenceNote}
                            </div>
                          )}
                          {!tx.taskName && !tx.referenceNote && (
                            <span className="text-slate-300">—</span>
                          )}
                        </td>
                      </tr>
                    ))
                  )}
              </tbody>
            </table>
          </div>

          {/* Sayfalama */}
          <div className="flex items-center justify-between px-6 py-3.5 border-t border-slate-100 text-xs text-slate-500">
            <span>
              {data
                ? `${(page - 1) * PAGE_SIZE + 1}–${Math.min(page * PAGE_SIZE, data.totalCount)} / ${data.totalCount} kayıt`
                : "—"}
            </span>
            <div className="flex items-center gap-1">
              <button
                onClick={() => setPage((p) => Math.max(1, p - 1))}
                disabled={page === 1}
                className="p-1.5 rounded hover:bg-slate-100 disabled:opacity-40 disabled:cursor-not-allowed cursor-pointer"
              >
                <ChevronLeft className="w-4 h-4" />
              </button>
              <span className="px-2">
                {page} / {totalPages}
              </span>
              <button
                onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                disabled={page === totalPages}
                className="p-1.5 rounded hover:bg-slate-100 disabled:opacity-40 disabled:cursor-not-allowed cursor-pointer"
              >
                <ChevronRight className="w-4 h-4" />
              </button>
            </div>
          </div>
        </div>
      </main>
    </>
  );
}
