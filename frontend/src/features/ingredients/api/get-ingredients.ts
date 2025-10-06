import { api } from "@/lib/api-client";
import type { IngredientSummary } from "@/features/ingredients/types";

export async function getIngredients(signal?: AbortSignal): Promise<IngredientSummary[]> {
  return api.get("api/ingredients", { signal }).json<IngredientSummary[]>();
}
