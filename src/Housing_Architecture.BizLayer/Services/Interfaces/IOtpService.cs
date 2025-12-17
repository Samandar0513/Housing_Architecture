using Housing_Architecture.Domain.Entities;

namespace Housing_Architecture.BizLayer.Services.Interfaces;

public interface IOtpService
{
    string GenerateAndSaveOtp(string userEmail);
    UserOTPs? GetLatestOtp(int userId, string code);
}
