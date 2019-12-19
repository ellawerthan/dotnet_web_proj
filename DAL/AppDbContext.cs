using System;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AppDbContext: DbContext
    {
        public DbSet<Calculation> Calculations { get; set; } = default!;
        public DbSet<GameState> GameStates { get; set; } = default!;

        public AppDbContext()
        {
            
        }

        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=/Users/akaver/Development/temp/app.db");
        }
*/
        public AppDbContext(DbContextOptions options): base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        
    }
}