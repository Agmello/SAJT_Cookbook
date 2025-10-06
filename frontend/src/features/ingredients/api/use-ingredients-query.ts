import { useQuery } from "@tanstack/react-query";

import { getIngredients } from "@/features/ingredients/api/get-ingredients";
import type { IngredientSummary } from "@/features/ingredients/types";

export function useIngredientsQuery() {
  return useQuery<IngredientSummary[], Error>({
    queryKey: ["ingredients", "list"],
    queryFn: ({ signal }) => getIngredients(signal),
  });
}
