import type { Metadata } from "next";

import { AppShell } from "@/components/layout/app-shell";
import { RecipeDetailsView } from "@/features/recipes/components/recipe-details-view";

export const metadata: Metadata = {
  title: "Recipe Details | SAJT Cookbook",
  description: "View a recipe with full instructions and ingredients.",
};

type RecipeDetailsPageProps = {
  params: Promise<{ recipeId: string }>;
};

export default async function RecipeDetailsPage({ params }: RecipeDetailsPageProps) {
  const { recipeId } = await params;

  return (
    <AppShell>
      <RecipeDetailsView recipeId={Number(recipeId)} />
    </AppShell>
  );
}
