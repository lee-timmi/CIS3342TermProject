# CIS3342 Projects

Coursework for **CIS 3342 – Server-Side Web Application Development** (Temple University, Fall 2026).
Built with C#, ASP.NET Web Forms, ASP.NET Core MVC, SQL Server, and Bootstrap.

🌐 **Live homepage:** https://cis-iis2.temple.edu/Fall2026/CIS3342_tut54751/index.html

---

## Project 1 – Honest Plates (Restaurant Review Site)
**Status:** ✅ Complete
**Live app:** https://cis-iis2.temple.edu/Fall2026/CIS3342_tut54751/Project1/Home.aspx

A restaurant review site, backed by a SQL Server database, that helps diners find a place to eat and see what other people thought of it.

**What it does:**
- **Search:** filter restaurants by one or more cuisine types (American, Italian, Asian, and more) and by city/state. Each result card shows the restaurant's logo, its overall rating, and its food, service, and price ratings.
- **Restaurant profile:** a full page for each restaurant with its photo, address, description, cuisine, hours, phone, owner, and website/social links, plus an overall rating calculated from its reviews.
- **Reviews:** every review for the restaurant appears on its profile page, with the reviewer's name, the date, the written review, and food/service/price scores.
- **Rate a review:** visitors can mark any review **Helpful** or **Not Helpful**, and the counts update on the page.

**Data:** 30 restaurants with full profiles (16 in Philadelphia, PA) and 66 reviews.

---

## Project 2 – Honest Plates v2 (Accounts, Reviews & Reservations)
**Status:** ✅ Complete
**Live app:** https://cis-iis2.temple.edu/Fall2026/CIS3342_tut54751/Project2/Home.aspx

Turns Project 1 into a multi-user app. People can create accounts, log in, and see different tools depending on who they are.

**User types:**
| User | Account? | What they can do |
|------|----------|------------------|
| **Reviewer** | Yes | Post reviews, and edit or delete only their own reviews |
| **Restaurant Representative** | Yes | Create and manage one restaurant's profile, manage its reservations, respond to reviews, and review *other* restaurants (never their own) |
| **Site Visitor** | No | Search, view restaurants and reviews, and make reservations |

**What it does:**
- **Home:** a photo banner with the search (one or more cuisines + city/state), the top-rated restaurants in Philadelphia, and a "How it works" section for each kind of user.
- **Accounts & login:** register as a Reviewer or a Representative. Each user type sees its own dashboard of tools after logging in. Logging in or registering from a restaurant's "write a review" link goes straight to that review form.
- **Restaurant profiles (Reps):** add and edit a restaurant with a split-up address, cuisine, weekly hours, contact info, website/social links, and multiple photos with captions (Logo, Profile, Gallery). Photos are stored in the database and are limited to 1 MB each.
- **Reviews:** rate a visit on Food, Service, Atmosphere (1–5 ★), and Price (1–5 $), with written comments, the visit date and time, and an optional photo. Reviewers manage only their own reviews.
- **Rep responses:** reps can reply to reviews of their restaurant, and the reply shows publicly under the review.
- **Reservations (everyone):** book a table with name, phone, email, date/time, party size, and special requests, checked against the restaurant's hours. The restaurant's rep is emailed and can confirm, change, or cancel the reservation, which emails the customer.
- **Profiles:** calculated overall rating, Yelp-style hours table, photo gallery, atmosphere scores, and rep responses.

**How the code is organized (component-based):**
- **Pages** (`*.aspx.cs`) only read forms and show results.
- **Classes** (`RestaurantReview/classes/`): `Restaurant`, `Review`, `Reservation`, `UserAccount`, `RestaurantImage`, `Rating`, `Validation`, `ListHelper`.
- **`Utilities` class library:** `DBConnect` (the database helper, in `Connection.cs`) and `Email`.
- **34 stored procedures** (`Database/StoredProcedures/`): every database operation runs through one. No SQL text in any C# file.

**Data:** 31 restaurants (16 in Philadelphia, PA), each with a week of hours and at least one review; 67 reviews.

### Data Model (ERD)
Project 2 keeps both Project 1 tables, so the Project 1 site still works. It **adds columns** to those tables and **adds four new tables**: `UsersTable`, `RestaurantImages`, `RestaurantHours`, and `Reservations`.

