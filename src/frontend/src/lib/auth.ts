import api from "./api";

export interface LoginRequest {
  email: string;
  password: string;
}

export interface CurrentUser {
  id: string;
  email: string;
  userName: string;
  fullName: string;
  roles: string[];
  permissions: string[];
}

export interface LoginResponse {
  accessToken: string;
  tokenType: string;
  expiresAtUtc: string;
  currentUser: CurrentUser;
  user: CurrentUser;
}

export async function login(data: LoginRequest): Promise<LoginResponse> {
  const res = await api.post<{ success: boolean; data: LoginResponse }>(
    "/auth/login",
    data
  );
  return res.data.data;
}

export function saveToken(token: string) {
  localStorage.setItem("access_token", token);
}

export function getToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem("access_token");
}

export function removeToken() {
  localStorage.removeItem("access_token");
}

export function isAuthenticated(): boolean {
  return !!getToken();
}
