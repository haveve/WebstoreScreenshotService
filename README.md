# Website Screenshot Service

A scalable, distributed, message-driven website screenshot platform built with **.NET 10**, **React**, **RabbitMQ**, **Playwright**, **PostgreSQL**, **Stripe**, and **Azure Blob Storage**.

The application enables users to purchase subscription plans, submit website screenshot requests, and retrieve generated screenshots through a modern web interface. Screenshot generation is performed asynchronously by independent Playwright workers, allowing the system to scale horizontally while keeping the API Gateway responsive under load.

---

# Application Preview

## Administration Dashboard

<img width="848" height="352" alt="image" src="https://github.com/user-attachments/assets/640afef3-9a22-408a-ac5a-24071c5eae48" />

> Administrative analytics dashboard for monitoring platform usage, subscription metrics, and operational statistics.
---

## Create Screenshot Request

<img width="543" height="369" alt="image" src="https://github.com/user-attachments/assets/a58f536f-3dd3-4066-aefd-f9a4cc687293" />

> Configure screenshot options and submit new screenshot requests.

---

## Screenshot History

<img width="799" height="294" alt="image" src="https://github.com/user-attachments/assets/cec4b628-54be-472f-911a-61564de1befe" />

> Browse previously generated screenshots and monitor processing status.

---

## Subscription Management

<img width="298" height="296" alt="image" src="https://github.com/user-attachments/assets/c80aad5e-762e-4959-a4c7-b54107da5103" />

> Manage subscription plans and billing powered by Stripe.

---

## System Architecture

<img width="467" height="364" alt="image" src="https://github.com/user-attachments/assets/a44170e3-c77c-43f2-8351-d317d68f2fd3" />

> High-level architecture of the API Gateway, RabbitMQ, Playwright workers, PostgreSQL, Azure Blob Storage, Stripe integration.

---

# Highlights

- Distributed, message-driven architecture
- Horizontally scalable Playwright worker services
- Queue-per-subscription workload isolation
- Shared.Core library for shared domain logic and validation
- Envelope encryption using AES-GCM
- Stripe subscription management
- Azure Blob Storage integration
- PostgreSQL persistence
- JWT-secured REST API
- Built with .NET 10 and React

---

# Architecture

The application is composed of several independent components that communicate asynchronously through RabbitMQ.

## API Gateway

The API Gateway is responsible for:

- User authentication and authorization
- Subscription management
- Stripe payment processing
- Screenshot request validation
- Publishing screenshot jobs to RabbitMQ
- Retrieving generated screenshots
- Exposing the REST API consumed by the React frontend

The API remains lightweight by delegating browser automation to dedicated worker services.

---

## RabbitMQ

RabbitMQ acts as the messaging backbone of the platform.

Instead of generating screenshots during the HTTP request lifecycle, the API Gateway publishes screenshot jobs to RabbitMQ, allowing browser automation to execute asynchronously.

Screenshot jobs are routed to dedicated queues according to the user's subscription tier, enabling workload isolation and differentiated processing priorities.

---

## Playwright Workers

Worker services consume screenshot jobs from RabbitMQ.

Each worker:

- Launches an isolated Playwright browser instance
- Navigates to the requested website
- Captures the screenshot
- Uploads the image to Azure Blob Storage
- Persists processing status and metadata in PostgreSQL

Workers are stateless and can be horizontally scaled by deploying additional instances without affecting the API Gateway.

---

## Shared.Core

The solution contains a dedicated **Shared.Core** project shared between the API Gateway and worker services.

It centralizes:

- Domain models
- Business rules
- Validation
- Shared DTOs
- Contracts
- Common utilities

Keeping domain logic in a single shared library ensures consistent business behavior across services while reducing duplicated code and simplifying maintenance.

---

# Message Processing Workflow

Screenshot generation follows an asynchronous processing pipeline.

