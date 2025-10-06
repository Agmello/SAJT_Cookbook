import type { Metadata } from "next";

import { AppShell } from "@/components/layout/app-shell";
import { AddRecipeIngredientForm } from "@/features/recipes/components/add-recipe-ingredient-form";

export const metadata: Metadata = {
  title: "Add Ingredient | SAJT Cookbook",
  description: "Attach ingredients to an existing recipe.",
};

export default async function AddRecipeIngredientPage({
  params,
}: {
  params: Promise<{ recipeId: string }>;
}) {
  const { recipeId } = await params;

  return (
    <AppShell>
      <div className="space-y-6">
        <div className="flex flex-col gap-2">
          <h1 className="text-3xl font-bold tracking-tight">Add ingredient to recipe</h1>
          <p className="text-muted-foreground">
            Select an ingredient, amount, and measurement unit to attach it to this recipe.
          </p>
        </div>
        <AddRecipeIngredientForm recipeId={Number(recipeId)} />
      </div>
    </AppShell>
  );
}
