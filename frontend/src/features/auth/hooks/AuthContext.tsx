import { createContext, useCallback, useContext, useEffect, useMemo, useState } from "react";
import { clearTokens, getAccessToken, saveAccessToken, saveRefreshToken } from "../../../lib/auth-storage";
import { getMe, login as loginRequest, register as registerRequest } from "../services/authService";
import type { LoginRequest, RegisterRequest, User } from "../types/authTypes";

type AuthContextValue = {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (request: LoginRequest) => Promise<void>;
  register: (request: RegisterRequest) => Promise<void>;
  logout: () => void;
};

const AuthContext = createContext<AuthContextValue | null>(null);

type AuthProviderProps = {
  children: React.ReactNode;
};

export function AuthProvider({ children }: AuthProviderProps) {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const logout = useCallback(() => {
    clearTokens();
    setUser(null);
  }, []);

  useEffect(() => {
    let isMounted = true;

    async function loadUser() {
      const accessToken = getAccessToken();
      if (!accessToken) {
        if (isMounted) {
          setIsLoading(false);
        }
        return;
      }

      try {
        const currentUser = await getMe();
        if (isMounted) {
          setUser(currentUser);
        }
      } catch {
        if (isMounted) {
          logout();
        }
      } finally {
        if (isMounted) {
          setIsLoading(false);
        }
      }
    }

    void loadUser();

    return () => {
      isMounted = false;
    };
  }, [logout]);

  const login = useCallback(async (request: LoginRequest) => {
    const auth = await loginRequest(request);
    saveAccessToken(auth.accessToken);
    saveRefreshToken(auth.refreshToken);
    setUser(auth.user);
  }, []);

  const register = useCallback(async (request: RegisterRequest) => {
    const auth = await registerRequest(request);
    saveAccessToken(auth.accessToken);
    saveRefreshToken(auth.refreshToken);
    setUser(auth.user);
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      isAuthenticated: user !== null,
      isLoading,
      login,
      register,
      logout,
    }),
    [isLoading, login, logout, register, user],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === null) {
    throw new Error("useAuth must be used within AuthProvider");
  }

  return context;
}
