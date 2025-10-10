# CSE Muscle Cars 🏎️

**A Modern Web Application for Muscle Car Sales & Repair Services**

### Team 6 Members

- **Giuseppe Brandi**
- **Braxton Goode**
- **Ifeanyi**
- **Antonio (Jose) Melendez**
- **Jaydan Valencia**

---

CSE Muscle Cars is a comprehensive web platform that connects muscle car enthusiasts with their dream vehicles and provides professional repair services. Whether you're looking to purchase a classic American muscle car or need expert maintenance for your vehicle, our platform makes it simple and convenient.

## 📖 Project Overview

### What is CSE Muscle Cars?

CSE Muscle Cars is a full-featured e-commerce and service management website built for muscle car dealerships. The application serves three main purposes:

1. **Vehicle Sales Platform** - Browse and purchase muscle cars from brands like Chevrolet, Ford, and Dodge
2. **Service Center** - Schedule and manage vehicle repairs and maintenance
3. **Customer Management** - Create accounts, track orders, and communicate with the dealership

### Who is it for?

- **Car Buyers**: Enthusiasts looking to purchase muscle cars
- **Current Owners**: Customers needing repair and maintenance services
- **Dealership Staff**: Manage inventory, customer inquiries, and service requests

### Key Features

✅ Browse available muscle car inventory with detailed specifications  
✅ User registration and account management  
✅ Contact form for inquiries and service requests  
✅ Responsive design that works on desktop, tablet, and mobile  
✅ Detailed information about repair services offered  
✅ Modern, fast, and secure platform

---

## 🚀 Installation / Configuration Guide

Follow these step-by-step instructions to set up the project on your local machine.

### Prerequisites

Before you begin, make sure you have the following installed:

