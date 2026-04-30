"use client";

import { useEffect, useState } from "react";
import { ArrowRight, ArrowLeft, ArrowLeftRight, CheckCircle2 } from "lucide-react";
import toast from "react-hot-toast";
import Topbar from "@/components/Topbar";
import api from "@/lib/api";

/* ─── Tipler ──────────────────────────────────────────────── */
interface Product {
  id: string;
  code: string;
  name: string;
  unit: string;
  isActive: boolean;
}
interface Warehouse {
  id: string;
  code: string;
  name: string;
  region: string;
  isActive: boolean;
}
interface Vehicle {
  id: string;
  code: string;
  licensePlate: string;
  vehicleType: string;
  isActive: boolean;
  activeTaskName: string | null;
}
interface TaskItem {
  id: string;
  name: string;
  status: string;
}
interface PagedResult<T> {
  items: T[];
  totalCount: number;
}

/* ─── Yardımcı bileşenler ────────────────────────────────── */
function Label({ children }: { children: React.ReactNode }) {
  return (
    <label className="block text-xs font-medium text-slate-600 mb-1">
      {children}
    </label>
  );
}

function Select({
  value,
  onChange,
  disabled,
  placeholder,
  children,
}: {
  value: string;
  onChange: (v: string) => void;
  disabled?: boolean;
  placeholder: string;
  children: React.ReactNode;
}) {
  return (
    <select
      value={value}
      onChange={(e) => onChange(e.target.value)}
      disabled={disabled}
      className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm text-slate-800 bg-white focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:bg-slate-50 disabled:text-slate-400"
    >
      <option value="">{placeholder}</option>
      {children}
    </select>
  );
}

function NumberInput({
  value,
  onChange,
}: {
  value: number | "";
  onChange: (v: number | "") => void;
}) {
  return (
    <input
      type="number"
      min={1}
      value={value}
      onChange={(e) =>
        onChange(e.target.value === "" ? "" : Number(e.target.value))
      }
      placeholder="0"
      className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm text-slate-800 bg-white focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
    />
  );
}

function TextArea({
  value,
  onChange,
}: {
  value: string;
  onChange: (v: string) => void;
}) {
  return (
    <textarea
      value={value}
      onChange={(e) => onChange(e.target.value)}
      rows={2}
      maxLength={500}
      placeholder="Opsiyonel not..."
      className="w-full border border-slate-200 rounded-lg px-3 py-2.5 text-sm text-slate-800 bg-white focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent resize-none"
    />
  );
}

/* ─── Ana Sayfa ──────────────────────────────────────────── */
type Tab = "w2v" | "v2w";

