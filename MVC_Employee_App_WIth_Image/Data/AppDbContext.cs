using Microsoft.EntityFrameworkCore;
using MVC_Employee_App_WIth_Image.Models;

namespace MVC_Employee_App_WIth_Image.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeDetails> EmployeeDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // One-to-One Relationship
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.EmployeeDetails)
                .WithOne(d => d.Employee)
                .HasForeignKey<EmployeeDetails>(d => d.EmpId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
