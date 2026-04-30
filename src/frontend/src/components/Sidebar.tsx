"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import {
  Package,
  Warehouse,
  Truck,
  ClipboardList,
  ArrowLeftRight,
  History,
  LayoutDashboard,
} from "lucide-react";

const NAV_ITEMS = [
  { href: "/", label: "Dashboard", icon: LayoutDashboard },
  { href: "/products", label: "Ürünler", icon: Package },
  { href: "/warehouses", label: "Depolar", icon: Warehouse },
  { href: "/vehicles", label: "Araçlar", icon: Truck },
  { href: "/tasks", label: "Görevler", icon: ClipboardList },
  { href: "/stock-transfers", label: "Stok Transferi", icon: ArrowLeftRight },
  { href: "/inventory-transactions", label: "Hareket Geçmişi", icon: History },
];

export default function Sidebar() {
  const pathname = usePathname();

  return (
    <aside className="w-56 shrink-0 bg-slate-900 min-h-screen flex flex-col">
      {/* Logo */}
      <div className="px-5 py-5 border-b border-slate-700/60">
        <div className="flex items-center gap-2.5">
          <div className="w-7 h-7 bg-blue-500 rounded-lg flex items-center justify-center shrink-0">
            <Package className="w-4 h-4 text-white" />
          </div>
          <span className="text-white font-semibold text-sm leading-tight">
            PTN Inventory
          </span>
        </div>
      </div>

      {/* Nav */}
      <nav className="flex-1 px-3 py-4 space-y-0.5">
        {NAV_ITEMS.map(({ href, label, icon: Icon }) => {
          const active = pathname === href;
          return (
            <Link
              key={href}
              href={href}
              className={`flex items-center gap-2.5 px-3 py-2 rounded-lg text-sm font-medium transition-colors
                ${
                  active
                    ? "bg-blue-600 text-white"
                    : "text-slate-400 hover:text-white hover:bg-slate-800"
                }`}
            >
              <Icon className="w-4 h-4 shrink-0" />
              {label}
            </Link>
          );
        })}
      </nav>

      {/* Footer */}
      <div className="px-5 py-4 border-t border-slate-700/60">
        <p className="text-xs text-slate-500">v1.0 · PTN Technology</p>
      </div>
    </aside>
  );
}
