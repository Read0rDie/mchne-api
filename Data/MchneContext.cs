using mchne_api.Models;
using Microsoft.EntityFrameworkCore;

namespace mchne_api.Data
{
    public class MchneContext : DbContext
    {
        public MchneContext(DbContextOptions<MchneContext> options) : base(options)
        {
        }

        public DbSet<Novel> Novels { get; set; }
        public DbSet<Chapter> Chapters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Novel>().ToTable("Novel");
            modelBuilder.Entity<Chapter>().ToTable("Chapter");            
        }        
    }
}