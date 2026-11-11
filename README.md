# CIS3342 Projects

Coursework for **CIS 3342 – Server-Side Web Application Development** (Temple University, Fall 2026).
Built with C#, ASP.NET Web Forms, ASP.NET Core MVC, SQL Server, and Bootstrap.

🌐 **Live homepage:** https://cis-iis2.temple.edu/Fall2026/CIS3342_tut54751/index.html

---

## Earlier Projects
The Term Project builds on these two Web Forms apps (individual projects, both ✅ complete):

| Project | What it added | Live app |
|---|---|---|
| **Project 1: Honest Plates** | Search by cuisine and city/state, restaurant profiles with an overall rating, reviews, and Helpful / Not Helpful votes. 30 restaurants, 66 reviews. | [Project1](https://cis-iis2.temple.edu/Fall2026/CIS3342_tut54751/Project1/Home.aspx) |
| **Project 2: Honest Plates v2** | Accounts for Reviewers and Restaurant Reps, write/edit reviews with photos, rep responses, reservations with email, restaurant photos and hours. 34 stored procedures, no SQL in C#. | [Project2](https://cis-iis2.temple.edu/Fall2026/CIS3342_tut54751/Project2/Home.aspx) |

Project 2's data model is in [`Notes/ERD_Project2.drawio`](Notes/ERD_Project2.drawio) (open at [app.diagrams.net](https://app.diagrams.net)).

---

## Term Project – Honest Plates v3 (ASP.NET Core MVC + Web APIs)
**Status:** 🚧 In progress (due Thu 12/3/2026) · **Team:** Thimmy Le · Tri Le

Project 2 rebuilt in **ASP.NET Core MVC** with Razor Views. The site now runs on our own two Web APIs, adds restaurant menus from a third-party API, and makes accounts more secure.

**User types:** the same three as Project 2: **Reviewers**, **Restaurant Representatives**, and **Site Visitors** (no account needed to search, view restaurants, and make reservations).

**What it does:**
- **Home and search:** a photo banner, search by one or more cuisines and city/state, the top-rated restaurants, and a "How it works" section. Each result card shows the restaurant's name, short description, overall rating, and address.
- **Restaurant profile:** full details, weekly hours, the menu, a photo gallery with captions, every review with its ratings and the rep's response, a **Positive / Negative badge** on each review with an overall "% positive" score, and a **rating trend chart** (3 months, 6 months, 1 year, or all time) with a one-line summary of the trend.
- **Accounts:** register as a Reviewer or a Representative with **3 security questions**. New accounts get an email with a **verification code and link**, and must verify before logging in.
- **Login:** optionally remember the login in a cookie, and delete that cookie from the Login page. **Forgot username** and **reset password** each ask one of the user's security questions at random.
- **Security:** passwords, security answers, and the login cookie are **encrypted**. Pages for logged-in users or reps can't be opened by typing their URL.
- **Reviewer tools:** write a review (Food, Service, Atmosphere, Price, visit date, optional photo), then edit or delete it from **My Reviews**.
- **Representative tools:** add and edit a restaurant and its hours, upload several photos at once (1 MB max each), respond to reviews, and manage reservations.
- **Reservations:** anyone can book a table. The rep is emailed about each new reservation, and the customer is emailed when the rep changes or cancels it.
- **Menus (Documenu API):** reps can import a real restaurant's menu by name and ZIP code, or enter their own and get suggested prices from similar dishes nearby. A saved copy of each menu is kept, so profiles don't call the API every time.
- **Saved searches and recently viewed:** each user's searches and recently viewed restaurants are saved (using serialization) and come back on their next visit.

**How the code is organized:**
- **Web app (ASP.NET Core MVC):** controllers and Razor Views for every page. The logic lives in classes, not in the controllers.
- **Restaurants API** and **Reviews API:** RESTful Web APIs (GET, POST, PUT, DELETE) that take and return our own classes as JSON, with form-data versions of some methods. The web app calls them from server-side code.
- **Third-party API:** Documenu for menus.
- **Database:** every table and stored procedure starts with `TP_`, and all database work goes through stored procedures.
- **Learning opportunities:** review sentiment with **ML.NET** (Thimmy) and **rating trend charts** (Tri).

**Data:** 30+ restaurants with every profile field filled in, carried over from Project 2 and expanded.

### Team Responsibilities
📄 [CIS3342 Team Responsibilities - Thimmy Le & Tri Le](https://docs.google.com/document/d/1_CpPLqnRtl3_PIvtlF6skocC61DojvC4bnua_L42f-4/edit?usp=sharing) (Google Doc: who does what, and when)

**Completed:**
| Responsibility | Section | Done by | Completed |
|---|---|---|---|
| GitHub repo + project board | A. Setup | Thimmy | ✅ |
| Board columns, labels, Sprint 1 tasks | A. Setup | Tri | ✅ |
| MVC web project + database helper class | A. Setup | Thimmy | ✅ Wed 11/11 |
| Two API projects + shared model classes | A. Setup | Thimmy (Tri's item) | ✅ Wed 11/11 |
| DB: Users, Security Questions, Restaurants, Restaurant Hours | B. Database | Thimmy | ✅ Wed 11/11 |
| Copy Project 2 restaurants, hours, users | B. Database | Thimmy | ✅ Wed 11/11 |
| DB: Reviews, Restaurant Photos, Reservations, Menu Items | B. Database | Thimmy (Tri's item) | ✅ Wed 11/11 |
| Copy Project 2 reviews, photos, reservations | B. Database | Thimmy (Tri's item) | ✅ Wed 11/11 |
| Navigation for visitors | A. Setup | Thimmy | ✅ Wed 11/11 |
| Site colors, fonts, buttons | A. Setup | Thimmy | ✅ Wed 11/11 |

---

## Repository Layout
| Folder | Contents |
|--------|----------|
| `RestaurantReview/` | Project 1 & 2 web app |
| `Utilities/` | Shared database connection class (`Connection.cs` is git-ignored and kept locally on each computer) |
| `Database/` | Stored procedures (`StoredProcedures/`) |
| `Lab01_Simple_Homepage/` | Course homepage (`index.html`) |
| `Notes/` | ERD and project planning |
| `Resources/` | Course textbooks (PDFs, stored with Git LFS) |
