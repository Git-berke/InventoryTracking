"use client";

import { useEffect, useState } from "react";
import {
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from "@microsoft/signalr";
import { getToken } from "@/lib/auth";
import { getInventoryHubUrl } from "@/lib/realtime";
import type { InventoryRealtimeEvent } from "@/types/realtime";

const RECEIVE_METHOD = "inventory-event";

interface UseInventoryRealtimeOptions {
  enabled?: boolean;
  onEvent?: (event: InventoryRealtimeEvent) => void | Promise<void>;
}

export function useInventoryRealtime({
  enabled = true,
  onEvent,
}: UseInventoryRealtimeOptions) {
  const [status, setStatus] = useState<HubConnectionState | "error">(
    HubConnectionState.Disconnected
  );

  useEffect(() => {
    if (!enabled) {
      setStatus(HubConnectionState.Disconnected);
      return;
    }

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
      void onEvent?.(event);
    });

    connection.onreconnecting(() => {
      setStatus(HubConnectionState.Reconnecting);
    });

    connection.onreconnected(() => {
      setStatus(HubConnectionState.Connected);
    });

    connection.onclose((error) => {
      setStatus(error ? "error" : HubConnectionState.Disconnected);
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
      } catch {
        if (!isMounted) {
          return;
        }

        setStatus("error");
      }
    }

    void startConnection();

    return () => {
      isMounted = false;
      connection.off(RECEIVE_METHOD);
      void connection.stop();
    };
  }, [enabled, onEvent]);

  return status;
}
