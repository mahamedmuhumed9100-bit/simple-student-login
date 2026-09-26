using StudentLogin;

var auth = new AuthService();

// Demo accounts (a real app would load these from a database).
auth.Register("student1@example.com", "Password123!");
auth.Register("student2@example.com", "MySecurePass!1");

Console.WriteLine("=== Student Login System ===\n");

while (true)
{
    Console.Write("Student email: ");
    string email = Console.ReadLine() ?? "";

    Console.Write("Password: ");
    string password = ReadPassword();
    Console.WriteLine();

    switch (auth.Login(email, password))
    {
        case LoginResult.Success:
            Console.WriteLine("\n✅ Login successful! Welcome to the student dashboard.");
            return;
        case LoginResult.InvalidCredentials:
            // Same message for a wrong email or a wrong password, so the login
            // form can't be used to discover which emails are registered.
            Console.WriteLine("❌ Invalid email or password. Please try again.\n");
            break;
        case LoginResult.LockedOut:
            Console.WriteLine($"🔒 Too many failed attempts ({AuthService.MaxFailedAttempts}). This account is locked.");
            return;
    }
}

// Reads a password without echoing it, showing * for each character.
static string ReadPassword()
{
    var password = new System.Text.StringBuilder();
    ConsoleKeyInfo key;
    while ((key = Console.ReadKey(intercept: true)).Key != ConsoleKey.Enter)
    {
        if (key.Key == ConsoleKey.Backspace && password.Length > 0)
        {
            password.Length--;
            Console.Write("\b \b");
        }
        else if (!char.IsControl(key.KeyChar))
        {
            password.Append(key.KeyChar);
            Console.Write('*');
        }
    }
    return password.ToString();
}
