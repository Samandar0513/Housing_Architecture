using FluentValidation;
using Housing_Architecture.BizLayer.Common;
using Housing_Architecture.BizLayer.Models;
using Housing_Architecture.BizLayer.Models.User;
using Housing_Architecture.BizLayer.Services.Interfaces;
using Housing_Architecture.DataAccess.Persistence;
using Housing_Architecture.Domain.Entities;
using Housing_Architecture.Domain.Enums;

namespace Housing_Architecture.BizLayer.Services.Implementations;

public class UserService : IUserService
{
    private readonly AppDbContext _db;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly Helper _helper;
    private readonly IOtpService _otpService;
    private readonly IEmailService _emailService;
    private readonly IValidator<UserRegistrDTO> _validator;
    private readonly IValidator<UserUpdateDTO> _updateValidator;
    private readonly IValidator<ChangePasswordDTO> _changePasswordValidator;

    public UserService(
        AppDbContext db,
        IJwtTokenService jwtTokenService,
        Helper helper,
        IValidator<UserRegistrDTO> validator,
        IEmailService emailService,
        IOtpService otpService,
        IValidator<UserUpdateDTO> updateValidator,
        IValidator<ChangePasswordDTO> changePasswordValidator)
    {
        _db = db;
        _jwtTokenService = jwtTokenService;
        _helper = helper;
        _validator = validator;
        _emailService = emailService;
        _otpService = otpService;
        _updateValidator = updateValidator;
        _changePasswordValidator = changePasswordValidator;
    }

    public ResponseModel<UserAuthResponeDTO> Register(UserRegistrDTO registrDto)
    {
        var validationResult = _validator.Validate(registrDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ResponseModel<UserAuthResponeDTO>.Fail("Validation xatoligi!", errors);
        }

        if (_db.Users.Any(u => u.Email == registrDto.Email))
        {
            return ResponseModel<UserAuthResponeDTO>.Fail("Registratsiya xatoligi", "Bu email oldin ro'yxatdan o'tgan!");
        }

        var salt = Guid.NewGuid().ToString();
        var hashPassword = _helper.Encript(registrDto.Password, salt);

        var newUser = new User
        {
            Name = registrDto.Name,
            Email = registrDto.Email,
            Phone = registrDto.Phone,
            Password = hashPassword,
            Salt = salt,
            Role = "User"
        };

        _db.Users.Add(newUser);
        _db.SaveChanges();

        var otp = _otpService.GenerateAndSaveOtp(newUser.Email);
        var _ = _emailService.SendOtpAsync(newUser.Email, otp);

        var token = _jwtTokenService.GenerateJwtToken(newUser);
        var userAuthResponse = new UserAuthResponeDTO
        {
            Name = newUser.Name,
            Email = newUser.Email,
            Phone = newUser.Phone,
            Role = newUser.Role,
            Token = token
        };

        return ResponseModel<UserAuthResponeDTO>.Ok(userAuthResponse, "Ro'yxatdan o'tish muvaffaqiyatli amalga oshirildi. Emailni tasdiqlang!");
    }

