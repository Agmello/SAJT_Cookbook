"use client";

import * as React from "react";
import { Loader2 } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useUserSession } from "@/components/providers/user-session-provider";
import { useUsersQuery } from "@/features/users/api/use-users-query";
import type { UserSummary } from "@/features/users/types";

interface UserGateProps {
  children: React.ReactNode;
}

export function UserGate({ children }: UserGateProps) {
  const { user, selectUser } = useUserSession();
  const { data, isLoading, isError, refetch } = useUsersQuery({ enabled: !user });

  if (user) {
    return <>{children}</>;
  }

  if (isLoading) {
    return (
      <div className="flex min-h-screen flex-col items-center justify-center bg-background">
        <div className="flex items-center gap-2 text-muted-foreground">
          <Loader2 className="h-5 w-5 animate-spin" aria-hidden="true" />
          <span>Loading users...</span>
        </div>
      </div>
    );
  }

  if (isError) {
    return (
      <div className="flex min-h-screen flex-col items-center justify-center bg-background px-4">
        <Card className="max-w-md">
          <CardHeader>
            <CardTitle>Select your profile</CardTitle>
            <CardDescription>We ran into a problem while loading the available users.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <p className="text-sm text-muted-foreground">
              Please check that the API is running and try again.
            </p>
            <Button onClick={() => refetch()}>Retry</Button>
          </CardContent>
        </Card>
      </div>
    );
  }

  const users = data ?? [];

  return (
    <div className="flex min-h-screen flex-col items-center justify-center bg-background px-4">
      <Card className="w-full max-w-lg">
        <CardHeader>
          <CardTitle>Select your profile</CardTitle>
          <CardDescription>Choose the user you want to browse and create recipes as.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          {users.length === 0 ? (
            <p className="text-sm text-muted-foreground">
              No users are available yet. Create one through the API (e.g. Swagger&apos;s <code>POST /api/users</code>) and
              refresh this page.
            </p>
          ) : (
            <div className="grid gap-3">
              {users.map((candidate: UserSummary) => (
                <Button
                  key={candidate.id}
                  variant="outline"
                  className="justify-between"
                  onClick={() => selectUser(candidate)}
                >
                  <span>{candidate.name}</span>
                  <span className="text-xs text-muted-foreground">{candidate.id}</span>
                </Button>
              ))}
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
