"use client";

import { useEffect, useState, useCallback } from "react";
import Link from "next/link";
import {
  Package,
  ChevronLeft,
  ChevronRight,
  TrendingUp,
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

interface Product {
  id: string;
  code: string;
  name: string;
  unit: string;
  isActive: boolean;
}
interface PagedResult {
  items: Product[];
  totalCount: number;
  page: number;
  pageSize: number;
}
interface ProductForm {
  code: string;
  name: string;
  description: string;
  unit: string;
  isActive: boolean;
}

const EMPTY_FORM: ProductForm = {
  code: "",
  name: "",
  description: "",
  unit: "",
  isActive: true,
};

/* ── Modal ─────────────────────────────────────────────── */
function ProductModal({
  mode,
  initial,
  onClose,
  onSaved,
}: {
  mode: "create" | "edit";
  initial: (ProductForm & { id: string }) | null;
  onClose: () => void;
  onSaved: () => void;
}) {
  const [form, setForm] = useState<ProductForm>(
    initial
      ? { code: initial.code, name: initial.name, description: initial.description, unit: initial.unit, isActive: initial.isActive }
      : EMPTY_FORM
  );
  const [saving, setSaving] = useState(false);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!form.code.trim() || !form.name.trim() || !form.unit.trim()) {
      toast.error("Zorunlu alanları doldurun.");
      return;
    }
    setSaving(true);
    try {
      if (mode === "create") {
        await api.post("/products", { code: form.code, name: form.name, description: form.description || null, unit: form.unit });
        toast.success("Ürün eklendi.");
      } else {
        await api.put(`/products/${initial!.id}`, { ...form, description: form.description || null });
        toast.success("Ürün güncellendi.");
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
            {mode === "create" ? "Yeni Ürün" : "Ürünü Düzenle"}
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
                placeholder="PRD-001"
                maxLength={50}
                className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label className="block text-xs font-medium text-slate-600 mb-1">Birim *</label>
              <input
                value={form.unit}
                onChange={(e) => setForm((s) => ({ ...s, unit: e.target.value }))}
                placeholder="adet, kg, litre..."
                maxLength={30}
                className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>
          <div>
            <label className="block text-xs font-medium text-slate-600 mb-1">Ürün Adı *</label>
            <input
              value={form.name}
              onChange={(e) => setForm((s) => ({ ...s, name: e.target.value }))}
              placeholder="Ürün adını girin..."
              maxLength={150}
              className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div>
            <label className="block text-xs font-medium text-slate-600 mb-1">Açıklama</label>
            <textarea
              value={form.description}
              onChange={(e) => setForm((s) => ({ ...s, description: e.target.value }))}
              placeholder="Opsiyonel..."
              maxLength={500}
              rows={2}
              className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
            />
          </div>
          {mode === "edit" && (
            <div className="flex items-center gap-2">
              <input
                type="checkbox"
                id="pIsActive"
                checked={form.isActive}
                onChange={(e) => setForm((s) => ({ ...s, isActive: e.target.checked }))}
                className="w-4 h-4 accent-blue-600 cursor-pointer"
              />
              <label htmlFor="pIsActive" className="text-sm text-slate-700 cursor-pointer">Aktif</label>
            </div>
          )}
          <div className="flex gap-3 pt-2">
            <button
              type="button"
              onClick={onClose}
              className="flex-1 py-2.5 rounded-lg border border-slate-200 text-sm font-medium text-slate-600 hover:bg-slate-50 cursor-pointer"
            >
              İptal
            </button>
            <button
              type="submit"
              disabled={saving}
              className="flex-1 py-2.5 rounded-lg bg-blue-600 hover:bg-blue-700 disabled:bg-blue-300 text-white text-sm font-semibold flex items-center justify-center gap-2 cursor-pointer"
            >
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
export default function ProductsPage() {
  const { user } = useAuth();
  const canWrite = user?.permissions?.includes("products.create");
  const canDelete = user?.permissions?.includes("products.delete");

  const [data, setData] = useState<PagedResult | null>(null);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const [modal, setModal] = useState<{ mode: "create" | "edit"; product: (ProductForm & { id: string }) | null } | null>(null);
  const [deleting, setDeleting] = useState<string | null>(null);
  const PAGE_SIZE = 20;

  const loadData = useCallback(() => {
    setLoading(true);
    api
      .get<{ data: PagedResult }>(`/products?page=${page}&pageSize=${PAGE_SIZE}`)
      .then((res) => setData(res.data.data))
      .finally(() => setLoading(false));
  }, [page]);

  useEffect(() => { loadData(); }, [loadData]);

  async function handleDelete(p: Product) {
    if (!confirm(`"${p.name}" ürününü silmek istediğinize emin misiniz?`)) return;
    setDeleting(p.id);
    try {
      await api.delete(`/products/${p.id}`);
      toast.success("Ürün silindi.");
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
        <ProductModal
          mode={modal.mode}
          initial={modal.product}
          onClose={() => setModal(null)}
          onSaved={() => { setModal(null); loadData(); }}
        />
      )}
      <Topbar title="Ürünler" />
      <main className="flex-1 p-6">
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
          {/* Başlık */}
          <div className="flex items-center justify-between px-6 py-4 border-b border-slate-100">
            <div className="flex items-center gap-2">
              <Package className="w-4 h-4 text-slate-400" />
              <span className="font-semibold text-slate-800 text-sm">
                {data ? `${data.totalCount} ürün` : "Yükleniyor..."}
              </span>
            </div>
            {canWrite && (
              <button
                onClick={() => setModal({ mode: "create", product: null })}
                className="flex items-center gap-1.5 px-3 py-1.5 bg-blue-600 hover:bg-blue-700 text-white text-xs font-semibold rounded-lg transition-colors cursor-pointer"
              >
                <Plus className="w-3.5 h-3.5" />
                Yeni Ürün
              </button>
            )}
          </div>

          {/* Tablo */}
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="text-xs text-slate-400 font-medium bg-slate-50 border-b border-slate-100">
                  <th className="text-left px-6 py-3">Ürün Adı</th>
                  <th className="text-left px-4 py-3">Kod</th>
                  <th className="text-left px-4 py-3">Birim</th>
                  <th className="text-center px-4 py-3">Durum</th>
                  <th className="text-center px-6 py-3">İşlemler</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-50">
                {loading
                  ? Array.from({ length: 6 }).map((_, i) => (
                      <tr key={i}>
                        {Array.from({ length: 5 }).map((__, j) => (
                          <td key={j} className="px-6 py-4">
                            <div className="h-4 bg-slate-100 rounded animate-pulse" />
                          </td>
                        ))}
                      </tr>
                    ))
                  : data?.items.map((p) => (
                      <tr key={p.id} className="hover:bg-slate-50/70 transition-colors">
                        <td className="px-6 py-3.5 font-medium text-slate-800">{p.name}</td>
                        <td className="px-4 py-3.5 text-slate-500 font-mono text-xs">{p.code}</td>
                        <td className="px-4 py-3.5 text-slate-500">{p.unit}</td>
                        <td className="px-4 py-3.5 text-center">
                          {p.isActive ? (
                            <span className="inline-flex items-center gap-1 text-xs font-medium text-emerald-600 bg-emerald-50 px-2 py-0.5 rounded-full">
                              <CheckCircle2 className="w-3 h-3" /> Aktif
                            </span>
                          ) : (
                            <span className="inline-flex items-center gap-1 text-xs font-medium text-slate-500 bg-slate-100 px-2 py-0.5 rounded-full">
                              <XCircle className="w-3 h-3" /> Pasif
                            </span>
                          )}
                        </td>
                        <td className="px-6 py-3.5">
                          <div className="flex items-center justify-center gap-3">
                            <Link
                              href={`/products/${p.id}`}
                              className="inline-flex items-center gap-1 text-xs text-blue-600 hover:text-blue-800 font-medium transition-colors"
                            >
                              <TrendingUp className="w-3.5 h-3.5" />
                              Stok
                            </Link>
                            {canWrite && (
                              <button
                                onClick={() =>
                                  setModal({
                                    mode: "edit",
                                    product: { id: p.id, code: p.code, name: p.name, description: "", unit: p.unit, isActive: p.isActive },
                                  })
                                }
                                className="text-slate-400 hover:text-slate-700 transition-colors cursor-pointer"
                                title="Düzenle"
                              >
                                <Pencil className="w-3.5 h-3.5" />
                              </button>
                            )}
                            {canDelete && (
                              <button
                                onClick={() => handleDelete(p)}
                                disabled={deleting === p.id}
                                className="text-slate-400 hover:text-red-500 transition-colors cursor-pointer disabled:opacity-40"
                                title="Sil"
                              >
                                {deleting === p.id ? (
                                  <Loader2 className="w-3.5 h-3.5 animate-spin" />
                                ) : (
                                  <Trash2 className="w-3.5 h-3.5" />
                                )}
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
                <button onClick={() => setPage((p) => Math.max(1, p - 1))} disabled={page === 1} className="p-1.5 rounded hover:bg-slate-100 disabled:opacity-40 cursor-pointer">
                  <ChevronLeft className="w-4 h-4" />
                </button>
                <button onClick={() => setPage((p) => Math.min(totalPages, p + 1))} disabled={page === totalPages} className="p-1.5 rounded hover:bg-slate-100 disabled:opacity-40 cursor-pointer">
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
