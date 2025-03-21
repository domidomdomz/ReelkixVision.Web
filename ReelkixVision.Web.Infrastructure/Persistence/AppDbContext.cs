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
        public DbSet<ShoeAnalysisResult> ShoeAnalysisResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure one-to-one relationship
            modelBuilder.Entity<ShoeAnalysisResult>()
                .HasOne(s => s.RequestLog)
                .WithOne(r => r.ShoeAnalysisResult)
                .HasForeignKey<ShoeAnalysisResult>(s => s.RequestLogId);
        }
    }
}