    public ResponseModel<UserAuthResponeDTO> RegisterByTelegram(UserRegistrDTO registrDto, string otpCode)
    {
        var validationResult = _validator.Validate(registrDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ResponseModel<UserAuthResponeDTO>.Fail("Validation xatoligi!", errors);
        }

        if (_db.Users.Any(u => u.Email == registrDto.Email))
        {
            return ResponseModel<UserAuthResponeDTO>.Fail("Registratsiya xatoligi", "Bu email oldin ro'yxatdan o'tgan!");
        }

        var tempUser = _db.TempUsers.FirstOrDefault(x => x.PhoneNumber == registrDto.Phone);
        if (tempUser == null)
        {
            return ResponseModel<UserAuthResponeDTO>.Fail("Registratsiya xatoligi", "Telefon raqam uchun vaqtincha foydalanuvchi topilmadi!");
        }

        if (tempUser.OtpCode != otpCode || tempUser.ExpiresAt < DateTime.UtcNow)
        {
            return ResponseModel<UserAuthResponeDTO>.Fail("Registratsiya xatoligi", "OTP kod noto'g'ri yoki muddati tugagan!");
        }

        var salt = Guid.NewGuid().ToString();
        var hashPassword = _helper.Encript(registrDto.Password, salt);

        var newUser = new User
        {
            Name = registrDto.Name,
            Email = registrDto.Email,
            Phone = registrDto.Phone,
            Password = hashPassword,
            Salt = salt,
            Role = "User",
            IsVerified = true
        };

        _db.Users.Add(newUser);
        _db.TempUsers.Remove(tempUser);
        _db.SaveChanges();

        var token = _jwtTokenService.GenerateJwtToken(newUser);
        var userAuthResponse = new UserAuthResponeDTO
        {
            Name = newUser.Name,
            Email = newUser.Email,
            Phone = newUser.Phone,
            Role = newUser.Role,
            Token = token
        };

        return ResponseModel<UserAuthResponeDTO>.Ok(userAuthResponse, "Ro'yxatdan o'tish muvaffaqiyatli amalga oshirildi.");
    }

    public ResponseModel<UserAuthResponeDTO> Login(UserLoginDTO loginDto)
    {
        var user = _db.Users.FirstOrDefault(x => x.Email == loginDto.Email);
        if (user == null || !_helper.Verify(user.Password, loginDto.Password, user.Salt))
        {
            return ResponseModel<UserAuthResponeDTO>.Fail("Login xatoligi", "Email yoki parol noto'g'ri kiritildi!");
        }

        if (!user.IsVerified)
        {
            return ResponseModel<UserAuthResponeDTO>.Fail("Login xatoligi", "Iltimos, avval emailingizni tasdiqlang!");
        }

        var token = _jwtTokenService.GenerateJwtToken(user);
        var userAuthResponse = new UserAuthResponeDTO
        {
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            Token = token
        };

        return ResponseModel<UserAuthResponeDTO>.Ok(userAuthResponse, "Tizimga muvaffaqiyatli kirdingiz.");
    }

    public ResponseModel<UserDTO> GetUserById(int userId)
    {
        var user = _db.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return ResponseModel<UserDTO>.Fail("Xatolik", "Foydalanuvchi topilmadi!");
        }

        var userDto = new UserDTO
        {
            Id = user.Id,
            Name = user.Name,
            Phone = user.Phone,
            Email = user.Email,
            Role = user.Role,
            RegisteredAt = user.RegisteredAt,
            IsVerified = user.IsVerified
        };

