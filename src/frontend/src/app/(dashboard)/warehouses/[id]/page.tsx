"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import {
  ArrowLeft,
  Warehouse,
  MapPin,
  Hash,
  CheckCircle2,
  XCircle,
  Building2,
} from "lucide-react";
import Topbar from "@/components/Topbar";
import api from "@/lib/api";

interface WarehouseDetail {
  id: string;
  code: string;
  name: string;
  region: string;
  address: string | null;
  isActive: boolean;
}

function Skeleton({ className }: { className?: string }) {
  return (
    <div className={`bg-slate-100 rounded animate-pulse ${className ?? ""}`} />
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
    <div className="flex items-start gap-3 py-3.5 border-b border-slate-50 last:border-0">
      <div className="mt-0.5 text-slate-400">{icon}</div>
      <div className="flex-1 min-w-0">
        <p className="text-xs text-slate-400 mb-0.5">{label}</p>
        <div className="text-sm font-medium text-slate-800">{value}</div>
      </div>
    </div>
  );
}

export default function WarehouseDetailPage() {
  const params = useParams();
  const router = useRouter();
  const id = params.id as string;

  const [warehouse, setWarehouse] = useState<WarehouseDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [notFound, setNotFound] = useState(false);

  useEffect(() => {
    api
      .get<{ data: WarehouseDetail }>(`/warehouses/${id}`)
      .then((res) => setWarehouse(res.data.data))
      .catch((err) => {
        if (err?.response?.status === 404) setNotFound(true);
      })
      .finally(() => setLoading(false));
  }, [id]);

  if (!loading && notFound) {
    return (
      <>
        <Topbar title="Depo Detayı" />
        <main className="flex-1 p-6 flex flex-col items-center justify-center gap-4">
          <Warehouse className="w-12 h-12 text-slate-300" />
          <p className="text-slate-500 font-medium">Depo bulunamadı.</p>
          <button
            onClick={() => router.push("/warehouses")}
            className="text-sm text-blue-600 hover:underline flex items-center gap-1"
          >
            <ArrowLeft className="w-4 h-4" /> Depolara dön
          </button>
        </main>
      </>
    );
  }

  return (
    <>
      <Topbar title="Depo Detayı" />
      <main className="flex-1 p-6 space-y-6">
        {/* Geri butonu */}
        <button
          onClick={() => router.push("/warehouses")}
          className="flex items-center gap-1.5 text-sm text-slate-500 hover:text-slate-800 transition-colors"
        >
          <ArrowLeft className="w-4 h-4" />
          Tüm depolar
        </button>

        {/* Ana Kart */}
        <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
          {/* Kart Başlığı */}
          <div className="flex items-start justify-between px-6 py-5 border-b border-slate-100">
            {loading ? (
              <div className="space-y-2 flex-1">
                <Skeleton className="h-6 w-48" />
                <Skeleton className="h-4 w-24" />
              </div>
            ) : (
              <>
                <div className="flex items-center gap-3">
                  <div className="w-10 h-10 bg-indigo-50 rounded-xl flex items-center justify-center shrink-0">
                    <Building2 className="w-5 h-5 text-indigo-500" />
                  </div>
                  <div>
                    <h1 className="text-lg font-bold text-slate-800">
                      {warehouse!.name}
                    </h1>
                    <span className="font-mono text-xs bg-slate-100 text-slate-500 px-2 py-0.5 rounded">
                      {warehouse!.code}
                    </span>
                  </div>
                </div>
                {warehouse!.isActive ? (
                  <span className="inline-flex items-center gap-1 px-3 py-1 rounded-full text-xs font-medium bg-emerald-50 text-emerald-700 shrink-0">
                    <CheckCircle2 className="w-3.5 h-3.5" />
                    Aktif
                  </span>
                ) : (
                  <span className="inline-flex items-center gap-1 px-3 py-1 rounded-full text-xs font-medium bg-slate-100 text-slate-500 shrink-0">
                    <XCircle className="w-3.5 h-3.5" />
                    Pasif
                  </span>
                )}
              </>
            )}
          </div>

          {/* Detay Bilgiler */}
          <div className="px-6 py-2">
            {loading ? (
              <div className="space-y-4 py-4">
                {Array.from({ length: 3 }).map((_, i) => (
                  <Skeleton key={i} className="h-10" />
                ))}
              </div>
            ) : (
              <>
                <InfoRow
                  icon={<Hash className="w-4 h-4" />}
                  label="Depo Kodu"
                  value={
                    <span className="font-mono">{warehouse!.code}</span>
                  }
                />
                <InfoRow
                  icon={<MapPin className="w-4 h-4" />}
                  label="Bölge"
                  value={warehouse!.region}
                />
                <InfoRow
                  icon={<Building2 className="w-4 h-4" />}
                  label="Adres"
                  value={
                    warehouse!.address ? (
                      warehouse!.address
                    ) : (
                      <span className="text-slate-400 font-normal">
                        Belirtilmemiş
                      </span>
                    )
                  }
                />
              </>
            )}
          </div>
        </div>
      </main>
    </>
  );
}
