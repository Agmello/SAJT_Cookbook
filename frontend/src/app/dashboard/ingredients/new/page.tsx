import type { Metadata } from "next";

import { AppShell } from "@/components/layout/app-shell";
import { CreateIngredientForm } from "@/features/ingredients/components/create-ingredient-form";

export const metadata: Metadata = {
  title: "Create Ingredient | SAJT Cookbook",
  description: "Add new ingredients to the cookbook library.",
};

export default function NewIngredientPage() {
  return (
    <AppShell>
      <div className="space-y-6">
        <div className="flex flex-col gap-2">
          <h1 className="text-3xl font-bold tracking-tight">Create a new ingredient</h1>
          <p className="text-muted-foreground">
            Define ingredient details that authors can reuse while composing recipes.
          </p>
        </div>
        <CreateIngredientForm />
      </div>
    </AppShell>
  );
}
