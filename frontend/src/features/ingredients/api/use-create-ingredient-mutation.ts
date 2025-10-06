import { useMutation, useQueryClient } from "@tanstack/react-query";

import { createIngredient, type CreateIngredientPayload } from "@/features/ingredients/api/create-ingredient";
import type { IngredientSummary } from "@/features/ingredients/types";

export function useCreateIngredientMutation() {
  const queryClient = useQueryClient();

  return useMutation<IngredientSummary, Error, CreateIngredientPayload>({
    mutationFn: (payload) => createIngredient(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["ingredients", "list"] });
    },
  });
}