```mermaid
erDiagram
    UsersTable ||--o| RestaurantTable : "manages (1 rep per restaurant)"
    UsersTable ||--o{ ReviewsTable : "writes"
    UsersTable |o--o{ Reservations : "books (optional, visitors need no account)"
    RestaurantTable ||--o{ ReviewsTable : "receives"
    RestaurantTable ||--o{ RestaurantImages : "has"
    RestaurantTable ||--o{ Reservations : "receives"
    RestaurantTable ||--o{ RestaurantHours : "is open"

    UsersTable {
        int UserID PK
        varchar Username UK
        varchar Password
        varchar FirstName
        varchar LastName
        varchar Email
        varchar Phone
        varchar UserType "Reviewer or Representative"
    }

    RestaurantTable {
        int RestaurantID PK
        varchar RestaurantName
        varchar RestaurantAddress
        varchar RestaurantCity
        varchar RestaurantState
        varchar RestaurantZipCode "NEW"
        varchar RestaurantCuisine
        varchar RestaurantDescription
        varchar RestaurantWebsite
        varchar RestaurantSocialMedia
        varchar RestaurantHoursOfOperation
        varchar RestaurantPhone
        varchar RestaurantOwner
        int RepresentativeUserID FK,UK "NEW"
    }

    ReviewsTable {
        int ReviewID PK
        int RestaurantID FK
        int ReviewUserID FK "NEW"
        varchar ReviewText
        int FoodRating "1-5"
        int ServiceRating "1-5"
        int AtmosphereRating "NEW, 1-5"
        int PriceRating "1-5"
        datetime ReviewDate "when the review was posted"
        datetime VisitDateTime "NEW, visit date and time"
        int HelpfulCount
        int NotHelpfulCount
        varbinary ReviewImage "NEW, optional photo"
        varchar ReviewImageType "NEW"
        varchar ResponseText "NEW, rep response"
        datetime ResponseDate "NEW"
    }

    RestaurantImages {
        int ImageID PK
        int RestaurantID FK
        varbinary ImageData "max 1 MB"
        varchar ImageContentType
        varchar Caption
        varchar ImageCategory "type of photo"
        datetime UploadDate
    }

    RestaurantHours {
        int HoursID PK
        int RestaurantID FK
        tinyint DayOfWeek "0 = Sunday, UK with RestaurantID"
        bit IsClosed
        time OpenTime
        time CloseTime
    }

    Reservations {
        int ReservationID PK
        int RestaurantID FK
        int UserID FK "NULL for visitors"
        varchar ContactName
        varchar ContactPhone
        varchar ContactEmail
        datetime ReservationDateTime
        int PartySize
        varchar SpecialRequests
        varchar Status "reservation status"
        datetime CreatedDate
    }
```

**How the tables relate:**
| Relationship | Meaning |
|--------------|---------|
| UsersTable → RestaurantTable | A Representative manages **one** restaurant, and each restaurant has at most one rep |
| UsersTable → ReviewsTable | A Reviewer can write many reviews, including several for the same restaurant (a Representative can too, except for their own restaurant) |
| UsersTable → Reservations | Logged-in users can book reservations; visitors book without an account (`UserID` left empty) |
| RestaurantTable → ReviewsTable | A restaurant receives many reviews; the rep's response is stored on the review |
| RestaurantTable → RestaurantImages | A restaurant has many photos stored in the database, each with a caption |
| RestaurantTable → Reservations | A restaurant receives many reservations, which its rep confirms, changes, or cancels |
| RestaurantTable → RestaurantHours | A restaurant has one row per day of the week (open/close time, or closed) |

