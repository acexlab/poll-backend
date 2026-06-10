namespace pollbackend.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var h1 = BCrypt.Net.BCrypt.HashPassword("admin");
        var h2 = BCrypt.Net.BCrypt.HashPassword("Admin@123");
        throw new System.Exception($"admin: {h1}\nAdmin@123: {h2}");
    }
}