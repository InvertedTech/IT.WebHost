# Feature Parity Roadmap: IT.WebHost/Admin vs it.admin-web

Audit date: 2026-07-03

## Stack Summary

- **it.admin-web**: Next.js 15 (App Router), React 19, TypeScript. Server Actions + REST proxy routes to backend. JWT cookie auth with per-route RBAC (`requireRole`/`requireAnyRole`, 14 roles).
- **IT.WebHost/Admin**: Blazor Server (net10.0), `NeoUI.Blazor` component kit, gRPC clients to backend microservices. JWT cookie auth via custom `/auth/set-cookie`/`/auth/logout`, mix of a global `RequireAnyAdminRole` policy and per-page `<AuthorizeView Roles="...">` checks.

Both share the same backend role model and protobuf contracts — Blazor is the intended successor UI to the Next.js admin.

---

## Priority 1 — Wired UI that silently does nothing on submit

These are the most dangerous gaps: the page looks complete, but the save/create action is a TODO stub. Fix before anything else.

- [x] **Create User** (`/users/create`) — submit handler is a TODO stub. Wire to the equivalent of `POST /auth/admin/createuser`.
    - Blazor: `IT.WebHost/Admin/Components/Pages/Users/CreateUser.razor`
    - Next.js: `it.admin-web/src/components/forms/admin-create-user-form.tsx`
- [x] **Role grant/edit dialog** (`GrantRolesDialog`) — save handler is a TODO stub.
    - Blazor: `IT.WebHost/Admin/Components/Pages/Users/ViewUser.razor` (inline, no separate dialog file)
    - Next.js: `it.admin-web/src/components/users/edit-user/edit-other-user-dialog.tsx`
- [x] **Password reset dialog** (`ResetPasswordDialog`) — save handler is a TODO stub; also missing confirm-password validation.
    - Blazor: `IT.WebHost/Admin/Components/Pages/Users/ViewUser.razor` (inline)
    - Next.js: `it.admin-web/src/components/users/edit-user/change-other-password-dialog.tsx`
- [x] **Asset upload** (`ImageAssetForm`) — upload UI exists but isn't wired to actual asset creation; also lacks client-side image compression that Next.js has.
    - Blazor: `IT.WebHost/Admin/Components/Common/ImageAssetForm.razor`
    - Next.js: `it.admin-web/src/app/(dashboard)/assets/page.tsx` (no dedicated form file)
    - Wired the creation path: `ImageAssetForm` now binds Title/Caption, injects `AssetClient`/`IToastService`, and its own "Create Asset" button calls `AssetClient.Create` and raises `OnCreated`. Fixed the same dead "Create" button in the other consumer, `ImageGalleryDialog`'s upload tab, so it now creates and auto-selects the asset instead of doing nothing. Added a "New Asset" button + dialog on `Assets.razor` that uses the form directly and reloads the list on success.
    - **Not done**: client-side image compression (mentioned in the same roadmap line) — still relies on the raw uploaded bytes; Next.js's compression step has no Blazor equivalent yet.
- [x] **Personalization Settings** — read-only, no Save handler at all.
    - Blazor: `IT.WebHost/Admin/Components/Pages/Settings/Personalization/PersonalizationSettings.razor`
    - Next.js: `it.admin-web/src/app/(dashboard)/settings/personalization/page.tsx`
- [x] **Notification Settings** — loads Sendgrid config but has no Save button/handler.
    - Blazor: `IT.WebHost/Admin/Components/Pages/Settings/Notification/NotificationSettings.razor`
    - Next.js: `it.admin-web/src/app/(dashboard)/settings/notifications/page.tsx`
- [x] **Subscription Settings** — `HandleSave` is a TODO stub; tiers/rules/processor toggles don't persist.
    - Blazor: `IT.WebHost/Admin/Components/Pages/Settings/Subscription/PaymentSettings.razor`
    - Next.js: `it.admin-web/src/app/(dashboard)/settings/subscriptions/page.tsx`
- [x] **Merch "Sync Global"** button — handler is empty.
    - Blazor: `IT.WebHost/Admin/Components/Pages/Settings/Merch/MerchSettings.razor`
    - Next.js: `it.admin-web/src/app/(dashboard)/settings/merch/page.tsx`
    - Wired to `AdminMerchInterface` (`MerchBulkActionStart`/`Status`/`Cancel`, `Action = PullFromAll`), which wasn't even DI-registered — added it to `IT.WebServices/Clients/DIExtensions.cs`. On click, starts the bulk action and polls `MerchBulkActionStatus` every 1.5s via a `PeriodicTimer`, updating a `Progress` bar + status message; a Cancel button appears while running and calls `MerchBulkActionCancel`. Also resumes/shows progress on page load if a sync is already running (e.g. started by another admin or a prior page load). Note: the backend removes a bulk job from `RunningActions` the instant it finishes or is canceled (`BulkHelper.CheckAll`/`CancelAction`), so there's no way to distinguish "completed" from "canceled" from the status response alone — the UI infers it from whether *this* browser session clicked Cancel.
