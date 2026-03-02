import { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';
import type { UserProfile, AuthResult } from '../services/auth';
import * as authService from '../services/auth';
import { getToken, setToken, clearToken } from '../services/api';

interface AuthContextType {
  user: UserProfile | null;
  householdId: string | null;
  activeHouseholdId: string | null;
  setActiveHouseholdId: (id: string) => void;
  loading: boolean;
  login: (data: { email: string; password: string }) => Promise<void>;
  register: (data: { email: string; password: string; displayName: string }) => Promise<void>;
  logout: () => void;
}

/** Safe default — used when no AuthProvider is in the tree (e.g. isolated tests). */
const AuthContext = createContext<AuthContextType>({
  user: null,
  householdId: null,
  activeHouseholdId: null,
  setActiveHouseholdId: () => {},
  loading: false,
  login: async () => {},
  register: async () => {},
  logout: () => {},
});

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<UserProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [activeHouseholdId, setActiveHouseholdId] = useState<string | null>(null);

  useEffect(() => {
    const token = getToken();
    if (!token) {
      setLoading(false);
      return;
    }
    authService
      .getMe()
      .then((profile) => {
        setUser(profile);
        setActiveHouseholdId((prev) => prev ?? profile.householdId ?? null);
      })
      .catch(() => clearToken())
      .finally(() => setLoading(false));
  }, []);

  const login = async (data: { email: string; password: string }) => {
    const result: AuthResult = await authService.login(data);
    setToken(result.token);
    const profile = await authService.getMe();
    setUser(profile);
    setActiveHouseholdId(profile.householdId ?? null);
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
    setActiveHouseholdId(profile.householdId ?? null);
  };

  const logout = () => {
    clearToken();
    setUser(null);
    setActiveHouseholdId(null);
  };

  const effectiveHouseholdId = activeHouseholdId ?? user?.householdId ?? null;

  return (
    /* React 19: render context directly instead of <AuthContext.Provider> */
    <AuthContext value={{
      user,
      householdId: effectiveHouseholdId,
      activeHouseholdId: effectiveHouseholdId,
      setActiveHouseholdId,
      loading,
      login,
      register,
      logout,
    }}>
      {children}
    </AuthContext>
  );
}

// eslint-disable-next-line react-refresh/only-export-components
export function useAuth(): AuthContextType {
  return useContext(AuthContext);
}
