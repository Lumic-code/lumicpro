using LumicPro.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LumicPro.Infrastructure.Context
{
    public class LumicProContext : DbContext
    {
        public LumicProContext(DbContextOptions<LumicProContext> options) : base(options)
        {
        }
        public DbSet<AppUser> AppUsers { get; set; }
    }
}