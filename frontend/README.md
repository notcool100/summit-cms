# Summit — frontend

SvelteKit + TypeScript port of the eight-page Summit marketing site (previously
static `.dc.html` prototypes at the repo root). Same content and interactions,
rebuilt as a typed, componentized SvelteKit app.

## Stack

- **SvelteKit 2 / Svelte 5** (runes mode) + TypeScript
- File-based routing, server-rendered pages
- No CSS framework — component-scoped `<style>` blocks, six CSS-variable themes

## Getting started

```sh
npm install
npm run dev -- --open   # dev server
npm run check            # type-check (svelte-check)
npm run build             # production build
npm run preview           # serve the production build locally
```

## Structure

```
src/
  app.html                 Document shell + pre-hydration theme script (no flash)
  lib/
    actions/                Svelte actions replacing the old site.js behavior engine
                             (reveal, countUp, magnetic, hoverZoom, parallax, tilt,
                             marquee) plus a shared rAF scheduler and pointer tracker
    components/
      layout/                Site chrome: SiteHeader, SiteFooter, OverlayMenu,
                             ThemeSwitcher, CustomCursor, GrainOverlay, PageTransition
      ui/                    Reusable primitives: FloatingInput/Textarea/Select,
                             Marquee, HorizontalScroll, SectionLabel, HeroIndex
      common/                ResponsiveImage (plain <img>, replaces the old
                             design-tool-only <image-slot> custom element)
    config/                 site.ts (constants), nav.ts (single source of nav links)
    data/                   Typed content per page (home.ts, about.ts, careers.ts, …)
    stores/                 Svelte 5 rune-based stores: theme.svelte.ts, menu.svelte.ts
    styles/                 theme.css (design tokens), base.css (reset), motion.css
    types.ts                 Shared content interfaces
  routes/
    +layout.svelte          Global chrome (header/footer/cursor/transitions)
    +page.svelte             Home
    about/, capabilities/, careers/, contact/, industries/, projects/,
    projects/phoenix/        One route per page; project detail lives at
                             /projects/phoenix (was project-phoenix.dc.html)
```

## Notes on the port

- **Theming**: six themes (light/dark/blueprint/concrete/safety/steel) via a
  `data-theme` attribute on `<html>`, same CSS variables as the original.
  Persisted to `localStorage`; applied by an inline script in `app.html`
  before hydration so there's no flash on load.
- **Motion**: the original's single global `site.js` engine is now a set of
  small, typed Svelte actions (`use:reveal`, `use:magnetic`, …), all sharing
  one `requestAnimationFrame` loop (`lib/actions/scheduler.ts`) instead of
  each effect polling independently. All motion respects
  `prefers-reduced-motion`.
- **Images**: the original used a design-tool-only `<image-slot>` custom
  element (drag-drop placeholder editor tied to that tool's runtime). That's
  replaced with a plain `ResponsiveImage` component — same photos, real
  `<img>` tags, lazy-loaded.
- **Page transitions**: driven by SvelteKit's `onNavigate`/`afterNavigate`
  hooks rather than intercepting link clicks, so it composes correctly with
  SvelteKit's router and prefetching.
