"use client";

import { useEffect, useState, useCallback } from "react";
import Link from "next/link";
import {
  ClipboardList,
  ChevronLeft,
  ChevronRight,
  MapPin,
  Calendar,
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
interface TaskForm {
  name: string;
  description: string;
  region: string;
  startDate: string;
  endDate: string;
  status: number;
}

const EMPTY_FORM: TaskForm = {
  name: "",
  description: "",
  region: "",
  startDate: new Date().toISOString().slice(0, 10),
  endDate: "",
  status: 1,
};

const STATUS_OPTIONS = [
  { value: 1, label: "Taslak" },
  { value: 2, label: "Devam Ediyor" },
  { value: 3, label: "Tamamlandı" },
];

const STATUS_CONFIG: Record<string, { label: string; cls: string }> = {
  draft: { label: "Taslak", cls: "bg-slate-100 text-slate-600" },
  in_progress: { label: "Devam Ediyor", cls: "bg-blue-100 text-blue-700" },
  completed: { label: "Tamamlandı", cls: "bg-emerald-100 text-emerald-700" },
};

function StatusBadge({ status }: { status: string }) {
  const cfg = STATUS_CONFIG[status] ?? { label: status, cls: "bg-slate-100 text-slate-600" };
  return (
    <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium ${cfg.cls}`}>
      {cfg.label}
    </span>
  );
}

function formatDate(d: string) {
  return new Date(d).toLocaleDateString("tr-TR", { day: "2-digit", month: "short", year: "numeric" });
}

/* ── Modal ─────────────────────────────────────────────── */
function TaskModal({
  mode,
  initial,
  onClose,
  onSaved,
}: {
  mode: "create" | "edit";
  initial: (TaskForm & { id: string }) | null;
  onClose: () => void;
  onSaved: () => void;
}) {
  const [form, setForm] = useState<TaskForm>(
    initial
      ? { name: initial.name, description: initial.description, region: initial.region, startDate: initial.startDate, endDate: initial.endDate, status: initial.status }
      : EMPTY_FORM
  );
  const [saving, setSaving] = useState(false);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!form.name.trim() || !form.region.trim() || !form.startDate) {
      toast.error("Zorunlu alanları doldurun.");
      return;
    }
    setSaving(true);
    try {
      const body = {
        name: form.name,
        description: form.description || null,
        region: form.region,
        startDate: form.startDate,
        endDate: form.endDate || null,
        status: form.status,
      };
      if (mode === "create") {
        await api.post("/tasks", body);
        toast.success("Görev oluşturuldu.");
      } else {
        await api.put(`/tasks/${initial!.id}`, body);
        toast.success("Görev güncellendi.");
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
      <div className="bg-white rounded-2xl shadow-xl w-full max-w-lg mx-4 max-h-[90vh] overflow-y-auto">
        <div className="flex items-center justify-between px-6 py-4 border-b border-slate-100 sticky top-0 bg-white">
          <h2 className="font-semibold text-slate-800">
            {mode === "create" ? "Yeni Görev" : "Görevi Düzenle"}
          </h2>
          <button onClick={onClose} className="text-slate-400 hover:text-slate-600 cursor-pointer">
            <X className="w-5 h-5" />
          </button>
        </div>
        <form onSubmit={handleSubmit} className="p-6 space-y-4">
          <div>
            <label className="block text-xs font-medium text-slate-600 mb-1">Görev Adı *</label>
            <input
              value={form.name}
              onChange={(e) => setForm((s) => ({ ...s, name: e.target.value }))}
              placeholder="Görev adını girin..."
              maxLength={150}
              className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-xs font-medium text-slate-600 mb-1">Bölge *</label>
              <input
                value={form.region}
                onChange={(e) => setForm((s) => ({ ...s, region: e.target.value }))}
                placeholder="İzmir, Ankara..."
                maxLength={100}
                className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label className="block text-xs font-medium text-slate-600 mb-1">Durum *</label>
              <select
                value={form.status}
                onChange={(e) => setForm((s) => ({ ...s, status: Number(e.target.value) }))}
                className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                {STATUS_OPTIONS.map((o) => (
                  <option key={o.value} value={o.value}>{o.label}</option>
                ))}
              </select>
            </div>
          </div>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-xs font-medium text-slate-600 mb-1">Başlangıç Tarihi *</label>
              <input
                type="date"
                value={form.startDate}
                onChange={(e) => setForm((s) => ({ ...s, startDate: e.target.value }))}
                className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label className="block text-xs font-medium text-slate-600 mb-1">Bitiş Tarihi</label>
              <input
                type="date"
                value={form.endDate}
                onChange={(e) => setForm((s) => ({ ...s, endDate: e.target.value }))}
                className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
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
          <div className="flex gap-3 pt-2">
            <button type="button" onClick={onClose} className="flex-1 py-2.5 rounded-lg border border-slate-200 text-sm font-medium text-slate-600 hover:bg-slate-50 cursor-pointer">
              İptal
            </button>
            <button type="submit" disabled={saving} className="flex-1 py-2.5 rounded-lg bg-blue-600 hover:bg-blue-700 disabled:bg-blue-300 text-white text-sm font-semibold flex items-center justify-center gap-2 cursor-pointer">
              {saving && <Loader2 className="w-4 h-4 animate-spin" />}
              {mode === "create" ? "Oluştur" : "Kaydet"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

/* ── Ana Sayfa ─────────────────────────────────────────── */
export default function TasksPage() {
  const { user } = useAuth();
  const canWrite = user?.permissions?.includes("tasks.create");
  const canDelete = user?.permissions?.includes("tasks.delete");

  const [data, setData] = useState<PagedResult | null>(null);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const [modal, setModal] = useState<{ mode: "create" | "edit"; task: (TaskForm & { id: string }) | null } | null>(null);
  const [deleting, setDeleting] = useState<string | null>(null);
  const PAGE_SIZE = 20;

  const loadData = useCallback(() => {
    setLoading(true);
    api
      .get<{ data: PagedResult }>(`/tasks?page=${page}&pageSize=${PAGE_SIZE}`)
      .then((res) => setData(res.data.data))
      .finally(() => setLoading(false));
  }, [page]);

  useEffect(() => { loadData(); }, [loadData]);

  function openEdit(t: Task) {
    const statusMap: Record<string, number> = { draft: 1, in_progress: 2, completed: 3 };
    setModal({
      mode: "edit",
      task: {
        id: t.id,
        name: t.name,
        description: "",
        region: t.region,
        startDate: t.startDate,
        endDate: t.endDate ?? "",
        status: statusMap[t.status] ?? 1,
      },
    });
  }

  async function handleDelete(t: Task) {
    if (!confirm(`"${t.name}" görevini silmek istediğinize emin misiniz?`)) return;
    setDeleting(t.id);
    try {
      await api.delete(`/tasks/${t.id}`);
      toast.success("Görev silindi.");
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
        <TaskModal
          mode={modal.mode}
          initial={modal.task}
          onClose={() => setModal(null)}
          onSaved={() => { setModal(null); loadData(); }}
        />
      )}
      <Topbar title="Görevler" />
      <main className="flex-1 p-6">
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
          <div className="flex items-center justify-between px-6 py-4 border-b border-slate-100">
            <div className="flex items-center gap-2">
              <ClipboardList className="w-4 h-4 text-slate-400" />
              <span className="font-semibold text-slate-800 text-sm">
                {data ? `${data.totalCount} görev` : "Yükleniyor..."}
              </span>
            </div>
            {canWrite && (
              <button
                onClick={() => setModal({ mode: "create", task: null })}
                className="flex items-center gap-1.5 px-3 py-1.5 bg-blue-600 hover:bg-blue-700 text-white text-xs font-semibold rounded-lg transition-colors cursor-pointer"
              >
                <Plus className="w-3.5 h-3.5" /> Yeni Görev
              </button>
            )}
          </div>

          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="text-xs text-slate-400 font-medium bg-slate-50 border-b border-slate-100">
                  <th className="text-left px-6 py-3">Görev Adı</th>
                  <th className="text-left px-4 py-3">Bölge</th>
                  <th className="text-left px-4 py-3">Durum</th>
                  <th className="text-left px-4 py-3">Başlangıç</th>
                  <th className="text-left px-4 py-3">Bitiş</th>
                  <th className="text-center px-6 py-3">İşlemler</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-50">
                {loading
                  ? Array.from({ length: 5 }).map((_, i) => (
                      <tr key={i}>{Array.from({ length: 6 }).map((__, j) => (
                        <td key={j} className="px-6 py-4"><div className="h-4 bg-slate-100 rounded animate-pulse" /></td>
                      ))}</tr>
                    ))
                  : data?.items.map((t) => (
                      <tr key={t.id} className="hover:bg-slate-50/70 transition-colors">
                        <td className="px-6 py-3.5">
                          <div className="flex items-center gap-2">
                            <div className={`w-2 h-2 rounded-full shrink-0 ${t.status === "in_progress" ? "bg-blue-500" : t.status === "completed" ? "bg-emerald-500" : "bg-slate-300"}`} />
                            <span className="font-medium text-slate-800">{t.name}</span>
                          </div>
                        </td>
                        <td className="px-4 py-3.5">
                          <span className="inline-flex items-center gap-1 text-slate-600 text-xs">
                            <MapPin className="w-3 h-3 text-slate-400" />{t.region}
                          </span>
                        </td>
                        <td className="px-4 py-3.5"><StatusBadge status={t.status} /></td>
                        <td className="px-4 py-3.5">
                          <span className="inline-flex items-center gap-1 text-xs text-slate-500">
                            <Calendar className="w-3 h-3" />{formatDate(t.startDate)}
                          </span>
                        </td>
                        <td className="px-4 py-3.5 text-xs text-slate-500">
                          {t.endDate ? formatDate(t.endDate) : <span className="text-slate-300">—</span>}
                        </td>
                        <td className="px-6 py-3.5">
                          <div className="flex items-center justify-center gap-3">
                            <Link href={`/tasks/${t.id}`} className="text-xs text-blue-600 hover:text-blue-800 font-medium transition-colors">
                              Detay
                            </Link>
                            {canWrite && (
                              <button onClick={() => openEdit(t)} className="text-slate-400 hover:text-slate-700 transition-colors cursor-pointer" title="Düzenle">
                                <Pencil className="w-3.5 h-3.5" />
                              </button>
                            )}
                            {canDelete && (
                              <button onClick={() => handleDelete(t)} disabled={deleting === t.id} className="text-slate-400 hover:text-red-500 transition-colors cursor-pointer disabled:opacity-40" title="Sil">
                                {deleting === t.id ? <Loader2 className="w-3.5 h-3.5 animate-spin" /> : <Trash2 className="w-3.5 h-3.5" />}
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
