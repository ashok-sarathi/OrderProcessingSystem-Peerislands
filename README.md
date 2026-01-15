# OrderProcessingSystem-Peerislands

A robust **Order Processing System** built with **.NET 8**, **ASP.NET Core Web API**, **Entity Framework Core InMemory**, and **MediatR**.
The system manages orders through a complete **order lifecycle** and demonstrates **clean architecture, CQRS pattern, background processing, and unit testing**.

---

## 🧱 Features

* Create, read, and update orders via API endpoints
* Orders automatically progress through states:
  `Pending → Processing → Shipped → Delivered ✖ Canceled`
* **Input validation** using FluentValidation
* **InMemory EF Core** database for quick prototyping/testing
* **CQRS-style architecture** using MediatR and Business logic rules
* Background service for processing order state transitions
* Unit tests for API and business logic

---

## ⚙️ Tech Stack

| Layer           | Technology                       |
| --------------- | -------------------------------- |
| API             | ASP.NET Core Web API (.NET 8)    |
| Business Logic  | MediatR (CQRS pattern and rules) |
| Data Access     | Entity Framework Core (InMemory) |
| Validation      | FluentValidation                 |
| Testing         | xUnit + Moq                      |
| Background Jobs | IHostedService                   |

---

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/ashok-sarathi/OrderProcessingSystem-Peerislands.git
cd OrderProcessingSystem-Peerislands
```

### 2. Build the solution

```bash
dotnet build
```

### 3. Run the API

```bash
dotnet run --project OrderProcessingSystem.Api
```

The API will be available at: `http://localhost:5000` (default Kestrel port)

---

## 🧪 Testing

* The solution includes **unit tests** for the API and application layer.
* Run all tests:

```bash
dotnet test
```

* Tests cover:

  * Command/Query handlers
  * Validation rules
  * API endpoints

---

## 📌 Notes & Best Practices

* **InMemory database is for testing/development only**
* For production:

  * Replace `UseInMemoryDatabase` with `UseSqlServer` or `UsePostgreSQL`
  * Add migrations using EF Core
* Background service updates orders every **configured interval** (default 5 mins)
* Use **FluentValidation** for request validation

---

## 🔗 Resources

* [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/?view=aspnetcore-8.0)
* [Entity Framework Core InMemory Provider](https://learn.microsoft.com/en-us/ef/core/providers/in-memory/?tabs=dotnet-core-cli)
* [MediatR Documentation](https://github.com/jbogard/MediatR)
* [FluentValidation Documentation](https://docs.fluentvalidation.net/en/latest/)
* [REST Client for VS Code](https://marketplace.visualstudio.com/items?itemName=humao.rest-client)

---

## 🤝 Contributing

Feel free to:

* Open issues
* Submit pull requests
* Suggest features