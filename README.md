# 📊 MyBudgetNET

**MyBudgetNET** is a RESTful API for personal expense tracking, built using ASP.NET Core Web API 8, PostgreSQL, and Mapster. The project follows a 3-layer architecture with clear separation between the DAL, BLL, JWT and API layers.

## 🔧 Technologies Used

- ASP.NET Core 8 (Web API)
- Entity Framework Core (Code First)
- PostgreSQL (via Npgsql)
- Mapster for DTO/Entity mapping
- ASP.NET Identity + JWT for authentication
- 3-layer architecture (DAL, BLL, API) + JWT 

## 🧱 Architecture

The project is structured into three main layers + one for jwt auth:

- **MyBudget.DAL** — Data access layer (models, DbContext, migrations)
- **MyBudget.BLL** — Business logic layer (services, interfaces)
- **MyBudget.API** — Presentation layer (controllers, routing, DI configuration)
- **MyBudget.JWT** - Auth layer (controllers, middlewares, DI configuration)

This ensures scalability, testability, and maintainability.

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/vladyslavplus/MyBudgetNET.git
cd MyBudgetNET
```

### 2. Configure the database

Create a PostgreSQL database (e.g., `mybudget_db`) and update your connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=mybudget_db;Username=postgres;Password=yourpassword"
}
```

### 3. Apply migrations

```bash
dotnet ef database update --project MyBudget.DAL --startup-project MyBudget.API
```

### 4. Run the API

```bash
dotnet run --project MyBudget.API
dotnet run --project MyBudget.JWT
```

API will be available at: `https://localhost:7001` or `http://localhost:5063`.
API will be available at: `https://localhost:7048`.

## 🧪 MVP (Minimum Viable Product)

- User registration and login (JWT)
- Full CRUD for expenses
- Filtering expenses by date, category, amount
- Pagination and sorting of expenses
- CRUD for user-defined categories

## 📌 Product Backlog

### 👤 User

| ID  | User Story                                                                                                      |
| --- | --------------------------------------------------------------------------------------------------------------- |
| US1 | As a user, I want to register and log in to track my expenses.                                                  |
| US2 | As a user, I want to add new expenses with amount, category, date, and description.                             |
| US3 | As a user, I want to edit and delete my expenses to manage my history.                                          |
| US5 | As a user, I want to view a paginated list of my expenses for easier navigation through large data sets.        |
| US6 | As a user, I want to filter expenses by date, category, or amount to find specific records quickly.             |
| US7 | As a user, I want to update my password.                                                                        |

### 🛡 Admin

| ID   | User Story                                                                                                                       |
| ---- | -------------------------------------------------------------------------------------------------------------------------------- |
| US9  | As an admin, I want to see a list of all registered users to monitor activity.                                                   |
| US10 | As an admin, I want to block or delete users to maintain system security.                                                        |
| US11 | As an admin, I want to view global statistics — total expenses, common categories.                                               |
| US12 | As an admin, I want to create/edit/delete global categories available to new users.                                              |
| US13 | As an admin, I want to create new admins or manually assign roles.                                                               |
| US14 | As an admin, I want to see all expenses from any user.                                                                           |

## 🧰 Project Structure

```
MyBudgetNET/
│
├── MyBudget.API/       # Controllers, DI, configurations
├── MyBudget.BLL/       # Services, interfaces, business logic
├── MyBudget.DAL/       # Models, DbContext, migrations
├── MyBudget.JWT/       # Controllers, Middlewares, DI, configurations
```

## 🔐 Authentication

- ASP.NET Identity for user management
- JWT for secure API authentication

## 📌 Future Plans

- Global categories for new users
- Expense dashboard with charts
- Multi-language support (i18n)

## 📄 License

This project is licensed under the MIT License.