"use client";

import { useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";

import { useIngredientsQuery } from "@/features/ingredients/api/use-ingredients-query";
import type { IngredientSummary } from "@/features/ingredients/types";
import { useAddRecipeIngredientMutation } from "@/features/recipes/api/use-add-recipe-ingredient-mutation";
import type { MeasurementUnit } from "@/features/recipes/types";
import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import { Skeleton } from "@/components/ui/skeleton";

const measurementUnits: { value: MeasurementUnit; label: string }[] = [
  { value: "Unitless", label: "Unitless" },
  { value: "Gram", label: "Gram" },
  { value: "Milliliter", label: "Milliliter" },
  { value: "Tablespoon", label: "Tablespoon" },
  { value: "Teaspoon", label: "Teaspoon" },
  { value: "Cup", label: "Cup" },
  { value: "Piece", label: "Piece" },
];

const formSchema = z.object({
  ingredientId: z.coerce.number({ invalid_type_error: "Select an ingredient." }).int().positive(),
  amount: z.coerce.number({ invalid_type_error: "Enter an amount." }).gt(0, "Amount must be greater than zero."),
  unit: z.enum(["Unitless", "Gram", "Milliliter", "Tablespoon", "Teaspoon", "Cup", "Piece"]),
  note: z
    .string()
    .max(200, "Note cannot exceed 200 characters.")
    .optional()
    .or(z.literal(""))
    .transform((value) => (value === "" ? undefined : value)),
});

interface AddRecipeIngredientFormProps {
  recipeId: number;
}

export function AddRecipeIngredientForm({ recipeId }: AddRecipeIngredientFormProps) {
  const router = useRouter();
  const { data: ingredients, isLoading: isLoadingIngredients } = useIngredientsQuery();
  const { mutateAsync, isPending } = useAddRecipeIngredientMutation(recipeId);
  const [submissionError, setSubmissionError] = useState<string | null>(null);
  const [submissionSuccess, setSubmissionSuccess] = useState<string | null>(null);

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      ingredientId: undefined,
      amount: 0,
      unit: "Gram",
      note: "",
    },
  });

  useEffect(() => {
    if (!isLoadingIngredients && ingredients && ingredients.length > 0) {
      const firstActive = ingredients.find((ingredient) => ingredient.isActive);
      if (firstActive) {
        form.setValue("ingredientId", firstActive.id, { shouldValidate: true });
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isLoadingIngredients, ingredients]);

  const ingredientOptions = useMemo(() => {
    if (!ingredients) {
      return [] as IngredientSummary[];
    }

    return ingredients.filter((ingredient) => ingredient.isActive);
  }, [ingredients]);

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    setSubmissionError(null);
    setSubmissionSuccess(null);

    try {
      await mutateAsync({
        ingredientId: values.ingredientId,
        amount: values.amount,
        unit: values.unit,
        note: values.note,
      });

      setSubmissionSuccess("Ingredient added to recipe.");
      form.reset({
        ingredientId: values.ingredientId,
        amount: 0,
        unit: values.unit,
        note: "",
      });
      router.refresh();
    } catch (error) {
      setSubmissionError(error instanceof Error ? error.message : "Unable to add ingredient. Please try again.");
    }
  };

  if (isLoadingIngredients) {
    return <Skeleton className="h-40 w-full" />;
  }

  if (!ingredientOptions.length) {
    return (
      <div className="rounded-md border border-dashed px-6 py-10 text-center text-muted-foreground">
        <p className="text-lg font-medium text-foreground">No ingredients available</p>
        <p className="mt-2 text-sm">Create ingredients first before adding them to recipes.</p>
      </div>
    );
  }

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
        <div className="grid gap-4 md:grid-cols-2">
          <FormField
            control={form.control}
            name="ingredientId"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Ingredient</FormLabel>
                <FormControl>
                  <Select onValueChange={(value) => field.onChange(Number(value))} value={field.value?.toString()}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select ingredient" />
                    </SelectTrigger>
                    <SelectContent>
                      {ingredientOptions.map((ingredient) => (
                        <SelectItem key={ingredient.id} value={ingredient.id.toString()}>
                          {ingredient.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="unit"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Unit</FormLabel>
                <FormControl>
                  <Select onValueChange={field.onChange} value={field.value}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select unit" />
                    </SelectTrigger>
                    <SelectContent>
                      {measurementUnits.map((unit) => (
                        <SelectItem key={unit.value} value={unit.value}>
                          {unit.label}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        <div className="grid gap-4 md:grid-cols-2">
          <FormField
            control={form.control}
            name="amount"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Amount</FormLabel>
                <FormControl>
                  <Input type="number" step="0.01" min="0" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="note"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Note</FormLabel>
                <FormControl>
                  <Textarea rows={2} placeholder="Optional preparation note" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        {submissionError && <p className="text-sm text-destructive">{submissionError}</p>}
        {submissionSuccess && <p className="text-sm text-emerald-600">{submissionSuccess}</p>}

        <Button type="submit" size="lg" className="w-full md:w-auto" disabled={isPending}>
          {isPending ? "Adding..." : "Add ingredient"}
        </Button>
      </form>
    </Form>
  );
}



