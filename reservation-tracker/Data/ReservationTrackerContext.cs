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
            entity.HasKey(e => e.GuestId).HasName("PK__Guests__0C423C12D88BF9A5");

            entity.Property(e => e.State).IsFixedLength();
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PK__Reservat__B7EE5F243DD5C896");

            entity.Property(e => e.DateReserved).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Guest).WithMany(p => p.Reservations)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Guest");

            entity.HasOne(d => d.Room).WithMany(p => p.Reservations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Room");

            entity.HasOne(d => d.User).WithMany(p => p.Reservations)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_User");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Rooms__328639390B8E7939");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C1662C344");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
