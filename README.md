# Ecommerce API

![.NET](https://img.shields.io/badge/.NET-9.0-blue)
![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)
![Azure](https://img.shields.io/badge/Azure-Blob%20Storage-blue)
![Stripe](https://img.shields.io/badge/Stripe-Payments-blue)

---

## Table of Contents
- [Project Overview](#project-overview)
- [Features](#features)
- [Quick Start](#quick-start)
- [Technologies Used](#technologies-used)
- [Endpoints](#endpoints)
- [Authentication (JWT)](#authentication-jwt)
- [Image Storage (Azure Blob Storage)](#image-storage-azure-blob-storage)
- [Payments (Stripe)](#payments-stripe)
- [Contributing](#contributing)
- [License](#license)
- [Acknowledgements](#acknowledgements)

---

## Project Overview

A modern, scalable RESTful API for an ecommerce platform built with ASP.NET Core, featuring JWT authentication, Azure Blob Storage for image management, and Stripe for payment processing. The API is designed for extensibility, security, and cloud readiness.

---

## Features
- User Registration & Login (JWT Authentication)
- Menu Item CRUD with image upload to Azure Blob Storage
- Shopping Cart management
- Order Placement & Tracking
- Stripe Payment Integration
- Role-based Authorization
- Robust Error Handling & API Responses

---

## Quick Start

1. **Clone the Repository**
    ```sh
    git clone https://github.com/Sonseldeep/E-commerce-Assignment.git
    cd Ecommerce.Api
    ```
2. **Configure Environment**
    - Edit `Ecommerce.Api/appsettings.json`:
      - `ConnectionStrings:DefaultConnection`: SQL Server connection string
      - `AzureBlobStorage:ConnectionString`: Azure Storage connection string
      - `StripeSettings:SecretKey`: Stripe secret key
      - `Jwt:Key, Jwt:Issuer, Jwt:Audience`: JWT settings
3. **Run Database Migration**
    ```sh
    dotnet ef migrations add InitialCreate --project Ecommerce.Api
    dotnet ef database update --project Ecommerce.Api
    ```
4. **Start the API**
    ```sh
    dotnet run --project Ecommerce.Api
    ```
    The API will be available at `https://localhost:5001` (or as configured).

---

## Technologies Used
- ASP.NET Core 9 Web API
- Entity Framework Core (Code First, Migrations)
- Azure Blob Storage (for image uploads)
- JWT (JSON Web Token) for authentication
- Stripe for payment processing
- SQL Server (default, configurable)

---

## Endpoints

### Authentication
| Method | Endpoint                | Description                  |
|--------|-------------------------|------------------------------|
| POST   | /api/auth/register      | Register a new user          |
| POST   | /api/auth/login         | Login and receive JWT token  |

### Menu Items
| Method | Endpoint                | Description                          |
|--------|-------------------------|--------------------------------------|
| GET    | /api/menuitems          | Get all menu items                   |
| GET    | /api/menuitems/{id}     | Get menu item by ID                  |
| POST   | /api/menuitems          | Create a new menu item (image upload)|
| PUT    | /api/menuitems/{id}     | Update a menu item                   |

### Shopping Cart
| Method | Endpoint                        | Description                                 |
|--------|----------------------------------|---------------------------------------------|
| GET    | /api/shoppingcart?userId={userId}| Get shopping cart for a user                |
| POST   | /api/shoppingcart                | Add/update item in cart (userId, menuItemId, updateQuantityBy) |

### Orders
| Method | Endpoint                        | Description                  |
|--------|----------------------------------|------------------------------|
| POST   | /api/orders                      | Place a new order            |
| GET    | /api/orders?userId={userId}      | Get orders for a user        |

### Payments
| Method | Endpoint                | Description                                  |
|--------|-------------------------|----------------------------------------------|
| POST   | /api/payment/{userId}   | Create Stripe payment intent for user's cart |

---

## Authentication (JWT)
- All protected endpoints require the `Authorization: Bearer <token>` header.
- JWT is issued on successful login and must be included in subsequent requests.

**Example Login Request:**
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "yourpassword"
}
```
**Response:**
```json
{
  "token": "<JWT_TOKEN>",
  "expires": "2025-10-24T12:00:00Z"
}
```

---

## Image Storage (Azure Blob Storage)
- Menu item images are uploaded to Azure Blob Storage.
- Ensure your Azure Storage connection string and container name are set in `appsettings.json`.
- Containers are auto-created if missing.

**Example Create Menu Item (multipart/form-data):**
```http
POST /api/menuitems
Content-Type: multipart/form-data

name: "Pizza"
price: 12.99
category: "Food"
specialTag: "Hot"
description: "Delicious pizza"
file: <image-file>
```

---

## Payments (Stripe)
- Stripe is used for secure payment processing.
- Set your Stripe secret key in `appsettings.json`.
- Payment intent is created and client secret returned for frontend integration.

**Example Payment Request:**
```http
POST /api/payment/{userId}
```
**Response:**
```json
{
  "isSuccess": true,
  "result": {
    "stripePaymentIntentId": "pi_123456789",
    "clientSecret": "sk_test_..."
  },
  "statusCode": 200
}
```

---

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

---

## License
This project is licensed under the MIT License.

---

## Acknowledgements
- [Microsoft ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
- [Azure Blob Storage](https://docs.microsoft.com/en-us/azure/storage/blobs/)
- [Stripe](https://stripe.com/docs/api)

---
