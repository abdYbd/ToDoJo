using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using ToDoJo.Data; // подставь свой неймспейс

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDBContext>
{
    public AppDBContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDBContext>();

        // Для разработки безопаснее брать строку из переменной окружения
        // но тут простой пример — замени на свою строку подключения
        var conn = Environment.GetEnvironmentVariable("TODOLIST_DB")
                   ?? "server=127.0.0.1;user=root;password=00892204BEkKY!;database=todolist_db";

        // Если используешь Pomelo (рекомендуется для MySQL)
        optionsBuilder.UseMySql(conn, ServerVersion.AutoDetect(conn));

        return new AppDBContext(optionsBuilder.Options);
    }
}