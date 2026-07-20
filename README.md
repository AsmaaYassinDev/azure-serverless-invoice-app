# Azure Serverless Invoice Processing API (.NET 8)

An Azure Functions backend designed to handle invoice creation, database persistence, and PDF generation. This project is built using .NET 8 (Isolated Worker Model) and is structured around Clean Architecture principles.

## Key Features

* **Serverless Architecture:** Utilizes Azure Functions (v4) with .NET 8 Isolated Worker Process for independent scaling and performance.
* **NoSQL Database Persistence:** Uses Azure Cosmos DB for reliable, schema-agnostic storage.
* **Document Generation:** Generates invoice PDFs directly from incoming JSON payloads.
* **CI/CD Verification:** Includes a pre-configured GitHub Actions pipeline (`azure-function-build.yml`) that automatically restores and builds the application on every push.
* **ASP.NET Core Integration:** Uses standard `HttpRequest` and `IActionResult` interfaces for unified, clean endpoint controllers.
* **Automated Unit Testing & CI/CD:**  Fully covered by a comprehensive suite of xUnit and Moq unit tests, integrated into a GitHub Actions pipeline that enforces a quality gate before deploying to Azure.

## Tech Stack & Architecture

* **Framework:** .NET 8.0 (Isolated Worker)
* **Database:** Azure Cosmos DB (Extension v4, version 4.11.0)
* **Design Patterns:** Dependency Injection (DI), Repository Pattern, Interface Segregation.
* **CI/CD:** GitHub Actions Automation

### Project Layout
```text
├── .github/workflows/      # Automated Build Pipelines
├── assets/                 # Documentation Images & Screenshots
├── GenerateInvoice.cs      # HTTP Trigger (API Controller Endpoint)
├── IInvoiceRepository.cs   # Data Layer Abstraction
├── InvoiceRepository.cs     # Cosmos DB Implementation
├── IInvoiceService.cs      # Business Logic Abstraction
├── InvoiceService.cs       # Document Processing Engine
├── InvoiceModel.cs         # Core Data Contracts (DTO)
├── Program.cs              # Dependency Injection & Bootstrapper
└── host.json               # Serverless Environment Meta
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
