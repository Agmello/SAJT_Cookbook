"use client";

import Link from "next/link";
import { useMemo } from "react";
import { AlertCircle, Clock, Loader2, Users } from "lucide-react";
import { formatDistanceToNow } from "date-fns";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { Separator } from "@/components/ui/separator";
import { useRecipeQuery } from "@/features/recipes/api/use-recipe-query";
import type { RecipeDifficulty } from "@/features/recipes/types";

interface RecipeDetailsViewProps {
  recipeId: number;
}

const difficultyCopy: Record<RecipeDifficulty, string> = {
  Unknown: "Mystery",
  Easy: "Easy",
  Medium: "Intermediate",
  Hard: "Advanced",
};

const measurementCopy: Record<string, string> = {
  Unitless: "",
  Gram: "g",
  Milliliter: "ml",
  Tablespoon: "tbsp",
  Teaspoon: "tsp",
  Cup: "cup",
  Piece: "piece",
};

export function RecipeDetailsView({ recipeId }: RecipeDetailsViewProps) {
  const amountFormatter = useMemo(
    () =>
      new Intl.NumberFormat(undefined, {
        minimumFractionDigits: 0,
        maximumFractionDigits: 2,
      }),
    []
  );
  const {
    data: recipe,
    error,
    isError,
    isLoading,
    isFetching,
    refetch,
  } = useRecipeQuery(recipeId);

  if (!Number.isFinite(recipeId) || recipeId <= 0) {
    return (
      <div className="rounded-md border border-dashed px-6 py-10 text-center">
        <p className="text-lg font-semibold text-foreground">Invalid recipe identifier</p>
        <p className="mt-2 text-sm text-muted-foreground">
          The requested recipe could not be loaded because the identifier in the URL is not valid.
        </p>
      </div>
    );
  }

  if (isLoading) {
    return <RecipeDetailsSkeleton />;
  }

  if (isError) {
    return (
      <div className="space-y-4 rounded-md border border-destructive/20 bg-destructive/5 p-6">
        <div className="flex items-start gap-3">
          <AlertCircle className="mt-0.5 h-5 w-5 text-destructive" />
          <div className="space-y-1">
            <p className="text-base font-semibold text-destructive">Failed to load recipe</p>
            <p className="text-sm text-muted-foreground">
              {error instanceof Error ? error.message : "Something went wrong while fetching the recipe."}
            </p>
          </div>
        </div>
        <Button onClick={() => refetch()} disabled={isFetching} size="sm" variant="outline" className="gap-2">
          {isFetching ? <Loader2 className="h-4 w-4 animate-spin" /> : null}
          Try again
        </Button>
      </div>
    );
  }

  if (!recipe) {
    return (
      <div className="rounded-md border border-dashed px-6 py-10 text-center text-muted-foreground">
        The recipe could not be found. It may have been deleted or unpublished.
      </div>
    );
  }

  const totalTimeMinutes = recipe.prepTimeMinutes + recipe.cookTimeMinutes;
  const updatedAtLabel = formatDistanceToNow(new Date(recipe.updatedAtUtc), { addSuffix: true });
  const createdAtLabel = formatDistanceToNow(new Date(recipe.createdAtUtc), { addSuffix: true });

  return (
    <div className="space-y-8">
      <Card>
        <CardHeader className="space-y-4">
          <div className="space-y-2">
            <CardTitle className="text-3xl font-bold leading-tight text-foreground">{recipe.title}</CardTitle>
            {recipe.description ? (
              <p className="text-base text-muted-foreground">{recipe.description}</p>
            ) : null}
          </div>

          <div className="flex flex-wrap items-center gap-3 text-sm text-muted-foreground">
            <Badge variant="outline" className="uppercase tracking-wide">
              {difficultyCopy[recipe.difficulty]}
            </Badge>
            <Separator orientation="vertical" className="hidden h-4 sm:block" />
            <span className="flex items-center gap-1 text-foreground">
              <Users className="h-4 w-4" />
              {recipe.servings} servings
            </span>
            <Separator orientation="vertical" className="hidden h-4 sm:block" />
            <span className="flex items-center gap-1 text-foreground">
              <Clock className="h-4 w-4" />
              {totalTimeMinutes} min total ({recipe.prepTimeMinutes} prep · {recipe.cookTimeMinutes} cook)
            </span>
            <Separator orientation="vertical" className="hidden h-4 sm:block" />
            <span>
              Created {createdAtLabel}
              {recipe.authorName ? ` by ${recipe.authorName}` : ""}
            </span>
            <Separator orientation="vertical" className="hidden h-4 sm:block" />
            <span>Updated {updatedAtLabel}</span>
          </div>

          {recipe.tags.length ? (
            <div className="flex flex-wrap gap-2">
              {recipe.tags.map((tag) => (
                <Badge key={tag.id} variant="secondary" className="capitalize">
                  #{tag.name}
                </Badge>
              ))}
            </div>
          ) : null}
        </CardHeader>
      </Card>

      <div className="grid gap-6 lg:grid-cols-[minmax(0,2fr)_minmax(0,1fr)]">
        <Card className="order-2 lg:order-1">
          <CardHeader>
            <CardTitle className="text-2xl font-semibold">Steps</CardTitle>
            <p className="text-sm text-muted-foreground">
              Follow the instructions in order for best results.
            </p>
          </CardHeader>
          <CardContent>
            {recipe.steps.length ? (
              <ol className="space-y-6">
                {recipe.steps
                  .slice()
                  .sort((a, b) => a.stepNumber - b.stepNumber)
                  .map((step) => (
                    <li key={step.id} className="space-y-2">
                      <div className="flex items-start gap-3">
                        <div className="mt-0.5 h-7 w-7 rounded-full bg-primary/10 text-center text-sm font-semibold leading-7 text-primary">
                          {step.stepNumber}
                        </div>
                        <div className="space-y-1">
                          <p className="font-medium text-foreground">{step.instruction}</p>
                          {step.durationMinutes ? (
                            <p className="text-sm text-muted-foreground">Approx. {step.durationMinutes} minutes</p>
                          ) : null}
                          {step.mediaUrl ? (
                            <p className="text-sm">
                              <Link href={step.mediaUrl} className="text-primary underline" target="_blank" rel="noopener noreferrer">
                                View reference media
                              </Link>
                            </p>
                          ) : null}
                        </div>
                      </div>
                    </li>
                  ))}
              </ol>
            ) : (
              <p className="text-sm text-muted-foreground">This recipe does not have any documented steps yet.</p>
            )}
          </CardContent>
        </Card>

        <Card className="order-1 lg:order-2">
          <CardHeader>
            <CardTitle className="text-2xl font-semibold">Ingredients</CardTitle>
            <p className="text-sm text-muted-foreground">
              Quantities based on {recipe.servings} servings.
            </p>
          </CardHeader>
          <CardContent className="space-y-4">
            {recipe.ingredients.length ? (
              <ul className="space-y-4">
                {recipe.ingredients.map((ingredient) => (
                  <li key={ingredient.id} className="rounded-md border p-3">
                    <div className="flex flex-col gap-1">
                      <p className="text-base font-semibold text-foreground">
                        {amountFormatter.format(ingredient.amount)}
                        {measurementCopy[ingredient.unit] ? ` ${measurementCopy[ingredient.unit]}` : ""} {ingredient.ingredientName}
                      </p>
                      {ingredient.note ? (
                        <p className="text-sm text-muted-foreground">{ingredient.note}</p>
                      ) : null}
                    </div>
                  </li>
                ))}
              </ul>
            ) : (
              <p className="text-sm text-muted-foreground">No ingredients are attached to this recipe yet.</p>
            )}

            <Button asChild variant="outline" size="sm" className="w-full">
              <Link href={`/dashboard/recipes/${recipe.id}/ingredients`}>Manage ingredients</Link>
            </Button>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}

function RecipeDetailsSkeleton() {
  return (
    <div className="space-y-8">
      <Card>
        <CardHeader className="space-y-3">
          <Skeleton className="h-10 w-3/4" />
          <Skeleton className="h-4 w-full" />
          <Skeleton className="h-4 w-2/3" />
        </CardHeader>
      </Card>

      <div className="grid gap-6 lg:grid-cols-[minmax(0,2fr)_minmax(0,1fr)]">
        <Card className="order-2 lg:order-1">
          <CardHeader>
            <Skeleton className="h-6 w-32" />
            <Skeleton className="h-4 w-48" />
          </CardHeader>
          <CardContent className="space-y-4">
            {Array.from({ length: 3 }).map((_, index) => (
              <div key={index} className="space-y-2">
                <Skeleton className="h-6 w-16" />
                <Skeleton className="h-4 w-full" />
                <Skeleton className="h-4 w-2/3" />
              </div>
            ))}
          </CardContent>
        </Card>

        <Card className="order-1 lg:order-2">
          <CardHeader>
            <Skeleton className="h-6 w-32" />
            <Skeleton className="h-4 w-40" />
          </CardHeader>
          <CardContent className="space-y-3">
            {Array.from({ length: 4 }).map((_, index) => (
              <Skeleton key={index} className="h-16 w-full" />
            ))}
            <Skeleton className="h-10 w-full" />
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