The editable source diagram is [`Notes/ERD_Project2.drawio`](Notes/ERD_Project2.drawio). Open it at [app.diagrams.net](https://app.diagrams.net).

> Legacy Project 1 columns `RestaurantImage` and `ReviewerName` still exist in the database but aren't used in Project 2, so they're left out of the diagram.

---

## Term Project – Honest Plates (ASP.NET Core MVC + Web APIs)
**Status:** 🚧 In progress · **Due:** Thu 12/3/2026, 11:59 PM
**Team:** Thimmy Le · Tri Le
**Project board:** https://github.com/users/lee-timmi/projects/2

Projects 1 and 2 rebuilt in **ASP.NET Core MVC** (Razor Views, no Razor Pages), with our own **Restaurants API** and **Reviews API**, the **Documenu** menu API, encrypted logins, security questions, email verification, serialization, and one learning opportunity each (review sentiment with ML.NET and rating trend charts). Every database table and stored procedure starts with `TP_`.

---

## How We Work

### Sprints
One-week sprints, **Tuesday to Monday**, so each Tuesday lab ends one sprint and plans the next.

| Sprint | Dates | Focus |
|--------|-------|-------|
| 1 | Nov 3 – Nov 9 | Setup, `TP_` tables, navigation, styling (carried into Sprint 2) |
| 2 | Nov 10 – Nov 16 | Search, profile, dashboards, login, registration, first API methods |
| 3 | Nov 17 – Nov 23 | Feedback meeting (11/19), rep and reviewer tools, menu API, security, serialization |
| 4 | Nov 24 – Nov 30 | Learning opportunities, apply feedback, fill in 30+ restaurants, full test pass |
| Final | Dec 1 – Dec 3 | Publish to `TermProject`, Canvas comment, submit |

Who does what and when is in our Team Responsibilities document. Every feature is split in half, and each of us does the database, API, and pages for our half.

### The board
Each task is an issue on the [project board](https://github.com/users/lee-timmi/projects/2), with an owner, due date, sprint, size, and a **Done when** checklist.

**Backlog → Ready → In Progress → In Review → Done**

| Column | Meaning |
|--------|---------|
| Backlog | Planned for a later sprint |
| Ready | In this sprint, not started |
| In Progress | Being worked on (2 cards at most per person) |
| In Review | Pull request open; partner is checking the **Done when** list |
| Done | Every box checked and the pull request merged |

### Making a change
`main` is locked: nobody pushes to it directly, and every change needs the other partner's approval. In Visual Studio:

1. **Start:** switch to `main` and **Git → Pull** to get the latest code.
2. **Branch:** **Git → New Branch**, named after the issue (e.g. `13-search-form`).
3. **Work:** commit and push to your branch as often as you like.
4. **Finish:** open a pull request on GitHub, write `Closes #13` in it, and move the card to **In Review**.
5. **Review:** your partner runs the branch and checks off each **Done when** box.
   - All pass: they approve and merge, and the card moves to **Done**.
   - Something fails: they comment, and you fix it on the same branch.
6. **Next task:** switch back to `main`, pull, and start again at step 2.

**One task = one branch = one pull request.**

### Commit messages
```
[{type} #{issue}]_[{scope}] : {what changed} / [{Day} {MM/DD/YYYY} {h:mm AM/PM}]
```
Types: `feat`, `fix`, `refactor`, `docs`, `chore`. The issue number links the commit to its task.
Example: `[feat #13]_[search] : add cuisine checkboxes and city/state dropdowns / [Wed 11/11/2026 3:15 PM]`

### Definition of Done
A task is done when its **Done when** list passes, plus for every coding task:
- All database work goes through `TP_` stored procedures (no SQL text in C#)
- Razor Views only, with the logic in classes, not in the controllers
- Server-side validation and clear error messages; the app never crashes
- Tested and approved by the partner
- Its Canvas comment line is written (how it was done + the page URL)

### First-time setup
1. Install [Git LFS](https://git-lfs.com) (the textbooks in `Resources/` use it), then clone the repo in Visual Studio.
2. Add your own database login file. Connection settings are git-ignored and stay on each computer. Never commit them.

---

## Repository Layout
| Folder | Contents |
|--------|----------|
| `RestaurantReview/` | Project 1 & 2 web app |
| `Utilities/` | Shared database connection class (`Connection.cs` is git-ignored and kept locally on each computer) |
| `Database/` | Stored procedures (`StoredProcedures/`) |
| `Lab01_Simple_Homepage/` | Course homepage (`index.html`) |
| `LabDemo1/`, `LabDemo2/` | In-class lab demos |
| `Notes/` | ERD and project planning |
| `Resources/` | Course textbooks (PDFs, stored with Git LFS) |
