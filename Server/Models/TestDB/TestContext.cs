using Microsoft.EntityFrameworkCore;
using ToolLibrary;
using System.Linq;
using System.Threading.Tasks;


namespace Server.Models.TestDB;
public partial class TestContext : DbContext
{
    public TestContext()
    {
    }

    public TestContext(DbContextOptions<TestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Test;Trusted_Connection=True;TrustServerCertificate=True");

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
