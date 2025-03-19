using Microsoft.EntityFrameworkCore;
using ReelkixVision.Web.Domain.Entities;

namespace ReelkixVision.Web.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<RequestLog> RequestLogs { get; set; }
    }
}