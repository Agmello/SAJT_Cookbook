"use client";

import Link from "next/link";
import { useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import { zodResolver } from "@hookform/resolvers/zod";
import { useFieldArray, useForm } from "react-hook-form";
import { z } from "zod";
import { Plus, Trash2 } from "lucide-react";

import { useUserSession } from "@/components/providers/user-session-provider";
import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Switch } from "@/components/ui/switch";
import { Textarea } from "@/components/ui/textarea";
import { useIngredientsQuery } from "@/features/ingredients/api/use-ingredients-query";
import { addRecipeIngredient } from "@/features/recipes/api/add-recipe-ingredient";
import { useCreateRecipeMutation } from "@/features/recipes/api/use-create-recipe-mutation";
import type { MeasurementUnit, RecipeDifficulty } from "@/features/recipes/types";

const difficulties: RecipeDifficulty[] = ["Unknown", "Easy", "Medium", "Hard"];

const measurementUnits: { value: MeasurementUnit; label: string }[] = [
  { value: "Unitless", label: "Unitless" },
  { value: "Gram", label: "Gram" },
  { value: "Milliliter", label: "Milliliter" },
  { value: "Tablespoon", label: "Tablespoon" },
  { value: "Teaspoon", label: "Teaspoon" },
  { value: "Cup", label: "Cup" },
  { value: "Piece", label: "Piece" },
];

