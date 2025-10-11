import Link from "next/link";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import type { RecipeSummary } from "@/features/recipes/types";
import { formatDistanceToNow } from "date-fns";

interface RecipeCardProps {
  recipe: RecipeSummary;
}

const difficultyCopy: Record<RecipeSummary["difficulty"], string> = {
  Unknown: "Mystery",
  Easy: "Easy",
  Medium: "Intermediate",
  Hard: "Advanced",
};

export function RecipeCard({ recipe }: RecipeCardProps) {
  const totalTime = recipe.prepTimeMinutes + recipe.cookTimeMinutes;
  const updatedAtLabel = formatDistanceToNow(new Date(recipe.updatedAtUtc), {
    addSuffix: true,
  });

  return (
    <Card className="h-full">
      <CardHeader className="space-y-2">
        <div className="flex items-center justify-between gap-2">
          <CardTitle className="text-xl font-semibold leading-tight">
            {recipe.title}
          </CardTitle>
          <Badge variant={recipe.isPublished ? "default" : "secondary"}>
            {recipe.isPublished ? "Published" : "Draft"}
          </Badge>
        </div>
        {recipe.description && (
          <CardDescription className="line-clamp-3 text-base">
            {recipe.description}
          </CardDescription>
        )}
      </CardHeader>
      <Separator />
      <CardContent className="flex flex-col gap-4 py-6 text-sm text-muted-foreground">
        <div className="flex flex-wrap items-center gap-3">
          <span className="text-foreground">
            <strong>Total time</strong>: {totalTime} min
          </span>
          <span className="hidden text-muted-foreground sm:block">|</span>
          <span className="text-foreground">
            <strong>Prep</strong>: {recipe.prepTimeMinutes} min
          </span>
          <span className="hidden text-muted-foreground sm:block">|</span>
          <span className="text-foreground">
            <strong>Serves</strong>: {recipe.servings}
          </span>
        </div>
        <div className="flex items-center gap-2">
          <Badge variant="outline" className="uppercase tracking-wide">
            {difficultyCopy[recipe.difficulty]}
          </Badge>
        </div>
      </CardContent>
      <CardFooter className="flex items-center justify-between text-sm text-muted-foreground">
        <span>Updated {updatedAtLabel}</span>
        <div className="flex items-center gap-3">
          <Link
            href={`/recipes/${recipe.id}`}
            className="text-sm font-medium text-primary hover:underline"
          >
            View details
          </Link>
          <Link
            href={`/dashboard/recipes/${recipe.id}/ingredients`}
            className="text-sm font-medium text-primary hover:underline"
          >
            Add ingredient
          </Link>
        </div>
      </CardFooter>
    </Card>
  );
}

