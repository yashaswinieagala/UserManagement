# UserHub — Blazor WebAssembly Full-Stack App

A complete Blazor WebAssembly + ASP.NET Core hosted solution demonstrating:
- Full CRUD for User Details (Username, Email, Age, Department)
- EditForm with DataAnnotationsValidator (client + server validation)
- Two-way data binding (@bind, @bind:event)
- SignalR real-time notifications across browser tabs
- EF Core In-Memory database with seed data
- Search & filter with query parameters
- Reusable components (UserCard, UserForm, ConfirmDialog)
- Dashboard with bar chart and statistics
- EventCallback parent-child communication
- IDisposable / lifecycle management

---

## Prerequisites

Install .NET 8 SDK:  https://dotnet.microsoft.com/download/dotnet/8.0

Verify installation:
```
dotnet --version
```
Must show 8.x.x

---

## Run the App

1. Open a terminal in the folder containing `UserManagement.sln`

2. Restore packages:
```
dotnet restore
```

3. Run the server (this also serves the Blazor WASM client):
```
dotnet run --project UserManagement.Server
```

4. Open your browser at:
```
https://localhost:7xxx   (check the terminal for the exact port)
```
   or
```
http://localhost:5xxx
```

That's it. No database setup needed — EF Core InMemory database is seeded automatically.

---

## Project Structure

```
UserManagement.sln
├── UserManagement.Shared/          # Shared models (UserDetail, UserNotification)
│   └── Models/
├── UserManagement.Server/          # ASP.NET Core host + API + SignalR Hub
│   ├── Controllers/UsersController.cs
│   ├── Data/AppDbContext.cs
│   ├── Hubs/UserHub.cs
│   └── Program.cs
└── UserManagement.Client/          # Blazor WebAssembly
    ├── Pages/
    │   ├── Index.razor             Home page (SVG hero + stats)
    │   ├── Users.razor             User list + search + edit modal + delete confirm
    │   ├── AddUser.razor           Add user form
    │   └── Dashboard.razor         Stats + bar chart + donut chart
    ├── Shared/
    │   ├── MainLayout.razor        Sidebar nav + SignalR toast notifications
    │   ├── UserForm.razor          Reusable EditForm with validation
    │   ├── UserCard.razor          Reusable card component
    │   └── ConfirmDialog.razor     Delete confirmation modal
    ├── Services/
    │   ├── IUserService.cs         HTTP service interface
    │   ├── UserService.cs          HTTP service implementation
    │   └── NotificationService.cs  SignalR client service
    └── wwwroot/css/app.css         Complete stylesheet
```

---

## API Endpoints

| Method | Endpoint                     | Description               |
|--------|------------------------------|---------------------------|
| GET    | /api/users                   | Get all users             |
| GET    | /api/users/{id}              | Get user by ID            |
| GET    | /api/users/search?term=&...  | Search/filter users       |
| GET    | /api/users/departments       | List all departments      |
| GET    | /api/users/stats             | Statistics                |
| POST   | /api/users                   | Create user               |
| PUT    | /api/users/{id}              | Update user               |
| DELETE | /api/users/{id}              | Delete user               |

SignalR Hub: `/userhub`  
Event: `ReceiveNotification` → `UserNotification` object

---

## Blazor Concepts Demonstrated

| Concept | Where |
|---------|-------|
| EditForm + DataAnnotationsValidator | UserForm.razor |
| Two-way data binding (@bind) | UserForm.razor, Users.razor filter bar |
| @bind:event="oninput" | Search box in Users.razor |
| EventCallback (child → parent) | UserCard, UserForm, ConfirmDialog |
| Cascading parameters / layout | MainLayout.razor |
| Component lifecycle (OnInitializedAsync, Dispose) | All pages |
| IDisposable | Users.razor, Dashboard.razor |
| IAsyncDisposable | MainLayout.razor, NotificationService |
| @inject dependency injection | All pages |
| NavigationManager | AddUser, Index |
| Conditional rendering (@if) | All pages |
| List rendering (@foreach) | Users.razor, Dashboard.razor |
| SignalR client | NotificationService.cs |
| HttpClient + GetFromJsonAsync | UserService.cs |
| EF Core InMemory | AppDbContext.cs |
| Scoped services | Program.cs (Server + Client) |
