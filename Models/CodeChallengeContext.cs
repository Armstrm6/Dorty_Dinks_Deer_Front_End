using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DinksCodeChallenge.Models
{
    public partial class CodeChallengeContext : DbContext
    {
        public CodeChallengeContext()
        {
        }

        public CodeChallengeContext(DbContextOptions<CodeChallengeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DinkData> DinkData { get; set; }
        public virtual DbSet<DinkPictures> DinkPictures { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source = mc-database.database.windows.net;Initial Catalog=CodeChallenge;Persist Security Info=False;User ID=MorelandConnect;Password=C0nn3ct234284;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DinkData>(entity =>
            {
                entity.HasKey(e => e.DeerId)
                    .HasName("PK__Dink_Dat__877992981F03F454");

                entity.ToTable("Dink_Data");

                entity.Property(e => e.DeerId).HasColumnName("DeerID");

                entity.Property(e => e.DeerDate).HasColumnType("date");
            });

            modelBuilder.Entity<DinkPictures>(entity =>
            {
                entity.HasKey(e => e.PictureId)
                    .HasName("PK__Dink_Pic__8C2866F822CF4CFE");

                entity.ToTable("Dink_Pictures");

                entity.Property(e => e.PictureId).HasColumnName("PictureID");

                entity.Property(e => e.DeerId).HasColumnName("DeerID");

                entity.Property(e => e.Picture).IsRequired();

                entity.HasOne(d => d.Deer)
                    .WithMany(p => p.DinkPictures)
                    .HasForeignKey(d => d.DeerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Dink_Pict__DeerI__66603565");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
