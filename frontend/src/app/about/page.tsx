import type { Metadata } from "next";

import { AppShell } from "@/components/layout/app-shell";

export const metadata: Metadata = {
  title: "About | SAJT Cookbook",
  description: "Learn more about the SAJT Cookbook product vision and stack.",
};

const highlights = [
  {
    title: "SOLID, CQRS, and DDD",
    description:
      "The backend follows a clean architecture with MediatR-powered commands and queries, backed by a SQL Server persistence layer.",
  },
  {
    title: "Composable React UI",
    description:
      "Next.js App Router, Tailwind CSS, and shadcn/ui deliver accessible primitives, reusable layouts, and themeable design tokens.",
  },
  {
    title: "Productivity baked in",
    description:
      "React Query, React Hook Form, and Zod streamline data fetching and validation for future authoring workflows.",
  },
];

export default function AboutPage() {
  return (
    <AppShell>
      <div className="space-y-10">
        <section className="space-y-4">
          <h1 className="text-3xl font-bold tracking-tight">Why SAJT Cookbook?</h1>
          <p className="text-pretty text-muted-foreground">
            SAJT Cookbook combines a robust ASP.NET Core backend with a modern React frontend so development teams can focus on
            crafting delightful culinary experiences instead of plumbing. This project will grow into an end-to-end reference
            for modular, cloud-ready food applications.
          </p>
        </section>
        <section className="grid gap-6 md:grid-cols-3">
          {highlights.map((highlight) => (
            <article key={highlight.title} className="rounded-xl border bg-card p-6 shadow-sm">
              <h2 className="text-xl font-semibold text-card-foreground">{highlight.title}</h2>
              <p className="mt-3 text-sm text-muted-foreground">{highlight.description}</p>
            </article>
          ))}
        </section>
      </div>
    </AppShell>
  );
}
