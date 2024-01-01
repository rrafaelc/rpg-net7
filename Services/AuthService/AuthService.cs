using rpg.Models;
using rpg.Repositories.AuthRepository;

namespace rpg.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            var serviceResponse = new ServiceResponse<string>();

            try
            {
                var user = await _authRepository.FindUserByUsername(username.ToLower());
                if (user is null)
                    throw new Exception("Username or password incorrect");
                var isValidPassword = VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt);
                if (!isValidPassword)
                    throw new Exception("Username or password incorrect");
                serviceResponse.Data = user.Id.ToString();
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

            var userCreated = await _authRepository.AddUser(user);
            serviceResponse.Data = userCreated.Id;
            return serviceResponse;
        }

        private async Task<bool> UserExists(string username)
        {
            return await _authRepository.UserExists(username);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
