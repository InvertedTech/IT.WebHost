# Feature Parity: IT.WebHost.App vs. prototype.web

`IT.WebHost.App` (this repo, Blazor) is the open-source counterpart to `prototype.web`
(Next.js), the closed-source production site. This doc tracks parity in **pages, flows, and
components only** — layout, branding, and theming are intentionally out of scope
since those will be configured through the CMS.

Sources reviewed: `prototype.web/src/app/**/page.tsx` route tree, its component
directories, and a live pass over the prototype site. Compared against
`App/App/Components/Pages/**` and `App/App/Components/Common/**` in this repo.

Last reviewed: 2026-09-21.

## Route comparison

| Route | prototype.web | IT.WebHost.App | Status |
|---|---|---|---|
| `/` | Home feed | `Home.razor` | OK |
| `/watch` | Channel-grouped video hub (`getContentGroupedByChannels`, ordered gallery) | — | **Missing** — see note below on unified `/content` routing |
| `/written` | Article listing (filtered `ContentType.ContentWritten`) | — | **Missing** — see note below on unified `/content` routing |
| `/channel/[channelId]` | Per-channel content page w/ pagination + SEO metadata | — | **Missing** |
| `/members` | Channel showcase sections + Discord/perks intro | `MembersArea.razor` | **Partial** — showcase sections exist, intro/CTA block missing |
| `/search` | Search + results grid | `SearchContent.razor` (`/content`) | OK |
| `/shop` | `Catalog` w/ category filter, price range, in-stock filter, pagination | `Merch.razor` | **Partial** — pagination and load-error state done; no filters |
| `/login` | Login form | `Login.razor` | OK |
| `/join-us` | Signup | `Signup.razor` | OK |
| `/forgot-password`, `/reset-password` | Password recovery | `ForgotPassword.razor`, `ResetPassword.razor` | OK — minor polish only (no loading state; reset skips ProtoValidate) |
| `/subscribe`, `/subscribe/[level]`, `/subscribe/cancel`, `/subscribe/success`, `/subscribe/fortis` | Tier selection, Fortis payment flow, cancel/success pages | `Subscribe/Subscribe.razor`, `SubscribeController` (`/subscribe/success`) | **Partial** — Stripe only; Fortis path commented out; `/subscribe/cancel` has no route (404); Cancel button is an empty method |
| `/profile` | `ProfileClient`: details, change password, subscriptions list, TOTP devices | `Profile.razor` | **Partial** — details edit works; subscriptions card, password change, and MFA device list/removal are commented out; no save/error toasts |
| `/profile/library` | Saved/liked content ("Library") | — | **Missing** |
| `/profile/subscription/stripe/updatecard(/cancel|/finish)` | Update payment method flow | — | **Missing** |
| `/discord` | Discord link/verify flow, tier→role mapping, server widget | — | **Out of scope for now** — will be a CMS page module |
| `/jobs`, `/jobs/[jobId]` | Public careers listing + detail (backed by existing Careers proto) | — | **Missing** (Careers CRUD exists in `Admin`, but no public-facing page consumes it) |
| `/about` | About page | — | **Out of scope for now** — will be a CMS page module |
| Video/article detail (`/watch/video/[slug]`, `/written/[slug]`) | Player/body + metadata + comments + related content | `ViewContent.razor` (`/content/{slug}`) | **Partial** — matches structurally; no related-content section |

> **Routing decision**: unlike prototype.web, which splits video and article detail
> pages across `/watch/video/[slug]` and `/written/[slug]`, this app keeps a single
> content route (`/content/{slug}` via `ViewContent.razor`) for both types, switching
> on `ContentDataOneofCase` as it already does. The "Watch" and "Read" landing pages
> should follow the same pattern: build them as filtered views over `/content`
> (e.g. `/content?type=video`, `/content?type=written`) rather than introducing
> separate `/watch` and `/written` routes — reuse `ContentShowcaseSection` the way
> `MembersArea.razor` already does, just with a content-type filter applied.

## Component-level gaps

- **Merch/shop**: pagination and a load-error alert are now in place
  (`Merch.razor` + `MerchClient.Search`). Still missing: category filter
  (`coffee`/`apparel`/`skateboards`), price range or in-stock-only
  filter, no featured/category showcase sections. prototype.web's `components/products/`
  has `catalog`, `product-filters`, `product-grid`, `category-showcase`, and
  `featured-products` — all with loading-skeleton counterparts.
- **Payments**: `Subscribe.razor` only offers Stripe; Fortis is present in
  prototype.web (`FortisElements.tsx`, `/subscribe/fortis`) but commented out here.
  The provider dropdown is hardcoded rather than driven by each provider's
  `IsEnabled` setting, and `SubmitSubscribe` shows no error when no payment link
  comes back.
  There's also a `subscription-profile-gate-modal` (prompts non-subscribers to
  subscribe when hitting gated content) with no Blazor equivalent.
