using KoiCareSys.Data.DTO;

namespace KoiCareSys.Service.Service.Interface
{
    public interface ITokenService
    {
        string GenerateToken(LoginDTO dto);
    }
}
