"use client";

import { useEffect, useState, useCallback } from "react";
import Link from "next/link";
import {
  Warehouse,
  ChevronLeft,
  ChevronRight,
  MapPin,
  CheckCircle2,
  XCircle,
  Plus,
  Pencil,
  Trash2,
  X,
  Loader2,
} from "lucide-react";
import toast from "react-hot-toast";
import Topbar from "@/components/Topbar";
import api from "@/lib/api";
import { useAuth } from "@/context/AuthContext";

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
}
interface WarehouseForm {
  code: string;
  name: string;
  region: string;
  address: string;
  isActive: boolean;
}

const EMPTY_FORM: WarehouseForm = {
  code: "",
  name: "",
  region: "",
  address: "",
  isActive: true,
};

/* ── Modal ─────────────────────────────────────────────── */
function WarehouseModal({
  mode,
  initial,
  onClose,
  onSaved,
}: {
  mode: "create" | "edit";
  initial: (WarehouseForm & { id: string }) | null;
  onClose: () => void;
  onSaved: () => void;
}) {
  const [form, setForm] = useState<WarehouseForm>(
    initial
      ? { code: initial.code, name: initial.name, region: initial.region, address: initial.address, isActive: initial.isActive }
      : EMPTY_FORM
  );
  const [saving, setSaving] = useState(false);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!form.code.trim() || !form.name.trim() || !form.region.trim()) {
      toast.error("Zorunlu alanları doldurun.");
      return;
    }
    setSaving(true);
    try {
      const body = { code: form.code, name: form.name, region: form.region, address: form.address || null };
      if (mode === "create") {
        await api.post("/warehouses", body);
        toast.success("Depo eklendi.");
      } else {
        await api.put(`/warehouses/${initial!.id}`, { ...body, isActive: form.isActive });
        toast.success("Depo güncellendi.");
      }
      onSaved();
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message ?? "İşlem başarısız.";
      toast.error(msg);
    } finally {
      setSaving(false);
    }
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm">
      <div className="bg-white rounded-2xl shadow-xl w-full max-w-md mx-4">
        <div className="flex items-center justify-between px-6 py-4 border-b border-slate-100">
          <h2 className="font-semibold text-slate-800">
            {mode === "create" ? "Yeni Depo" : "Depoyu Düzenle"}
          </h2>
          <button onClick={onClose} className="text-slate-400 hover:text-slate-600 cursor-pointer">
            <X className="w-5 h-5" />
          </button>
        </div>
        <form onSubmit={handleSubmit} className="p-6 space-y-4">
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-xs font-medium text-slate-600 mb-1">Kod *</label>
              <input
                value={form.code}
                onChange={(e) => setForm((s) => ({ ...s, code: e.target.value }))}
                placeholder="WH-001"
                maxLength={50}
                className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label className="block text-xs font-medium text-slate-600 mb-1">Bölge *</label>
              <input
                value={form.region}
                onChange={(e) => setForm((s) => ({ ...s, region: e.target.value }))}
                placeholder="İstanbul, Ankara..."
                maxLength={100}
                className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>
          <div>
            <label className="block text-xs font-medium text-slate-600 mb-1">Depo Adı *</label>
            <input
              value={form.name}
              onChange={(e) => setForm((s) => ({ ...s, name: e.target.value }))}
              placeholder="Depo adını girin..."
              maxLength={150}
              className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div>
            <label className="block text-xs font-medium text-slate-600 mb-1">Adres</label>
            <textarea
              value={form.address}
              onChange={(e) => setForm((s) => ({ ...s, address: e.target.value }))}
              placeholder="Opsiyonel..."
              maxLength={300}
              rows={2}
              className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
            />
          </div>
          {mode === "edit" && (
            <div className="flex items-center gap-2">
              <input
                type="checkbox"
                id="wIsActive"
                checked={form.isActive}
                onChange={(e) => setForm((s) => ({ ...s, isActive: e.target.checked }))}
                className="w-4 h-4 accent-blue-600 cursor-pointer"
              />
              <label htmlFor="wIsActive" className="text-sm text-slate-700 cursor-pointer">Aktif</label>
            </div>
          )}
          <div className="flex gap-3 pt-2">
            <button type="button" onClick={onClose} className="flex-1 py-2.5 rounded-lg border border-slate-200 text-sm font-medium text-slate-600 hover:bg-slate-50 cursor-pointer">
              İptal
            </button>
            <button type="submit" disabled={saving} className="flex-1 py-2.5 rounded-lg bg-blue-600 hover:bg-blue-700 disabled:bg-blue-300 text-white text-sm font-semibold flex items-center justify-center gap-2 cursor-pointer">
              {saving && <Loader2 className="w-4 h-4 animate-spin" />}
              {mode === "create" ? "Ekle" : "Kaydet"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

/* ── Ana Sayfa ─────────────────────────────────────────── */
export default function WarehousesPage() {
  const { user } = useAuth();
  const canWrite = user?.permissions?.includes("warehouses.create");
  const canDelete = user?.permissions?.includes("warehouses.delete");

  const [data, setData] = useState<PagedResult | null>(null);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [modal, setModal] = useState<{ mode: "create" | "edit"; warehouse: (WarehouseForm & { id: string }) | null } | null>(null);
  const [deleting, setDeleting] = useState<string | null>(null);
  const PAGE_SIZE = 20;

  const loadData = useCallback(() => {
    setLoading(true);
    setError(null);
    api
      .get<{ data: PagedResult }>(`/warehouses?page=${page}&pageSize=${PAGE_SIZE}`)
      .then((res) => setData(res.data.data))
      .catch((err: unknown) => {
        const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message ?? "Depolar yüklenemedi.";
        setData(null);
        setError(msg);
        toast.error(msg);
      })
      .finally(() => setLoading(false));
  }, [page]);

  useEffect(() => { loadData(); }, [loadData]);

  async function handleDelete(w: WarehouseItem) {
    if (!confirm(`"${w.name}" deposunu silmek istediğinize emin misiniz?`)) return;
    setDeleting(w.id);
    try {
      await api.delete(`/warehouses/${w.id}`);
      toast.success("Depo silindi.");
      loadData();
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message ?? "Silme işlemi başarısız.";
      toast.error(msg);
    } finally {
      setDeleting(null);
    }
  }

  const totalPages = data ? Math.ceil(data.totalCount / PAGE_SIZE) : 1;

  return (
    <>
      {modal && (
        <WarehouseModal
          mode={modal.mode}
          initial={modal.warehouse}
          onClose={() => setModal(null)}
          onSaved={() => { setModal(null); loadData(); }}
        />
      )}
      <Topbar title="Depolar" />
      <main className="flex-1 p-6">
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
          <div className="flex items-center justify-between px-6 py-4 border-b border-slate-100">
            <div className="flex items-center gap-2">
              <Warehouse className="w-4 h-4 text-slate-400" />
              <span className="font-semibold text-slate-800 text-sm">
                {loading ? "Yükleniyor..." : data ? `${data.totalCount} depo` : "Depolar"}
              </span>
            </div>
            {canWrite && (
              <button
                onClick={() => setModal({ mode: "create", warehouse: null })}
                className="flex items-center gap-1.5 px-3 py-1.5 bg-blue-600 hover:bg-blue-700 text-white text-xs font-semibold rounded-lg transition-colors cursor-pointer"
              >
                <Plus className="w-3.5 h-3.5" /> Yeni Depo
              </button>
            )}
          </div>

          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="text-xs text-slate-400 font-medium bg-slate-50 border-b border-slate-100">
                  <th className="text-left px-6 py-3">Depo Adı</th>
                  <th className="text-left px-4 py-3">Kod</th>
                  <th className="text-left px-4 py-3">Bölge</th>
                  <th className="text-center px-4 py-3">Durum</th>
                  <th className="text-center px-6 py-3">İşlemler</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-50">
                {loading
                  ? Array.from({ length: 4 }).map((_, i) => (
                      <tr key={i}>{Array.from({ length: 5 }).map((__, j) => (
                        <td key={j} className="px-6 py-4"><div className="h-4 bg-slate-100 rounded animate-pulse" /></td>
                      ))}</tr>
                    ))
                  : error ? (
                      <tr>
                        <td colSpan={5} className="px-6 py-10 text-center">
                          <div className="flex flex-col items-center gap-3 text-sm text-slate-500">
                            <span>{error}</span>
                            <button
                              onClick={loadData}
                              className="px-3 py-1.5 rounded-lg bg-blue-600 text-white text-xs font-semibold hover:bg-blue-700 cursor-pointer"
                            >
                              Tekrar dene
                            </button>
                          </div>
                        </td>
                      </tr>
                    )
                  : data?.items.length === 0 ? (
                      <tr>
                        <td colSpan={5} className="px-6 py-10 text-center text-sm text-slate-500">
                          Depo bulunamadı.
                        </td>
                      </tr>
                    )
                  : data?.items.map((w) => (
                      <tr key={w.id} className="hover:bg-slate-50/70 transition-colors">
                        <td className="px-6 py-3.5 font-medium text-slate-800">{w.name}</td>
                        <td className="px-4 py-3.5">
                          <span className="font-mono text-xs bg-slate-100 text-slate-600 px-2 py-0.5 rounded">{w.code}</span>
                        </td>
                        <td className="px-4 py-3.5">
                          <span className="inline-flex items-center gap-1 text-xs text-slate-600">
                            <MapPin className="w-3 h-3 text-slate-400" />{w.region}
                          </span>
                        </td>
                        <td className="px-4 py-3.5 text-center">
                          {w.isActive ? (
                            <span className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs font-medium bg-emerald-50 text-emerald-700">
                              <CheckCircle2 className="w-3 h-3" /> Aktif
                            </span>
                          ) : (
                            <span className="inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs font-medium bg-slate-100 text-slate-500">
                              <XCircle className="w-3 h-3" /> Pasif
                            </span>
                          )}
                        </td>
                        <td className="px-6 py-3.5">
                          <div className="flex items-center justify-center gap-3">
                            <Link href={`/warehouses/${w.id}`} className="text-xs text-blue-600 hover:text-blue-800 font-medium transition-colors">
                              Detay
                            </Link>
                            {canWrite && (
                              <button
                                onClick={() => setModal({ mode: "edit", warehouse: { id: w.id, code: w.code, name: w.name, region: w.region, address: "", isActive: w.isActive } })}
                                className="text-slate-400 hover:text-slate-700 transition-colors cursor-pointer"
                                title="Düzenle"
                              >
                                <Pencil className="w-3.5 h-3.5" />
                              </button>
                            )}
                            {canDelete && (
                              <button
                                onClick={() => handleDelete(w)}
                                disabled={deleting === w.id}
                                className="text-slate-400 hover:text-red-500 transition-colors cursor-pointer disabled:opacity-40"
                                title="Sil"
                              >
                                {deleting === w.id ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : <Trash2 className="w-3.5 h-3.5" />}
                              </button>
                            )}
                          </div>
                        </td>
                      </tr>
                    ))}
              </tbody>
            </table>
          </div>

          {totalPages > 1 && (
            <div className="flex items-center justify-between px-6 py-3.5 border-t border-slate-100 text-xs text-slate-500">
              <span>Sayfa {page} / {totalPages} · {data?.totalCount} kayıt</span>
              <div className="flex items-center gap-1">
                <button onClick={() => setPage((p) => Math.max(1, p - 1))} disabled={page === 1} className="p-1.5 rounded hover:bg-slate-100 disabled:opacity-40 cursor-pointer"><ChevronLeft className="w-4 h-4" /></button>
                <button onClick={() => setPage((p) => Math.min(totalPages, p + 1))} disabled={page === totalPages} className="p-1.5 rounded hover:bg-slate-100 disabled:opacity-40 cursor-pointer"><ChevronRight className="w-4 h-4" /></button>
              </div>
            </div>
          )}
        </div>
      </main>
    </>
  );
}
