namespace StudentLogin;

public enum LoginResult
{
    Success,
    InvalidCredentials,
    LockedOut,
}

/// <summary>
/// In-memory user store with login and account lockout. Kept separate from the
/// console UI so the rules can be unit tested.
/// </summary>
public class AuthService
{
    public const int MaxFailedAttempts = 3;
    public const int MinPasswordLength = 8;

    // Email addresses are case-insensitive, so Alice@x.com and alice@x.com are the same user.
    private readonly Dictionary<string, string> _passwordHashes = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _failedAttempts = new(StringComparer.OrdinalIgnoreCase);

    // Used to spend the same time on unknown emails as on real ones, so the
    // response time doesn't reveal which emails are registered.
    private static readonly string DummyHash = PasswordHasher.Hash("dummy-password-for-timing");

    public void Register(string email, string password)
    {
        email = email.Trim();
        if (email.Length == 0 || !email.Contains('@'))
            throw new ArgumentException("A valid email address is required.", nameof(email));
        if (password.Length < MinPasswordLength)
            throw new ArgumentException($"Passwords must be at least {MinPasswordLength} characters.", nameof(password));
        if (!_passwordHashes.TryAdd(email, PasswordHasher.Hash(password)))
            throw new InvalidOperationException($"{email} is already registered.");
    }

    public LoginResult Login(string email, string password)
    {
        email = email.Trim();

        if (_failedAttempts.GetValueOrDefault(email) >= MaxFailedAttempts)
            return LoginResult.LockedOut;

        bool isKnownUser = _passwordHashes.TryGetValue(email, out string? storedHash);
        bool passwordMatches = PasswordHasher.Verify(password, storedHash ?? DummyHash);

        if (isKnownUser && passwordMatches)
        {
            _failedAttempts.Remove(email);
            return LoginResult.Success;
        }

        int failures = _failedAttempts.GetValueOrDefault(email) + 1;
        _failedAttempts[email] = failures;
        return failures >= MaxFailedAttempts ? LoginResult.LockedOut : LoginResult.InvalidCredentials;
    }
}
