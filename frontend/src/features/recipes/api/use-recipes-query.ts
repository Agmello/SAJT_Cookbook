import { useQuery } from "@tanstack/react-query";

import { getRecipes } from "@/features/recipes/api/get-recipes";

export function useRecipesQuery() {
  return useQuery({
    queryKey: ["recipes", "list"],
    queryFn: ({ signal }) => getRecipes(signal),
  });
}