- **.NET 9.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **PostgreSQL Database** - [Download here](https://www.postgresql.org/download/)
- **Git** - [Download here](https://git-scm.com/downloads)
- **Code Editor** - Visual Studio 2022, VS Code, or Rider

### Step 1: Clone the Repository

```bash
git clone https://github.com/giuseppebrandi-byui/cse325-Team6-Project.git
cd cse325-Team6-Project
```

### Step 2: Set Up the Database

1. **Create a PostgreSQL Database**:

   - Open PostgreSQL and create a new database named `cse_muscle_cars`
   - Note your database connection details (host, port, username, password)

2. **Create Environment File**:
   - In the project root directory, create a file named `.env`
   - Add your database connection string:

```
DATABASE_URL=Host=localhost;Port=5432;Database=cse_muscle_cars;Username=your_username;Password=your_password
```

### Step 3: Install Dependencies

The project dependencies are managed by NuGet and will be restored automatically:

```bash
dotnet restore
```

### Step 4: Run Database Migrations

Apply the database schema to your PostgreSQL database:

```bash
dotnet ef database update
```

### Step 5: Run the Application

Start the web server:

```bash
dotnet run
```

The application will be available at:

- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`

### Step 6: Access the Application

Open your web browser and navigate to `https://localhost:5001` to start using the application!

---

## 📘 User Manual

### For Car Buyers

#### 1. Browsing Available Vehicles

- Click on **"Vehicles"** in the navigation menu
- View all available muscle cars with photos, specifications, and pricing
- Each car displays:
  - Make and Model
  - Year
  - Price
  - Mileage
  - Color
  - Engine specifications

#### 2. Creating an Account

- Click on **"Login"** in the navigation menu
- Select **"Create New Account"** (if register page is available)
- Fill in your information:
  - First Name
  - Last Name
  - Email Address
  - Password
- Submit the form to create your account

#### 3. Contacting the Dealership

- Click on **"Contact"** in the navigation menu
- Fill out the contact form with:
  - Your full name
  - Email address
  - Your message or inquiry
- Click **"Send Message"**
- You'll receive a confirmation that your message was received

#### 4. Learning About Services

- Click on **"Services"** in the navigation menu
- Browse available repair and maintenance services:
  - Engine Repair
  - Brake Repair
  - Tire Services
  - Battery Replacement
  - Steering Repair
- Each service includes a description and pricing information

### For Dealership Staff

#### Accessing Inventory Data

The application provides an API endpoint for managing inventory:

**Get All Vehicles**:

```
GET /api/inventory
```

**Get Specific Vehicle**:

```
GET /api/inventory/{id}
```

Example response:

```json
{
  "invId": 1,
  "invMake": "Chevrolet",
  "invModel": "Camaro SS",
  "invYear": "2024",
  "invPrice": 45000,
  "invMiles": 0,
  "invColor": "Red",
  "invDescription": "Brand new 2024 Camaro SS with V8 engine"
}
```

---

## 🔧 Troubleshooting Guide

### Common Issues and Solutions

#### Issue 1: Application Won't Start

**Error Message**: `Connection to the database failed`

**Solution**:

1. Verify PostgreSQL is running
2. Check your `.env` file exists and has the correct connection string
3. Ensure the database `cse_muscle_cars` exists
4. Verify username and password are correct

#### Issue 2: Database Migration Errors

**Error Message**: `Could not execute migrations`

**Solution**:

1. Make sure Entity Framework Core tools are installed:

```bash
dotnet tool install --global dotnet-ef
```

2. Delete the database and recreate it:

```bash
dotnet ef database drop
dotnet ef database update
```

#### Issue 3: Missing Dependencies

**Error Message**: `Package restore failed`

**Solution**:

1. Clear the NuGet cache:

```bash
dotnet nuget locals all --clear
```

2. Restore packages again:

```bash
dotnet restore
```

#### Issue 4: Port Already in Use

**Error Message**: `Address already in use`

**Solution**:

1. Stop any other applications using ports 5000 or 5001
2. Or change the port in `Properties/launchSettings.json`

#### Issue 5: Images Not Loading

**Solution**:

1. Ensure all image files are in the `wwwroot/images/` directory
2. Check file paths are correct (case-sensitive on some systems)
3. Clear browser cache and refresh the page

#### Issue 6: Changes Not Appearing

**Solution**:

1. Stop the application (Ctrl+C)
2. Clear build artifacts:

```bash
dotnet clean
```

3. Rebuild and run:

```bash
dotnet build
dotnet run
```

---

## ❓ Frequently Asked Questions (FAQs)

### Q1: Is this application ready for production use?

**A**: This is currently an academic project developed as part of CSE 325 coursework. While it includes core functionality for vehicle browsing and contact management, it would require additional features for production deployment, including:

- Complete user authentication and authorization
- Payment processing integration
- Enhanced security measures
- Admin dashboard for inventory management
- Order tracking and history

### Q2: Can I add my own vehicles to the inventory?

**A**: Currently, inventory management requires direct database access. To add vehicles:

1. Connect to your PostgreSQL database
2. Insert records into the `inventory` and `make` tables
3. Or use the API endpoints (if you have admin authentication implemented)

Future versions will include an admin panel for easier inventory management.

### Q3: What browsers are supported?

**A**: CSE Muscle Cars works best on modern browsers:

- ✅ Google Chrome (recommended)
- ✅ Mozilla Firefox
- ✅ Microsoft Edge
- ✅ Safari

### Q4: How do I deploy this application to a web server?

**A**: The project includes deployment documentation in `RENDER_DEPLOY.md` for deploying to Render.com. For other platforms:

1. Build the production version:

```bash
dotnet publish -c Release
```

2. Copy the contents of `bin/Release/net9.0/publish/` to your server
3. Configure your web server (IIS, Nginx, Apache) to run .NET applications
4. Set up your production database connection string

### Q5: Where are customer messages stored?

**A**: All contact form submissions are stored in the PostgreSQL database in the `contact_message` table. Staff can query this table directly or through the API to view customer inquiries.

### Q6: Can I customize the styling and branding?

**A**: Yes! All styles are located in:

- `wwwroot/css/site.css` - Main stylesheet
- Component-specific `.css` files in `Components/Layout/` and `Components/Pages/`

Images and logos are in `wwwroot/images/`. Replace these files with your own branding materials.

---

## 👥 Team & Contributions

### Development Team

This project was developed by **Team 6** for CSE 325 at Brigham Young University - Idaho:

#### **Giuseppe Brandi**

- Project scaffolding and initial architecture setup
- Vehicles page with inventory display
- Single vehicle detail page
- Checkout page functionality
- Frontend development and code to fetch data from API
- Home page polishing and dynamic features
- Basic styling and UI design
- Docker configuration and Render cloud deployment
- Project coordination, pull request reviews, and version control

#### **Braxton Goode**

- User authentication system with JWT Cookie implementation
- Register and Login functionality
- Password hashing and security implementation
- Edit profile functionality
- Delete profile feature
- "My Account" page with profile management
- User session management

#### **Ifeanyi**

- CRUD operations for vehicle inventory
- Database integration and data management
- Contact form backend functionality
- Code documentation and comments
- Data validation and processing

#### **Antonio (Jose) Melendez**

- Error handling implementation across the application
- Services page development and content
- Code commenting and documentation
- Bug fixes and error validation
- CSS styling and responsive design
- Quality assurance and testing

#### **Jaydan Valencia**

- Comprehensive project documentation (README.md)
- Installation and configuration guide
- User manual and usage instructions
- Troubleshooting guide with common issues
- Frequently Asked Questions (FAQs) section
- Contributing guidelines and team credits
- Technical writing and documentation structure

### Technologies Used

- **Framework**: ASP.NET Core 9.0
- **UI**: Blazor Server Components
- **Database**: PostgreSQL 16
- **ORM**: Entity Framework Core 9.0
- **Configuration**: DotNetEnv
- **Styling**: Custom CSS with responsive design

---

## 🤝 How to Contribute

We welcome contributions from the community! Here's how you can help:

### Reporting Issues

If you find a bug or have a suggestion:

1. Check if the issue already exists in the [Issues](https://github.com/giuseppebrandi-byui/cse325-Team6-Project/issues) section
2. If not, create a new issue with a clear description
3. Include steps to reproduce bugs and your environment details

### Contributing Code

1. **Fork the repository**
2. **Create a feature branch**:

```bash
git checkout -b feature/your-feature-name
```

3. **Make your changes**
4. **Commit with clear messages**:

```bash
git commit -m "Add feature: description of what you added"
```

5. **Push to your fork**:

```bash
git push origin feature/your-feature-name
```

6. **Open a Pull Request** with a description of your changes

### Coding Standards

- Follow C# naming conventions
- Add comments for complex logic
- Write meaningful commit messages
- Test your changes before submitting

---

## 📞 Support & Contact

For questions, support, or feedback:

- **Course**: CSE 325 - Web Engineering II
- **Institution**: Brigham Young University - Idaho
- **Project Repository**: [GitHub](https://github.com/giuseppebrandi-byui/cse325-Team6-Project)

---

## 🎯 Project Status

**Current Version**: 1.0.0  
**Status**: ✅ Active Development  
**Last Updated**: December 2024

### Completed Features

- ✅ Vehicle inventory display
- ✅ User registration system
- ✅ Contact form with database storage
- ✅ Service information pages
- ✅ Responsive navigation
- ✅ RESTful API endpoints
- ✅ Database integration

### Planned Enhancements

- 🔄 User authentication and sessions
- 🔄 Shopping cart and checkout
- 🔄 Payment processing
- 🔄 Admin dashboard
- 🔄 Order tracking
- 🔄 Customer reviews and ratings

---

**Built by Team 6 | BYU-Idaho CSE 325 | Fall 2024**
