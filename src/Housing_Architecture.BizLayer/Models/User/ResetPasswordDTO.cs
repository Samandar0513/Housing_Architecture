namespace Housing_Architecture.BizLayer.Models.User;

public class ResetPasswordDTO
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
