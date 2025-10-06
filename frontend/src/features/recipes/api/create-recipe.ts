import { api } from "@/lib/api-client";
import type { RecipeSummary } from "@/features/recipes/types";

export interface CreateRecipePayload {
  authorId: string;
  title: string;
  description?: string | null;
  prepTimeMinutes: number;
  cookTimeMinutes: number;
  servings: number;
  difficulty: string;
  isPublished: boolean;
}

export async function createRecipe(payload: CreateRecipePayload, signal?: AbortSignal): Promise<RecipeSummary> {
  return api
    .post("api/recipes", {
      json: payload,
      signal,
    })
    .json<RecipeSummary>();
}

