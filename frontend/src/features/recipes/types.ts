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

