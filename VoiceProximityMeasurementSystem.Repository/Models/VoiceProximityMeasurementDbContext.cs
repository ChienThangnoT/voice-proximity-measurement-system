using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VoiceProximityMeasurementSystem.Repository.Models;

public partial class VoiceProximityMeasurementDbContext : DbContext
{
    public VoiceProximityMeasurementDbContext()
    {
    }

    public VoiceProximityMeasurementDbContext(DbContextOptions<VoiceProximityMeasurementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<LandoltE> LandoltEs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAnswer> UserAnswers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-9COIMEED;uid=sa;pwd=12345;database=VoiceProximityMeasurementDB;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LandoltE>(entity =>
        {
            entity.ToTable("LandoltE");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Entity).HasMaxLength(155);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(250);
        });

        modelBuilder.Entity<UserAnswer>(entity =>
        {
            entity.ToTable("UserAnswer");

            entity.Property(e => e.UserAnswerId).ValueGeneratedNever();

            entity.HasOne(d => d.Landolt).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.LandoltId)
                .HasConstraintName("FK_UserAnswer_LandoltE");

            entity.HasOne(d => d.User).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserAnswer_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
