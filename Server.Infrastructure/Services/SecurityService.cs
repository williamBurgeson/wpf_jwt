using Server.Infrastructure.Data;
using Server.Infrastructure.Entities;
using SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Infrastructure.Services
{
    public interface ISecurityService
    {
        UserEntity Authenticate(LoginModel loginModel);

        UserEntity GenerateUserEntity(LoginModel loginModel);

        UserEntity GetByUsername(string username);
    }

    public class SecurityService : ISecurityService
    {
        private SecurityDataContext _dataContext;

        public SecurityService(SecurityDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public UserEntity Authenticate(LoginModel loginModel)
        {
            if (string.IsNullOrWhiteSpace(loginModel.Username)) return null;
            if (string.IsNullOrWhiteSpace(loginModel.Password)) return null;

            var user = GetByUsername(loginModel.Username);

            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(loginModel.Password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public UserEntity GenerateUserEntity(LoginModel loginModel)
        {
            var seedValue = GenerateRandomSeedValue();

            var userEntity = new UserEntity
            {
                Username = loginModel.Username,
                LastUpdated = DateTime.UtcNow
            };

            using (var hmac = new System.Security.Cryptography.HMACSHA512(seedValue))
            {
                userEntity.PasswordSalt = hmac.Key;
                userEntity.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginModel.Password));
            }

            return userEntity;
        }

        public UserEntity GetByUsername(string username) => 
            _dataContext.Users.SingleOrDefault(x => x.Username.ToLower() == username.ToLower());

        private byte[] GenerateRandomSeedValue()
        {
            var random = new Random(DateTime.Now.Ticks.GetHashCode());

            var saltValue = new byte[128];

            random.NextBytes(saltValue);

            return saltValue;
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordSalt");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}
