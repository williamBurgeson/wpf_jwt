using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class SecurityDataContext : DbContext
    {
        public SecurityDataContext(DbContextOptions<SecurityDataContext> options) : base(options) { }

        public DbSet<UserEntity> Users { get; set; }
    }
}
