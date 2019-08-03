using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Entities
{
    public class UserEntity
    {
        public string Username { get; internal set; }
        public byte[] PasswordHash { get; internal set; }
        public byte[] PasswordSalt { get; internal set; }
    }
}
