using Desafio.Leve.Domain.Models;
using Desafio.Leve.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Leve.Infrastructure
{
  public class AppDbContext : IdentityDbContext<Desafio.Leve.Infrastructure.Identity.ApplicationUser>
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<TaskItem> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      // Domain `TaskItem` holds user IDs only; any additional configuration can be added here.
    }
  }
}