- [x] **User account enable/disable** — "Disable Account" button exists in UI only, not wired.
    - Blazor: `IT.WebHost/Admin/Components/Pages/Users/ViewUser.razor`
    - Next.js: `it.admin-web/src/app/(dashboard)/users/[userId]/page.tsx`
- [x] **User subscription cancel** — `SubscriptionCard` is rendered read-only (`ShowUpdate=false`); no cancel action.
    - Blazor: `IT.WebHost/Admin/Components/Common/Payment/SubscriptionCard.razor`
    - Next.js: `it.admin-web/src/components/users/view-user/user-subscriptions.tsx` (+ `subscription-list-table.tsx`)
    - This note was stale by the time we got to it: `ShowUpdate=false` only suppresses the "Update Card" button — the Cancel button and its `AdminPaymentInterface.CancelOtherSubscriptionAsync` call were already fully wired (`ShowUpdate` never gated it). The real gap versus Next.js was that Blazor's cancel fired immediately on click with zero confirmation, unlike Next's `AlertDialog` confirmation. Wired the existing (previously unused anywhere) `ConfirmDialog` component in front of it — clicking "Cancel Subscription" now opens a confirm dialog ("This will cancel the subscription immediately and stop future renewals.") before the request fires. Also added `Reason = "Canceled via admin portal"` to the request, matching what Next.js sends.

---

## Priority 2 — Entire modules missing from Blazor

- [x] **Careers module** — no `/careers`, `/careers/create`, `/careers/{id}` pages at all. Note: `CareersInterface` is already DI-registered and unused — client wiring is a head start.
    - Blazor: not found
    - Next.js: `it.admin-web/src/app/(dashboard)/careers/page.tsx`, `careers/create/page.tsx`, `careers/[careerId]/page.tsx`; actions: `it.admin-web/src/app/actions/careers.ts`
- [x] **Audit Log viewer** — no page or nav item; Next has a read-only paginated `/audit-log`.
    - Blazor: not found
    - Next.js: `it.admin-web/src/app/(dashboard)/audit-log/page.tsx`
- [x] **Comment moderation UI** — no comment section on content detail page (view/pin/unpin/delete/undelete). `CommentInterface` is registered but unused.
    - Blazor: not found
    - Next.js: `it.admin-web/src/app/(dashboard)/comments/page.tsx`; actions: `it.admin-web/src/app/actions/comments.ts`
- [x] **QR membership verification** — no equivalent of `/users/verify-qr`.
    - Blazor: `IT.WebHost/Admin/Components/Pages/Users/VerifyQr.razor`
    - Next.js: `it.admin-web/src/app/(dashboard)/users/verify-qr/page.tsx`
    - Turned out to be a much thinner feature than the name suggests: the Next.js page makes **no API/gRPC call at all** — it's a pure display page that reads `valid`/`name`/`level`/`reason` straight from the URL query string and renders a green "verified" or red "invalid" card. There is no QR-verification RPC anywhere in `IT.WebServices` (confirmed via proto search) — the actual scan/verify logic must live outside this repo (e.g. baked into the QR payload or an external scanner redirecting here with a precomputed verdict). Ported it faithfully as the same query-string-driven display page, gated with `RoleAbilities.ROLE_IS_MEMBER_MANAGER_OR_HIGHER` (matching Next's `isMemberManagerOrHigher`). Also matched Next.js in **not** adding a sidebar/nav entry — Next's own sidebar has none either, since the page is meant to be reached via an external redirect, not manual navigation.
- [ ] **Dedicated login/auth pages** — Blazor login is an inline form in the layout's unauthorized branch. Missing dedicated `/login`, `/login-failed`, `/logged-out` routes and SSO scaffolding.
    - Blazor: `IT.WebHost/Admin/Layout/MainLayout.razor` (inline login form, ~lines 193-220)
    - Next.js: `it.admin-web/src/app/(auth)/login/page.tsx`, `login-failed/page.tsx`, `logged-out/page.tsx`

---

## Priority 3 — UX / structural gaps on existing pages

