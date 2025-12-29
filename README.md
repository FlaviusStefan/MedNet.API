# MedNet.API

A **Medical Clinic Management System API** built with **.NET Core** and **MS SQL Server**, designed using Clean Architecture principles to deliver a high-quality, maintainable, and scalable solution.

---

## Table of Contents
- [Overview](#overview)  
- [Built With](#built-with)  
- [Architecture](#architecture)  
- [Features](#features)  
- [Getting Started](#getting-started)  
  - [Prerequisites](#prerequisites)  
  - [Installation & Setup](#installation--setup)  
- [Usage](#usage)  
- [Contributing](#contributing)  
- [License](#license)  
- [Contact](#contact)

---

## Overview
The MedNet.API serves as the backend for managing a medical clinic’s operations—covering patients, appointments, doctors, and other core entities. Built with **Clean Code**, adhering to **SOLID** principles, making it robust, testable, and extensible.

---

## Built With
- **.NET Core**  
- **Entity Framework Core**  
- **MS SQL Server**  
- **ASP.NET Core Identity** (Authentication & Authorization)  
- **Dependency Injection**  
- **Controller–Service–Repository** Pattern  

---

## Architecture
This project uses a layered architecture with clear boundaries:
- **Controllers** – Handle HTTP requests and orchestrate between UI and business logic.  
- **Services** – Contain business logic, implement interfaces, and assure single responsibility.  
- **Repositories** – Handle data access, separation of concerns, and easy mocking for tests.  
- **Integration of SOLID principles** – Ensures modularity, maintainability, and scalability.  

---

## Features
- [x] **User Authentication & Authorization** via ASP.NET Core Identity  
- [x] CRUD endpoints for core clinic entities (Patients, Appointments, Doctors, etc.)  
- [x] Flexible **Dependency Injection** setup for loose coupling  
- [x] Adherence to **SOLID principles** and Clean Code best practices  
- [x] Scalable and well-organized architecture (Controller → Service → Repository)

---

## Getting Started

### Prerequisites
- [.NET 8 SDK or later](https://dotnet.microsoft.com/download)  
- [MS SQL Server LocalDB or full version]  
- An IDE of your choice (e.g., Visual Studio, VS Code)

### Installation & Setup
1. Clone the repo:  
   ```bash
   git clone https://github.com/FlaviusStefan/MedNet.API.git
   cd MedNet.API
   ```
2. Setup connection string in `appsettings.json`:  
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=MedNetDb;Trusted_Connection=True;"
   }
   ```
3. Apply Entity Framework Core migrations and create the database:  
   ```bash
   dotnet ef database update
   ```
4. Run the application:  
   ```bash
   dotnet run
   ```
5. The API should now be running at `https://localhost:5001` (or another available port).

---

## Usage
- Use tools like **Postman** or **Swagger** to explore API endpoints (e.g., `/api/patients`, `/api/appointments`, `/api/auth/register`, `/api/auth/login`, etc.).  
- Protect endpoints with authentication; for example, `[Authorize]` attributes on controllers or individual actions as needed.

---

## Contributing
Contributions, issues, and feature requests are welcome!  
1. Fork the repository  
2. Create a new branch (`git checkout -b feature/YourFeature`)  
3. Commit your changes (`git commit -m 'Add new feature'`)  
4. Push to your branch (`git push origin feature/YourFeature`)  
5. Open a Pull Request

---

## License
This project is licensed under the [MIT License](./LICENSE).

---

## Contact
**Flavius Stefan** — [GitHub Profile](https://github.com/FlaviusStefan)  
Feel free to reach out for collaboration or questions!
