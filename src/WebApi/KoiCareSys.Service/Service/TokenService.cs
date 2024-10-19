using KoiCareSys.Data;
using KoiCareSys.Data.DTO;
using KoiCareSys.Service.Service.Interface;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KoiCareSys.Service.Service
{
    public class TokenService : ITokenService
    {
        private readonly string _secretKey;
        private readonly int _expirationMinutes;
        private readonly UnitOfWork _unitOfWork;

        public TokenService(string secretKey, int expirationMinutes, UnitOfWork unitOfWork)
        {
            _expirationMinutes = expirationMinutes;
            _unitOfWork = unitOfWork;
            _secretKey = secretKey;
        }

        public string GenerateToken(LoginDTO dto)
        {
            var user = _unitOfWork.User.GetByEmailAsync(dto.Email);
            if (user.Result == null) return "User not found.";

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Result.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(_expirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
