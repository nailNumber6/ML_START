﻿using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using ToolLibrary;


namespace Server.Models.TestDB;
public partial class TestContext : DbContext
{
    private string? _connectionString = Program.ConfigSettings.ConnectionStrings?["MSSQL Server"];
    public TestContext()
    {
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
            LoggingTool.LogByTemplate(Serilog.Events.LogEventLevel.Error, ex);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Login)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.AuthorizationDate)
                .HasColumnType("datetime")
                .HasColumnName("Authorization date");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
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