- **Profile**: no subscription list/cancel/change-plan UI, no change-password card,
  no MFA device list/removal (only a count badge), no "Library" (saved content) page.
  All exist as working components in
  prototype.web (`SubscriptionsCard`, `ChangePasswordCard`, `profile/library`).
- **Discord integration**: entirely absent. Out of scope — handled later by a
  CMS page module on the server, same as `/about`.
- **Careers/Jobs**: backend (`Careers.proto`) and `Admin` CRUD already exist and
  are fully built; only the public listing/detail pages are missing from `App`.
- **Channel pages**: `MembersArea.razor` reuses `ContentShowcaseSection` per
  channel already, so the pattern exists — it just isn't reused for Watch/Read
  filtered views on `/content`, or a dedicated `/channel/{id}` route with its
  own pagination.
- **Video detail**: no "Related Videos" section (prototype.web shows a 4-item
  related carousel below the player).

## Stubbed / non-functional items

Things that exist in `App/App` but don't work yet. Line numbers as of 2026-09-21.

**Broken or dead ends**

1. `CancelSubscription` is an empty method (`Subscribe.razor.cs:76`); the Cancel
   button on the active-subscription view does nothing.
2. `/subscribe/cancel` has no route. `Subscribe.razor.cs:59` passes it as `CancelUrl`,
   but `SubscribeController` only implements `success`, so users land on NotFound.
3. Profile subscriptions list: `GetUserSubs()` body is commented out
   (`Profile.razor.cs:48`, "Figure Out Why This Isn't Working") and `<SubscriptionCard>`
   is commented out (`Profile.razor:179`).
4. Profile MFA devices: list rendering is commented out (`Profile.razor:195`) and
   `DisableTotp` is commented out (`Profile.razor.cs:82`); only a count badge shows.
5. Profile has no toast feedback — every `ToastService` call is commented out
   (`Profile.razor.cs:94,103,130,141`), so save success/failure is silent.
6. Password reset from Profile: `<ResetPasswordDialog>` is commented out
   (`Profile.razor:48`); `isResetPasswordOpen` / `ToggleResetPassword` are unused.

**Half-wired flows**

7. `Subscribe.razor.cs`: three open TODOs — service reports total amount due as null
    (:62), only Stripe supported (:67), no error display (:73).
8. Payment provider dropdown is hardcoded instead of built from provider
    `IsEnabled` settings (`Subscribe.razor:51`).
9. `ForgotPassword` / `ResetPassword`: `isLoading` is set but never bound (no spinner,
    double-submit possible); `ResetPassword` only checks the passwords match, unlike
    `Signup`, which runs ProtoValidate.
10. `ViewContent.razor.cs:16` has an unexplained `// TODO: FIx`; no loading or
    not-found handling.
11. Subscription-level gating on `ContentCard` is commented out (`ContentCard.razor:53`).

**To verify**

- `SubscribeController.Success` only finishes the subscription when `processor` and
  `session_id` are in the query string; `Subscribe.razor.cs:58` builds `SuccessUrl`
  with neither. Confirm the payment provider appends them.

## Already at parity

- Signup fields (first/last/username/display name/email/password/postal code).
- Login form fields (username/email + password, forgot-password link).
- Merch listing with pagination and load-error state (`Merch.razor` + `Paginator`).
- Video player: both Rumble and YouTube embeds supported (`VideoPlayer.razor`
  mirrors `rumble-player.tsx` / `youtube-player.tsx`).
- Content detail page structure: player/body, metadata, tags, live badge,
  like/save (`ContentStatsSection`), share dialog, comments.
- Search with pagination (`SearchContent.razor` + `Paginator`).
- Subscription tier cards (Stripe path) on `/subscribe`.

## Suggested priority order

0. Fix the dead ends first (stub items 1–2): implement `CancelSubscription` and add
   a `/subscribe/cancel` route so the Stripe cancel URL stops 404ing.
1. `/profile` subscription management (wire up the commented-out subscription
   card + cancel/change plan) — most-referenced gap; linked from member gating,
   etc. on the live site. Bring back the MFA list/removal and toasts in the same pass.
2. `/shop` (Merch) filters — pagination is done.
3. Watch/Read filtered views under `/content`, reusing the `ContentShowcaseSection`
   pattern already built for `/members`.
4. `/channel/{channelId}` route for the "View All Content" links in showcase
   sections to point to.
5. Public `/jobs` listing + detail — backend and admin already support it.
6. Related-content section on `ViewContent.razor`.
7. `/profile/library` (saved content).
