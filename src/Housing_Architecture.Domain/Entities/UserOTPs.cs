namespace Housing_Architecture.Domain.Entities;

public class UserOTPs
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiredAt { get; set; }

    // Navigation
    public User User { get; set; } = null!;
}