export default function StockTransfersPage() {
  const [tab, setTab] = useState<Tab>("w2v");

  /* Ortak dropdown verileri */
  const [products, setProducts] = useState<Product[]>([]);
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [vehicles, setVehicles] = useState<Vehicle[]>([]);
  const [tasks, setTasks] = useState<TaskItem[]>([]);
  const [loadingDropdowns, setLoadingDropdowns] = useState(true);

  /* Depo → Araç form */
  const [w2v, setW2v] = useState({
    productId: "",
    sourceWarehouseId: "",
    destinationVehicleId: "",
    quantity: "" as number | "",
    taskId: "",
    referenceNote: "",
  });

  /* Araç → Depo form */
  const [v2w, setV2w] = useState({
    productId: "",
    sourceVehicleId: "",
    destinationWarehouseId: "",
    quantity: "" as number | "",
    referenceNote: "",
  });

  const [submitting, setSubmitting] = useState(false);
  const [lastSuccess, setLastSuccess] = useState<Tab | null>(null);

  /* Dropdown verilerini yükle */
  useEffect(() => {
    Promise.all([
      api.get<{ data: PagedResult<Product> }>("/products?page=1&pageSize=200"),
      api.get<{ data: PagedResult<Warehouse> }>("/warehouses?page=1&pageSize=200"),
      api.get<{ data: PagedResult<Vehicle> }>("/vehicles?page=1&pageSize=200"),
      api.get<{ data: PagedResult<TaskItem> }>("/tasks?page=1&pageSize=200"),
    ])
      .then(([p, w, v, t]) => {
        setProducts(p.data.data.items.filter((x) => x.isActive));
        setWarehouses(w.data.data.items.filter((x) => x.isActive));
        setVehicles(v.data.data.items.filter((x) => x.isActive));
        setTasks(t.data.data.items.filter((x) => x.status === "InProgress"));
      })
      .finally(() => setLoadingDropdowns(false));
  }, []);

  /* Depo → Araç gönder */
  async function submitW2V(e: React.FormEvent) {
    e.preventDefault();
    if (!w2v.productId || !w2v.sourceWarehouseId || !w2v.destinationVehicleId || !w2v.quantity || !w2v.taskId) {
      toast.error("Lütfen tüm zorunlu alanları doldurun.");
      return;
    }
    setSubmitting(true);
    try {
      await api.post("/stock-transfers/warehouse-to-vehicle", {
        productId: w2v.productId,
        sourceWarehouseId: w2v.sourceWarehouseId,
        destinationVehicleId: w2v.destinationVehicleId,
        quantity: w2v.quantity,
        taskId: w2v.taskId,
        referenceNote: w2v.referenceNote || null,
      });
      toast.success("Transfer başarıyla tamamlandı.");
      setLastSuccess("w2v");
      setW2v({ productId: "", sourceWarehouseId: "", destinationVehicleId: "", quantity: "", taskId: "", referenceNote: "" });
    } catch (err: unknown) {
      const msg =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
        "Transfer sırasında bir hata oluştu.";
      toast.error(msg);
    } finally {
      setSubmitting(false);
    }
  }

  /* Araç → Depo gönder */
  async function submitV2W(e: React.FormEvent) {
    e.preventDefault();
    if (!v2w.productId || !v2w.sourceVehicleId || !v2w.destinationWarehouseId || !v2w.quantity) {
      toast.error("Lütfen tüm zorunlu alanları doldurun.");
      return;
    }
    setSubmitting(true);
    try {
      await api.post("/stock-transfers/vehicle-to-warehouse", {
        productId: v2w.productId,
        sourceVehicleId: v2w.sourceVehicleId,
        destinationWarehouseId: v2w.destinationWarehouseId,
        quantity: v2w.quantity,
        referenceNote: v2w.referenceNote || null,
      });
      toast.success("İade başarıyla tamamlandı.");
      setLastSuccess("v2w");
      setV2w({ productId: "", sourceVehicleId: "", destinationWarehouseId: "", quantity: "", referenceNote: "" });
    } catch (err: unknown) {
      const msg =
        (err as { response?: { data?: { message?: string } } })?.response?.data?.message ??
        "İade sırasında bir hata oluştu.";
      toast.error(msg);
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <>
      <Topbar title="Stok Transferi" />
      <main className="flex-1 p-6">
        <div className="max-w-2xl mx-auto space-y-4">
          {/* Sekme butonları */}
          <div className="flex gap-2 bg-slate-100 p-1 rounded-xl">
            <button
              onClick={() => { setTab("w2v"); setLastSuccess(null); }}
              className={`flex-1 flex items-center justify-center gap-2 py-2.5 rounded-lg text-sm font-medium transition-all cursor-pointer ${
                tab === "w2v"
                  ? "bg-white text-slate-800 shadow-sm"
                  : "text-slate-500 hover:text-slate-700"
              }`}
            >
              <ArrowRight className="w-4 h-4" />
              Depo → Araç
            </button>
            <button
              onClick={() => { setTab("v2w"); setLastSuccess(null); }}
              className={`flex-1 flex items-center justify-center gap-2 py-2.5 rounded-lg text-sm font-medium transition-all cursor-pointer ${
                tab === "v2w"
                  ? "bg-white text-slate-800 shadow-sm"
                  : "text-slate-500 hover:text-slate-700"
              }`}
            >
              <ArrowLeft className="w-4 h-4" />
              Araç → Depo
            </button>
          </div>

          {/* Kart */}
          <div className="bg-white rounded-xl border border-slate-200 shadow-sm">
            {/* Başlık */}
            <div className="flex items-center gap-2.5 px-6 py-4 border-b border-slate-100">
              <div className="w-8 h-8 bg-blue-50 rounded-lg flex items-center justify-center">
                <ArrowLeftRight className="w-4 h-4 text-blue-600" />
              </div>
              <div>
                <h2 className="text-sm font-semibold text-slate-800">
                  {tab === "w2v" ? "Depodan Araca Transfer" : "Araçtan Depoya İade"}
                </h2>
                <p className="text-xs text-slate-400">
                  {tab === "w2v"
                    ? "Depodaki ürünü bir göreve atanmış araca yükle"
                    : "Araçtaki ürünü depoya geri iade et"}
                </p>
              </div>
            </div>

            {/* Başarı banner */}
            {lastSuccess === tab && (
              <div className="mx-6 mt-4 flex items-center gap-2 p-3 bg-emerald-50 border border-emerald-200 rounded-lg text-sm text-emerald-700">
                <CheckCircle2 className="w-4 h-4 shrink-0" />
                Transfer başarıyla tamamlandı. Yeni bir transfer yapabilirsiniz.
              </div>
            )}

            {/* ── Depo → Araç Formu ── */}
            {tab === "w2v" && (
              <form onSubmit={submitW2V} className="p-6 space-y-4">
                <div>
                  <Label>Ürün *</Label>
                  <Select
                    value={w2v.productId}
                    onChange={(v) => setW2v((s) => ({ ...s, productId: v }))}
                    disabled={loadingDropdowns}
                    placeholder="Ürün seçin..."
                  >
                    {products.map((p) => (
                      <option key={p.id} value={p.id}>
                        {p.name} ({p.code}) — {p.unit}
                      </option>
                    ))}
                  </Select>
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <Label>Kaynak Depo *</Label>
                    <Select
                      value={w2v.sourceWarehouseId}
                      onChange={(v) => setW2v((s) => ({ ...s, sourceWarehouseId: v }))}
                      disabled={loadingDropdowns}
                      placeholder="Depo seçin..."
                    >
                      {warehouses.map((w) => (
                        <option key={w.id} value={w.id}>
                          {w.name} — {w.region}
                        </option>
                      ))}
                    </Select>
                  </div>
                  <div>
                    <Label>Hedef Araç *</Label>
                    <Select
                      value={w2v.destinationVehicleId}
                      onChange={(v) => setW2v((s) => ({ ...s, destinationVehicleId: v }))}
                      disabled={loadingDropdowns}
                      placeholder="Araç seçin..."
                    >
                      {vehicles.map((v) => (
                        <option key={v.id} value={v.id}>
                          {v.licensePlate} ({v.code})
                          {v.activeTaskName ? ` · ${v.activeTaskName}` : ""}
                        </option>
                      ))}
                    </Select>
                  </div>
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <Label>Miktar *</Label>
                    <NumberInput
                      value={w2v.quantity}
                      onChange={(v) => setW2v((s) => ({ ...s, quantity: v }))}
                    />
                  </div>
                  <div>
                    <Label>Görev *</Label>
                    <Select
                      value={w2v.taskId}
                      onChange={(v) => setW2v((s) => ({ ...s, taskId: v }))}
                      disabled={loadingDropdowns}
                      placeholder="Görev seçin..."
                    >
                      {tasks.map((t) => (
                        <option key={t.id} value={t.id}>
                          {t.name}
                        </option>
                      ))}
                    </Select>
                  </div>
                </div>

                <div>
                  <Label>Referans Notu</Label>
                  <TextArea
                    value={w2v.referenceNote}
                    onChange={(v) => setW2v((s) => ({ ...s, referenceNote: v }))}
                  />
                </div>

                <button
                  type="submit"
                  disabled={submitting || loadingDropdowns}
                  className="w-full flex items-center justify-center gap-2 bg-blue-600 hover:bg-blue-700 disabled:bg-blue-300 text-white text-sm font-semibold py-2.5 rounded-lg transition-colors cursor-pointer disabled:cursor-not-allowed"
                >
                  {submitting ? (
                    <span className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />
                  ) : (
                    <ArrowRight className="w-4 h-4" />
                  )}
                  {submitting ? "Aktarılıyor..." : "Transferi Başlat"}
                </button>
              </form>
            )}

            {/* ── Araç → Depo Formu ── */}
            {tab === "v2w" && (
              <form onSubmit={submitV2W} className="p-6 space-y-4">
                <div>
                  <Label>Ürün *</Label>
                  <Select
                    value={v2w.productId}
                    onChange={(v) => setV2w((s) => ({ ...s, productId: v }))}
                    disabled={loadingDropdowns}
                    placeholder="Ürün seçin..."
                  >
                    {products.map((p) => (
                      <option key={p.id} value={p.id}>
                        {p.name} ({p.code}) — {p.unit}
                      </option>
                    ))}
                  </Select>
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <Label>Kaynak Araç *</Label>
                    <Select
                      value={v2w.sourceVehicleId}
                      onChange={(v) => setV2w((s) => ({ ...s, sourceVehicleId: v }))}
                      disabled={loadingDropdowns}
                      placeholder="Araç seçin..."
                    >
                      {vehicles.map((v) => (
                        <option key={v.id} value={v.id}>
                          {v.licensePlate} ({v.code})
                          {v.activeTaskName ? ` · ${v.activeTaskName}` : ""}
                        </option>
                      ))}
                    </Select>
                  </div>
                  <div>
                    <Label>Hedef Depo *</Label>
                    <Select
                      value={v2w.destinationWarehouseId}
                      onChange={(v) => setV2w((s) => ({ ...s, destinationWarehouseId: v }))}
                      disabled={loadingDropdowns}
                      placeholder="Depo seçin..."
                    >
                      {warehouses.map((w) => (
                        <option key={w.id} value={w.id}>
                          {w.name} — {w.region}
                        </option>
                      ))}
                    </Select>
                  </div>
                </div>

                <div>
                  <Label>Miktar *</Label>
                  <NumberInput
                    value={v2w.quantity}
                    onChange={(v) => setV2w((s) => ({ ...s, quantity: v }))}
                  />
                </div>

                <div>
                  <Label>Referans Notu</Label>
                  <TextArea
                    value={v2w.referenceNote}
                    onChange={(v) => setV2w((s) => ({ ...s, referenceNote: v }))}
                  />
                </div>

                <button
                  type="submit"
                  disabled={submitting || loadingDropdowns}
                  className="w-full flex items-center justify-center gap-2 bg-blue-600 hover:bg-blue-700 disabled:bg-blue-300 text-white text-sm font-semibold py-2.5 rounded-lg transition-colors cursor-pointer disabled:cursor-not-allowed"
                >
                  {submitting ? (
                    <span className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />
                  ) : (
                    <ArrowLeft className="w-4 h-4" />
                  )}
                  {submitting ? "İade ediliyor..." : "İadeyi Başlat"}
                </button>
              </form>
            )}
          </div>

          {/* Bilgi kutusu */}
          <div className="rounded-xl border border-amber-200 bg-amber-50 px-5 py-4 text-xs text-amber-700 space-y-1">
            <p className="font-semibold">Dikkat</p>
            <ul className="space-y-0.5 list-disc list-inside">
              <li>Depo → Araç transferi için aktif bir görev seçilmesi zorunludur.</li>
              <li>Transferin tamamlanması stok kayıtlarını ve hareket geçmişini otomatik günceller.</li>
              <li>Yeterli stok yoksa transfer reddedilir.</li>
            </ul>
          </div>
        </div>
      </main>
    </>
  );
}
