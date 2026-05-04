const DEFAULT_API_URL = "http://localhost:5227/api/v1";
const HUB_ROUTE = "/hubs/inventory-events";

export function getInventoryHubUrl() {
  const apiUrl = process.env.NEXT_PUBLIC_API_URL ?? DEFAULT_API_URL;

  try {
    const url = new URL(apiUrl);
    return `${url.origin}${HUB_ROUTE}`;
  } catch {
    if (typeof window !== "undefined") {
      return `${window.location.origin}${HUB_ROUTE}`;
    }

    return `http://localhost:5227${HUB_ROUTE}`;
  }
}

export { HUB_ROUTE };
