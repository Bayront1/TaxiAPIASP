using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TaxiAPI.Models;

public partial class TaxiContext : DbContext
{
    public TaxiContext()
    {
    }

    public TaxiContext(DbContextOptions<TaxiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Driverlocation> Driverlocations { get; set; }

    public virtual DbSet<Passengerrequest> Passengerrequests { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Ride> Rides { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=localhost;port=3306;user=root;password=12345;database=Taxi");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Driverlocation>(entity =>
        {
            entity.HasKey(e => e.DriverId).HasName("PRIMARY");

            entity.ToTable("driverlocation");

            entity.Property(e => e.DriverId).HasColumnName("driverID");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnName("longitude");

            // Добавляем свойство для статуса
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(50).IsRequired().HasDefaultValue("Свободен");
        });

        modelBuilder.Entity<Passengerrequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PRIMARY");

            entity.ToTable("passengerrequests");

            entity.HasIndex(e => e.PassengerId, "passengerID");

            entity.Property(e => e.RequestId).HasColumnName("requestID");
            entity.Property(e => e.EndPointLat).HasColumnName("endPoint_lat");
            entity.Property(e => e.EndPointLng).HasColumnName("endPoint_lng");
            entity.Property(e => e.PassengerId).HasColumnName("passengerID");
            entity.Property(e => e.RequestTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("requestTime");
            entity.Property(e => e.StartPointLat).HasColumnName("startPoint_lat");
            entity.Property(e => e.StartPointLng).HasColumnName("startPoint_lng");

            // Добавляем свойство для статуса
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(50).IsRequired().HasDefaultValue("Поиск");
        });


        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PRIMARY");

            entity.ToTable("ratings");

            entity.HasIndex(e => e.UserId, "userID");

            entity.Property(e => e.RatingId).HasColumnName("ratingID");
            entity.Property(e => e.Rating1).HasColumnName("rating");
            entity.Property(e => e.RatingSize).HasColumnName("ratingSize");
            entity.Property(e => e.UserId).HasColumnName("userID");

        });

        modelBuilder.Entity<Ride>(entity =>
        {
            entity.HasKey(e => e.RideId).HasName("PRIMARY");

            entity.ToTable("rides");

            entity.HasIndex(e => e.DriverId, "driverID");

            entity.HasIndex(e => e.PassengerId, "passengerID");

            entity.Property(e => e.RideId).HasColumnName("rideID");
            entity.Property(e => e.DriverId).HasColumnName("driverID");
            entity.Property(e => e.EndPointLat).HasColumnName("endPoint_lat");
            entity.Property(e => e.EndPointLng).HasColumnName("endPoint_lng");
            entity.Property(e => e.PassengerId).HasColumnName("passengerID");
            entity.Property(e => e.StartPointLat).HasColumnName("startPoint_lat");
            entity.Property(e => e.StartPointLng).HasColumnName("startPoint_lng");

        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.UserId).HasColumnName("userID");
            entity.Property(e => e.Carname)
                .HasMaxLength(100)
                .HasColumnName("carname");
            entity.Property(e => e.Carnumber)
                .HasMaxLength(20)
                .HasColumnName("carnumber");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PassengerRideId).HasColumnName("passengerRideID");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UserType)
                .HasColumnType("enum('Driver','Passenger')")
                .HasColumnName("userType");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
