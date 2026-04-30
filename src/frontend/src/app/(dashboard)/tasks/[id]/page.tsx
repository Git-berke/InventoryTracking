"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import {
  ArrowLeft,
  MapPin,
  Calendar,
  Truck,
  Package,
  ClipboardList,
} from "lucide-react";
import Topbar from "@/components/Topbar";
import api from "@/lib/api";

interface TaskDetail {
  id: string;
  name: string;
  description: string | null;
  region: string;
  startDate: string;
  endDate: string | null;
  status: string;
}

interface TaskVehicle {
  vehicleId: string;
  vehicleCode: string;
  licensePlate: string;
  vehicleType: string;
  assignedAtUtc: string;
  releasedAtUtc: string | null;
}

interface TaskInventoryItem {
  vehicleId: string;
  licensePlate: string;
  productId: string;
  productCode: string;
  productName: string;
  unit: string;
  quantity: number;
}

const STATUS_CONFIG: Record<string, { label: string; cls: string }> = {
  Draft: { label: "Taslak", cls: "bg-slate-100 text-slate-600" },
  InProgress: { label: "Devam Ediyor", cls: "bg-blue-100 text-blue-700" },
  Completed: { label: "Tamamlandı", cls: "bg-emerald-100 text-emerald-700" },
};

function formatDate(d: string) {
  return new Date(d).toLocaleDateString("tr-TR", {
    day: "2-digit",
    month: "short",
    year: "numeric",
  });
}

