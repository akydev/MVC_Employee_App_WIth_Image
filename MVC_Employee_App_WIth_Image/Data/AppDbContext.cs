using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVC_Employee_App_WIth_Image.Models;

namespace MVC_Employee_App_WIth_Image.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeDetails> EmployeeDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Recommended: configure from dependent entity
            modelBuilder.Entity<EmployeeDetails>()
                .HasOne(d => d.Employee)
                .WithOne(e => e.EmployeeDetails)
                .HasForeignKey<EmployeeDetails>(d => d.EmpId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}