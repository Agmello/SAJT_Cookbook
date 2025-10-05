import { AppShell } from "@/components/layout/app-shell";
import { HeroSection } from "@/components/marketing/hero-section";
import { RecipesList } from "@/features/recipes/components/recipes-list";

export default function HomePage() {
  return (
    <AppShell>
      <div className="space-y-12">
        <HeroSection />
        <section className="space-y-6">
          <div className="flex flex-col gap-2">
            <h2 className="text-2xl font-semibold tracking-tight">Latest recipes</h2>
            <p className="text-muted-foreground">
              Fresh from the kitchen. Pulled directly from the SAJT Cookbook API using React Query.
            </p>
          </div>
          <RecipesList />
        </section>
      </div>
    </AppShell>
  );
}
