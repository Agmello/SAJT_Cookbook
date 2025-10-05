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
