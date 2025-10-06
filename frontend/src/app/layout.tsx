import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";

import { QueryProvider } from "@/components/providers/query-provider";
import { ThemeProvider } from "@/components/providers/theme-provider";
import { UserGate } from "@/components/providers/user-gate";
import { UserSessionProvider } from "@/components/providers/user-session-provider";
import { cn } from "@/lib/utils";

const inter = Inter({
  subsets: ["latin"],
  variable: "--font-sans",
});

export const metadata: Metadata = {
  title: "SAJT Cookbook",
  description: "Discover, cook, and share your favorite recipes.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body className={cn("min-h-screen bg-background font-sans text-base text-foreground antialiased", inter.variable)}>
        <ThemeProvider attribute="class" defaultTheme="system" enableSystem>
          <QueryProvider>
            <UserSessionProvider>
              <UserGate>{children}</UserGate>
            </UserSessionProvider>
          </QueryProvider>
        </ThemeProvider>
      </body>
    </html>
  );
}
