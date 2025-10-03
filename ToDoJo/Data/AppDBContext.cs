using ToDoJo.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ToDoJo.Data
{
    public class AppDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<TodoTask> Tasks { get; set; }

        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseMySql(
        //            "server=127.0.0.1;user=root;password=00892204BEkKY!;database=todolist_db;",
        //            new MySqlServerVersion(new Version(9, 4, 0)) // ← Здесь версия 9.4.0
        //        );
        //    }
        //}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Рекомендуется взять connection string из env var
                var cs = Environment.GetEnvironmentVariable("TODOLIST_DB")
                         ?? "server=127.0.0.1;user=root;password=00892204BEkKY!;database=todolist_db;";
                optionsBuilder.UseMySql(cs, ServerVersion.AutoDetect(cs));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TodoTask>()
                .HasOne(t =>  t.User)
                .WithMany(u  => u.Tasks)
                .HasForeignKey(t  => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
