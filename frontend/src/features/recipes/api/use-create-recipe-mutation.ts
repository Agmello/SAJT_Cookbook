import type { RecipeSummary } from "@/features/recipes/types";
import { useMutation, useQueryClient } from "@tanstack/react-query";

import { createRecipe, type CreateRecipePayload } from "@/features/recipes/api/create-recipe";

export function useCreateRecipeMutation() {
  const queryClient = useQueryClient();

  return useMutation<RecipeSummary, Error, CreateRecipePayload>({
    mutationFn: (payload) => createRecipe(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["recipes", "list"] });
    },
  });
}

