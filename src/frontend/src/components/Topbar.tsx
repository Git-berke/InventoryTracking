"use client";

import { LogOut, User } from "lucide-react";
import { useAuth } from "@/context/AuthContext";

export default function Topbar({ title }: { title: string }) {
  const { user, logout } = useAuth();

  return (
    <header className="h-14 bg-white border-b border-slate-200 px-6 flex items-center justify-between shrink-0">
      <h1 className="text-base font-semibold text-slate-800">{title}</h1>

      <div className="flex items-center gap-3">
        <div className="flex items-center gap-2 text-sm">
          <div className="w-7 h-7 bg-blue-100 rounded-full flex items-center justify-center">
            <User className="w-3.5 h-3.5 text-blue-600" />
          </div>
          <div className="text-right leading-tight">
            <p className="text-slate-700 font-medium text-xs">{user?.fullName}</p>
            <p className="text-slate-400 text-xs">{user?.roles[0]}</p>
          </div>
        </div>
        <button
          onClick={logout}
          title="Çıkış yap"
          className="flex items-center gap-1 text-slate-400 hover:text-red-500 transition-colors text-xs cursor-pointer"
        >
          <LogOut className="w-4 h-4" />
        </button>
      </div>
    </header>
  );
}
