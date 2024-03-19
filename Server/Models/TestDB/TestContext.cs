using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Serilog;

using ToolLibrary;


namespace Server.Models.TestDB;
public partial class TestContext : DbContext
{
    private readonly string? _connectionString;

    public string DatabasePath { get; }
    public TestContext()
    {
        try
        {
            _connectionString = Program.Configuration.GetConnectionString("MSSQL Server")!;
        }
        catch
        {
            Log.Error("Произошла ошибка при чтении строки подключения из файла конфигурации");
        }
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DatabasePath = System.IO.Path.Join(path, "Test.db");
    }

    public TestContext(DbContextOptions<TestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        try
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
        catch (Exception ex)
        {
            Log.Error("При настройке конфигурации контекста данных было вызвано исключение {exType} : {exMessage}"
                , ex.GetType(), ex.Message);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Login)
                .HasMaxLength(20)
                .IsUnicode(true);
            entity.Property(e => e.AuthorizationDate)
                .HasColumnType("datetime")
                .HasColumnName("Authorization date");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(true);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    internal bool UserExists(string login, string password)
    {
        return Users.Any(user =>
        user.Login == login &&
        user.Password == Md5HashingTool.GetHash(password));
    }

    internal async Task<bool> UserExistsAsync(string login, string password)
    {
        return await Users.AnyAsync(user =>
        user.Login == login &&
        user.Password == Md5HashingTool.GetHash(password));
    }
}
