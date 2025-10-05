export function SiteFooter() {
  return (
    <footer className="border-t bg-background/80">
      <div className="container flex h-16 items-center justify-between text-sm text-muted-foreground">
        <p>&copy; {new Date().getFullYear()} SAJT Cookbook. All rights reserved.</p>
        <p className="hidden md:block">Crafted with Next.js, Tailwind CSS, and shadcn/ui.</p>
      </div>
    </footer>
  );
}
