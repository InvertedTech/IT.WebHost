# Feature Parity: IT.WebHost.Admin vs. it.admin-web

`IT.WebHost.Admin` (this repo, Blazor, `Admin/`) is the open-source counterpart to
`it.admin-web` (Next.js, `C:\projects\timcast\general\IT\it.admin-web`), the
closed-source admin console. This doc tracks parity in **pages, flows, and
components only** — layout, branding, and theming are intentionally out of scope.

Sources reviewed: `it.admin-web/src/app/**/page.tsx` route tree. Compared against
`Admin/Components/Pages/**` and `Admin/Components/Common/**` in this repo.

Last reviewed: 2026-09-22.

## Route comparison

| Route | it.admin-web | IT.WebHost.Admin | Status |
|---|---|---|---|
| `/` (dashboard) | KPI dashboard | `Home.razor` | OK — backed by real `DashboardInterfaceClient` calls, not mock data |
| `(auth)/login`, `/login-failed`, `/logged-out` | Dedicated auth routes | Inline login form in `MainLayout`'s `NotAuthorized` block | **Different shape, functionally OK** — one form covers login; no separate "login failed" or "logged out" pages, no forgot-password link |
| `/content` | Content listing | `Content.razor` | OK |
| `/content/create` | Create content | `CreateContent.razor` | **Partial** — only Article and Video content types are wired; Picture and Audio are commented out of both the type selector and the content step |
| `/content/[contentId]` | Content detail/edit | `ViewContent.razor` | **Partial** — edit/publish work; unpublish/delete/undelete respond to every request with a success toast without checking `res.Error` |
| `/assets` | Asset library | `Assets.razor` | OK |
| `/users` | User listing | `Users.razor` | OK |
| `/users/[userId]` | User detail | `ViewUser.razor` | OK — reset password, grant roles, subscriptions, TOTP devices, enable/disable all wired |
| `/users/verify-qr` | QR verification | `VerifyQr.razor` | OK |
| `/careers`, `/careers/create`, `/careers/[careerId]` | Careers CRUD | `ListCareers.razor`, `NewCareer.razor`, `ViewCareer.razor` | OK |
| `/audit-log` | Audit log | `AuditLog.razor` | OK |
| `/comments` | Comment moderation queue | — | **Missing** — only comment *settings* exist (`/settings/comments`); no page to review/moderate individual comments |
| `/events` | Events management | — | **Missing** — no route, no components. `RoleAbilities.ROLE_EVENT_MANAGER` / `ROLE_EVENT_TICKET_MANAGER` exist and are referenced by `MainLayout`'s admin-role policy, but nothing in the UI uses them |
| `/settings` (index) | Settings landing page | — | **Missing** — no `/settings` route; the settings nav goes straight to sub-pages |
| `/settings/cms` | CMS settings | `CMSSettings.razor` | **Partial — no role gating** (see Security gaps below) |
| `/settings/merch` | Merch settings | `MerchSettings.razor` | **Partial — no role gating**; provider-credentials UI still stubbed (see below) |
| `/settings/subscriptions` | Subscription/payment settings | `PaymentSettings.razor` (`/settings/subscription`) | **Broken — no role gating, and nothing on the page persists except the two Merch calls elsewhere** (see below) |
| `/settings/personalization` | Personalization settings | `PersonalizationSettings.razor` | **Partial** — image upload fields not implemented (`@*TODO: Add Images*@`) |
| `/settings/notifications` | Notification settings | `NotificationSettings.razor` | OK |
| `/settings/comments` | Comment settings | `CommentsSettings.razor` | OK |
| `/settings/bulk-actions` | Bulk actions (general) | — | **Missing** |
| `/settings/merch-bulk-actions` | Merch bulk actions | Sync Global / Cancel Sync on `/settings/merch` | **Partial equivalent** — global Shopify sync exists inline on the Merch settings page, not as a separate bulk-actions page |
| `/settings/events` | Event settings | — | **Missing** — tied to the missing Events feature |
| `/unauthorized` | Unauthorized page | Inline `NotAuthorized` block in `MainLayout` | **Different shape, functionally OK** — same content is shown in place rather than as a redirect |

