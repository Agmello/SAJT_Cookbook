"use client";

import { useRecipesQuery } from "@/features/recipes/api/use-recipes-query";
import { RecipeCard } from "@/features/recipes/components/recipe-card";
import { Skeleton } from "@/components/ui/skeleton";
import { AlertCircle } from "lucide-react";

export function RecipesList() {
  const { data, isLoading, isError, error } = useRecipesQuery();

  if (isLoading) {
    return (
      <div className="grid gap-6 md:grid-cols-2 xl:grid-cols-3">
        {Array.from({ length: 6 }).map((_, index) => (
          <Skeleton key={index} className="h-64 w-full" />
        ))}
      </div>
    );
  }

  if (isError) {
    return (
      <div className="flex items-center gap-3 rounded-md border border-destructive/30 bg-destructive/10 px-4 py-3 text-sm text-destructive">
        <AlertCircle className="h-5 w-5" />
        <span>Failed to load recipes. {error instanceof Error ? error.message : "Try again soon."}</span>
      </div>
    );
  }

  if (!data || data.length === 0) {
    return (
      <div className="rounded-md border border-dashed px-6 py-10 text-center text-muted-foreground">
        <p className="text-lg font-medium text-foreground">No recipes yet</p>
        <p className="mt-2 text-sm">Add a new recipe from the author dashboard to see it appear here.</p>
      </div>
    );
  }

  return (
    <div className="grid gap-6 md:grid-cols-2 xl:grid-cols-3">
      {data.map((recipe) => (
        <RecipeCard key={recipe.id} recipe={recipe} />
      ))}
    </div>
  );
}
