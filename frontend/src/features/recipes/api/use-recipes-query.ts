import type { RecipeSummary } from "@/features/recipes/types";
import { useQuery } from "@tanstack/react-query";

import { getRecipes } from "@/features/recipes/api/get-recipes";

export function useRecipesQuery() {
  return useQuery<RecipeSummary[], Error>({
    queryKey: ["recipes", "list"],
    queryFn: ({ signal }) => getRecipes(signal),
  });
}


