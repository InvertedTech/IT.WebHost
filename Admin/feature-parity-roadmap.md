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
- [ ] **Asset upload** (`ImageAssetForm`) — upload UI exists but isn't wired to actual asset creation; also lacks client-side image compression that Next.js has.
    - Blazor: `IT.WebHost/Admin/Components/Common/ImageAssetForm.razor`
    - Next.js: `it.admin-web/src/app/(dashboard)/assets/page.tsx` (no dedicated form file)
- [x] **Personalization Settings** — read-only, no Save handler at all.
    - Blazor: `IT.WebHost/Admin/Components/Pages/Settings/Personalization/PersonalizationSettings.razor`
    - Next.js: `it.admin-web/src/app/(dashboard)/settings/personalization/page.tsx`
- [x] **Notification Settings** — loads Sendgrid config but has no Save button/handler.
    - Blazor: `IT.WebHost/Admin/Components/Pages/Settings/Notification/NotificationSettings.razor`
    - Next.js: `it.admin-web/src/app/(dashboard)/settings/notifications/page.tsx`
- [x] **Subscription Settings** — `HandleSave` is a TODO stub; tiers/rules/processor toggles don't persist.
    - Blazor: `IT.WebHost/Admin/Components/Pages/Settings/Subscription/PaymentSettings.razor`
    - Next.js: `it.admin-web/src/app/(dashboard)/settings/subscriptions/page.tsx`
- [ ] **Merch "Sync Global"** button — handler is empty.
    - Blazor: `IT.WebHost/Admin/Components/Pages/Settings/Merch/MerchSettings.razor`
    - Next.js: `it.admin-web/src/app/(dashboard)/settings/merch/page.tsx`
- [ ] **User account enable/disable** — "Disable Account" button exists in UI only, not wired.
    - Blazor: `IT.WebHost/Admin/Components/Pages/Users/ViewUser.razor`
    - Next.js: `it.admin-web/src/app/(dashboard)/users/[userId]/page.tsx`
- [ ] **User subscription cancel** — `SubscriptionCard` is rendered read-only (`ShowUpdate=false`); no cancel action.
    - Blazor: not found (no subscription card/cancel control on `ViewUser.razor`)
    - Next.js: `it.admin-web/src/components/users/view-user/user-subscriptions.tsx` (+ `subscription-list-table.tsx`)

---

## Priority 2 — Entire modules missing from Blazor

- [ ] **Careers module** — no `/careers`, `/careers/create`, `/careers/{id}` pages at all. Note: `CareersInterface` is already DI-registered and unused — client wiring is a head start.
    - Blazor: not found
    - Next.js: `it.admin-web/src/app/(dashboard)/careers/page.tsx`, `careers/create/page.tsx`, `careers/[careerId]/page.tsx`; actions: `it.admin-web/src/app/actions/careers.ts`
- [ ] **Audit Log viewer** — no page or nav item; Next has a read-only paginated `/audit-log`.
    - Blazor: not found
    - Next.js: `it.admin-web/src/app/(dashboard)/audit-log/page.tsx`
- [ ] **Comment moderation UI** — no comment section on content detail page (view/pin/unpin/delete/undelete). `CommentInterface` is registered but unused.
    - Blazor: not found
    - Next.js: `it.admin-web/src/app/(dashboard)/comments/page.tsx`; actions: `it.admin-web/src/app/actions/comments.ts`
- [ ] **QR membership verification** — no equivalent of `/users/verify-qr`.
    - Blazor: not found
    - Next.js: `it.admin-web/src/app/(dashboard)/users/verify-qr/page.tsx`
- [ ] **Dedicated login/auth pages** — Blazor login is an inline form in the layout's unauthorized branch. Missing dedicated `/login`, `/login-failed`, `/logged-out` routes and SSO scaffolding.
    - Blazor: `IT.WebHost/Admin/Layout/MainLayout.razor` (inline login form, ~lines 193-220)
    - Next.js: `it.admin-web/src/app/(auth)/login/page.tsx`, `login-failed/page.tsx`, `logged-out/page.tsx`
- [ ] **Settings hub page** (`/settings`) — Blazor only exposes settings via sidebar sub-nav; Next has a card-grid landing page too.
    - Blazor: not found
    - Next.js: `it.admin-web/src/app/(dashboard)/settings/page.tsx`

---

## Priority 3 — UX / structural gaps on existing pages

- [ ] **List filtering** — Content, Users, and Assets list pages in Blazor have only basic pagination. Next has title/author/type/access-level/channel filters (Content), search+roles+date-range filters (Users), and URL-synced/shareable filter state throughout.
    - Blazor: `IT.WebHost/Admin/Components/Pages/Content/Content.razor`, `.../Users/Users.razor`, `.../Assets/Assets.razor`
    - Next.js: `it.admin-web/src/app/(dashboard)/content/page.tsx`, `.../users/page.tsx`, `.../assets/page.tsx`
- [ ] **"Include deleted" toggle** — missing on all Blazor list views; present on Users and Careers in Next.
    - Blazor: not found
    - Next.js: `it.admin-web/src/app/(dashboard)/users/page.tsx`, `it.admin-web/src/app/(dashboard)/careers/page.tsx`
- [ ] **Role-based UI guards** — several Blazor pages (Users create/list, Assets, CMS/Merch/Personalization/Notification settings) have `// TODO` comments for missing/inconsistent `<AuthorizeView>` gating. Next applies RBAC consistently per-route via `rbac.ts`.
    - Blazor: `Components/Pages/Users/Users.razor.cs`, `Assets.razor.cs`, `Content/Content.razor.cs`, `Content/CreateContent.razor.cs`, `Content/ViewContent.razor.cs`, `Settings/Notification/NotificationSettings.razor(.cs)`, `Settings/Comments/CommentsSettings.razor(.cs)`, `Settings/Personalization/PersonalizationSettings.razor(.cs)`, `Home.razor`, `Common/Selects/ImagePicker/ImageGalleryDialog.razor`, `Common/Selects/AuthorSelect.razor` (all under `IT.WebHost/Admin/`)
    - Next.js: `it.admin-web/src/lib/rbac.ts` (reference implementation)
- [ ] **Command palette (Ctrl+K)** — exists in Blazor but only has stub nav items (Home/Counter/Weather); needs real content.
    - Blazor: `IT.WebHost/Admin/Components/Common/SpotlightCommandPalette.razor`
    - Next.js: not found (no command palette equivalent)

---

## Not gaps — Blazor is ahead or at parity, no action needed

- Dashboard: Blazor has 8 KPI cards + 5 chart types vs Next's lighter KPI + calendar + activity feed.
- Theme switcher: Blazor has a 10-theme picker plus dark mode; Next only has light/dark.
- MFA/TOTP device view + revoke: roughly at parity in both apps.
- Merch nav placement is inconsistent in _both_ apps (Next omits it from the sidebar array; Blazor has it under settings) — worth a follow-up decision, not a Blazor deficiency.

---

## Suggested sequencing

1. Priority 1 items first — each is a small, contained fix (wire an existing form to an existing gRPC client call) and closes silent-failure risk.
2. Priority 2, roughly in this order: Careers (client already registered) → Audit Log → Comment moderation (client already registered) → dedicated login pages → QR verification → settings hub.
3. Priority 3 as ongoing polish alongside 1 and 2.
