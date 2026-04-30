"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import {
  Package,
  Warehouse,
  Truck,
  ClipboardList,
  ArrowRight,
  TrendingUp,
  AlertCircle,
} from "lucide-react";
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
  WarehouseToVehicle: "Depoya → Araç",
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

  useEffect(() => {
    async function load() {
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
          tasks.status === "fulfilled"
            ? tasks.value.data.data.totalCount
            : null,
        activeTasks: null,
      });

      if (transactions.status === "fulfilled") {
        setRecentTx(transactions.value.data.data.items ?? []);
      }
      setTxLoading(false);
    }
    load();
  }, []);

  const STAT_CARDS = [
    {
      label: "Toplam Ürün",
      value: stats.products,
      icon: Package,
      color: "bg-blue-500",
      light: "bg-blue-50",
      text: "text-blue-600",
      href: "/products",
    },
    {
      label: "Depolar",
      value: stats.warehouses,
      icon: Warehouse,
      color: "bg-amber-500",
      light: "bg-amber-50",
      text: "text-amber-600",
      href: "/warehouses",
    },
    {
      label: "Araçlar",
      value: stats.vehicles,
      icon: Truck,
      color: "bg-emerald-500",
      light: "bg-emerald-50",
      text: "text-emerald-600",
      href: "/vehicles",
    },
    {
      label: "Görevler",
      value: stats.tasks,
      icon: ClipboardList,
      color: "bg-purple-500",
      light: "bg-purple-50",
      text: "text-purple-600",
      href: "/tasks",
    },
  ];

  return (
    <>
      <Topbar title="Dashboard" />
      <main className="flex-1 p-6 space-y-6">
        {/* İstatistik Kartları */}
        <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
          {STAT_CARDS.map(({ label, value, icon: Icon, light, text, href, color }) => (
            <Link
              key={label}
              href={href}
              className="bg-white rounded-xl border border-slate-200 p-5 shadow-sm hover:shadow-md transition-shadow group"
            >
              <div className="flex items-start justify-between mb-3">
                <div className={`w-10 h-10 ${light} rounded-lg flex items-center justify-center`}>
                  <Icon className={`w-5 h-5 ${text}`} />
                </div>
                <ArrowRight className="w-4 h-4 text-slate-300 group-hover:text-slate-500 transition-colors" />
              </div>
              <div>
                {value === null ? (
                  <div className="h-7 w-10 bg-slate-100 rounded animate-pulse mb-1" />
                ) : (
                  <p className="text-2xl font-bold text-slate-800">{value}</p>
                )}
                <p className="text-sm text-slate-500">{label}</p>
              </div>
            </Link>
          ))}
        </div>

        {/* Son İşlemler */}
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
          <div className="flex items-center justify-between px-6 py-4 border-b border-slate-100">
            <div className="flex items-center gap-2">
              <TrendingUp className="w-4 h-4 text-slate-400" />
              <h2 className="font-semibold text-slate-800 text-sm">
                Son Stok Hareketleri
              </h2>
            </div>
            <Link
              href="/inventory-transactions"
              className="text-xs text-blue-600 hover:text-blue-800 font-medium transition-colors"
            >
              Tümünü gör →
            </Link>
          </div>

          {txLoading ? (
            <div className="divide-y divide-slate-50">
              {Array.from({ length: 4 }).map((_, i) => (
                <div key={i} className="flex items-center gap-4 px-6 py-3.5">
                  <div className="h-4 bg-slate-100 rounded w-24 animate-pulse" />
                  <div className="h-4 bg-slate-100 rounded w-36 animate-pulse" />
                  <div className="h-4 bg-slate-100 rounded w-16 animate-pulse" />
                  <div className="ml-auto h-4 bg-slate-100 rounded w-28 animate-pulse" />
                </div>
              ))}
            </div>
          ) : recentTx.length === 0 ? (
            <div className="flex flex-col items-center justify-center py-12 text-slate-400">
              <AlertCircle className="w-8 h-8 mb-2" />
              <p className="text-sm">Henüz stok hareketi yok.</p>
            </div>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="text-xs text-slate-400 font-medium bg-slate-50/50">
                    <th className="text-left px-6 py-2.5">Ürün</th>
                    <th className="text-left px-4 py-2.5">İşlem Türü</th>
                    <th className="text-right px-4 py-2.5">Miktar</th>
                    <th className="text-left px-4 py-2.5 hidden md:table-cell">Not</th>
                    <th className="text-right px-6 py-2.5">Tarih</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-50">
                  {recentTx.map((tx) => (
                    <tr key={tx.id} className="hover:bg-slate-50/70 transition-colors">
                      <td className="px-6 py-3 font-medium text-slate-700">
                        {tx.productName}
                      </td>
                      <td className="px-4 py-3">
                        <span
                          className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium ${
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
                      <td className="px-4 py-3 text-slate-400 text-xs hidden md:table-cell max-w-xs truncate">
                        {tx.referenceNote ?? "—"}
                      </td>
                      <td className="px-6 py-3 text-right text-xs text-slate-400 whitespace-nowrap">
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
      </main>
    </>
  );
}
