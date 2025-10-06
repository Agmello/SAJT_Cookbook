"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";

import { useCreateIngredientMutation } from "@/features/ingredients/api/use-create-ingredient-mutation";
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
import type { MeasurementUnit } from "@/features/recipes/types";

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
  name: z
    .string()
    .min(1, "Name is required.")
    .max(150, "Name cannot exceed 150 characters."),
  pluralName: z
    .string()
    .max(150, "Plural name cannot exceed 150 characters.")
    .optional()
    .or(z.literal(""))
    .transform((value) => (value === "" ? undefined : value)),
  defaultUnit: z.enum(["Unitless", "Gram", "Milliliter", "Tablespoon", "Teaspoon", "Cup", "Piece"]).optional(),
  isActive: z.boolean().default(true),
});

export function CreateIngredientForm() {
  const router = useRouter();
  const { mutateAsync, isPending } = useCreateIngredientMutation();
  const [submissionError, setSubmissionError] = useState<string | null>(null);
  const [submissionSuccess, setSubmissionSuccess] = useState<string | null>(null);

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      name: "",
      pluralName: "",
      defaultUnit: undefined,
      isActive: true,
    },
  });

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    setSubmissionError(null);
    setSubmissionSuccess(null);

    try {
      const normalizedName = values.name.trim().toLowerCase();
      const normalizedPluralName = values.pluralName ? values.pluralName.trim().toLowerCase() : undefined;

      await mutateAsync({
        name: normalizedName,
        pluralName: normalizedPluralName,
        defaultUnit: values.defaultUnit ?? null,
        isActive: values.isActive,
      });

      setSubmissionSuccess("Ingredient created.");
      form.reset({
        name: "",
        pluralName: "",
        defaultUnit: values.defaultUnit,
        isActive: true,
      });
      router.refresh();
    } catch (error) {
      setSubmissionError(error instanceof Error ? error.message : "Unable to create ingredient. Please try again.");
    }
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
        <div className="grid gap-4 md:grid-cols-2">
          <FormField
            control={form.control}
            name="name"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Name</FormLabel>
                <FormControl>
                  <Input placeholder="Ingredient name" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="pluralName"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Plural name</FormLabel>
                <FormControl>
                  <Input placeholder="e.g. Tomatoes" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        <div className="grid gap-4 md:grid-cols-[minmax(0,1fr)_auto] md:items-center">
          <FormField
            control={form.control}
            name="defaultUnit"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Default unit</FormLabel>
                <FormControl>
                  <Select onValueChange={field.onChange} value={field.value}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select default unit" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="Unitless">Unitless</SelectItem>
                      {measurementUnits
                        .filter((unit) => unit.value !== "Unitless")
                        .map((unit) => (
                          <SelectItem key={unit.value} value={unit.value}>
                            {unit.label}
                          </SelectItem>
                        ))}
                    </SelectContent>
                  </Select>
                </FormControl>
                <FormDescription>Optional default measurement used when creating recipe entries.</FormDescription>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="isActive"
            render={({ field }) => (
              <FormItem className="flex items-center justify-between rounded-lg border p-4">
                <div className="space-y-0.5">
                  <FormLabel>Active</FormLabel>
                  <FormDescription>Inactive ingredients are hidden from recipe forms.</FormDescription>
                </div>
                <FormControl>
                  <Switch checked={field.value} onCheckedChange={field.onChange} />
                </FormControl>
              </FormItem>
            )}
          />
        </div>

        {submissionError && <p className="text-sm text-destructive">{submissionError}</p>}
        {submissionSuccess && <p className="text-sm text-emerald-600">{submissionSuccess}</p>}

        <Button type="submit" size="lg" className="w-full md:w-auto" disabled={isPending}>
          {isPending ? "Creating..." : "Create ingredient"}
        </Button>
      </form>
    </Form>
  );
}


