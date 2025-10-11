import { useQuery } from "@tanstack/react-query";

import { getRecipe } from "@/features/recipes/api/get-recipe";
import type { RecipeDetails } from "@/features/recipes/types";

export function useRecipeQuery(recipeId: number) {
  return useQuery<RecipeDetails, Error>({
    queryKey: ["recipes", recipeId],
    queryFn: ({ signal }) => getRecipe(recipeId, signal),
    enabled: Number.isFinite(recipeId) && recipeId > 0,
  });
}
