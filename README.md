# IT.WebHost

Blazor Server front ends for the IT web services: a public-facing site (`App`) and an admin console (`Admin`). Both are built on [NeoUI](https://www.nuget.org/packages/NeoUI.Blazor) and Tailwind CSS, and talk to the [IT.WebServices](../IT.WebServices) backend over gRPC.

The feature status of the public site, including known stubs, is tracked in [`docs/FEATURE_PARITY.md`](docs/FEATURE_PARITY.md).

---

## Solution Structure

```
IT.WebHost/
├── IT.WebHost.slnx      # Solution file (also pulls in the sibling IT.WebServices projects)
├── App/App/             # Public site: Blazor Server, entry point
├── Admin/               # Admin console: Blazor Server, entry point
├── Components/          # Razor Class Library: UI shared by App and Admin
├── Core/                # Class library: config, DI extensions, auth middleware, services
└── docs/                # Project docs
```

`IT.WebHost` expects the `IT.WebServices` repo to sit next to it (`../IT.WebServices`). Every project references its `Authentication.Shared`, `Clients` and `Fragments` projects directly, so the solution won't restore without it.

### App (`App/App`)

The public site. Handles routing, layout and page-level concerns.

| Path                       | Purpose                                                                          |
| -------------------------- | -------------------------------------------------------------------------------- |
| `Program.cs`               | Service registration, auth policy, middleware                                     |
| `App.razor`                | Root HTML shell: loads CSS and scripts, sets the Interactive Server render mode  |
| `Routes.razor`             | Router with not-found fallback                                                    |
| `Layout/`                  | `MainLayout` and nav                                                              |
| `Components/Pages/`        | Routable pages (`@page`): home, content, members, merch, profile, subscribe, etc.|
| `Components/Common/`       | Reusable pieces used by the pages (cards, feeds, video player, comments, etc.)   |
| `Controllers/`             | MVC controllers for things that can't be Blazor pages (`/auth/*`, `/subscribe/success`) |
| `styles/app-input.css`     | Tailwind source file                                                              |
| `styles/theme.css`         | Theme tokens (colors, radius, etc.)                                               |
| `wwwroot/css/app.css`      | Compiled Tailwind output (see [CSS](#css))                                        |

### Admin (`Admin`)

The admin console: content, users, careers, assets and site settings (CMS, comments, merch, notifications, payments, personalization). Same stack as `App`, with its own `styles/` and `wwwroot/css/app.css`.

### Components (`Components`)

A [Razor Class Library](https://learn.microsoft.com/en-us/aspnet/core/razor-pages/ui-class) for UI shared between `App` and `Admin`: `Comments/`, `Navigation/` (the `Paginator`), `Assets/` (`FeaturedImage`) and `Overlay/` (`SaveChangesBar`). Its `_Imports.razor` adds the `IT.WebHost.Shared` namespace, so its components are available in both apps without further imports.

### Core (`Core`)

Non-UI code shared by both apps: `AppSettings`, DI extensions (gRPC clients, auth, ProtoValidate), the JWT cookie authentication middleware, and `SiteSettingsService`.

---

## Stack

| Layer      | Technology                                                                                       |
| ---------- | ------------------------------------------------------------------------------------------------ |
| Framework  | .NET 10, Blazor Server with Interactive Server render mode                                        |
| UI library | NeoUI (`NeoUI.Blazor` 4.0.x, `NeoUI.Blazor.Primitives`), a shadcn/ui-style component library     |
| Icons      | `NeoUI.Icons.Lucide`, used as `<LucideIcon Name="..." />`                                         |
| CSS        | Tailwind CSS v4, compiled with `@tailwindcss/cli`                                                 |
| Backend    | IT.WebServices over gRPC (generated clients in `IT.WebServices.Clients` / `Fragments`)            |
| Validation | [ProtoValidate](https://www.nuget.org/packages/ProtoValidate), driven by the proto definitions     |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org), used to compile Tailwind as part of the build
- The `IT.WebServices` repo checked out next to this one, with its API running (default `http://localhost:8001/api`)

### Run

```bash
# Public site: http://localhost:5002 (https://localhost:5003)
dotnet run --project App/App

# Admin console: http://localhost:5000 (https://localhost:5001)
dotnet run --project Admin
```

Each project's `Properties/launchSettings.json` also defines profiles that set the development JWT keys the apps use to validate the auth cookie. Use one of those profiles (or set the same variables yourself) if login fails.

### Build

```bash
dotnet build
```

The build runs `npm install` if `node_modules` is missing, then `npm run build:css`, so a normal build also regenerates the CSS.

### Configuration

Settings live under `AppSettings` in each project's `appsettings.json`:

| Key            | Default                     | Used by      | Purpose                                                                 |
| -------------- | --------------------------- | ------------ | ----------------------------------------------------------------------- |
| `API_BASE_URL` | `http://localhost:8001/api` | App, Admin   | Base URL of the IT.WebServices API (also used to build asset image URLs) |
| `APP_BASE_URL` | `http://localhost:5003`     | App          | Public URL of this site; used for payment success/cancel redirects       |

---

## CSS

Tailwind v4 scans the `.razor`, `.html` and `.cs` files (see the `@source` lines in `styles/app-input.css`) and writes `wwwroot/css/app.css`. That file is committed and regenerated on every build, so it will show up as modified after you add or change utility classes. Commit it along with the change that caused it.

For faster iteration, run the watcher from inside the project folder:

```bash
cd App/App        # or: cd Admin
npm run watch:css
```

### Dynamic class names

Tailwind only generates classes it can find as literal strings in your source. A class that a component library assembles at runtime never gets generated. If a NeoUI parameter that is supposed to change styling has no visible effect, this is a likely cause. Size or style the element yourself with a literal class or a scoped `.razor.css` file.

### Scoped CSS

A component can have a sibling `Component.razor.css` file (for example `MerchCard.razor.css`). Its rules are scoped to that component. Use `::deep` to reach elements rendered by child components.

---

## Adding Components

**Shared between App and Admin:** add a `.razor` file under `Components/`. No extra imports are needed in either app.

**Used by one app only:** add it under that project's `Components/Common/` folder (`App/App/Components/Common/` or `Admin/Components/Common/`).

Example:

```razor
<!-- Components/Overlay/StatusBadge.razor -->
<Badge Variant="@Variant">@Text</Badge>

@code {
    [Parameter] public string Text { get; set; } = "";
    [Parameter] public BadgeVariant Variant { get; set; } = BadgeVariant.Default;
}
```