function formatDateTime(d: string) {
  return new Date(d).toLocaleString("tr-TR", {
    day: "2-digit",
    month: "short",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

function Skeleton({ className }: { className?: string }) {
  return (
    <div className={`bg-slate-100 rounded animate-pulse ${className ?? ""}`} />
  );
}

export default function TaskDetailPage() {
  const params = useParams();
  const router = useRouter();
  const id = params.id as string;

  const [task, setTask] = useState<TaskDetail | null>(null);
  const [vehicles, setVehicles] = useState<TaskVehicle[]>([]);
  const [inventory, setInventory] = useState<TaskInventoryItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [notFound, setNotFound] = useState(false);

  useEffect(() => {
    Promise.all([
      api.get<{ data: TaskDetail }>(`/tasks/${id}`),
      api.get<{ data: TaskVehicle[] }>(`/tasks/${id}/vehicles`),
      api.get<{ data: TaskInventoryItem[] }>(`/tasks/${id}/inventory`),
    ])
      .then(([taskRes, vehiclesRes, inventoryRes]) => {
        setTask(taskRes.data.data);
        setVehicles(vehiclesRes.data.data ?? []);
        setInventory(inventoryRes.data.data ?? []);
      })
      .catch((err) => {
        if (err?.response?.status === 404) setNotFound(true);
      })
      .finally(() => setLoading(false));
  }, [id]);

  if (!loading && notFound) {
    return (
      <>
        <Topbar title="Görev Detayı" />
        <main className="flex-1 p-6 flex flex-col items-center justify-center gap-4">
          <ClipboardList className="w-12 h-12 text-slate-300" />
          <p className="text-slate-500 font-medium">Görev bulunamadı.</p>
          <button
            onClick={() => router.push("/tasks")}
            className="text-sm text-blue-600 hover:underline flex items-center gap-1"
          >
            <ArrowLeft className="w-4 h-4" /> Görevlere dön
          </button>
        </main>
      </>
    );
  }

  const totalQty = inventory.reduce((s, i) => s + i.quantity, 0);
  const statusCfg =
    STATUS_CONFIG[task?.status ?? ""] ?? {
      label: task?.status ?? "",
      cls: "bg-slate-100 text-slate-600",
    };

  // Envanter araç bazında grupla
  const byVehicle = inventory.reduce<Record<string, TaskInventoryItem[]>>(
    (acc, item) => {
      (acc[item.vehicleId] ??= []).push(item);
      return acc;
    },
    {}
  );

  return (
    <>
      <Topbar title="Görev Detayı" />
      <main className="flex-1 p-6 space-y-6">
        {/* Geri butonu */}
        <button
          onClick={() => router.push("/tasks")}
          className="flex items-center gap-1.5 text-sm text-slate-500 hover:text-slate-800 transition-colors"
        >
          <ArrowLeft className="w-4 h-4" />
          Tüm görevler
        </button>

        {/* Görev Bilgi Kartı */}
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm p-6">
          {loading ? (
            <div className="space-y-3">
              <Skeleton className="h-6 w-48" />
              <Skeleton className="h-4 w-72" />
              <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mt-4">
                {Array.from({ length: 4 }).map((_, i) => (
                  <Skeleton key={i} className="h-14" />
                ))}
              </div>
            </div>
          ) : (
            <>
              <div className="flex flex-wrap items-start justify-between gap-3 mb-4">
                <div>
                  <h1 className="text-xl font-bold text-slate-800">
                    {task!.name}
                  </h1>
                  {task!.description && (
                    <p className="mt-1 text-sm text-slate-500">
                      {task!.description}
                    </p>
                  )}
                </div>
                <span
                  className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-medium ${statusCfg.cls}`}
                >
                  {statusCfg.label}
                </span>
              </div>

              <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                <div className="bg-slate-50 rounded-lg px-4 py-3">
                  <p className="text-xs text-slate-400 mb-1">Bölge</p>
                  <p className="text-sm font-semibold text-slate-700 flex items-center gap-1">
                    <MapPin className="w-3.5 h-3.5 text-slate-400" />
                    {task!.region}
                  </p>
                </div>
                <div className="bg-slate-50 rounded-lg px-4 py-3">
                  <p className="text-xs text-slate-400 mb-1">Başlangıç</p>
                  <p className="text-sm font-semibold text-slate-700 flex items-center gap-1">
                    <Calendar className="w-3.5 h-3.5 text-slate-400" />
                    {formatDate(task!.startDate)}
                  </p>
                </div>
                <div className="bg-slate-50 rounded-lg px-4 py-3">
                  <p className="text-xs text-slate-400 mb-1">Bitiş</p>
                  <p className="text-sm font-semibold text-slate-700">
                    {task!.endDate ? (
                      <span className="flex items-center gap-1">
                        <Calendar className="w-3.5 h-3.5 text-slate-400" />
                        {formatDate(task!.endDate)}
                      </span>
                    ) : (
                      <span className="text-slate-400">Belirtilmemiş</span>
                    )}
                  </p>
                </div>
                <div className="bg-slate-50 rounded-lg px-4 py-3">
                  <p className="text-xs text-slate-400 mb-1">Toplam Envanter</p>
                  <p className="text-sm font-bold text-blue-700">
                    {totalQty} adet
                  </p>
                </div>
              </div>
            </>
          )}
        </div>

        {/* Atanmış Araçlar */}
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
          <div className="flex items-center gap-2 px-6 py-4 border-b border-slate-100">
            <Truck className="w-4 h-4 text-slate-400" />
            <span className="font-semibold text-slate-800 text-sm">
              {loading ? "Yükleniyor..." : `Atanmış Araçlar (${vehicles.length})`}
            </span>
          </div>
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="text-xs text-slate-400 font-medium bg-slate-50 border-b border-slate-100">
                  <th className="text-left px-6 py-3">Plaka</th>
                  <th className="text-left px-4 py-3">Kod</th>
                  <th className="text-left px-4 py-3">Tür</th>
                  <th className="text-left px-4 py-3">Atanma Tarihi</th>
                  <th className="text-left px-4 py-3">Bırakılma Tarihi</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-50">
                {loading ? (
                  Array.from({ length: 3 }).map((_, i) => (
                    <tr key={i}>
                      {Array.from({ length: 5 }).map((__, j) => (
                        <td key={j} className="px-6 py-4">
                          <Skeleton className="h-4" />
                        </td>
                      ))}
                    </tr>
                  ))
                ) : vehicles.length === 0 ? (
                  <tr>
                    <td
                      colSpan={5}
                      className="px-6 py-8 text-center text-slate-400 text-sm"
                    >
                      Bu göreve atanmış araç yok.
                    </td>
                  </tr>
                ) : (
                  vehicles.map((v) => (
                    <tr
                      key={v.vehicleId}
                      className="hover:bg-slate-50/70 transition-colors"
                    >
                      <td className="px-6 py-3.5 font-semibold text-slate-800 font-mono text-xs">
                        {v.licensePlate}
                      </td>
                      <td className="px-4 py-3.5 text-slate-600 text-xs">
                        {v.vehicleCode}
                      </td>
                      <td className="px-4 py-3.5">
                        <span className="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium bg-indigo-50 text-indigo-700">
                          {v.vehicleType}
                        </span>
                      </td>
                      <td className="px-4 py-3.5 text-xs text-slate-500">
                        {formatDateTime(v.assignedAtUtc)}
                      </td>
                      <td className="px-4 py-3.5 text-xs text-slate-500">
                        {v.releasedAtUtc ? (
                          formatDateTime(v.releasedAtUtc)
                        ) : (
                          <span className="inline-flex items-center px-2 py-0.5 rounded-full text-xs bg-blue-50 text-blue-600 font-medium">
                            Aktif
                          </span>
                        )}
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>

        {/* Envanter - Araç Bazında */}
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
          <div className="flex items-center gap-2 px-6 py-4 border-b border-slate-100">
            <Package className="w-4 h-4 text-slate-400" />
            <span className="font-semibold text-slate-800 text-sm">
              {loading
                ? "Yükleniyor..."
                : `Envanter (${inventory.length} kalem · ${totalQty} adet)`}
            </span>
          </div>

          {loading ? (
            <div className="p-6 space-y-3">
              {Array.from({ length: 4 }).map((_, i) => (
                <Skeleton key={i} className="h-10" />
              ))}
            </div>
          ) : inventory.length === 0 ? (
            <div className="px-6 py-8 text-center text-slate-400 text-sm">
              Bu görevde envanter kaydı yok.
            </div>
          ) : (
            <div className="divide-y divide-slate-100">
              {Object.entries(byVehicle).map(([vehicleId, items]) => {
                const v = vehicles.find((x) => x.vehicleId === vehicleId);
                const vehicleTotal = items.reduce(
                  (s, i) => s + i.quantity,
                  0
                );
                return (
                  <div key={vehicleId}>
                    {/* Araç başlığı */}
                    <div className="flex items-center justify-between px-6 py-2.5 bg-slate-50">
                      <div className="flex items-center gap-2 text-xs font-semibold text-slate-600">
                        <Truck className="w-3.5 h-3.5 text-slate-400" />
                        {v ? (
                          <>
                            <span className="font-mono">{v.licensePlate}</span>
                            <span className="text-slate-400">·</span>
                            <span>{v.vehicleCode}</span>
                          </>
                        ) : (
                          <span className="font-mono text-slate-400">
                            {vehicleId.slice(0, 8)}…
                          </span>
                        )}
                      </div>
                      <span className="text-xs text-slate-500 font-medium">
                        {vehicleTotal} adet
                      </span>
                    </div>

                    {/* Ürünler */}
                    <table className="w-full text-sm">
                      <tbody className="divide-y divide-slate-50">
                        {items.map((item) => (
                          <tr
                            key={`${item.vehicleId}-${item.productId}`}
                            className="hover:bg-slate-50/50 transition-colors"
                          >
                            <td className="px-8 py-3 font-medium text-slate-700">
                              {item.productName}
                            </td>
                            <td className="px-4 py-3 text-xs text-slate-400 font-mono">
                              {item.productCode}
                            </td>
                            <td className="px-4 py-3 text-xs text-slate-500">
                              {item.unit}
                            </td>
                            <td className="px-6 py-3 text-right">
                              <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-semibold bg-blue-50 text-blue-700">
                                {item.quantity}
                              </span>
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                );
              })}
            </div>
          )}
        </div>
      </main>
    </>
  );
}
