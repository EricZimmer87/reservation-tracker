import { useEffect, useState, type ReactNode } from "react";
import { AuthContext } from "./AuthContext";
import type { User } from "../types/User";

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetch("http://localhost:8080/api/users/me", { credentials: "include" })
      .then(async res => {
        if (res.status === 401) {
          return null;
        }
        if (!res.ok) {
          const text = await res.text();
          throw new Error(`API error ${res.status}: ${text}`);
        }
        return res.json();
      })
      .then((data: User | null) => {
        setUser(data);
        setLoading(false);
      })
      .catch(err => {
        console.debug("Auth check failed:", err.message);
        setLoading(false);
      });
  }, []);

  return (
    <AuthContext.Provider value={{ user, loading, setUser }}>
      {children}
    </AuthContext.Provider>
  );
}