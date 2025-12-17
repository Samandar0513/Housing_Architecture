using Housing_Architecture.BizLayer.Models;
using Housing_Architecture.BizLayer.Models.User;
using Housing_Architecture.Domain.Enums;

namespace Housing_Architecture.BizLayer.Services.Interfaces;

public interface IUserService
{
    ResponseModel<UserAuthResponeDTO> Register(UserRegistrDTO registrDto);
    ResponseModel<UserAuthResponeDTO> RegisterByTelegram(UserRegistrDTO registrDto, string otpCode);
    ResponseModel<UserAuthResponeDTO> Login(UserLoginDTO loginDto);
    ResponseModel<UserDTO> GetUserById(int userId);
    ResponseModel<UserDTO> UpdateUser(int userId, UserUpdateDTO updateDto);
    ResponseModel<bool> DeleteUser(int userId);
    ResponseModel<IEnumerable<UserDTO>> GetAllUsers();
    ResponseModel<bool> ChangePassword(int userId, ChangePasswordDTO changePasswordDto);
    ResponseModel<string> ResetPassword(ResetPasswordDTO resetPasswordDto);
    ResponseModel<string> VerifyOtp(OtpVerificationModel model);
    ResponseModel<bool> ChangeUserRole(int userId, UserRole newRole);
}
