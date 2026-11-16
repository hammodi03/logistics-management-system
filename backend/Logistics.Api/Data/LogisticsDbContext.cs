using System;
using System.Collections.Generic;
using Logistics.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Api.Data;

public partial class LogisticsDbContext : DbContext
{
    public LogisticsDbContext(DbContextOptions<LogisticsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Delivery> Deliveries { get; set; }

    public virtual DbSet<DeliveryEvent> DeliveryEvents { get; set; }

    public virtual DbSet<Driver> Drivers { get; set; }

    public virtual DbSet<Package> Packages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("customers_pkey");

            entity.ToTable("customers");

            entity.HasIndex(e => e.Email, "customers_email_key").IsUnique();

            entity.Property(e => e.CustomerId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("customer_id");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.HasKey(e => e.DeliveryId).HasName("deliveries_pkey");

            entity.ToTable("deliveries");

            entity.Property(e => e.DeliveryId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("delivery_id");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("assigned_at");
            entity.Property(e => e.DeliveryStatus)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Assigned'::character varying")
                .HasColumnName("delivery_status");
            entity.Property(e => e.DriverId).HasColumnName("driver_id");
            entity.Property(e => e.PackageId).HasColumnName("package_id");

            entity.HasOne(d => d.Driver).WithMany(p => p.Deliveries)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_delivery_driver");

            entity.HasOne(d => d.Package).WithMany(p => p.Deliveries)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_delivery_package");
        });

        modelBuilder.Entity<DeliveryEvent>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("delivery_events_pkey");

            entity.ToTable("delivery_events");

            entity.Property(e => e.EventId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("event_id");
            entity.Property(e => e.DeliveryId).HasColumnName("delivery_id");
            entity.Property(e => e.EventTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("event_time");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");

            entity.HasOne(d => d.Delivery).WithMany(p => p.DeliveryEvents)
                .HasForeignKey(d => d.DeliveryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_event_delivery");
        });

        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.DriverId).HasName("drivers_pkey");

            entity.ToTable("drivers");

            entity.HasIndex(e => e.Phone, "drivers_phone_key").IsUnique();

            entity.Property(e => e.DriverId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("driver_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("packages_pkey");

            entity.ToTable("packages");

            entity.HasIndex(e => e.Destination, "idx_packages_destination");

            entity.Property(e => e.PackageId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("package_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Destination)
                .HasMaxLength(100)
                .HasColumnName("destination");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");

            entity.HasOne(d => d.Customer).WithMany(p => p.Packages)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_packages_customer");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
