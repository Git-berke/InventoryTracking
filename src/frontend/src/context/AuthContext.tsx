"use client";

import {
  createContext,
  useContext,
  useState,
  useEffect,
  ReactNode,
} from "react";
import { CurrentUser, getToken, removeToken } from "@/lib/auth";
import api from "@/lib/api";

interface AuthContextValue {
  user: CurrentUser | null;
  token: string | null;
  setAuth: (token: string, user: CurrentUser) => void;
  logout: () => void;
  loading: boolean;
}

const AuthContext = createContext<AuthContextValue>({
  user: null,
  token: null,
  setAuth: () => {},
  logout: () => {},
  loading: true,
});

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<CurrentUser | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const stored = getToken();
    if (!stored) {
      setLoading(false);
      return;
    }
    setToken(stored);
    api
      .get<{ success: boolean; data: CurrentUser }>("/auth/me")
      .then((res) => setUser(res.data.data))
      .catch(() => {
        removeToken();
        setToken(null);
      })
      .finally(() => setLoading(false));
  }, []);

  function setAuth(newToken: string, newUser: CurrentUser) {
    localStorage.setItem("access_token", newToken);
    setToken(newToken);
    setUser(newUser);
  }

  function logout() {
    removeToken();
    setToken(null);
    setUser(null);
    window.location.href = "/login";
  }

  return (
    <AuthContext.Provider value={{ user, token, setAuth, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}
