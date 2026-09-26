using StudentLogin;

namespace StudentLogin.Tests;

public class AuthServiceTests
{
    private readonly AuthService _auth = new();

    public AuthServiceTests()
    {
        _auth.Register("student@example.com", "Password123!");
    }

    [Fact]
    public void Login_WithCorrectCredentials_Succeeds()
    {
        Assert.Equal(LoginResult.Success, _auth.Login("student@example.com", "Password123!"));
    }

    [Fact]
    public void Login_EmailIsCaseInsensitiveAndTrimmed()
    {
        Assert.Equal(LoginResult.Success, _auth.Login("  Student@Example.com ", "Password123!"));
    }

    [Fact]
    public void Login_WithWrongPassword_Fails()
    {
        Assert.Equal(LoginResult.InvalidCredentials, _auth.Login("student@example.com", "wrong-password"));
    }

    [Fact]
    public void Login_WithUnknownEmail_FailsTheSameWayAsAWrongPassword()
    {
        Assert.Equal(LoginResult.InvalidCredentials, _auth.Login("nobody@example.com", "Password123!"));
    }

    [Fact]
    public void Login_LocksAccountAfterMaxFailedAttempts()
    {
        for (int i = 1; i < AuthService.MaxFailedAttempts; i++)
            Assert.Equal(LoginResult.InvalidCredentials, _auth.Login("student@example.com", "wrong"));

        Assert.Equal(LoginResult.LockedOut, _auth.Login("student@example.com", "wrong"));
    }

    [Fact]
    public void Login_LockedAccount_RejectsEvenTheCorrectPassword()
    {
        for (int i = 0; i < AuthService.MaxFailedAttempts; i++)
            _auth.Login("student@example.com", "wrong");

        Assert.Equal(LoginResult.LockedOut, _auth.Login("student@example.com", "Password123!"));
    }

    [Fact]
    public void Login_SuccessResetsTheFailureCount()
    {
        for (int i = 1; i < AuthService.MaxFailedAttempts; i++)
            _auth.Login("student@example.com", "wrong");
        _auth.Login("student@example.com", "Password123!");

        // Back to a full set of attempts after a successful login.
        Assert.Equal(LoginResult.InvalidCredentials, _auth.Login("student@example.com", "wrong"));
    }

    [Fact]
    public void Register_DuplicateEmail_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => _auth.Register("STUDENT@example.com", "AnotherPass1!"));
    }

    [Theory]
    [InlineData("", "Password123!")]
    [InlineData("not-an-email", "Password123!")]
    [InlineData("new@example.com", "short")]
    public void Register_InvalidInput_Throws(string email, string password)
    {
        Assert.Throws<ArgumentException>(() => _auth.Register(email, password));
    }
}
