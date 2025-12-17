using Housing_Architecture.BizLayer.Services.Interfaces;
using Housing_Architecture.DataAccess.Persistence;
using Housing_Architecture.Domain.Entities;

namespace Housing_Architecture.BizLayer.Services.Implementations;

public class OtpService : IOtpService
{
    private readonly AppDbContext _context;

    public OtpService(AppDbContext context)
    {
        _context = context;
    }

    public string GenerateAndSaveOtp(string userEmail)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
        if (user == null)
            throw new InvalidOperationException("Foydalanuvchi topilmadi.");

        var otpCode = new Random().Next(100000, 999999).ToString();

        var otp = new UserOTPs
        {
            UserId = user.Id,
            Code = otpCode,
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddMinutes(5)
        };

        _context.UserOTPs.Add(otp);
        _context.SaveChanges();

        return otpCode;
    }

    public UserOTPs? GetLatestOtp(int userId, string code)
    {
        return _context.UserOTPs
            .Where(o => o.UserId == userId && o.Code == code && o.ExpiredAt > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefault();
    }
}
