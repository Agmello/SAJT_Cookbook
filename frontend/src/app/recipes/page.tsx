import type { Metadata } from "next";

import { AppShell } from "@/components/layout/app-shell";
import { RecipesList } from "@/features/recipes/components/recipes-list";

export const metadata: Metadata = {
  title: "Recipes | SAJT Cookbook",
  description: "Browse every recipe available in the SAJT Cookbook platform.",
};

export default function RecipesPage() {
  return (
    <AppShell>
      <div className="space-y-6">
        <div className="flex flex-col gap-2">
          <h1 className="text-3xl font-bold tracking-tight">All recipes</h1>
          <p className="text-muted-foreground">
            Browse every recipe available in the SAJT Cookbook. Filter and search features are coming soon.
          </p>
        </div>
        <RecipesList />
      </div>
    </AppShell>
  );
}
