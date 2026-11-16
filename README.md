<div align="center">

# 💊 Pharmacy Management System

### Service-Oriented Architecture (SOA)

**Hệ thống quản lý nhà thuốc theo kiến trúc hướng dịch vụ**

![Spring Boot](https://img.shields.io/badge/Spring_Boot-6DB33F?style=for-the-badge&logo=springboot&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Next.js](https://img.shields.io/badge/Express.js-000000?style=for-the-badge&logo=nextdotdotjs&logoColor=white)
![FastAPI](https://img.shields.io/badge/FastAPI-009688?style=for-the-badge&logo=fastapi&logoColor=white)

![Status](https://img.shields.io/badge/Status-In_Development-orange)
![Architecture](https://img.shields.io/badge/Architecture-SOA-blue)

</div>

---

## 📖 Introduction

Hệ thống quản lý nhà thuốc được xây dựng theo kiến trúc **SOA**, gồm 4 services độc lập, hỗ trợ quản lý thuốc, kho, bán hàng, tài khoản và thống kê.

## 🛠️ Tech Stack

| Service              | Framework    | Responsibility                  |
| -------------------- | ------------ | ------------------------------- |
| 🔐 Auth Service      | Spring Boot  | Authentication & Authorization  |
| 💊 Core Service      | ASP.NET Core | Medicine & Inventory Management |
| 🛒 Sales Service     | Express.js   | Sales & Payment                 |
| 📊 Reporting Service | FastAPI      | Statistics & Reporting          |

## 📁 Project Structure

```text
pharmacy-management-soa/
├── services/
│   ├── auth-service/        # Spring Boot
│   ├── core-service/        # ASP.NET Core
│   ├── sales-service/       # Express.js
│   └── reporting-service/   # FastAPI
├── frontend/
├── database/
├── docs/
├── docker-compose.yml
└── README.md
```

## ✨ Features

- 🔐 Authentication & Role-Based Authorization
- 💊 Medicine & Supplier Management
- 📦 Inventory & Batch Tracking
- 🛒 Sales & Invoice Management
- 📅 Expiration Date Monitoring
- 📊 Revenue & Inventory Reports

## 🚀 Getting Started

```bash
git clone https://github.com/OWNER/pharmacy-management-soa.git
cd pharmacy-management-soa
```

## 👥 Team

Developed by the Pharmacy Management SOA Team.

1. Đặng Phong Hào
2. Nguyễn Trần Thiên Bảo
3. Bùi Thế Sơn
4. Phạm Hoàng An Khang
5. Tô Hoàng Hào

---

<div align="center">

**Spring Boot • ASP.NET Core • Next.js • FastAPI**

_Built for educational purposes._

</div>
