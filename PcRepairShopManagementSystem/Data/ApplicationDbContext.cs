using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PcRepairShopManagementSystem.Models; 

namespace PcRepairShopManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Custom tables
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Component> PCComponents { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentDetail> AppointmentDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Manually configuring decimal precision for the price fields

            modelBuilder.Entity<Component>()
                .Property(c => c.Price)
                .HasColumnType("decimal(18,2)");

            
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            
            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.PricePerUnit)
                .HasColumnType("decimal(18,2)");

            // -----------------------------------------------------
            // ONE-TO-ONE RELATIONSHIP: CUSTOMER <-> IDENTITY USER
            // -----------------------------------------------------

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.ApplicationUser)
                .WithOne() // IdentityUser does not have a navigation property for Customer
                .HasForeignKey<Customer>(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // -----------------------------------------------------
            // ONE-TO-ONE RELATIONSHIP: STAFF <-> IDENTITY USER
            // -----------------------------------------------------
            
            modelBuilder.Entity<Staff>()
                .HasOne(s => s.ApplicationUser)
                .WithOne() // IdentityUser does not have a navigation property for Staff
                .HasForeignKey<Staff>(s => s.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // -----------------------------------------------------
            // APPOINTMENT RELATIONSHIPS
            // -----------------------------------------------------
            
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Customer)
                .WithMany() // One customer can have many appointments.
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Staff)
                .WithMany() // One staff can handle multiple appointments.
                .HasForeignKey(a => a.StaffId)
                .OnDelete(DeleteBehavior.Restrict);
            // Restrict deletion of a Staff record if appointments exist.

            // -----------------------------------------------------
            // ORDER RELATIONSHIPS
            // -----------------------------------------------------
            
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany() // One customer can place many orders.
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order has many OrderDetails (line items).
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // -----------------------------------------------------
            // ORDERDETAIL RELATIONSHIP
            // -----------------------------------------------------
            
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Component)
                .WithMany() // A component can appear in many order details.
                .HasForeignKey(od => od.ComponentId)
                .OnDelete(DeleteBehavior.Restrict);
            // Restrict deletion of components to preserve historical order records.

            // Configure one-to-one relationship between Appointment and AppointmentDetail.
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.AppointmentDetail)
                .WithOne(ad => ad.Appointment)
                .HasForeignKey<AppointmentDetail>(ad => ad.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