- [x] **List filtering** — Content, Users, and Assets list pages in Blazor have only basic pagination. Next has title/author/type/access-level/channel filters (Content), search+roles+date-range filters (Users), and URL-synced/shareable filter state throughout.
    - Blazor: `IT.WebHost/Admin/Components/Pages/Content/Content.razor`, `.../Users/Users.razor`, `.../Assets/Assets.razor`
    - Next.js: `it.admin-web/src/app/(dashboard)/content/page.tsx`, `.../users/page.tsx`, `.../assets/page.tsx`
    - Note: built only what the backend already supports, confirmed against the protos.
        - Content (`GetAllContentAdminRequest`): added Type, Channel, Category, Live-only filters. No `Title`/`Author` fields exist on this admin request, so those were **not** added (Next's title/author filters have no backend equivalent here yet).
        - Users: `GetAllUsersRequest` had no filter fields at all, so the page was switched to the richer `SearchUsersAdmin` RPC (already used elsewhere, e.g. `AuthorSelect.razor`) — adds Search, Roles, Created-after/before. Row/table now bind to `UserSearchRecord` instead of `UserNormalRecord`.
        - Assets (`SearchAssetRequest`): added Query and Type filters only — no date-range or delete-state field exists on this request server-side.
    - Also added `Paginator.ExtraQuery` (`Components/Navigation/Paginator.razor`) so filter state survives page navigation for any page that needs it.
- [x] **"Include deleted" toggle** — missing on all Blazor list views; present on Users and Careers in Next.
    - Blazor: `IT.WebHost/Admin/Components/Pages/Users/Users.razor`, `.../Careers/ListCareers.razor`
    - Next.js: `it.admin-web/src/app/(dashboard)/users/page.tsx`, `it.admin-web/src/app/(dashboard)/careers/page.tsx`
    - Added on Users (`SearchUsersAdminRequest.IncludeDeleted`) and Careers (`AdminListCareersRequest.IncludeDeleted`) — the two list types with backend support, matching Next. Also added a Deleted/Active status column on Careers since `CareerListRecord.DeletedOnUTC` is already returned. Content's admin request separately exposes a `Deleted` bool (different shape, no per-row deleted flag on `ContentListRecord`) — folded into the Content filter bar above rather than listed as a second toggle here. Assets has no delete-state field anywhere server-side, so no toggle was added there.
- [ ] **Role-based UI guards** — several Blazor pages (Users create/list, Assets, CMS/Merch/Personalization/Notification settings) have `// TODO` comments for missing/inconsistent `<AuthorizeView>` gating. Next applies RBAC consistently per-route via `rbac.ts`.
    - Blazor: `Components/Pages/Users/Users.razor.cs`, `Assets.razor.cs`, `Content/Content.razor.cs`, `Content/CreateContent.razor.cs`, `Content/ViewContent.razor.cs`, `Settings/Notification/NotificationSettings.razor(.cs)`, `Settings/Comments/CommentsSettings.razor(.cs)`, `Settings/Personalization/PersonalizationSettings.razor(.cs)`, `Home.razor`, `Common/Selects/ImagePicker/ImageGalleryDialog.razor`, `Common/Selects/AuthorSelect.razor` (all under `IT.WebHost/Admin/`)
    - Next.js: `it.admin-web/src/lib/rbac.ts` (reference implementation)
- [x] **Command palette (Ctrl+K)** — exists in Blazor but only has stub nav items (Home/Counter/Weather); needs real content.
    - Blazor: `IT.WebHost/Admin/Components/Common/SpotlightCommandPalette.razor`
    - Next.js: not found (no command palette equivalent)
    - Replaced the leftover template items with the real site nav: `MainLayout.razor.cs` already builds `navItems`/`settingsNavItems` (`List<AdminNavItem>`, the same data driving the sidebar) — passed those into the palette as parameters instead of duplicating a second hardcoded list, so the two stay in sync automatically. Each item is gated with the same `<AuthorizeView Roles="...">` pattern the sidebar uses, so the palette never surfaces a page the current user can't open. Added a third "Quick Actions" group (Create Content, Create User, New Career) pointing at the existing creation routes, gated by the same roles those pages themselves require.
    - Fix: `SpotlightCommandPalette` is rendered outside `MainLayout`'s `<CascadingAuthenticationState>` (as a sibling after it closes), so the new `<AuthorizeView>` usage inside it had no cascading `AuthenticationState` to read — wrapped it in its own `<CascadingAuthenticationState>` at the call site in `MainLayout.razor`.

---

## Not gaps — Blazor is ahead or at parity, no action needed

- Dashboard: Blazor has 8 KPI cards + 5 chart types vs Next's lighter KPI + calendar + activity feed.
- Theme switcher: Blazor has a 10-theme picker plus dark mode; Next only has light/dark.
- MFA/TOTP device view + revoke: roughly at parity in both apps.
- Merch nav placement is inconsistent in _both_ apps (Next omits it from the sidebar array; Blazor has it under settings) — worth a follow-up decision, not a Blazor deficiency.

---

## Suggested sequencing

1. Priority 1 items first — each is a small, contained fix (wire an existing form to an existing gRPC client call) and closes silent-failure risk.
2. Priority 2, roughly in this order: Careers (client already registered) → Audit Log → Comment moderation (client already registered) → dedicated login pages → QR verification
3. Priority 3 as ongoing polish alongside 1 and 2.
