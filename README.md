# 📚 Book E-Commerce

**GitHub:** https://github.com/UDAY-exit
**LinkedIn:** https://linkedin.com/in/uday-087892371/

A full-stack **Book E-Commerce web application** built with **ASP.NET Core MVC**. The application provides a complete shopping experience with customer features, admin management, online payments, and automated order notifications.

## 🚀 Features

### Customer

* User registration and login
* Browse books and categories
* View product details
* Shopping cart
* Checkout and order placement
* Order history

### Admin

* Product management
* Category and cover type management
* Order management
* Update order status
* Search and filter orders
* View order details

### Integrations

* 💳 **Stripe** — Online payments
* 📧 **SMTP** — Email notifications
* 📱 **Twilio** — SMS notifications
* 📞 **Twilio** — Voice-call notifications

## 🛠️ Tech Stack

| Area           | Technologies                                           |
| -------------- | ------------------------------------------------------ |
| Backend        | C#, ASP.NET Core MVC                                   |
| ORM            | Entity Framework Core                                  |
| Database       | SQL Server                                             |
| Authentication | ASP.NET Core Identity                                  |
| Frontend       | Razor Views, HTML5, CSS3, Bootstrap                    |
| JavaScript     | JavaScript, jQuery, DataTables                         |
| Architecture   | Repository Pattern, Unit of Work, Dependency Injection |
| Payments       | Stripe                                                 |
| Notifications  | SMTP, Twilio                                           |

## 🔑 Configuration

The application uses third-party services that require your own credentials.

| Service    | Purpose              |
| ---------- | -------------------- |
| Stripe     | Online payments      |
| Twilio     | SMS and voice calls  |
| SMTP       | Email notifications  |
| SQL Server | Application database |

Configure the required credentials in your local application settings before running the project.

> ⚠️ **Security:** Never commit API keys, passwords, authentication secrets, or database credentials to GitHub.

## ▶️ Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/UDAY-exit/Book-Ecommerce-Repository-Pattern_.net.git
```

### 2. Configure the Application

Configure your SQL Server connection string and the required third-party service credentials.

### 3. Update the Database

Open the **Package Manager Console** in Visual Studio and run:

```powershell
Update-Database
```

### 4. Run the Application

Open the solution in Visual Studio, build the project, and run the application.

## 🎥 Demo

A 5-minute walkthrough demonstrating the main functionality of the application, including customer shopping, admin management, Stripe payments, and email, SMS, and voice notifications.

**▶️ Watch the Project Demo:**
https://lnkd.in/p/gaQYuphH

## 🔄 Application Flow

```text
User
  ↓
Register / Login
  ↓
Browse Books
  ↓
Product Details
  ↓
Shopping Cart
  ↓
Checkout
  ↓
Stripe Payment
  ↓
Order Created
  ↓
Email + SMS + Voice Notification
  ↓
Admin Order Management
  ↓
Update Order Status
```

## 🏗️ Project Architecture

```text
ASP.NET Core MVC
│
├── Controllers
├── Views
├── Models
├── Areas
│   ├── Admin
│   └── Coustomer
│
├── Data
│   └── ApplicationDbContext
│
├── Repositories
│   ├── Repository
│   ├── Category
│   ├── Product
│   ├── ShoppingCart
│   ├── OrderHeader
│   └── OrderDetail
│
├── Unit of Work
│
├── Utility
│   ├── Email Service
│   ├── Twilio Service
│   └── Stripe Settings
│
└── wwwroot
```

## 📁 Project Structure

```text
Ecommerce12Aug_Project
│
├── Areas
│   ├── Admin
│   └── Coustomer
│
├── Controllers
├── Data
├── Models
├── Repositories
├── Views
├── Utility
├── wwwroot
│
├── Program.cs
└── appsettings.json
```

## 🔐 Security

* ASP.NET Core Identity for authentication and user management
* Role-based access for Admin and Customer areas
* Sensitive credentials are stored locally and are not committed to GitHub
* Stripe and Twilio credentials are configured through application settings

## 📌 What I Learned

Building this project helped me gain practical experience in:

* ASP.NET Core MVC
* Entity Framework Core
* SQL Server
* Repository Pattern
* Unit of Work
* Dependency Injection
* ASP.NET Core Identity
* Stripe payment integration
* Email notifications
* Twilio SMS and voice integration
* Admin and customer workflows
* Building and integrating a complete web application

## 👨‍💻 Author

**Uday**
.NET Full-Stack Developer
