# 📌 Gringotts: RFID-Based Access & Log Management System

## 📖 Project Overview

Gringotts is designed for **automated tracking, authentication, and monitoring** using RFID technology. Initially developed for **library environments**, it is adaptable to various applications, including:

- **🏢 Workplace Access Control** – Managing employee entry and exit.
- **🎟️ Event Management** – Tracking attendee movement in real-time.
- **🏫 Educational Institutions** – Automating student attendance.
- **🔐 Secure Facilities** – Controlling access to restricted areas.
- **🏭 Industrial & Commercial Use** – Personnel and asset tracking.

## 🚀 Key Features

- **🔑 Client Authentication** – Secure login system for dashboard access.
- **📡 RFID Reader Registration** – Virtual representation of physical RFID readers.
- **📋 Log Management** – Automatic recording of RFID interactions.
- **📊 Filtering & Exporting** – Advanced filtering and data export capabilities.
- **📍 Real-time Tracking** – Monitors last known locations based on RFID activity.
- **⚡ Minimal API Architecture** – Feature-based structure for scalability.

## 🛠 Technology Stack

### 🖥 Backend

- **.NET 8 (ASP.NET Core Minimal API)** – High-performance framework.
- **🐘 PostgreSQL** – Scalable relational database.

## 🏗 Getting Started

### ✅ Prerequisites

Ensure the following dependencies are installed:

- **.NET 8 SDK**
- **PostgreSQL**
- **Git**

### ⚙️ Setup Instructions

#### 📥 Clone the Repository

```sh
git clone https://github.com/andeng07/gringotts-backend
cd gringotts-backend
```

#### 🛠 Configure Environment Settings

Update `appsettings.json` and `appsettings.Development.json` with the appropriate configuration:

```json
{
  "Database": {
    "ConnectionString": "YOUR_CONNECTION_STRING"
  },
  "Jwt": {
    "Secret": "YOUR_SECRET",
    "Issuer": "Gringotts.Authentication",
    "Audience": "Gringotts.Services"
  }
}
```

#### 🗄 Apply Database Migrations

```
dotnet ef database update
```

#### ▶️ Run the Application

```
dotnet run
```

#### 🌐 Access the API

- **Base URL:** `http://<your-lan-ip>:7107`
- **Swagger UI:** `http://<your-lan-ip>:7107/swagger`

## 📂 Project Structure

```sh
rfid-log-management/
├── Features/
│   ├── Client/
│   ├── ClientAuthentication/
│   ├── Interactions/
│   ├── Reader/
│   ├── Stats/
│   ├── User/
│
├── Shared/
│   ├── Core/
│   ├── Errors/
│   ├── Filters/
│   ├── Pagination/
│   ├── Results/
│   ├── Utilities/
│
├── Program.cs
├── appsettings.json
└── appsettings.Development.json
```

## 🤝 Contributing

Contributions are welcome. To contribute:

1. **Fork** the repository.
2. **Create a new branch** (`git checkout -b feature-branch`).
3. **Make changes and commit them** (`git commit -m "Add new feature"`).
4. **Push to the branch** (`git push origin feature-branch`).
5. **Submit a pull request** for review.

## 📜 License

This project is licensed under the **Apache 2.0 License**. See the `LICENSE` file for details.

## 👥 Contributors

- **Andrei John Sumilang** – Lead Developer & Researcher

📩 Contact: **sumilangandreijohn@gmail.com**

