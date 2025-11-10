using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace StudentLoginSystem
{
    internal class Program
    {
        // Mock "database" of users: email + hashed password
        static Dictionary<string, string> users = new Dictionary<string, string>()
        {
            // email                     // password = "Password123!"
            { "student1@example.com", HashPassword("Password123!") },
            { "student2@example.com", HashPassword("MySecurePass!1") }
        };

        static void Main(string[] args)
        {
            Console.WriteLine("=== Student Login System ===\n");

            Console.Write("Enter your student email: ");
            string email = Console.ReadLine();

            Console.Write("Enter your password: ");
            string password = ReadPassword();

            string hashedPassword = HashPassword(password);

            Console.WriteLine("\n\n--- Login Summary ---");
            Console.WriteLine($"Email entered: {email}");
            Console.WriteLine($"Hashed password: {hashedPassword}");

            bool isAuthenticated = AuthenticateUser(email, hashedPassword);

            if (isAuthenticated)
            {
                Console.WriteLine("\n✅ Login successful! Welcome to the student dashboard.");
            }
            else
            {
                Console.WriteLine("\n❌ Login failed. Invalid email or password.");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        // Hash the password using SHA256
        static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Check if email + hashed password match the "database"
        static bool AuthenticateUser(string email, string hashedPassword)
        {
            if (users.ContainsKey(email))
            {
                string storedHash = users[email];
                return storedHash == hashedPassword;
            }
            return false;
        }

        // Hide password while typing
        static string ReadPassword()
        {
            StringBuilder password = new StringBuilder();
            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey(intercept: true);

                if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    password.Append(keyInfo.KeyChar);
                    Console.Write("*");
                }

            } while (keyInfo.Key != ConsoleKey.Enter);

            return password.ToString();
        }
    }
}
