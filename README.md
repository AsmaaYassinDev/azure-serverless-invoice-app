# Azure Serverless Full-Stack Invoice Processing System (.NET 8 & Angular)

A lightweight personal project designed to handle dynamic invoice creation, NoSQL database persistence, automated PDF generation, and a modern SaaS user interface.Built using .NET 8 (Isolated Worker Model) for the backend and Angular v22 for the frontend, following Clean Architecture principles.

## Key Features

* **Modern SaaS Frontend:** Built with Angular featuring a high-end gradient design, reactive `FormArray` for dynamic line items, and real-time input validations.
* **Serverless Architecture:** Utilises Azure Functions (v4) with a .NET 8 Isolated Worker Process for independent scaling, security, and performance.
* **NoSQL Database Persistence:** Uses Azure Cosmos DB for reliable, schema-agnostic storage of invoice records.
* **Dynamic Document Generation:** Transforms complex JSON payloads into beautifully styled, downloadable PDF invoices via server-side rendering.
* **Parallel Full-Stack CI Pipeline:** Powered by a pre-configured GitHub Actions workflow (`CI - Full Stack Azure Function .NET 8 & Angular Build & Test`) that simultaneously validates, restores, and builds both the .NET backend and the Angular frontend on every push.
* **Automated Testing:** Fully covered by a comprehensive suite of xUnit and Moq unit tests, serving as an automated quality gate before production deployment.

## Tech Stack & Architecture

* **Backend Framework:** .NET 8.0 (Isolated Worker)
* **Frontend Framework:** Angular v22 (Reactive Forms, SaaS UI)
* **Database:** Azure Cosmos DB (Extension v4, version 4.11.0)
* **Design Patterns:** Dependency Injection (DI), Repository Pattern, Interface Segregation.
* **CI/CD:** GitHub Actions Automation

### Project Layout

```text
├── .github/workflows/      # Automated CI Build & Test Pipelines
├── assets/                 # Documentation Images & Screenshots
├── invoice-generator/      # Angular v22 Modern SaaS Frontend UI
├── InvoiceBackend/         # C# .NET 8 Serverless Function App
│   ├── GenerateInvoice.cs  # HTTP Trigger (API Controller Endpoint)
│   ├── IInvoiceRepository.cs# Data Layer Abstraction
│   ├── InvoiceRepository.cs# Cosmos DB Implementation
│   ├── IInvoiceService.cs  # Business Logic Abstraction
│   ├── InvoiceService.cs   # Document Processing Engine & IronPDF
│   ├── InvoiceModel.cs     # Core Data Contracts (DTO)
│   ├── Program.cs          # Dependency Injection & Bootstrapper
│   └── host.json           # Serverless Environment Meta
└── InvoiceBackend.Tests/   # Automated Unit Testing Suite
```

### **Local Configuration**
To run the project locally, create a local.settings.json file in the root directory. This file is tracked by .gitignore to prevent secret leaks:
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "CosmosDBConnectionString": "AccountEndpoint=https://your-local-or-live-cosmos;"
  }
}

### **Database Verification & Testing**
For zero-cost local development and rapid integration testing, this project uses the Azure Cosmos DB Emulator. Data structural validation is performed inside a container matching the following layout:
-Database ID: InvoiceDB
-Container ID: Invoices
-Partition Key: /id

When an execution request hits the API endpoint, the application automatically handles structural serialisation and maps incoming records smoothly directly into the local collection database documents.
![Cosmos DB Local Emulator Document Verification](./assets/emulator-setup.png)

### Integration Testing & CI/CD Security
This project includes a dedicated integration testing suite (`InvoiceBackend.IntegrationTests`) designed to verify real-world connectivity and data persistence with Azure Cosmos DB. 

**Running Tests Locally:**
When running locally, the integration tests connect seamlessly to the Azure Cosmos DB Emulator. Ensure the emulator is running before executing `dotnet test`.

**CI/CD Pipeline Considerations:**
To protect sensitive cloud credentials, the GitHub Actions pipeline (`azure-functions-deploy.yml`) is engineered to dynamically inject the database connection string at runtime:
* **Unit Tests (Quality Gate 1):** Run automatically on every push, requiring no database connection.
* **Integration Tests (Quality Gate 2):** Require a valid Cosmos DB connection string stored in GitHub Secrets (`COSMOS_DB_TEST_CONNECTION`). The pipeline uses a secure Linux `echo` command to build a temporary `appsettings.json` file, runs the tests against a live cloud test-database, and then completely destroys the virtual environment to prevent secret leakage.

### **API Contract (POST)
-Endpoint: POST /api/invoice/generate
-Headers: Content-Type: application/json

{
  "CustomerName": "John Doe",
  "Description": "Project Development Services",
  "Amount": 1150.00,
"Items": [
    { "ItemName": "Frontend Setup (Angular)", "Quantity": 1, "Price": 350.00 },
    { "ItemName": "Backend Integration (.NET 8)", "Quantity": 1, "Price": 400.00 }
  ]
}

### **Response
Success (200 OK): Returns a raw binary file streaming response (application/pdf) prompting a direct download named Invoice_John_Doe.pdf.
Validation Failure (400 Bad Request): Returns a descriptive validation error string if required data payloads contain structural anomalies or missing attributes.
