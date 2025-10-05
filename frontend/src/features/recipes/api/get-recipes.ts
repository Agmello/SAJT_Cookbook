import { api } from "@/lib/api-client";
import type { RecipeSummary } from "@/features/recipes/types";

export async function getRecipes(signal?: AbortSignal): Promise<RecipeSummary[]> {
  return api.get("api/recipes", { signal }).json<RecipeSummary[]>();
}
