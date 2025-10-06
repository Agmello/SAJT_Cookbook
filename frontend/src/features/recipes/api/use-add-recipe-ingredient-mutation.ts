import { useMutation, useQueryClient } from "@tanstack/react-query";

import { addRecipeIngredient, type AddRecipeIngredientPayload } from "@/features/recipes/api/add-recipe-ingredient";
import type { RecipeIngredientSummary } from "@/features/recipes/types";

export function useAddRecipeIngredientMutation(recipeId: number) {
  const queryClient = useQueryClient();

  return useMutation<RecipeIngredientSummary, Error, AddRecipeIngredientPayload>({
    mutationFn: (payload) => addRecipeIngredient(recipeId, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["recipes", "list"] });
      queryClient.invalidateQueries({ queryKey: ["recipes", recipeId, "ingredients"] });
    },
  });
}