        return ResponseModel<UserDTO>.Ok(userDto, "Foydalanuvchi ma'lumotlari muvaffaqiyatli olindi.");
    }

    public ResponseModel<UserDTO> UpdateUser(int userId, UserUpdateDTO updateDto)
    {
        var user = _db.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return ResponseModel<UserDTO>.Fail("Update xatoligi", "Foydalanuvchi topilmadi!");
        }

        var validationResult = _updateValidator.Validate(updateDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ResponseModel<UserDTO>.Fail("Validation xatoligi!", errors);
        }

        user.Name = updateDto.Name;
        user.Phone = updateDto.Phone;
        user.Email = updateDto.Email;
        

        _db.Update(user);
        _db.SaveChanges();

        var userDto = new UserDTO
        {
            Id = user.Id,
            Name = user.Name,
            Phone = user.Phone,
            Email = user.Email,
            Role = user.Role,
            RegisteredAt = user.RegisteredAt,
            IsVerified = user.IsVerified
        };

        return ResponseModel<UserDTO>.Ok(userDto, "Foydalanuvchi ma'lumotlari muvaffaqiyatli yangilandi.");
    }

    public ResponseModel<bool> DeleteUser(int userId)
    {
        var user = _db.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return ResponseModel<bool>.Fail("Delete xatoligi", "Foydalanuvchi topilmadi!");
        }

        _db.Users.Remove(user);
        _db.SaveChanges();

        return ResponseModel<bool>.Ok(true, "Foydalanuvchi muvaffaqiyatli o'chirildi.");
    }

    public ResponseModel<IEnumerable<UserDTO>> GetAllUsers()
    {
        var users = _db.Users.ToList();
        if (users.Count == 0)
        {
            return ResponseModel<IEnumerable<UserDTO>>.Fail("Xatolik", "Hech qanday foydalanuvchi topilmadi!");
        }

        var userDtos = users.Select(user => new UserDTO
        {
            Id = user.Id,
            Name = user.Name,
            Phone = user.Phone,
            Email = user.Email,
            Role = user.Role,
            RegisteredAt = user.RegisteredAt,
            IsVerified = user.IsVerified
        });

        return ResponseModel<IEnumerable<UserDTO>>.Ok(userDtos, "Barcha foydalanuvchilar muvaffaqiyatli olindi.");
    }

    public ResponseModel<bool> ChangePassword(int userId, ChangePasswordDTO changePasswordDto)
    {
        var user = _db.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return ResponseModel<bool>.Fail("ChangePassword xatoligi", "Foydalanuvchi topilmadi!");
        }

        if (!_helper.Verify(user.Password, changePasswordDto.OldPassword, user.Salt))
        {
            return ResponseModel<bool>.Fail("ChangePassword xatoligi", "Eski parol noto'g'ri kiritildi!");
        }

        var validationResult = _changePasswordValidator.Validate(changePasswordDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return ResponseModel<bool>.Fail("Validation xatoligi!", errors);
        }

        var newHashedPassword = _helper.Encript(changePasswordDto.NewPassword, user.Salt);
        user.Password = newHashedPassword;

        _db.Update(user);
        _db.SaveChanges();

        return ResponseModel<bool>.Ok(true, "Parol muvaffaqiyatli o'zgartirildi.");
    }

    public ResponseModel<string> ResetPassword(ResetPasswordDTO resetPasswordDto)
    {
        var user = _db.Users.FirstOrDefault(u => u.Email == resetPasswordDto.Email);
        if (user == null)
        {
            return ResponseModel<string>.Fail("ResetPassword xatoligi", "Foydalanuvchi topilmadi!");
        }

        var otp = _otpService.GetLatestOtp(user.Id, resetPasswordDto.Code);
        if (otp == null || otp.ExpiredAt < DateTime.UtcNow)
        {
            return ResponseModel<string>.Fail("ResetPassword xatoligi", "Kod noto'g'ri yoki muddati tugagan!");
        }

        var newHashedPassword = _helper.Encript(resetPasswordDto.NewPassword, user.Salt);
        user.Password = newHashedPassword;

        _db.Update(user);
        _db.SaveChanges();

        return ResponseModel<string>.Ok("Parol muvaffaqiyatli tiklandi.", "Parol muvaffaqiyatli tiklandi.");
    }


    public ResponseModel<string> VerifyOtp(OtpVerificationModel model)
    {
        var user = _db.Users.FirstOrDefault(u => u.Email == model.Email);
        if (user == null)
        {
            return ResponseModel<string>.Fail("Verifikatsiya xatosi!", "Foydalanuvchi topilmadi.");
        }

        var otp = _otpService.GetLatestOtp(user.Id, model.Code);
        if (otp == null || otp.ExpiredAt < DateTime.UtcNow)
        {
            return ResponseModel<string>.Fail("Verifikatsiya xatosi!", "Kod noto'g'ri yoki muddati tugagan.");
        }

        user.IsVerified = true;
        _db.SaveChanges();

        return ResponseModel<string>.Ok("Verifikatsiya muvaffaqiyatli!", "OTP muvaffaqiyatli tasdiqlandi.");
    }

    public ResponseModel<bool> ChangeUserRole(int userId, UserRole newRole)
    {
        var user = _db.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return ResponseModel<bool>.Fail("ChangeUserRole xatoligi", "Foydalanuvchi topilmadi!");
        }

        user.Role = newRole.ToString();
        _db.Update(user);
        _db.SaveChanges();

        return ResponseModel<bool>.Ok(true, "Foydalanuvchi roli muvaffaqiyatli o'zgartirildi.");
    }
}
