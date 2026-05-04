"use client";

import { useEffect, useState, useCallback } from "react";
import Link from "next/link";
import {
  Truck,
  ChevronLeft,
  ChevronRight,
  Plus,
  Pencil,
  Trash2,
  X,
  CheckCircle2,
  XCircle,
  Loader2,
} from "lucide-react";
import toast from "react-hot-toast";
import Topbar from "@/components/Topbar";
import api from "@/lib/api";
import { useAuth } from "@/context/AuthContext";

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
}

interface VehicleForm {
  code: string;
  licensePlate: string;
  vehicleType: string;
  isActive: boolean;
}

const EMPTY_FORM: VehicleForm = {
  code: "",
  licensePlate: "",
  vehicleType: "",
  isActive: true,
};

/* ── Modal ─────────────────────────────────────────────── */
function VehicleModal({
  mode,
  initial,
  onClose,
  onSaved,
}: {
  mode: "create" | "edit";
  initial: (VehicleForm & { id: string }) | null;
  onClose: () => void;
  onSaved: () => void;
}) {
  const [form, setForm] = useState<VehicleForm>(
    initial
      ? { code: initial.code, licensePlate: initial.licensePlate, vehicleType: initial.vehicleType, isActive: initial.isActive }
      : EMPTY_FORM
  );
  const [saving, setSaving] = useState(false);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!form.code.trim() || !form.licensePlate.trim() || !form.vehicleType.trim()) {
      toast.error("Tüm zorunlu alanları doldurun.");
      return;
    }
    setSaving(true);
    try {
      if (mode === "create") {
        await api.post("/vehicles", { code: form.code, licensePlate: form.licensePlate, vehicleType: form.vehicleType });
        toast.success("Araç eklendi.");
      } else {
        await api.put(`/vehicles/${initial!.id}`, form);
        toast.success("Araç güncellendi.");
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
            {mode === "create" ? "Yeni Araç" : "Aracı Düzenle"}
          </h2>
          <button onClick={onClose} className="text-slate-400 hover:text-slate-600 cursor-pointer">
            <X className="w-5 h-5" />
          </button>
        </div>
        <form onSubmit={handleSubmit} className="p-6 space-y-4">
          <div>
            <label className="block text-xs font-medium text-slate-600 mb-1">Araç Kodu *</label>
            <input
              value={form.code}
              onChange={(e) => setForm((s) => ({ ...s, code: e.target.value }))}
              placeholder="VEH-004"
              maxLength={50}
              className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div>
            <label className="block text-xs font-medium text-slate-600 mb-1">Plaka *</label>
            <input
              value={form.licensePlate}
              onChange={(e) => setForm((s) => ({ ...s, licensePlate: e.target.value }))}
              placeholder="34 AB 1234"
              maxLength={20}
              className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div>
            <label className="block text-xs font-medium text-slate-600 mb-1">Araç Tipi *</label>
            <input
              value={form.vehicleType}
              onChange={(e) => setForm((s) => ({ ...s, vehicleType: e.target.value }))}
              placeholder="Kamyonet, Minibüs..."
              maxLength={50}
              className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          {mode === "edit" && (
            <div className="flex items-center gap-2">
              <input
                type="checkbox"
                id="isActive"
                checked={form.isActive}
                onChange={(e) => setForm((s) => ({ ...s, isActive: e.target.checked }))}
                className="w-4 h-4 accent-blue-600 cursor-pointer"
              />
              <label htmlFor="isActive" className="text-sm text-slate-700 cursor-pointer">Aktif</label>
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
export default function VehiclesPage() {
  const { user } = useAuth();
  const canWrite = user?.permissions?.includes("vehicles.create");
  const canDelete = user?.permissions?.includes("vehicles.delete");

  const [data, setData] = useState<PagedResult | null>(null);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const [modal, setModal] = useState<{ mode: "create" | "edit"; vehicle: (VehicleForm & { id: string }) | null } | null>(null);
  const [deleting, setDeleting] = useState<string | null>(null);
  const PAGE_SIZE = 20;

  const loadData = useCallback(() => {
    setLoading(true);
    api
      .get<{ data: PagedResult }>(`/vehicles?page=${page}&pageSize=${PAGE_SIZE}`)
      .then((res) => setData(res.data.data))
      .finally(() => setLoading(false));
  }, [page]);

  useEffect(() => { loadData(); }, [loadData]);

  async function handleDelete(v: Vehicle) {
    if (!confirm(`"${v.licensePlate}" plakalı aracı silmek istediğinize emin misiniz?`)) return;
    setDeleting(v.id);
    try {
      await api.delete(`/vehicles/${v.id}`);
      toast.success("Araç silindi.");
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
        <VehicleModal
          mode={modal.mode}
          initial={modal.vehicle}
          onClose={() => setModal(null)}
          onSaved={() => { setModal(null); loadData(); }}
        />
      )}
      <Topbar title="Araçlar" />
      <main className="flex-1 p-6">
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
          {/* Başlık */}
          <div className="flex items-center justify-between px-6 py-4 border-b border-slate-100">
            <div className="flex items-center gap-2">
              <Truck className="w-4 h-4 text-slate-400" />
              <span className="font-semibold text-slate-800 text-sm">
                {data ? `${data.totalCount} araç` : "Yükleniyor..."}
              </span>
            </div>
            {canWrite && (
              <button
                onClick={() => setModal({ mode: "create", vehicle: null })}
                className="flex items-center gap-1.5 px-3 py-1.5 bg-blue-600 hover:bg-blue-700 text-white text-xs font-semibold rounded-lg transition-colors cursor-pointer"
              >
                <Plus className="w-3.5 h-3.5" />
                Yeni Araç
              </button>
            )}
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
                  <th className="text-center px-6 py-3">İşlemler</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-50">
                {loading
                  ? Array.from({ length: 4 }).map((_, i) => (
                      <tr key={i}>
                        {Array.from({ length: 6 }).map((__, j) => (
                          <td key={j} className="px-6 py-4">
                            <div className="h-4 bg-slate-100 rounded animate-pulse" />
                          </td>
                        ))}
                      </tr>
                    ))
                  : data?.items.map((v) => (
                      <tr key={v.id} className="hover:bg-slate-50/70 transition-colors">
                        <td className="px-6 py-3.5 font-mono font-semibold text-xs text-slate-800">
                          {v.licensePlate}
                        </td>
                        <td className="px-4 py-3.5">
                          <span className="font-mono text-xs bg-slate-100 text-slate-600 px-2 py-0.5 rounded">
                            {v.code}
                          </span>
                        </td>
                        <td className="px-4 py-3.5 text-xs text-slate-600">{v.vehicleType}</td>
                        <td className="px-4 py-3.5">
                          {v.activeTaskName ? (
                            <span className="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium bg-blue-50 text-blue-700">
                              {v.activeTaskName}
                            </span>
                          ) : (
                            <span className="text-slate-300 text-xs">—</span>
                          )}
                        </td>
                        <td className="px-4 py-3.5 text-center">
                          {v.isActive ? (
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
                            <Link
                              href={`/vehicles/${v.id}`}
                              className="text-xs text-blue-600 hover:text-blue-800 font-medium transition-colors"
                            >
                              Envanter
                            </Link>
                            {canWrite && (
                              <button
                                onClick={() =>
                                  setModal({
                                    mode: "edit",
                                    vehicle: { id: v.id, code: v.code, licensePlate: v.licensePlate, vehicleType: v.vehicleType, isActive: v.isActive },
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
                                onClick={() => handleDelete(v)}
                                disabled={deleting === v.id}
                                className="text-slate-400 hover:text-red-500 transition-colors cursor-pointer disabled:opacity-40"
                                title="Sil"
                              >
                                {deleting === v.id ? (
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
