"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";

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
import { useCreateRecipeMutation } from "@/features/recipes/api/use-create-recipe-mutation";
import type { RecipeDifficulty } from "@/features/recipes/types";
import { environment } from "@/lib/env";

const difficulties: RecipeDifficulty[] = ["Unknown", "Easy", "Medium", "Hard"];

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
});

export function CreateRecipeForm() {
  const router = useRouter();
  const { mutateAsync, isPending } = useCreateRecipeMutation();
  const [submissionError, setSubmissionError] = useState<string | null>(null);

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      authorId: environment.defaultAuthorId,
      title: "",
      description: "",
      prepTimeMinutes: 0,
      cookTimeMinutes: 0,
      servings: 1,
      difficulty: "Easy",
      isPublished: false,
    },
  });

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
      setSubmissionError(null);
    try {
      await mutateAsync({
        authorId: values.authorId.trim(),
        title: values.title,
        description: values.description,
        prepTimeMinutes: values.prepTimeMinutes,
        cookTimeMinutes: values.cookTimeMinutes,
        servings: values.servings,
        difficulty: values.difficulty,
        isPublished: values.isPublished,
      });

      router.push("/recipes");
    } catch (error) {
      setSubmissionError(error instanceof Error ? error.message : "Unable to create recipe. Please try again.");
    }
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
        <div className="grid gap-4 md:grid-cols-2">
          <FormField
            control={form.control}
            name="authorId"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Author Id</FormLabel>
                <FormControl>
                  <Input placeholder="00000000-0000-0000-0000-000000000000" {...field} />
                </FormControl>
                <FormDescription>Temporary field until authentication is in place.</FormDescription>
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
                <Select onValueChange={field.onChange} defaultValue={field.value}>
                  <FormControl>
                    <SelectTrigger>
                      <SelectValue placeholder="Select difficulty" />
                    </SelectTrigger>
                  </FormControl>
                  <SelectContent>
                    {difficulties.map((option) => (
                      <SelectItem key={option} value={option}>
                        {option}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
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
        {submissionError && (
          <p className="text-sm text-destructive">{submissionError}</p>
        )}
        <Button type="submit" size="lg" className="w-full md:w-auto" disabled={isPending}>
          {isPending ? "Creating..." : "Create recipe"}
        </Button>
      </form>
    </Form>
  );
}










