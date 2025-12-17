namespace Housing_Architecture.Domain.Entities;

public class TempUser
{
    public int Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string OtpCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public string? TelegramUserId { get; set; }
}
