"use client";

import { useEffect, useEffectEvent, useState } from "react";
import Link from "next/link";
import {
  AlertCircle,
  ArrowRight,
  ClipboardList,
  Package,
  TrendingUp,
  Truck,
  Warehouse,
} from "lucide-react";
import InventoryLiveUpdates from "@/components/InventoryLiveUpdates";
import Topbar from "@/components/Topbar";
import api from "@/lib/api";

interface StatsData {
  products: number | null;
  warehouses: number | null;
  vehicles: number | null;
  tasks: number | null;
  activeTasks: number | null;
}

interface Transaction {
  id: string;
  productName: string;
  transactionType: string;
  quantity: number;
  performedAtUtc: string;
  referenceNote: string | null;
}

const TX_TYPE_LABELS: Record<string, string> = {
  InitialLoad: "İlk Yükleme",
  WarehouseToVehicle: "Depo → Araç",
  VehicleToWarehouse: "Araç → Depo",
  WarehouseToWarehouse: "Depo → Depo",
};

const TX_TYPE_COLORS: Record<string, string> = {
  InitialLoad: "bg-slate-100 text-slate-700",
  WarehouseToVehicle: "bg-blue-100 text-blue-700",
  VehicleToWarehouse: "bg-green-100 text-green-700",
  WarehouseToWarehouse: "bg-purple-100 text-purple-700",
};

