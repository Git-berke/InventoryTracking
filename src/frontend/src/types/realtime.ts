export interface InventoryRealtimeEvent {
  eventType: string;
  message: string;
  occurredAtUtc: string;
  context: Record<string, string | null>;
}
