"use client";

import * as React from "react";

import type { UserSummary } from "@/features/users/types";

interface UserSessionContextValue {
  user: UserSummary | null;
  selectUser: (user: UserSummary) => void;
  clearUser: () => void;
}

const UserSessionContext = React.createContext<UserSessionContextValue | undefined>(undefined);

const STORAGE_KEY = "sajt-cookbook:selected-user";

export function UserSessionProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = React.useState<UserSummary | null>(null);

  React.useEffect(() => {
    if (typeof window === "undefined") {
      return;
    }

    try {
      const stored = window.localStorage.getItem(STORAGE_KEY);
      if (!stored) {
        return;
      }

      const parsed = JSON.parse(stored) as UserSummary;
      if (parsed?.id && parsed?.name) {
        setUser(parsed);
      }
    } catch (error) {
      console.warn("Failed to load stored user selection", error);
      window.localStorage.removeItem(STORAGE_KEY);
    }
  }, []);

  const selectUser = React.useCallback((nextUser: UserSummary) => {
    setUser(nextUser);
    if (typeof window !== "undefined") {
      window.localStorage.setItem(STORAGE_KEY, JSON.stringify(nextUser));
    }
  }, []);

  const clearUser = React.useCallback(() => {
    setUser(null);
    if (typeof window !== "undefined") {
      window.localStorage.removeItem(STORAGE_KEY);
    }
  }, []);

  const value = React.useMemo(
    () => ({ user, selectUser, clearUser }),
    [user, selectUser, clearUser]
  );

  return <UserSessionContext.Provider value={value}>{children}</UserSessionContext.Provider>;
}

export function useUserSession(): UserSessionContextValue {
  const context = React.useContext(UserSessionContext);
  if (!context) {
    throw new Error("useUserSession must be used within a UserSessionProvider");
  }

  return context;
}