export default function DashboardPage() {
  const [stats, setStats] = useState<StatsData>({
    products: null,
    warehouses: null,
    vehicles: null,
    tasks: null,
    activeTasks: null,
  });
  const [recentTx, setRecentTx] = useState<Transaction[]>([]);
  const [txLoading, setTxLoading] = useState(true);

  const loadDashboard = useEffectEvent(async () => {
    setTxLoading(true);

    const [products, warehouses, vehicles, tasks, transactions] =
      await Promise.allSettled([
        api.get("/products?page=1&pageSize=1"),
        api.get("/warehouses?page=1&pageSize=1"),
        api.get("/vehicles?page=1&pageSize=1"),
        api.get("/tasks?page=1&pageSize=1"),
        api.get("/inventory-transactions?page=1&pageSize=6"),
      ]);

    setStats({
      products:
        products.status === "fulfilled"
          ? products.value.data.data.totalCount
          : null,
      warehouses:
        warehouses.status === "fulfilled"
          ? warehouses.value.data.data.totalCount
          : null,
      vehicles:
        vehicles.status === "fulfilled"
          ? vehicles.value.data.data.totalCount
          : null,
      tasks:
        tasks.status === "fulfilled" ? tasks.value.data.data.totalCount : null,
      activeTasks: null,
    });

    if (transactions.status === "fulfilled") {
      setRecentTx(transactions.value.data.data.items ?? []);
    }

    setTxLoading(false);
  });

  useEffect(() => {
    void loadDashboard();
  }, []);

  const statCards = [
    {
      label: "Toplam Ürün",
      value: stats.products,
      icon: Package,
      light: "bg-blue-50",
      text: "text-blue-600",
      href: "/products",
    },
    {
      label: "Depolar",
      value: stats.warehouses,
      icon: Warehouse,
      light: "bg-amber-50",
      text: "text-amber-600",
      href: "/warehouses",
    },
    {
      label: "Araçlar",
      value: stats.vehicles,
      icon: Truck,
      light: "bg-emerald-50",
      text: "text-emerald-600",
      href: "/vehicles",
    },
    {
      label: "Görevler",
      value: stats.tasks,
      icon: ClipboardList,
      light: "bg-purple-50",
      text: "text-purple-600",
      href: "/tasks",
    },
  ];

  return (
    <>
      <Topbar title="Dashboard" />
      <main className="flex-1 space-y-6 p-6">
        <div className="grid grid-cols-2 gap-4 lg:grid-cols-4">
          {statCards.map(({ label, value, icon: Icon, light, text, href }) => (
            <Link
              key={label}
              href={href}
              className="group rounded-xl border border-slate-200 bg-white p-5 shadow-sm transition-shadow hover:shadow-md"
            >
              <div className="mb-3 flex items-start justify-between">
                <div
                  className={`flex h-10 w-10 items-center justify-center rounded-lg ${light}`}
                >
                  <Icon className={`h-5 w-5 ${text}`} />
                </div>
                <ArrowRight className="h-4 w-4 text-slate-300 transition-colors group-hover:text-slate-500" />
              </div>
              <div>
                {value === null ? (
                  <div className="mb-1 h-7 w-10 animate-pulse rounded bg-slate-100" />
                ) : (
                  <p className="text-2xl font-bold text-slate-800">{value}</p>
                )}
                <p className="text-sm text-slate-500">{label}</p>
              </div>
            </Link>
          ))}
        </div>

        <div className="grid gap-6 xl:grid-cols-[minmax(0,1.6fr)_minmax(320px,1fr)]">
          <div className="rounded-xl border border-slate-200 bg-white shadow-sm">
            <div className="flex items-center justify-between border-b border-slate-100 px-6 py-4">
              <div className="flex items-center gap-2">
                <TrendingUp className="h-4 w-4 text-slate-400" />
                <h2 className="text-sm font-semibold text-slate-800">
                  Son Stok Hareketleri
                </h2>
              </div>
              <Link
                href="/inventory-transactions"
                className="text-xs font-medium text-blue-600 transition-colors hover:text-blue-800"
              >
                Tümünü gör →
              </Link>
            </div>

            {txLoading ? (
              <div className="divide-y divide-slate-50">
                {Array.from({ length: 4 }).map((_, index) => (
                  <div
                    key={index}
                    className="flex items-center gap-4 px-6 py-3.5"
                  >
                    <div className="h-4 w-24 animate-pulse rounded bg-slate-100" />
                    <div className="h-4 w-36 animate-pulse rounded bg-slate-100" />
                    <div className="h-4 w-16 animate-pulse rounded bg-slate-100" />
                    <div className="ml-auto h-4 w-28 animate-pulse rounded bg-slate-100" />
                  </div>
                ))}
              </div>
            ) : recentTx.length === 0 ? (
              <div className="flex flex-col items-center justify-center py-12 text-slate-400">
                <AlertCircle className="mb-2 h-8 w-8" />
                <p className="text-sm">Henüz stok hareketi yok.</p>
              </div>
            ) : (
              <div className="overflow-x-auto">
                <table className="w-full text-sm">
                  <thead>
                    <tr className="bg-slate-50/50 text-xs font-medium text-slate-400">
                      <th className="px-6 py-2.5 text-left">Ürün</th>
                      <th className="px-4 py-2.5 text-left">İşlem Türü</th>
                      <th className="px-4 py-2.5 text-right">Miktar</th>
                      <th className="hidden px-4 py-2.5 text-left md:table-cell">
                        Not
                      </th>
                      <th className="px-6 py-2.5 text-right">Tarih</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-slate-50">
                    {recentTx.map((tx) => (
                      <tr
                        key={tx.id}
                        className="transition-colors hover:bg-slate-50/70"
                      >
                        <td className="px-6 py-3 font-medium text-slate-700">
                          {tx.productName}
                        </td>
                        <td className="px-4 py-3">
                          <span
                            className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${
                              TX_TYPE_COLORS[tx.transactionType] ??
                              "bg-slate-100 text-slate-600"
                            }`}
                          >
                            {TX_TYPE_LABELS[tx.transactionType] ?? tx.transactionType}
                          </span>
                        </td>
                        <td className="px-4 py-3 text-right font-semibold text-slate-700">
                          {tx.quantity}
                        </td>
                        <td className="hidden max-w-xs truncate px-4 py-3 text-xs text-slate-400 md:table-cell">
                          {tx.referenceNote ?? "—"}
                        </td>
                        <td className="whitespace-nowrap px-6 py-3 text-right text-xs text-slate-400">
                          {new Date(tx.performedAtUtc).toLocaleDateString("tr-TR", {
                            day: "2-digit",
                            month: "short",
                            year: "numeric",
                          })}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </div>

          <InventoryLiveUpdates onEvent={loadDashboard} />
        </div>
      </main>
    </>
  );
}
