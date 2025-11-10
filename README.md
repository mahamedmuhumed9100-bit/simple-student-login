# Simple Student Login (C# Console App)

A simple **student login system** built in C# as a console application.

**Author:** Mahamed Muhumed  
**GitHub:** https://github.com/mahamedmuhumed9100-bit

## Features

- Prompts the user for:
  - Student email
  - Password (hidden as you type)
- Uses **SHA256 hashing** to securely hash the password
- Uses a mock in-memory "database" (C# dictionary) of users
- Authenticates the user by comparing the hashed password against stored values
- Displays a clear login summary and a success/failure message

## Technologies Used

- C#
- .NET Console Application
- `System.Security.Cryptography` for hashing

## How to Run

1. Open the project in **Visual Studio** or **VS Code**.
2. Make sure you have the .NET SDK installed.
3. Build and run the project.
4. Try logging in with:

```text
Email: student1@example.com
Password: Password123!
```

## Future Improvements

- Connect to a real database (e.g. SQL Server)
- Add user registration
- Add password reset functionality
