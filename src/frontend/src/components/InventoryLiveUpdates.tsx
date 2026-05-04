"use client";

import { useEffect, useState } from "react";
import {
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from "@microsoft/signalr";
import { Activity, BellRing, Cable, Radio, RefreshCw } from "lucide-react";
import toast from "react-hot-toast";
import { getToken } from "@/lib/auth";
import { getInventoryHubUrl } from "@/lib/realtime";
import type { InventoryRealtimeEvent } from "@/types/realtime";

const RECEIVE_METHOD = "inventory-event";
const MAX_EVENTS = 6;

type ConnectionTone = "success" | "warning" | "muted" | "danger";

const STATUS_STYLES: Record<
  ConnectionTone,
  {
    dot: string;
    badge: string;
    label: string;
    Icon: typeof Radio;
  }
> = {
  success: {
    dot: "bg-emerald-500",
    badge: "border-emerald-200 bg-emerald-50 text-emerald-700",
    label: "Bağlı",
    Icon: Radio,
  },
  warning: {
    dot: "bg-amber-500",
    badge: "border-amber-200 bg-amber-50 text-amber-700",
    label: "Yeniden bağlanıyor",
    Icon: RefreshCw,
  },
  muted: {
    dot: "bg-slate-400",
    badge: "border-slate-200 bg-slate-100 text-slate-600",
    label: "Bağlantı bekleniyor",
    Icon: Cable,
  },
  danger: {
    dot: "bg-rose-500",
    badge: "border-rose-200 bg-rose-50 text-rose-700",
    label: "Bağlantı hatası",
    Icon: Activity,
  },
};

function getConnectionTone(status: HubConnectionState | "error"): ConnectionTone {
  switch (status) {
    case HubConnectionState.Connected:
      return "success";
    case HubConnectionState.Reconnecting:
      return "warning";
    case "error":
      return "danger";
    default:
      return "muted";
  }
}

function formatEventType(value: string) {
  return value
    .split(".")
    .map((part) => part.charAt(0).toUpperCase() + part.slice(1))
    .join(" / ");
}

function buildContextSummary(context: Record<string, string | null>) {
  const quantity = context.quantity ? `${context.quantity} adet` : null;
  const status = context.status;
  const taskName = context.taskName;
  const transactionType = context.transactionType;

  return [quantity, status, taskName, transactionType].filter(Boolean).join(" • ");
}

interface InventoryLiveUpdatesProps {
  onEvent?: () => void | Promise<void>;
}

export default function InventoryLiveUpdates({
  onEvent,
}: InventoryLiveUpdatesProps) {
  const [events, setEvents] = useState<InventoryRealtimeEvent[]>([]);
  const [status, setStatus] = useState<HubConnectionState | "error">(
    HubConnectionState.Disconnected
  );
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  useEffect(() => {
    const token = getToken();

    if (!token) {
      setStatus(HubConnectionState.Disconnected);
      return;
    }

    const connection = new HubConnectionBuilder()
      .withUrl(getInventoryHubUrl(), {
        accessTokenFactory: () => getToken() ?? "",
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000])
      .configureLogging(LogLevel.Warning)
      .build();

    connection.on(RECEIVE_METHOD, (event: InventoryRealtimeEvent) => {
      setEvents((current) => [event, ...current].slice(0, MAX_EVENTS));
      setErrorMessage(null);
      toast.success(event.message, {
        id: `${event.eventType}-${event.occurredAtUtc}`,
      });
      void onEvent?.();
    });

    connection.onreconnecting(() => {
      setStatus(HubConnectionState.Reconnecting);
    });

    connection.onreconnected(() => {
      setStatus(HubConnectionState.Connected);
      setErrorMessage(null);
    });

    connection.onclose((error) => {
      setStatus(error ? "error" : HubConnectionState.Disconnected);
      setErrorMessage(error?.message ?? null);
    });

    let isMounted = true;

    async function startConnection() {
      try {
        await connection.start();
        if (!isMounted) {
          await connection.stop();
          return;
        }

        setStatus(HubConnectionState.Connected);
        setErrorMessage(null);
      } catch (error) {
        if (!isMounted) {
          return;
        }

        setStatus("error");
        setErrorMessage(
          error instanceof Error ? error.message : "SignalR bağlantısı kurulamadı."
        );
      }
    }

    void startConnection();

    return () => {
      isMounted = false;
      connection.off(RECEIVE_METHOD);
      void connection.stop();
    };
  }, [onEvent]);

  const statusTone = getConnectionTone(status);
  const statusMeta = STATUS_STYLES[statusTone];
  const StatusIcon = statusMeta.Icon;

  return (
    <section className="rounded-xl border border-slate-200 bg-white shadow-sm">
      <div className="flex items-center justify-between gap-3 border-b border-slate-100 px-6 py-4">
        <div className="flex items-center gap-2">
          <BellRing className="h-4 w-4 text-slate-400" />
          <div>
            <h2 className="text-sm font-semibold text-slate-800">
              Canlı Güncellemeler
            </h2>
            <p className="text-xs text-slate-400">
              SignalR ile anlık stok ve görev olayları
            </p>
          </div>
        </div>

        <div
          className={`inline-flex items-center gap-2 rounded-full border px-3 py-1 text-xs font-medium ${statusMeta.badge}`}
        >
          <span className={`h-2 w-2 rounded-full ${statusMeta.dot}`} />
          <StatusIcon
            className={`h-3.5 w-3.5 ${
              statusTone === "warning" ? "animate-spin" : ""
            }`}
          />
          {statusMeta.label}
        </div>
      </div>

      <div className="space-y-3 px-6 py-4">
        {errorMessage ? (
          <div className="rounded-lg border border-rose-200 bg-rose-50 px-3 py-2 text-xs text-rose-700">
            {errorMessage}
          </div>
        ) : null}

        {events.length === 0 ? (
          <div className="rounded-lg border border-dashed border-slate-200 bg-slate-50 px-4 py-8 text-center">
            <p className="text-sm font-medium text-slate-600">
              Henüz canlı olay alınmadı
            </p>
            <p className="mt-1 text-xs text-slate-400">
              Stok transferi veya görev güncellemesi olduğunda burada görünecek.
            </p>
          </div>
        ) : (
          <div className="space-y-3">
            {events.map((event, index) => {
              const summary = buildContextSummary(event.context);

              return (
                <article
                  key={`${event.eventType}-${event.occurredAtUtc}-${index}`}
                  className="rounded-xl border border-slate-200 bg-slate-50/70 px-4 py-3"
                >
                  <div className="flex items-start justify-between gap-3">
                    <div>
                      <p className="text-xs font-semibold uppercase tracking-[0.16em] text-slate-500">
                        {formatEventType(event.eventType)}
                      </p>
                      <p className="mt-1 text-sm font-medium text-slate-800">
                        {event.message}
                      </p>
                      {summary ? (
                        <p className="mt-1 text-xs text-slate-500">{summary}</p>
                      ) : null}
                    </div>
                    <span className="whitespace-nowrap text-xs text-slate-400">
                      {new Date(event.occurredAtUtc).toLocaleTimeString("tr-TR", {
                        hour: "2-digit",
                        minute: "2-digit",
                        second: "2-digit",
                      })}
                    </span>
                  </div>
                </article>
              );
            })}
          </div>
        )}
      </div>
    </section>
  );
}
