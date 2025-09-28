import { createContext } from "react";
import type { User } from "../types/User";

type AuthContextType = {
  user: User | null;
  loading: boolean;
  setUser: (user: User | null) => void;
};

export const AuthContext = createContext<AuthContextType | undefined>(undefined);