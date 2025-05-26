# 🏨 Hotel Booking System

A comprehensive hotel booking system built with ASP.NET Core MVC, enabling users to book hotels, manage reservations, and providing administrative features for efficient hotel management.

## ✨ Features

### 👤 User Features
- 🔐 User registration and authentication
- 🌐 External login support (Google)
- 🔑 Password reset with email confirmation
- 🔍 Hotel search and filtering
- 🛏️ Room availability checking
- 📋 Booking management
- ⭐ Review and rating system
- 🗺️ Interactive map for hotel locations
- 💳 Payment integration
- 📜 Booking history

### 🛠️ Admin Features
- 🏠 Hotel management (CRUD operations)
- 🛋️ Room management
- 📅 Booking management
- 👥 User management
- ✅ Review moderation

### 💻 Technical Features
- 📱 Responsive design
- ⚡ Real-time availability updates
- 🔒 Secure payment processing
- 📧 Email service integration
- 🌍 Map integration for location services
- 🛡️ Role-based authorization
- ✅ Data validation and sanitization
- 📊 Error logging and monitoring

## 📋 Prerequisites
- 🖥️ .NET 7.0 SDK or later
- 🗄️ SQL Server (Express or higher)
- 📧 SMTP server for email functionality (e.g., Gmail SMTP)
- 🗺️ Google Maps API key for location services
- 💰 Payment gateway account (e.g., Stripe or PayPal)

## 🚀 Getting Started

1. 📥 Clone the repository:
```bash
git clone [repository-url]
cd HotelBookingSystem

2. 🔧 Update the connection string in appsettings.json with your database details:
-"ConnectionStrings": {
  "DefaultConnection": "Server=your_server;Database=HotelBookingDB;Trusted_Connection=True;"
}
3. ⚙️ Update the following settings in appsettings.json:
   - 📧 Email configuration
   - 🗺️ Google Maps API key
   - 💳 Payment gateway credentials
   - 🌐 External login credentials

4. 🛠️ Run the following commands in the Package Manager Console:
```bash
Update-Database
```

5. ▶️ Run the application:
```bash
dotnet run
```

## 🔐 Default Admin Account
- UserName: admin@site.com
- Password: Admin@123

## 🌐 External Login Setup
1. Configure Google OAuth:
   - 🌐 Go to Google Cloud Console
   - 📋 Create a new project
   - ✅ Enable Google+ API
   - 🔑 Create OAuth 2.0 credentials
   - 🔗 Add authorized redirect URIs


## 📧 Email Configuration
The system uses SMTP for sending emails. Configure the following in `appsettings.json`:
```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "Port": 587,
  "Username": "your-email@gmail.com",
  "Password": "your-app-password",
  "FromEmail": "your-email@gmail.com",
  "FromName": "Hotel Booking System"
}
```

## 🗺️ Map Integration
The system uses Google Maps API for location services. Add your API key in `appsettings.json`:
```json
"GoogleMaps": {
  "ApiKey": "your-api-key"
}
```


#  Made with ❤️ for learning.*
 ## 👥 Contributors 
- Ahmed Abu-elmagd
- Ahmed Hatem
- Aya Elzoghby
- Alaa Elsisy
- Shimaa Aglan.

