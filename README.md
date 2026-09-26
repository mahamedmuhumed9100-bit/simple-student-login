# Student Login (C# Console App)

[![Build and Test](https://github.com/mahamedmuhumed9100-bit/simple-student-login/actions/workflows/ci.yml/badge.svg)](https://github.com/mahamedmuhumed9100-bit/simple-student-login/actions/workflows/ci.yml)

A console login system in C# that stores and checks passwords the way a real
application should, with the rules covered by xUnit tests.

## Security features

| Feature | Why it matters |
|---|---|
| **PBKDF2 with a random salt** (100,000 iterations, SHA-256) | Plain SHA-256 is fast and unsalted, so a leaked hash can be cracked with lookup tables. A unique salt per password plus a slow hash makes each guess expensive. |
| **Constant-time comparison** (`CryptographicOperations.FixedTimeEquals`) | A normal `==` stops at the first differing byte; that timing difference can leak information. |
| **Account lockout** after 3 failed attempts | Stops someone brute-forcing a password. |
| **Same error for wrong email and wrong password** | The login form can't be used to find out which emails are registered. |
| **Dummy hash check for unknown emails** | Unknown emails take as long to reject as real ones, so response time doesn't leak that either. |
| **Password hidden while typing** | Shows `*` instead of the characters. |

## Project structure

```
src/StudentLogin/
  PasswordHasher.cs   PBKDF2 hashing and verification
  AuthService.cs      user store, login, lockout rules
  Program.cs          console UI only
tests/StudentLogin.Tests/
  PasswordHasherTests.cs
  AuthServiceTests.cs
```

The console UI is kept separate from the logic in `AuthService`, so the login
and lockout rules can be tested without typing into a console.

## Running it

Needs the [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet run --project src/StudentLogin
```

Demo account:

```text
Email:    student1@example.com
Password: Password123!
```

## Running the tests

```bash
dotnet test tests/StudentLogin.Tests
```

## What I'd add next

- Store users in a real database (e.g. SQLite with EF Core) instead of memory
- Unlock accounts automatically after a timeout instead of permanently
- Registration and password reset from the console menu
