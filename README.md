This is the Backend API source code for the **"Food Market"** project. The system provides high-performance RESTful APIs, acting as the central hub for data processing and business logic for the Front-end, serving Buyers, Sellers, and Administrators.

## 🚀 Tech Stack
Based on the proposed architecture, the backend is built with the following technologies:
*   **Framework:** .NET (C#)
*   **Architecture:** RESTful API, Layered Architecture (Controller - Service - Repository)
*   **Database ORM:** Entity Framework Core (Supporting SQL Server/PostgreSQL)
*   **Authentication & Security:** JSON Web Token (JWT) - Bearer Token via Header
*   **Data Flow:** Asynchronous API Calls to optimize I/O operations

## 🗄️ Database Structure
The system manages 15 tightly integrated and normalized tables:
*   **User & Store Management:** `users` (Roles: User, Seller, Admin), `stores`
*   **Product Management:** `categories`, `products`, `product_images`, `product_options`, `product_option_values`
*   **Transactions & Cart:** `carts`, `cart_items`, `orders`, `order_items`
*   **Interactions & Promotions:** `reviews`, `favorites`, `vouchers`, `seller_notifications`

## 📡 Core API Endpoints

### 1. Authentication & Profile
*   `POST /api/auth/register`: Handle account registration.
*   `POST /api/auth/login`: Authenticate credentials and issue JWT Access Token.
*   `PUT /api/users/profile`: Update personal information and Avatar.
`...`

### 2. Buyer Module
*   `GET /api/products`: Retrieve product list (with filtering, pagination, and search).
*   `POST /api/cart`: Add items to the cart and automatically calculate subtotals, taxes, and fees.
*   `POST /api/orders`: Process checkout requests, validate vouchers, and create invoices.
*   `GET /api/orders/history`: Retrieve the user's order history.
*   `POST /api/favorites`: Toggle product favorite status.
`...`

### 3. Seller Module
*   `GET /api/seller/dashboard`: Provide aggregated data for charting (revenue, order count, average rating).
*   `POST /api/seller/products`: Create new dishes (supports image uploads via `multipart/form-data`).
*   `PUT /api/seller/orders/{id}/status`: Process order status updates (Pending -> Confirmed -> Delivering -> Completed).
`...`
