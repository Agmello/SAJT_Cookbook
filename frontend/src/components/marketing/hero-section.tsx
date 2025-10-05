import { Button } from "@/components/ui/button";
import Link from "next/link";

export function HeroSection() {
  return (
    <section className="grid gap-8 pb-12 pt-8 md:grid-cols-[1.1fr_minmax(0,0.9fr)] md:items-center md:gap-12">
      <div className="space-y-6">
        <span className="inline-flex items-center rounded-full bg-muted px-3 py-1 text-xs font-semibold uppercase tracking-wide text-muted-foreground">
          SAJT Cookbook Platform
        </span>
        <h1 className="text-balance text-4xl font-bold tracking-tight sm:text-5xl">
          Cook smarter, share faster, and collaborate on delightful recipes.
        </h1>
        <p className="text-pretty text-lg text-muted-foreground">
          Manage your culinary creations with a domain-driven backend, CQRS workflows, and a modern
          React + Next.js frontend. Discover new meals, track favorites, and publish to your community.
        </p>
        <div className="flex flex-wrap items-center gap-3">
          <Button asChild size="lg">
            <Link href="/recipes">Browse recipes</Link>
          </Button>
          <Button asChild variant="outline" size="lg">
            <Link href="/dashboard/recipes/new">Add a recipe</Link>
          </Button>
        </div>
      </div>
      <div className="relative overflow-hidden rounded-3xl border bg-muted/40 p-6 shadow-sm">
        <div className="grid gap-4 text-sm">
          <div className="flex items-center justify-between">
            <span className="font-medium">Total recipes</span>
            <span className="text-muted-foreground">coming soon</span>
          </div>
          <div className="flex items-center justify-between">
            <span className="font-medium">Popular tag</span>
            <span className="text-muted-foreground">fresh & seasonal</span>
          </div>
          <div className="flex items-center justify-between">
            <span className="font-medium">Community rating</span>
            <span className="text-muted-foreground">4.8 / 5</span>
          </div>
        </div>
        <div className="absolute inset-x-0 bottom-0 h-24 bg-gradient-to-t from-background" />
      </div>
    </section>
  );
}
