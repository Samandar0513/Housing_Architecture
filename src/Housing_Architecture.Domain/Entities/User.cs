using Housing_Architecture.Domain.Enums;

namespace Housing_Architecture.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public string Role { get; set; } = UserRole.User.ToString();
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public bool IsVerified { get; set; } = false;

    // Navigation
    public ICollection<Property> Properties { get; set; } = new List<Property>();
    public ICollection<UserOTPs> OtpCodes { get; set; } = new List<UserOTPs>();
}
