using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WebUI.Data.Entities;


namespace WebUI.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<DiagnosticCard> DiagnosticCards { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<DiagnosticCard>().HasOne(c => c.User)
                .WithMany(u => u.DiagnosticCards)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<DiagnosticCard>().HasIndex(e => e.VIN);
            builder.Entity<DiagnosticCard>().HasIndex(e => e.RegNumber);
            builder.Entity<DiagnosticCard>().HasIndex(e => e.DocumentSeries);
            builder.Entity<DiagnosticCard>().HasIndex(e => e.DocumentNumber);
            builder.Entity<DiagnosticCard>().HasIndex(e => e.BodyNumber);
            builder.Entity<DiagnosticCard>().HasIndex(e => e.FrameNumber);
        }

        public DbSet<WebUI.Data.Entities.User> User { get; set; }


    }
}
