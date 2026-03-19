# TODOs

## Done

- [x] Modified On and Canceled On dates in Reservations tables
- [x] Do NOT allow duplicate Guests
  - [x] Make Guests unique by First Name, Last Name, and Phone Number
- [x] Rebuild database and rescaffold models

---

## Guest / Reservations

- [ ] See all reservations for the guest
  - [ ] Make it a button on the Guest details page

---

## Reservation Logic

- [ ] ReturnUrl when clicking on a Guest from the Reservations Index
- [ ] Set `ModifiedOn` when reservation is edited
- [ ] Set `CanceledOn` when reservation is canceled

---

## Duplicate Guest UX

- [ ] Handle duplicate guest create/edit gracefully in the UI

---

## Login / Security

### Access Control

- [ ] Require login to access the app (use `[Authorize]` or global policy)
- [ ] Deny access if user is not in `Users` table
- [ ] Deny access if `IsBanned == true`
- [ ] Show "Access Denied" message/page for unauthorized users

### User Mapping (Google → App User)

- [ ] On login, get email from Google claims
- [ ] Look up user in `Users` table by email
- [ ] If user exists:
  - [ ] Allow access
  - [ ] Optionally update:
    - [ ] GoogleId
    - [ ] DisplayName
    - [ ] Picture
- [ ] If user does NOT exist:
  - [ ] Deny access (do NOT auto-create user)

### Reservation User Tracking

- [ ] On Create:
  - [ ] Set `UserId` = current logged-in user
- [ ] On Edit:
  - [ ] Set `ModifiedByUserId` = current logged-in user

---

## Admin Features (User Management)

- [ ] Create Users management page (admin only)
- [ ] Allow admin to:
  - [ ] Add new user (by email)
  - [ ] Set `IsAdmin`
  - [ ] Set `IsBanned`
- [ ] Restrict Users page to admin only

## Scaffold Command

dotnet ef dbcontext scaffold "Server=localhost,1433;Database=ReservationTracker;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer --context ReservationTrackerContext --context-dir Data --output-dir Models --force

## Drop Database Command

```SQL
USE master;
GO

ALTER DATABASE ReservationTracker SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

DROP DATABASE ReservationTracker;
GO
```

```C#

```
