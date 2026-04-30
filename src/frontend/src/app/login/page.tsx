"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import toast from "react-hot-toast";
import { Eye, EyeOff, Package, Loader2 } from "lucide-react";
import { login } from "@/lib/auth";
import { useAuth } from "@/context/AuthContext";

interface LoginForm {
  email: string;
  password: string;
}

export default function LoginPage() {
  const router = useRouter();
  const { setAuth } = useAuth();
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginForm>();

  async function onSubmit(data: LoginForm) {
    setLoading(true);
    try {
      const result = await login(data);
      const currentUser = result.currentUser ?? result.user;
      setAuth(result.accessToken, currentUser);
      toast.success(`Hoş geldiniz, ${currentUser.fullName}!`);
      router.push("/");
    } catch (err: unknown) {
      const status = (err as { response?: { status?: number } })?.response?.status;
      if (status === 401) {
        toast.error("E-posta veya şifre hatalı.");
      } else {
        toast.error("Sunucuya ulaşılamadı, lütfen tekrar deneyin.");
      }
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        {/* Logo & Başlık */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 bg-blue-600 rounded-2xl mb-4 shadow-lg">
            <Package className="w-8 h-8 text-white" />
          </div>
          <h1 className="text-2xl font-bold text-white">PTN Inventory</h1>
          <p className="text-slate-400 text-sm mt-1">
            Envanter Takip Sistemi
          </p>
        </div>

        {/* Kart */}
        <div className="bg-white rounded-2xl shadow-2xl p-8">
          <h2 className="text-xl font-semibold text-slate-800 mb-6">
            Hesabınıza giriş yapın
          </h2>

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
            {/* E-posta */}
            <div>
              <label className="block text-sm font-medium text-slate-700 mb-1.5">
                E-posta
              </label>
              <input
                type="email"
                autoComplete="email"
                placeholder="ornek@ptn.local"
                {...register("email", {
                  required: "E-posta zorunludur.",
                  pattern: {
                    value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                    message: "Geçerli bir e-posta giriniz.",
                  },
                })}
                className={`w-full px-4 py-2.5 rounded-lg border text-sm outline-none transition-all
                  ${
                    errors.email
                      ? "border-red-400 bg-red-50 focus:ring-2 focus:ring-red-200"
                      : "border-slate-300 bg-white focus:border-blue-500 focus:ring-2 focus:ring-blue-100"
                  }`}
              />
              {errors.email && (
                <p className="text-red-500 text-xs mt-1">{errors.email.message}</p>
              )}
            </div>

            {/* Şifre */}
            <div>
              <label className="block text-sm font-medium text-slate-700 mb-1.5">
                Şifre
              </label>
              <div className="relative">
                <input
                  type={showPassword ? "text" : "password"}
                  autoComplete="current-password"
                  placeholder="••••••••"
                  {...register("password", {
                    required: "Şifre zorunludur.",
                    minLength: {
                      value: 4,
                      message: "Şifre en az 4 karakter olmalıdır.",
                    },
                  })}
                  className={`w-full px-4 py-2.5 pr-11 rounded-lg border text-sm outline-none transition-all
                    ${
                      errors.password
                        ? "border-red-400 bg-red-50 focus:ring-2 focus:ring-red-200"
                        : "border-slate-300 bg-white focus:border-blue-500 focus:ring-2 focus:ring-blue-100"
                    }`}
                />
                <button
                  type="button"
                  onClick={() => setShowPassword((v) => !v)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400 hover:text-slate-600 transition-colors"
                  tabIndex={-1}
                >
                  {showPassword ? (
                    <EyeOff className="w-4 h-4" />
                  ) : (
                    <Eye className="w-4 h-4" />
                  )}
                </button>
              </div>
              {errors.password && (
                <p className="text-red-500 text-xs mt-1">{errors.password.message}</p>
              )}
            </div>

            {/* Giriş Butonu */}
            <button
              type="submit"
              disabled={loading}
              className="w-full bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400 text-white font-semibold py-2.5 rounded-lg text-sm transition-colors flex items-center justify-center gap-2 shadow-sm cursor-pointer"
            >
              {loading ? (
                <>
                  <Loader2 className="w-4 h-4 animate-spin" />
                  Giriş yapılıyor...
                </>
              ) : (
                "Giriş Yap"
              )}
            </button>
          </form>

          {/* Test Hesapları */}
          <div className="mt-6 pt-5 border-t border-slate-100">
            <p className="text-xs text-slate-400 font-medium mb-3 uppercase tracking-wide">
              Test Hesapları
            </p>
            <div className="space-y-2">
              {[
                { label: "Admin", email: "admin@ptn.local", password: "Admin123!" },
                { label: "Depo Operatörü", email: "warehouse@ptn.local", password: "Warehouse123!" },
                { label: "Görev Yöneticisi", email: "taskmanager@ptn.local", password: "Task123!" },
              ].map(({ label, email, password }) => (
                <TestAccountButton
                  key={email}
                  label={label}
                  email={email}
                  password={password}
                  onSubmit={onSubmit}
                />
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

function TestAccountButton({
  label,
  email,
  password,
  onSubmit,
}: {
  label: string;
  email: string;
  password: string;
  onSubmit: (data: LoginForm) => Promise<void>;
}) {
  const [busy, setBusy] = useState(false);
  async function handle() {
    setBusy(true);
    await onSubmit({ email, password });
    setBusy(false);
  }

  return (
    <button
      type="button"
      onClick={handle}
      disabled={busy}
      className="w-full flex items-center justify-between px-3 py-2 rounded-lg bg-slate-50 hover:bg-slate-100 border border-slate-200 transition-colors group cursor-pointer"
    >
      <div className="text-left">
        <p className="text-xs font-semibold text-slate-700">{label}</p>
        <p className="text-xs text-slate-400">{email}</p>
      </div>
      <span className="text-xs text-blue-500 group-hover:text-blue-700 font-medium transition-colors">
        {busy ? "Giriş..." : "Kullan →"}
      </span>
    </button>
  );
}
