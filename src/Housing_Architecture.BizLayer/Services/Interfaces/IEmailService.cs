namespace Housing_Architecture.BizLayer.Services.Interfaces;

public interface IEmailService
{
    Task<bool> SendOtpAsync(string toEmail, string otp);
}
