export type MeasurementUnit =
  | "Unitless"
  | "Gram"
  | "Milliliter"
  | "Tablespoon"
  | "Teaspoon"
  | "Cup"
  | "Piece";

export interface RecipeIngredientSummary {
  id: number;
  ingredientId: number;
  ingredientName: string;
  amount: number;
  unit: MeasurementUnit;
  note?: string | null;
}
export type RecipeDifficulty = "Unknown" | "Easy" | "Medium" | "Hard";

export interface RecipeSummary {
  id: number;
  title: string;
  description: string | null;
  prepTimeMinutes: number;
  cookTimeMinutes: number;
  servings: number;
  difficulty: RecipeDifficulty;
  isPublished: boolean;
  updatedAtUtc: string;
}

export interface RecipeStep {
  id: number;
  stepNumber: number;
  instruction: string;
  durationMinutes: number | null;
  mediaUrl: string | null;
}

export interface RecipeTagSummary {
  id: number;
  name: string;
  slug: string;
}

export interface RecipeDetails extends RecipeSummary {
  slug: string;
  authorId: string;
  authorName?: string | null;
  createdAtUtc: string;
  ingredients: RecipeIngredientSummary[];
  steps: RecipeStep[];
  tags: RecipeTagSummary[];
}
