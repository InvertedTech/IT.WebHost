# IT.WebHost

A Blazor Server web application for the Timcast IT team, built on [BlazorBlueprint](https://blazorblueprintui.com) and Tailwind CSS.

---

## Solution Structure

```
IT.WebHost/
├── IT.WebHost.slnx               # Solution file
├── IT.WebHost.App/               # Blazor Server web application (entry point)
└── IT.WebHost.Components/        # Razor Class Library — shared UI components
```

### IT.WebHost.App

The runnable Blazor Server application. Handles routing, layout, and page-level concerns. References `IT.WebHost.Components` for shared components.

Key files:

| Path                                | Purpose                                                                               |
| ----------------------------------- | ------------------------------------------------------------------------------------- |
| `App.razor`                         | Root HTML shell — loads CSS, scripts, sets render mode                                |
| `Routes.razor`                      | Router with 404 fallback                                                              |
| `Shared/MainLayout.razor`           | Top-nav header, footer, dark mode toggle                                              |
| `Shared/MainLayout.razor.cs`        | Code-behind for dark mode JS interop                                                  |
| `Pages/`                            | Application pages (`@page` components)                                                |
| `wwwroot/styles/themes/default.css` | CSS custom properties (colors, radius, etc.) — must load before `blazorblueprint.css` |
| `wwwroot/css/app-input.css`         | Tailwind CSS source file — scans `Pages/` and `Shared/` for utility classes           |
| `wwwroot/css/app.css`               | Compiled Tailwind CSS output — committed, rebuilt when layout/page classes change     |
| `tailwind.config.js`                | Tailwind v3 config — maps CSS variables to Tailwind color tokens                      |

### IT.WebHost.Components

A [Razor Class Library (RCL)](https://learn.microsoft.com/en-us/aspnet/core/razor-pages/ui-class) containing shared, reusable Blazor components. All BlazorBlueprint NuGet packages are declared here and flow to consumers transitively.

Add new shared components here. They are automatically available in `IT.WebHost.App` and any other project that references this library.

---

## Stack

| Layer                        | Technology                                                                                            |
| ---------------------------- | ----------------------------------------------------------------------------------------------------- |
| Framework                    | [.NET 10](https://dotnet.microsoft.com/) — Blazor Server with Interactive Server render mode          |
| UI Library                   | [BlazorBlueprint](https://blazorblueprintui.com) v3.5.x — shadcn/ui-inspired Blazor component library |
| Icons                        | [BlazorBlueprint Lucide Icons](https://blazorblueprintui.com/docs/icons) v2.x                         |
| CSS                          | [Tailwind CSS](https://tailwindcss.com) v3 — utility classes compiled from `app-input.css`            |
| Theming                      | CSS custom properties (OKLCH color space) via `styles/themes/default.css`                             |
| Popover/Dropdown positioning | [Floating UI](https://floating-ui.com) — loaded from CDN at runtime                                   |

---

## NuGet Packages

All declared in `IT.WebHost.Components.csproj` and available to all referencing projects:

| Package                               | Version | Purpose                                                    |
| ------------------------------------- | ------- | ---------------------------------------------------------- |
| `BlazorBlueprint.Components`          | 3.\*    | Core UI components (buttons, cards, dialogs, toasts, etc.) |
| `BlazorBlueprint.Primitives`          | 3.\*    | Headless/unstyled primitive components and services        |
| `BlazorBlueprint.Icons.Lucide`        | 2.\*    | Lucide icon set via `<LucideIcon Name="..." />`            |
| `Microsoft.AspNetCore.Components.Web` | 10.x    | ASP.NET Core Blazor web components (base)                  |

---

## CSS Architecture

There are three CSS layers, loaded in this order in `App.razor`:

1. **`styles/themes/default.css`** — defines all CSS custom properties (`--primary`, `--background`, `--border`, etc.) in OKLCH color space. Edit this file to change the theme. Must load first.

2. **`_content/BlazorBlueprint.Components/blazorblueprint.css`** — pre-compiled styles for BlazorBlueprint components. Served automatically from the NuGet package. Do not modify.

3. **`css/app.css`** — compiled Tailwind CSS output containing utility classes used in layout and page components. Rebuilt by running Tailwind against `app-input.css`.

### Rebuilding app.css

If you add new Tailwind utility classes to pages or layout files, recompile:

```bash
npx tailwindcss -i wwwroot/css/app-input.css -o wwwroot/css/app.css
```

Or watch mode during development:

```bash
npx tailwindcss -i wwwroot/css/app-input.css -o wwwroot/css/app.css --watch
```

### Dark Mode

Dark mode is toggled by adding/removing the `dark` class on `<html>`. Two JS functions are defined globally in `App.razor`:

```js
window.toggleDarkMode = () => document.documentElement.classList.toggle('dark');
window.isDarkModeEnabled = () =>
	document.documentElement.classList.contains('dark');
```

`MainLayout.razor.cs` calls these via `IJSRuntime` to sync the toggle button state.

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org) (for Tailwind CSS compilation)

### Run

```bash
cd IT.WebHost.App
dotnet run
```

### Build

```bash
dotnet build
```

### Tailwind (first time or after layout changes)

```bash
cd IT.WebHost.App
npm install -g tailwindcss   # or: npx tailwindcss ...
npx tailwindcss -i wwwroot/css/app-input.css -o wwwroot/css/app.css
```

---

## Adding Components

1. Create a `.razor` file in `IT.WebHost.Components/`
2. Use the namespace `IT.WebHost.Components` (set via `RootNamespace` in the csproj)
3. The component is immediately available in `IT.WebHost.App` — no additional imports needed (the namespace is in `_Imports.razor` for both projects)

Example:

```razor
<!-- IT.WebHost.Components/StatusBadge.razor -->
<BbBadge Variant="@Variant">@Text</BbBadge>

@code {
    [Parameter] public string Text { get; set; } = "";
    [Parameter] public BadgeVariant Variant { get; set; } = BadgeVariant.Default;
}
```
