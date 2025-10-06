import type { MeasurementUnit } from "@/features/recipes/types";

export interface IngredientSummary {
  id: number;
  name: string;
  pluralName?: string | null;
  defaultUnit?: MeasurementUnit | null;
  isActive: boolean;
}