## Security gaps (highest priority)

These aren't UI polish — they're pages reachable by any user who passes the
broad `RequireAnyAdminRole` policy (which includes lower roles like Content
Writer, Comment Moderator, Bot Verification, Event Manager, etc.), even though
the **sidebar link** to each is correctly hidden behind `ROLE_IS_ADMIN_OR_OWNER`.
Hiding the nav link doesn't protect the route — only an `<AuthorizeView Roles="…">`
around the page content does, and these three pages don't have one:

1. **`/settings/cms`** (`CMSSettings.razor`) — no `<AuthorizeView>` at all. Any
   authenticated admin-area user can create/edit channels and categories.
2. **`/settings/merch`** (`MerchSettings.razor`) — no `<AuthorizeView>` at all.
   The edit form includes each Shopify store's **Admin API token** in a
   password-type field — reachable (read and, once `HandleSave`/`PersistStores`
   works, write) by any low-privilege admin-area user.
3. **`/settings/subscription`** (`PaymentSettings.razor`) — has a literal
   `@*TODO: Add Role Gating*@` comment and no `<AuthorizeView>`. The page and
   `ProcessorCredentialsCard` expose Stripe/PayPal/Fortis client secrets and API
   keys to the same broad audience.

Compare with `NotificationSettings.razor`, `PersonalizationSettings.razor`, and
`CommentsSettings.razor`, which all correctly wrap their content in
`<AuthorizeView Roles="@RoleAbilities.ROLE_IS_ADMIN_OR_OWNER">` (or stricter).

4. **`GrantRolesDialog`** on `/users/{id}` (`ViewUser.razor:53`) has a literal
   `@*TODO: Add Role Authorization For Grant Roles*@` comment right above it.
   Any user who can reach `ViewUser` (`ROLE_IS_MEMBER_MANAGER_OR_HIGHER`) can
   open it and grant **any** role to **any** user, including Owner/Admin —
   there's no check that the granter already holds the roles they're handing out.

## Stubbed / non-functional items

**Broken — the whole page looks editable but nothing saves**

1. `/settings/subscription`: `HandleSave` is `await PersistPublic(); await PersistStores(); _isEditing = false;`
   but `PersistPublic`/`PersistStores` only exist on `MerchSettings.razor.cs` —
   `PaymentSettings.razor.cs:199` has `// TODO: persist changes via settingsClient`
   and the method does nothing but flip `_isEditing` off. Every field on this
   page — general rules, processor enable/disable, tier pricing — is
   pure UI state that's discarded on save.
2. Same page, `SubscriptionTiersSection`'s "+ Create New Tier" button calls
   `OnCreateTier()`, which is `private void OnCreateTier() { }` — an empty
   method. There is no way to add a subscription tier from the UI.
3. Same page, each processor row's gear/settings icon button
   (`PaymentProcessorsCard.razor:24,40,56,72`) calls `OnStripeSettings` /
   `OnPaypalSettings` / `OnFortisSettings` / `OnCryptoSettings`, which all funnel
   into `OnProcessorSettings(string processor) { }` — also empty.
4. Same page, each processor's enable/disable `<Switch Checked="@Public.Stripe.Enabled" .../>`
   is one-way bound (`Checked`, not `@bind-Checked`) with no `CheckedChanged`
   handler — toggling it while editing doesn't update `Public.Stripe.Enabled`
   at all, on top of (1) not persisting it anyway.

**Content admin**

5. `ViewContent.razor.cs`: `HandleUnpublish`, `HandleDelete`, and `HandleUnDelete`
   all show a success toast unconditionally — none of them check `res?.Error`
   before declaring success, unlike `SaveContent` and `HandlePublish` on the
   same file, which do.
6. `CreateContent.razor:41` — Picture and Audio content types are commented
   out of the type dropdown; `CreateContent.razor:108` — the matching body
   step for Picture is also commented out. Only Written and Video content can
   be authored from Admin, even though `ContentType` has all four values.
