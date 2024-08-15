using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Post_Office_Management.Models;
using System.Reflection.Emit;

namespace Post_Office_Management.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Delivery> Deliverables { get; set; }
        public DbSet<ChargeDetail> Charges { get; set; }
        public DbSet<ServiceType> Services { get; set; }
        public DbSet<Office> Locations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new DeliveryConfiguration());

            builder.Entity<Delivery>()
                .HasOne(d => d.ServiceType)
                .WithMany(st => st.Deliveries)
                .HasForeignKey(d => d.ServiceTypeId);

            builder.Entity<Delivery>()
                .HasOne(d => d.FromOffice)
                .WithMany(o => o.FromDeliveries)
                .HasForeignKey(d => d.FromOfficeId);

            builder.Entity<Delivery>()
                .HasOne(d => d.ToOffice)
                .WithMany(o => o.ToDeliveries)
                .HasForeignKey(d => d.ToOfficeId);

            builder.Entity<ChargeDetail>()
                .Property(e => e.Charge)
                .HasColumnType("decimal(18,2)");

            builder.Entity<ChargeDetail>()
                .HasOne(cd => cd.ServiceType)
                .WithMany(st => st.ChargeDetails)
                .HasForeignKey(cd => cd.ServiceTypeId);

            builder.Entity<ServiceType>()
                .Property(e => e.BaseCharge)
                .HasColumnType("decimal(18,2)");


            builder.Entity<Delivery>()
                .Property(e => e.Weight)
                .HasColumnType("decimal(18,2)");

            var admin = new IdentityRole("admin");
            admin.NormalizedName = "admin";

            var employee = new IdentityRole("employee");
            employee.NormalizedName = "employee";

            var user = new IdentityRole("user");
            user.NormalizedName = "user";

            builder.Entity<IdentityRole>().HasData(admin, employee, user);
        }
    }
}
