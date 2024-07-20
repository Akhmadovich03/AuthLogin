namespace AuthenticationLogin.Models;

public class User
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string? Position { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public DateTime? LastLoginTime { get; set; }

    public DateTime RegistrationTime { get; set; }

    public UserStatus Status { get; set; }
}
