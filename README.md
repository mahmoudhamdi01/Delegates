# Delegates Management System

🔗 **Live API / Swagger Docs:** https://delegatessystem.runasp.net/swagger/index.html

## What is this project?

A backend system that helps companies manage their sales team out in the field. Customer service staff create orders for customers, the system checks warehouse stock and assigns delivery reps, and each rep gets the order on their phone — accepts, delivers, or reschedules it. Management can see everything happening across the business at any time: orders, deliveries, and rep visits.

Built to support multiple companies on the same platform, each with its own isolated data, users, and permissions (admin, customer service, delivery rep).

## Key Features

- **Multi-tenant architecture** — each business account's data is fully isolated
- **Role-based access** — Super Admin, Admin, Customer Service, Delegate
- **Full order lifecycle** — inquiry → approval → delivery, with shortage handling via secondary warehouses and delayed orders pending company purchases
- **Inventory management** — per-warehouse stock tracking with expiry tracking
- **Delegate workflow** — order delivery/cancellation/postponement, client visit scheduling with GPS check-in
- **Admin dashboards** — filterable order and visit reports across the whole business
- **Push notifications** — real-time alerts to delegates via Firebase Cloud Messaging

## Tech Stack

ASP.NET Core Web API · Entity Framework Core · SQL Server · AutoMapper · JWT Authentication
