import { api } from "@/lib/api-client";
import type { RecipeDetails } from "@/features/recipes/types";

export async function getRecipe(recipeId: number, signal?: AbortSignal): Promise<RecipeDetails> {
  return api.get(`api/recipes/${recipeId}`, { signal }).json<RecipeDetails>();
}
