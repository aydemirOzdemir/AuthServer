using AuthServer.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data.Context
{
    public class AuthServerDbContext:IdentityDbContext<UserApp,IdentityRole,string>
    {
        public AuthServerDbContext(DbContextOptions<AuthServerDbContext> options):base(options) 
        {
            
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<UserRefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            base.OnModelCreating(builder);
        }

    }
}