const ingredientEntrySchema = z.object({
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

const formSchema = z.object({
  authorId: z.string().uuid("Provide a valid author id."),
  title: z
    .string()
    .min(1, "Title is required.")
    .max(200, "Title cannot exceed 200 characters."),
  description: z
    .string()
    .max(2000, "Description cannot exceed 2000 characters.")
    .optional()
    .or(z.literal(""))
    .transform((value) => (value === "" ? undefined : value)),
  prepTimeMinutes: z.coerce
    .number({ invalid_type_error: "Enter a number." })
    .int()
    .min(0, "Prep time cannot be negative."),
  cookTimeMinutes: z.coerce
    .number({ invalid_type_error: "Enter a number." })
    .int()
    .min(0, "Cook time cannot be negative."),
  servings: z.coerce
    .number({ invalid_type_error: "Enter a number." })
    .int()
    .min(1, "Servings must be at least 1."),
  difficulty: z.enum(["Unknown", "Easy", "Medium", "Hard"]),
  isPublished: z.boolean().default(false),
  ingredients: z.array(ingredientEntrySchema).default([]),
});

type CreateRecipeFormValues = z.infer<typeof formSchema>;

type IngredientFormEntry = z.infer<typeof ingredientEntrySchema>;

export function CreateRecipeForm() {
  const router = useRouter();
  const { user: activeUser } = useUserSession();
  const { data: ingredientsData, isLoading: isLoadingIngredients } = useIngredientsQuery();
  const ingredientOptions = useMemo(
    () => (ingredientsData ?? []).filter((ingredient) => ingredient.isActive),
    [ingredientsData]
  );
  const { mutateAsync, isPending } = useCreateRecipeMutation();
  const [submissionError, setSubmissionError] = useState<string | null>(null);
  const [createdRecipeId, setCreatedRecipeId] = useState<number | null>(null);
  const [ingredientsInitialized, setIngredientsInitialized] = useState(false);

  const form = useForm<CreateRecipeFormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      authorId: "",
      title: "",
      description: "",
      prepTimeMinutes: 0,
      cookTimeMinutes: 0,
      servings: 1,
      difficulty: "Easy",
      isPublished: false,
      ingredients: [],
    },
  });

  const ingredientFieldArray = useFieldArray({
    control: form.control,
    name: "ingredients",
  });

  useEffect(() => {
    if (activeUser) {
      form.setValue("authorId", activeUser.id, { shouldValidate: true });
    }
  }, [activeUser, form]);

  useEffect(() => {
    if (!ingredientsInitialized && ingredientOptions.length > 0) {
      ingredientFieldArray.append(createIngredientEntry(ingredientOptions[0]));
      setIngredientsInitialized(true);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [ingredientOptions, ingredientsInitialized]);

  const handleAddIngredientRow = () => {
    if (!ingredientOptions.length) {
      return;
    }

    ingredientFieldArray.append(createIngredientEntry(ingredientOptions[0]));
  };

  const handleRemoveIngredientRow = (index: number) => {
    ingredientFieldArray.remove(index);
  };

  const onSubmit = async (values: CreateRecipeFormValues) => {
    setSubmissionError(null);
    setCreatedRecipeId(null);

    try {
      const recipe = await mutateAsync({
        authorId: values.authorId.trim(),
        title: values.title,
        description: values.description,
        prepTimeMinutes: values.prepTimeMinutes,
        cookTimeMinutes: values.cookTimeMinutes,
        servings: values.servings,
        difficulty: values.difficulty,
        isPublished: values.isPublished,
      });

      if (values.ingredients.length > 0) {
        try {
          await Promise.all(
            values.ingredients.map((entry) =>
              addRecipeIngredient(recipe.id, {
                ingredientId: entry.ingredientId,
                amount: entry.amount,
                unit: entry.unit,
                note: entry.note,
              })
            )
          );
        } catch (ingredientError) {
          const message =
            ingredientError instanceof Error
              ? `Recipe created but failed to attach all ingredients: ${ingredientError.message}`
              : "Recipe created but failed to attach all ingredients. Please try again from the ingredient management screen.";
          setSubmissionError(message);
          setCreatedRecipeId(recipe.id);
          return;
        }
      }

      form.reset({
        authorId: activeUser?.id ?? "",
        title: "",
        description: "",
        prepTimeMinutes: 0,
        cookTimeMinutes: 0,
        servings: 1,
        difficulty: "Easy",
        isPublished: false,
        ingredients: [],
      });
      setIngredientsInitialized(false);

      router.push(`/recipes/${recipe.id}`);
    } catch (error) {
      setSubmissionError(
        error instanceof Error ? error.message : "Unable to create recipe. Please try again."
      );
    }
  };

  const ingredientFields = ingredientFieldArray.fields;
  const canManageIngredients = createdRecipeId !== null;

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
        <div className="grid gap-4 md:grid-cols-2">
          <FormField
            control={form.control}
            name="authorId"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Author</FormLabel>
                <FormControl>
                  <div className="space-y-2">
                    <input type="hidden" {...field} value={activeUser?.id ?? ""} />
                    <div className="rounded-md border bg-muted px-3 py-2 text-sm font-medium text-foreground">
                      {activeUser?.name ?? "Select a user to continue"}
                    </div>
                  </div>
                </FormControl>
                <FormDescription>The recipe will be associated with your selected user.</FormDescription>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="title"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Title</FormLabel>
                <FormControl>
                  <Input placeholder="Grandma's Apple Pie" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>
        <FormField
          control={form.control}
          name="description"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Description</FormLabel>
              <FormControl>
                <Textarea placeholder="Tell readers what makes this recipe special." rows={4} {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <div className="grid gap-4 md:grid-cols-3">
          <FormField
            control={form.control}
            name="prepTimeMinutes"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Prep time (min)</FormLabel>
                <FormControl>
                  <Input type="number" min={0} {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="cookTimeMinutes"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Cook time (min)</FormLabel>
                <FormControl>
                  <Input type="number" min={0} {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="servings"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Servings</FormLabel>
                <FormControl>
                  <Input type="number" min={1} {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>
        <div className="grid gap-4 md:grid-cols-[minmax(0,1fr)_auto] md:items-center">
          <FormField
            control={form.control}
            name="difficulty"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Difficulty</FormLabel>
                <FormControl>
                  <Select onValueChange={field.onChange} defaultValue={field.value}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select difficulty" />
                    </SelectTrigger>
                    <SelectContent>
                      {difficulties.map((option) => (
                        <SelectItem key={option} value={option}>
                          {option}
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
            name="isPublished"
            render={({ field }) => (
              <FormItem className="flex items-center justify-between rounded-lg border p-4">
                <div className="space-y-0.5">
                  <FormLabel>Publish immediately</FormLabel>
                  <FormDescription>Toggle to make the recipe visible on the public site.</FormDescription>
                </div>
                <FormControl>
                  <Switch checked={field.value} onCheckedChange={field.onChange} />
                </FormControl>
              </FormItem>
            )}
          />
        </div>

        <section className="space-y-4">
          <div className="flex flex-col gap-2 md:flex-row md:items-center md:justify-between">
            <div>
              <h2 className="text-xl font-semibold">Ingredients</h2>
              <p className="text-sm text-muted-foreground">
                Optional: attach one or more ingredients that will be created alongside the recipe.
              </p>
            </div>
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={handleAddIngredientRow}
              disabled={isLoadingIngredients || !ingredientOptions.length}
            >
              <Plus className="mr-2 h-4 w-4" /> Add ingredient
            </Button>
          </div>

          {isLoadingIngredients ? (
            <p className="text-sm text-muted-foreground">Loading ingredients…</p>
          ) : !ingredientOptions.length ? (
            <div className="rounded-md border border-dashed px-6 py-6 text-sm text-muted-foreground">
              No active ingredients available. Create ingredients from the dashboard to include them here.
            </div>
          ) : ingredientFields.length === 0 ? (
            <div className="rounded-md border border-dashed px-6 py-6 text-sm text-muted-foreground">
              Use the &quot;Add ingredient&quot; button to attach ingredients to this recipe.
            </div>
          ) : (
            <div className="space-y-4">
              {ingredientFields.map((field, index) => (
                <div key={field.id} className="space-y-4 rounded-lg border p-4">
                  <div className="flex items-center justify-between">
                    <h3 className="text-sm font-medium text-muted-foreground">Ingredient {index + 1}</h3>
                    {ingredientFields.length > 1 && (
                      <Button
                        type="button"
                        variant="ghost"
                        size="icon"
                        onClick={() => handleRemoveIngredientRow(index)}
                        aria-label="Remove ingredient"
                      >
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    )}
                  </div>
                  <div className="grid gap-4 md:grid-cols-2">
                    <FormField
                      control={form.control}
                      name={`ingredients.${index}.ingredientId` as const}
                      render={({ field: ingredientField }) => (
                        <FormItem>
                          <FormLabel>Ingredient</FormLabel>
                          <FormControl>
                            <Select
                              onValueChange={(value) => ingredientField.onChange(Number(value))}
                              value={ingredientField.value?.toString() ?? ""}
                            >
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
                      name={`ingredients.${index}.unit` as const}
                      render={({ field: unitField }) => (
                        <FormItem>
                          <FormLabel>Unit</FormLabel>
                          <FormControl>
                            <Select onValueChange={unitField.onChange} value={unitField.value}>
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
                      name={`ingredients.${index}.amount` as const}
                      render={({ field: amountField }) => (
                        <FormItem>
                          <FormLabel>Amount</FormLabel>
                          <FormControl>
                            <Input type="number" step="0.01" min={0} {...amountField} />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    <FormField
                      control={form.control}
                      name={`ingredients.${index}.note` as const}
                      render={({ field: noteField }) => (
                        <FormItem>
                          <FormLabel>Note</FormLabel>
                          <FormControl>
                            <Textarea rows={2} placeholder="Optional preparation note" {...noteField} />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </div>
                </div>
              ))}
            </div>
          )}
        </section>

        {submissionError && (
          <div className="space-y-2">
            <p className="text-sm text-destructive">{submissionError}</p>
            {canManageIngredients && createdRecipeId && (
              <Button asChild variant="outline" size="sm">
                <Link href={`/dashboard/recipes/${createdRecipeId}/ingredients`}>
                  Manage ingredients for this recipe
                </Link>
              </Button>
            )}
          </div>
        )}

        <Button type="submit" size="lg" className="w-full md:w-auto" disabled={isPending}>
          {isPending ? "Creating..." : "Create recipe"}
        </Button>
      </form>
    </Form>
  );
}

function createIngredientEntry(option: { id: number }): IngredientFormEntry {
  return {
    ingredientId: option.id,
    amount: 0,
    unit: "Unitless",
    note: "",
  };
}

