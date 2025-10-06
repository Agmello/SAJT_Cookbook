import { api } from "@/lib/api-client";
import type { IngredientSummary } from "@/features/ingredients/types";
import type { MeasurementUnit } from "@/features/recipes/types";

export interface CreateIngredientPayload {
  name: string;
  pluralName?: string;
  defaultUnit?: MeasurementUnit | null;
  isActive: boolean;
}

export async function createIngredient(payload: CreateIngredientPayload, signal?: AbortSignal): Promise<IngredientSummary> {
  return api
    .post("api/ingredients", {
      json: payload,
      signal,
    })
    .json<IngredientSummary>();
}
