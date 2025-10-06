import type { Metadata } from "next";

import { AppShell } from "@/components/layout/app-shell";
import { CreateRecipeForm } from "@/features/recipes/components/create-recipe-form";

export const metadata: Metadata = {
  title: "Create Recipe | SAJT Cookbook",
  description: "Author a new recipe and publish it to the SAJT Cookbook.",
};

export default function NewRecipePage() {
  return (
    <AppShell>
      <div className="space-y-6">
        <div className="flex flex-col gap-2">
          <h1 className="text-3xl font-bold tracking-tight">Create a new recipe</h1>
          <p className="text-muted-foreground">
            Provide the essential details for your recipe. Ingredient and step management will follow in future iterations.
          </p>
        </div>
        <CreateRecipeForm />
      </div>
    </AppShell>
  );
}