7. `AuthorSelect.razor` fetches up to 500 users with content-creation roles on
   every mount (`OnInitializedAsync`, `PageSize = 500`) instead of a lazy
   search — flagged in-code as `// TODO: Make an Actual UserClient with The
   Authors As A Lazy`. It also doesn't default to the current user
   (`// TODO: Set Current User As Author By Default`), so every new piece of
   content starts with no author selected.

**Merch**

8. `MerchSettings.razor:115-120` — a commented-out "View Integration Logs"
   footer link/button.
9. `MerchSettings.razor:121` — `@*TODO: Add Provider Credentials Thing*@`. The
   Shopify store sheet has store name/domain/token/collection IDs, but there's
   no equivalent section for a second commerce provider, if one is planned.

**Personalization**

10. `PersonalizationSettings.razor:40` — `@*TODO: Add Images*@`. No logo/image
    upload fields exist on this settings page.

**To verify (looks done, comment says otherwise)**

- `ViewUser.razor.cs:34` — `// TODO: This page requires member_manager,admin,owner
  roles … wrap corresponding UI sections in AuthorizeView` — but the page's root
  is already `<AuthorizeView Roles="@(RoleAbilities.ROLE_IS_MEMBER_MANAGER_OR_HIGHER)">`.
  Looks stale; leaving it in case there's a narrower per-section gate intended.
- `ViewUser.razor.cs:54` — `// TODO: Figure Out Why This Isn't Working` above
  `GetUserSubs()`, which calls `PaymentClient.GetOtherSubscriptionRecordsAsync`
  and looks complete and correctly wired into `SubscriptionCard` at
  `ViewUser.razor:184`. Possibly a stale comment left over from debugging;
  couldn't confirm without running it against live data.

## Missing pages (from route comparison above)

- Comment moderation queue (`/comments`) — settings only, no review/action UI.
- Events feature entirely (`/events`, `/settings/events`) — two role constants
  (`ROLE_EVENT_MANAGER`, `ROLE_EVENT_TICKET_MANAGER`) exist and are already
  wired into the admin-role policy, with nothing in the UI to use them.
- `/settings` index/landing page.
- `/settings/bulk-actions` (general bulk actions, distinct from merch sync).

## Already at parity

- Dashboard KPIs (users, subscriptions, content) — real data, not mock.
- Content listing/detail (for Written and Video), publish/edit flow.
- Asset library.
- User listing/detail, password reset, role grants (see security gap #4),
  subscriptions, TOTP device management, enable/disable.
- Careers CRUD.
- Audit log.
- Notification, Personalization (minus image upload), and Comment settings —
  all persist correctly and are role-gated.
- CMS channel/category management (functionally works; see security gap #1
  for the missing role gate).
- Merch: Shopify store CRUD and global sync/cancel-sync with live progress
  polling (functionally works; see security gap #2 and stub #8-9).

## Suggested priority order

0. Close the three missing `<AuthorizeView>` gates (Security gaps #1-3) and add
   a role check to `GrantRolesDialog` (#4) — these are access-control bugs, not
   feature gaps.
1. Wire up `/settings/subscription` for real: `HandleSave` persisting rules/tiers/
   processor state, `OnCreateTier`, and the four `OnProcessorSettings` handlers.
   Right now an admin can spend time configuring this page and lose every change.
2. Fix the unconditional success toasts on `HandleUnpublish`/`HandleDelete`/
   `HandleUnDelete` in `ViewContent.razor.cs` so failures are surfaced.
3. Decide on Picture/Audio content support — either build the authoring UI or
   drop those `ContentType` values from anywhere they're still offered.
4. `AuthorSelect` — lazy/searchable author lookup instead of loading 500 users
   up front, plus default-to-current-user.
5. Comment moderation queue, if the product needs one beyond settings.
6. Events feature, if the roadmap includes it — the role scaffolding already exists.
