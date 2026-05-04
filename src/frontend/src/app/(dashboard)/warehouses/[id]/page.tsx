"use client";

import { HubConnectionState } from "@microsoft/signalr";
import { useEffect, useEffectEvent, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import {
  ArrowLeft,
  Warehouse,
  MapPin,
  Hash,
  CheckCircle2,
  XCircle,
  Building2,
  Boxes,
  Package,
  AlertCircle,
  Radio,
  RefreshCw,
  WifiOff,
} from "lucide-react";
import Topbar from "@/components/Topbar";
import { useInventoryRealtime } from "@/hooks/useInventoryRealtime";
import api from "@/lib/api";
import type { InventoryRealtimeEvent } from "@/types/realtime";

interface WarehouseDetail {
  id: string;
  code: string;
  name: string;
  region: string;
  address: string | null;
  isActive: boolean;
}

interface WarehouseInventoryItem {
  productId: string;
  productCode: string;
  productName: string;
  unit: string;
  quantity: number;
}

interface WarehouseInventoryDetail {
  warehouseId: string;
  warehouseCode: string;
  warehouseName: string;
  region: string;
  inventories: WarehouseInventoryItem[];
}

function Skeleton({ className }: { className?: string }) {
  return (
    <div className={`animate-pulse rounded bg-slate-100 ${className ?? ""}`} />
  );
}

function InfoRow({
  icon,
  label,
  value,
}: {
  icon: React.ReactNode;
  label: string;
  value: React.ReactNode;
}) {
  return (
    <div className="flex items-start gap-3 border-b border-slate-50 py-3.5 last:border-0">
      <div className="mt-0.5 text-slate-400">{icon}</div>
      <div className="min-w-0 flex-1">
        <p className="mb-0.5 text-xs text-slate-400">{label}</p>
        <div className="text-sm font-medium text-slate-800">{value}</div>
      </div>
    </div>
  );
}

function isWarehouseRelatedEvent(
  event: InventoryRealtimeEvent,
  warehouseId: string
) {
  const sourceWarehouseId = event.context.sourceWarehouseId;
  const destinationWarehouseId = event.context.destinationWarehouseId;

  return sourceWarehouseId === warehouseId || destinationWarehouseId === warehouseId;
}

function getRealtimeBadge(status: HubConnectionState | "error") {
  switch (status) {
    case HubConnectionState.Connected:
      return {
        label: "Canlı bağlı",
        className: "border-emerald-200 bg-emerald-50 text-emerald-700",
        dot: "bg-emerald-500",
        Icon: Radio,
        spin: false,
      };
    case HubConnectionState.Reconnecting:
      return {
        label: "Yeniden bağlanıyor",
        className: "border-amber-200 bg-amber-50 text-amber-700",
        dot: "bg-amber-500",
        Icon: RefreshCw,
        spin: true,
      };
    case "error":
      return {
        label: "Canlı bağlantı hatası",
        className: "border-rose-200 bg-rose-50 text-rose-700",
        dot: "bg-rose-500",
        Icon: WifiOff,
        spin: false,
      };
    default:
      return {
        label: "Canlı bağlantı bekleniyor",
        className: "border-slate-200 bg-slate-100 text-slate-600",
        dot: "bg-slate-400",
        Icon: WifiOff,
        spin: false,
      };
  }
}

export default function WarehouseDetailPage() {
  const params = useParams();
  const router = useRouter();
  const id = params.id as string;

  const [warehouse, setWarehouse] = useState<WarehouseDetail | null>(null);
  const [inventory, setInventory] = useState<WarehouseInventoryDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [notFound, setNotFound] = useState(false);

  const loadWarehouse = useEffectEvent(async () => {
    const [warehouseResult, inventoryResult] = await Promise.allSettled([
      api.get<{ data: WarehouseDetail }>(`/warehouses/${id}`),
      api.get<{ data: WarehouseInventoryDetail }>(`/warehouses/${id}/inventories`),
    ]);

    if (warehouseResult.status === "fulfilled") {
      setWarehouse(warehouseResult.value.data.data);
      setNotFound(false);
    } else if (warehouseResult.reason?.response?.status === 404) {
      setNotFound(true);
    }

    if (inventoryResult.status === "fulfilled") {
      setInventory(inventoryResult.value.data.data);
      setNotFound(false);
    } else if (inventoryResult.reason?.response?.status === 404) {
      setNotFound(true);
    }

    setLoading(false);
  });

  useEffect(() => {
    setLoading(true);
    void loadWarehouse();
  }, [id]);

  const handleRealtimeEvent = useEffectEvent(
    async (event: InventoryRealtimeEvent) => {
      if (!isWarehouseRelatedEvent(event, id)) {
        return;
      }

      await loadWarehouse();
    }
  );

  const realtimeStatus = useInventoryRealtime({
    enabled: !notFound,
    onEvent: handleRealtimeEvent,
  });
  const realtimeBadge = getRealtimeBadge(realtimeStatus);
  const RealtimeIcon = realtimeBadge.Icon;

  const totalItems =
    inventory?.inventories.reduce((sum, item) => sum + item.quantity, 0) ?? 0;

  if (!loading && notFound) {
    return (
      <>
        <Topbar title="Depo Detayı" />
        <main className="flex flex-1 flex-col items-center justify-center gap-4 p-6">
          <Warehouse className="h-12 w-12 text-slate-300" />
          <p className="font-medium text-slate-500">Depo bulunamadı.</p>
          <button
            onClick={() => router.push("/warehouses")}
            className="flex items-center gap-1 text-sm text-blue-600 hover:underline"
          >
            <ArrowLeft className="h-4 w-4" /> Depolara dön
          </button>
        </main>
      </>
    );
  }

  return (
    <>
      <Topbar title="Depo Detayı" />
      <main className="flex-1 space-y-6 p-6">
        <button
          onClick={() => router.push("/warehouses")}
          className="flex items-center gap-1.5 text-sm text-slate-500 transition-colors hover:text-slate-800"
        >
          <ArrowLeft className="h-4 w-4" />
          Tüm depolar
        </button>

        <div className="rounded-xl border border-slate-200 bg-white shadow-sm">
          <div className="flex items-start justify-between border-b border-slate-100 px-6 py-5">
            {loading ? (
              <div className="flex-1 space-y-2">
                <Skeleton className="h-6 w-48" />
                <Skeleton className="h-4 w-24" />
              </div>
            ) : (
              <>
                <div className="flex items-center gap-3">
                  <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl bg-indigo-50">
                    <Building2 className="h-5 w-5 text-indigo-500" />
                  </div>
                  <div>
                    <h1 className="text-lg font-bold text-slate-800">
                      {warehouse!.name}
                    </h1>
                    <span className="rounded bg-slate-100 px-2 py-0.5 font-mono text-xs text-slate-500">
                      {warehouse!.code}
                    </span>
                  </div>
                </div>
                {warehouse!.isActive ? (
                  <span className="inline-flex shrink-0 items-center gap-1 rounded-full bg-emerald-50 px-3 py-1 text-xs font-medium text-emerald-700">
                    <CheckCircle2 className="h-3.5 w-3.5" />
                    Aktif
                  </span>
                ) : (
                  <span className="inline-flex shrink-0 items-center gap-1 rounded-full bg-slate-100 px-3 py-1 text-xs font-medium text-slate-500">
                    <XCircle className="h-3.5 w-3.5" />
                    Pasif
                  </span>
                )}
              </>
            )}
          </div>

          <div className="px-6 py-2">
            {loading ? (
              <div className="space-y-4 py-4">
                {Array.from({ length: 3 }).map((_, index) => (
                  <Skeleton key={index} className="h-10" />
                ))}
              </div>
            ) : (
              <>
                <InfoRow
                  icon={<Hash className="h-4 w-4" />}
                  label="Depo Kodu"
                  value={<span className="font-mono">{warehouse!.code}</span>}
                />
                <InfoRow
                  icon={<MapPin className="h-4 w-4" />}
                  label="Bölge"
                  value={warehouse!.region}
                />
                <InfoRow
                  icon={<Building2 className="h-4 w-4" />}
                  label="Adres"
                  value={
                    warehouse!.address ? (
                      warehouse!.address
                    ) : (
                      <span className="font-normal text-slate-400">
                        Belirtilmemiş
                      </span>
                    )
                  }
                />
              </>
            )}
          </div>
        </div>

        <div className="rounded-xl border border-slate-200 bg-white shadow-sm overflow-hidden">
          <div className="flex items-center justify-between gap-3 border-b border-slate-100 px-6 py-4">
            <div className="flex items-center gap-2">
              <Boxes className="h-4 w-4 text-slate-400" />
              <span className="text-sm font-semibold text-slate-800">
                Depo Envanteri
              </span>
              <span
                className={`inline-flex items-center gap-2 rounded-full border px-2.5 py-1 text-[11px] font-medium ${realtimeBadge.className}`}
              >
                <span className={`h-2 w-2 rounded-full ${realtimeBadge.dot}`} />
                <RealtimeIcon
                  className={`h-3 w-3 ${realtimeBadge.spin ? "animate-spin" : ""}`}
                />
                {realtimeBadge.label}
              </span>
              {!loading && inventory ? (
                <span className="text-xs text-slate-400">
                  ({inventory.inventories.length} ürün çeşidi)
                </span>
              ) : null}
            </div>
            {!loading && inventory ? (
              <div className="text-right">
                <p className="text-lg font-bold text-slate-800">{totalItems}</p>
                <p className="text-xs text-slate-400">toplam adet</p>
              </div>
            ) : null}
          </div>

          {loading ? (
            <div className="space-y-3 px-6 py-5">
              {Array.from({ length: 5 }).map((_, index) => (
                <Skeleton key={index} className="h-12" />
              ))}
            </div>
          ) : !inventory || inventory.inventories.length === 0 ? (
            <div className="flex flex-col items-center py-12 text-slate-400">
              <AlertCircle className="mb-2 h-8 w-8" />
              <p className="text-sm">Bu depoda envanter yok.</p>
            </div>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b border-slate-100 bg-slate-50 text-xs font-medium text-slate-400">
                    <th className="px-6 py-3 text-left">Ürün</th>
                    <th className="px-4 py-3 text-left">Kod</th>
                    <th className="px-4 py-3 text-left">Birim</th>
                    <th className="px-6 py-3 text-right">Miktar</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-50">
                  {inventory.inventories.map((item) => (
                    <tr
                      key={item.productId}
                      className="transition-colors hover:bg-slate-50/70"
                    >
                      <td className="px-6 py-3.5">
                        <div className="flex items-center gap-2">
                          <div className="flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-amber-50">
                            <Package className="h-3.5 w-3.5 text-amber-500" />
                          </div>
                          <span className="font-medium text-slate-800">
                            {item.productName}
                          </span>
                        </div>
                      </td>
                      <td className="px-4 py-3.5 font-mono text-xs text-slate-500">
                        {item.productCode}
                      </td>
                      <td className="px-4 py-3.5 text-slate-500">{item.unit}</td>
                      <td className="px-6 py-3.5 text-right">
                        <span className="text-lg font-bold text-slate-800">
                          {item.quantity}
                        </span>
                        <span className="ml-1 text-xs text-slate-400">
                          {item.unit}
                        </span>
                      </td>
                    </tr>
                  ))}
                </tbody>
                <tfoot>
                  <tr className="border-t border-slate-100 bg-slate-50">
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
      </main>
    </>
  );
}
