using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using reservation_tracker.Models;

namespace reservation_tracker.Data;

public partial class ReservationTrackerContext : DbContext
{
    public ReservationTrackerContext()
    {
    }

    public ReservationTrackerContext(DbContextOptions<ReservationTrackerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Guest> Guests { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost,1433;Database=ReservationTracker;User Id=sa;Password=Root1234*;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Guest>(entity =>
        {
            entity.HasKey(e => e.GuestId).HasName("PK__Guests__0C423C12B201C9D8");

            entity.HasIndex(e => new { e.FirstName, e.LastName, e.PhoneNumber }, "UQ_Guests_FirstName_LastName_PhoneNumber").IsUnique();

            entity.Property(e => e.Address).IsUnicode(false);
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Company)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Notes).IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.State)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Zipcode)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PK__Reservat__B7EE5F245FF014C8");

            entity.Property(e => e.CardLastFour)
                .HasMaxLength(4)
                .IsUnicode(false);
            entity.Property(e => e.DateReserved).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Notes).IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Guest).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.GuestId)
                .HasConstraintName("FK_Guest");

            entity.HasOne(d => d.ModifiedByUser).WithMany(p => p.ReservationModifiedByUsers)
                .HasForeignKey(d => d.ModifiedByUserId)
                .HasConstraintName("FK_ModifiedByUser");

            entity.HasOne(d => d.Room).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Room");

            entity.HasOne(d => d.User).WithMany(p => p.ReservationUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_User");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Rooms__3286393979F89071");

            entity.HasIndex(e => e.RoomNumber, "UQ__Rooms__AE10E07A765A6A55").IsUnique();

            entity.Property(e => e.Notes).IsUnicode(false);
            entity.Property(e => e.RoomNumber)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.RoomType)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C5490002D");

            entity.HasIndex(e => e.GoogleId, "UQ__Users__A6FBF2FBA1141124").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534C9C1EE3B").IsUnique();

            entity.Property(e => e.DisplayName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.GoogleId)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Picture).IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
