using Final_Project.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Final_Project.Data
{
  public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<TaskModel> Task { get; set; }
    public DbSet<Group> Group { get; set; }
    public DbSet<GroupMemberships> GroupMemberships { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
      // Customize the ASP.NET Identity model and override the defaults if needed.
      // For example, you can rename the ASP.NET Identity table names and more.
      // Add your customizations after calling base.OnModelCreating(builder);

      builder.Entity<User>().Property(u => u.Id).UseIdentityColumn();
    }
  }
}
