using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EJ2APIServices.Models
{
    public partial class EEJ2SERVICEEJ2WEBSERVICESSRCAPP_DATADIAGRAMMDFContext : DbContext
    {
        

        public EEJ2SERVICEEJ2WEBSERVICESSRCAPP_DATADIAGRAMMDFContext(DbContextOptions<EEJ2SERVICEEJ2WEBSERVICESSRCAPP_DATADIAGRAMMDFContext> options)
            : base(options)
        {
        }
         

        public virtual DbSet<DiagramData> DiagramData { get; set; }
         
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiagramData>(entity =>
            {
                entity.Property(e => e.DiagramName).HasMaxLength(50);
            });
        }
    }
}