1. A user submits a screenshot request.
2. The API Gateway validates authentication, request parameters, and subscription permissions.
3. A screenshot job is published to RabbitMQ.
4. RabbitMQ routes the message to the queue corresponding to the user's subscription tier.
5. An available Playwright worker consumes the message.
6. The worker generates the screenshot.
7. The screenshot is uploaded to Azure Blob Storage.
8. Processing metadata is persisted in PostgreSQL.
9. The client retrieves the completed screenshot through the REST API.

This architecture allows screenshot processing throughput to be increased simply by deploying additional worker instances.

---

# Design Decisions

## Distributed Architecture

Browser automation is isolated from the API Gateway to prevent resource-intensive screenshot generation from impacting API responsiveness.

## Message-Driven Processing

RabbitMQ decouples HTTP requests from screenshot generation, improving responsiveness, resilience, and throughput while allowing independent scaling of worker services.

## Queue Isolation

Dedicated RabbitMQ queues are used for different subscription tiers.

This allows workload isolation while ensuring premium workloads are not affected by lower-priority requests.

## Shared Domain Library

Business rules, validation, contracts, and domain models are centralized in **Shared.Core**, guaranteeing consistent behavior across all services.

## Stateless Workers

Workers maintain no local state.

Generated screenshots are stored in Azure Blob Storage, while application data is persisted in PostgreSQL, allowing worker instances to be created or removed without affecting the system.

---

# Security

Security was designed as a core aspect of the system.

## Authentication

The REST API is protected using JWT authentication with authorization applied to protected endpoints.

## Authenticated Encryption

Sensitive user data is encrypted using **AES-GCM (Advanced Encryption Standard - Galois/Counter Mode)**, providing authenticated encryption that guarantees both confidentiality and integrity. Any modification of encrypted data or associated metadata is automatically detected during decryption.

## Envelope Encryption

Each user is assigned an individual **Data Encryption Key (DEK)** used to encrypt sensitive information.

User DEKs are encrypted using a **Key Encryption Key (KEK)** before being persisted to the database.

This key hierarchy provides:

- Isolation between users
- Protection of encryption keys at rest
- Simplified master key rotation
- Reduced impact of a compromised user key

---

# Technology Stack

## Backend

- .NET 10
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- RabbitMQ
- Playwright
- Stripe
- Azure Blob Storage
- JWT Authentication

## Frontend

- React
- TypeScript

---

# Getting Started

## Prerequisites

- .NET 10 SDK
- Node.js 20+
- PostgreSQL
- RabbitMQ
- Azure Storage Account (or Azurite)
- Stripe Account

---

# Installation

Clone the repository:

```bash
git clone https://github.com/haveve/WebstoreScreenshotService.git
cd WebstoreScreenshotService
```

## Backend

```bash
dotnet restore
dotnet run
```

## Frontend

Navigate to the frontend project:

```bash
cd FrontEnd
```

Create a directory for development certificates:

```bash
mkdir .cert
```

Generate local certificates using **mkcert**:

```bash
mkcert -cert-file .cert/cert.pem -key-file .cert/key.pem localhost
```

Install dependencies and start the frontend:

```bash
npm install
npm start
```

---

# Configuration

Development configuration is managed through:

- `.env`
- `appsettings.Development.json`

Configure the following values before running the application:

- PostgreSQL connection string
- RabbitMQ configuration
- Azure Blob Storage connection string
- Stripe API keys
- JWT configuration

---

# Documentation

API documentation is generated using **DocFX**.

Generate and serve the documentation locally:
```bash
docfx WebsiteScreenshotService/docs/docfx.json --serve
```

Additional project documentation is available in the `docs/` directory.
---

# Swagger
The OpenAPI specification is available at:
```text
WebsiteScreenshotService/swagger/swagger.json
```
---

# GDPR
The application complies with GDPR requirements by requesting user consent before storing or processing personal data.
For more information, see **GDPR.md**.
---
# License

This project is licensed under the **CC BY-NC-ND 4.0** License. See the **LICENSE** file for details.
