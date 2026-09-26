using StudentLogin;

namespace StudentLogin.Tests;

public class PasswordHasherTests
{
    [Fact]
    public void Hash_DoesNotContainThePlainPassword()
    {
        Assert.DoesNotContain("Password123!", PasswordHasher.Hash("Password123!"));
    }

    [Fact]
    public void Hash_SamePasswordTwice_GivesDifferentHashes()
    {
        // Different random salts, so identical passwords can't be spotted in a leaked table.
        Assert.NotEqual(PasswordHasher.Hash("Password123!"), PasswordHasher.Hash("Password123!"));
    }

    [Fact]
    public void Verify_CorrectPassword_ReturnsTrue()
    {
        string hash = PasswordHasher.Hash("Password123!");
        Assert.True(PasswordHasher.Verify("Password123!", hash));
    }

    [Theory]
    [InlineData("password123!")]   // wrong case
    [InlineData("Password123")]    // missing a character
    [InlineData("")]
    public void Verify_WrongPassword_ReturnsFalse(string attempt)
    {
        string hash = PasswordHasher.Hash("Password123!");
        Assert.False(PasswordHasher.Verify(attempt, hash));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-hash")]
    [InlineData("abc.c2FsdA==.aGFzaA==")]      // iteration count isn't a number
    [InlineData("1000.!!!notbase64.aGFzaA==")]
    public void Verify_MalformedStoredHash_ReturnsFalseInsteadOfThrowing(string stored)
    {
        Assert.False(PasswordHasher.Verify("Password123!", stored));
    }
}
