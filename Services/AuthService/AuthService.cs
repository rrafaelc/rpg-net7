using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using rpg.Models;
using rpg.Repositories.UserRepository;

namespace rpg.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            var serviceResponse = new ServiceResponse<string>();

            try
            {
                var user = await _userRepository.FindUserByUsername(username.ToLower())
                ?? throw new Exception("Username or password incorrect");
                var isValidPassword = VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt);
                if (!isValidPassword)
                    throw new Exception("Username or password incorrect");
                serviceResponse.Data = CreateToken(user);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            var serviceResponse = new ServiceResponse<int>();
            if (await UserExists(user.Username.ToLower()))
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "User already exists";
                return serviceResponse;
            }
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Username = user.Username.ToLower();
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            var userCreated = await _userRepository.AddUser(user);
            serviceResponse.Data = userCreated.Id;
            return serviceResponse;
        }

        private async Task<bool> UserExists(string username)
        {
            return await _userRepository.UserExists(username);
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value
            ?? throw new Exception("AppSettings:Token is null");

            SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(appSettingsToken));

            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
