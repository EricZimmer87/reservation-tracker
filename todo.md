# TODOs

## Done

- [x] Modified On and Canceled On dates in Reservations tables
- [x] Do NOT allow duplicate Guests
  - [x] Make Guests unique by First Name, Last Name, and Phone Number
- [x] Rebuild database and rescaffold models

---

## Guest / Reservations

- [x] See all reservations for the guest
  - [x] Make it a button on the Guest details page
  - [x] Display "Guest for [Guest's Name]" in the header

---

## Reservation Logic

- [x] ReturnUrl when clicking on a Guest from the Reservations Index
- [x] Set `ModifiedOn` when reservation is edited
- [x] Set `CanceledOn` when reservation is canceled

---

## Rooms

- [x] Cannot delete room if it is being used by one or more reservations.

---

## Duplicate Guest UX

- [x] Handle duplicate guest create/edit gracefully in the UI
  - [ ] Future upgrade: find the duplicate guest and display a link to it with the error

---

## Login / Security

### Access Control

- [x] Require login to access the app (use `[Authorize]` or global policy)
- [x] Deny access if user is not in `Users` table
- [x] Deny access if `IsBanned == true`
- [x] Show "Access Denied" message/page for unauthorized users

### User Mapping (Google → App User)

- [x] On login, get email from Google claims
- [x] Look up user in `Users` table by email
- [x] If user exists:
  - [x] Allow access
  - [x] Optionally update:
    - [x] GoogleId
    - [x] DisplayName
    - [x] Picture
- [x] If user does NOT exist:
  - [x] Deny access (do NOT auto-create user)

### Reservation User Tracking

- [x] On Create:
  - [x] Set `UserId` = current logged-in user
- [x] On Edit:
  - [x] Set `ModifiedByUserId` = current logged-in user

- [ ] Revalidate authenticated user against `Users` table on each request
  - [ ] Deny access if user was banned after login
  - [ ] Deny access if user was removed after login
  - [ ] Sign out invalid users

---

## Admin Features (User Management)

- [x] Create Users management page (admin only)
- [x] Allow admin to:
  - [x] Add new user (by email)
  - [x] Set `IsAdmin`
  - [x] Set `IsBanned`
  - [x] admin cannot ban themselves
  - [x] admin cannot remove their own admin flag
  - [x] maybe don’t allow deleting users at all
- [x] Restrict Users page to admin only

## Run These

```sql
UPDATE Reservations
SET NumberOfGuests = 1
WHERE NumberOfGuests IS NULL;
```

```sql
ALTER TABLE Reservations
ALTER COLUMN NumberOfGuests INT NOT NULL;
```

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
