"use client";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import { ThemeToggle } from "@/components/theme-toggle";
import { useUserSession } from "@/components/providers/user-session-provider";
import { cn } from "@/lib/utils";

const links = [
  { href: "/", label: "Explore" },
  { href: "/recipes", label: "Recipes" },
  { href: "/about", label: "About" },
];

export function SiteHeader() {
  const { user, clearUser } = useUserSession();

  return (
    <header className="border-b bg-background/80 backdrop-blur">
      <div className="container flex h-16 items-center justify-between">
        <Link href="/" className="text-lg font-semibold">
          SAJT Cookbook
        </Link>
        <nav className="flex items-center gap-6 text-sm font-medium">
          {links.map((link) => (
            <Link
              key={link.href}
              href={link.href}
              className={cn(
                "transition-colors hover:text-foreground/80",
                "text-foreground/60"
              )}
            >
              {link.label}
            </Link>
          ))}
        </nav>
        <div className="flex items-center gap-2">
          {user ? (
            <Button
              type="button"
              variant="outline"
              size="sm"
              className="inline-flex"
              onClick={clearUser}
            >
              {user.name}
              <span className="ml-1 text-muted-foreground">(switch)</span>
            </Button>
          ) : null}
          <Button asChild size="sm" variant="default">
            <Link href="/dashboard/recipes/new">Add recipe</Link>
          </Button>
          <ThemeToggle />
        </div>
      </div>
    </header>
  );
}
