using Housing_Architecture.Domain.Entities;

namespace Housing_Architecture.BizLayer.Services.Interfaces;

public interface IJwtTokenService
{
    string GenerateJwtToken(User user);
}
