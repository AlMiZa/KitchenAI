import { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';
import type { UserProfile, AuthResult } from '../services/auth';
import * as authService from '../services/auth';
import { getToken, setToken, clearToken } from '../services/api';

interface AuthContextType {
  user: UserProfile | null;
  householdId: string | null;
  loading: boolean;
  login: (data: { email: string; password: string }) => Promise<void>;
  register: (data: { email: string; password: string; displayName: string }) => Promise<void>;
  logout: () => void;
}

/** Safe default — used when no AuthProvider is in the tree (e.g. isolated tests). */
const AuthContext = createContext<AuthContextType>({
  user: null,
  householdId: null,
  loading: false,
  login: async () => {},
  register: async () => {},
  logout: () => {},
});

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<UserProfile | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = getToken();
    if (!token) {
      setLoading(false);
      return;
    }
    authService
      .getMe()
      .then((profile) => setUser(profile))
      .catch(() => clearToken())
      .finally(() => setLoading(false));
  }, []);

  const login = async (data: { email: string; password: string }) => {
    const result: AuthResult = await authService.login(data);
    setToken(result.token);
    const profile = await authService.getMe();
    setUser(profile);
  };

  const register = async (data: {
    email: string;
    password: string;
    displayName: string;
  }) => {
    const result: AuthResult = await authService.register(data);
    setToken(result.token);
    const profile = await authService.getMe();
    setUser(profile);
  };

  const logout = () => {
    clearToken();
    setUser(null);
  };

  return (
    /* React 19: render context directly instead of <AuthContext.Provider> */
    <AuthContext value={{ user, householdId: user?.householdId ?? null, loading, login, register, logout }}>
      {children}
    </AuthContext>
  );
}

export function useAuth(): AuthContextType {
  return useContext(AuthContext);
}
