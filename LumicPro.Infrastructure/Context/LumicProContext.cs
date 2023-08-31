using LumicPro.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LumicPro.Infrastructure.Context
{
    public class LumicProContext : IdentityDbContext<AppUser>
    {
        public LumicProContext(DbContextOptions<LumicProContext> options) : base(options)
        {
        }
        
    }
}