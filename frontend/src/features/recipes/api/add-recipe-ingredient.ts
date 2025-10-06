import { api } from "@/lib/api-client";
import type { RecipeIngredientSummary } from "@/features/recipes/types";
import type { MeasurementUnit } from "@/features/recipes/types";

export interface AddRecipeIngredientPayload {
  ingredientId: number;
  amount: number;
  unit: MeasurementUnit;
  note?: string;
}

export async function addRecipeIngredient(
  recipeId: number,
  payload: AddRecipeIngredientPayload,
  signal?: AbortSignal
): Promise<RecipeIngredientSummary> {
  return api
    .post(`api/recipes/${recipeId}/ingredients`, {
      json: payload,
      signal,
    })
    .json<RecipeIngredientSummary>();
}
