using Microsoft.EntityFrameworkCore;
using Workforce360.Api.Domain;
namespace Workforce360.Api.Data;
public sealed class WorkforceDbContext(DbContextOptions<WorkforceDbContext> options):DbContext(options) {
 public DbSet<Employee> Employees=>Set<Employee>(); public DbSet<Department> Departments=>Set<Department>(); public DbSet<LeaveRequest> LeaveRequests=>Set<LeaveRequest>();
 protected override void OnModelCreating(ModelBuilder b) {
  b.Entity<Department>().HasIndex(x=>x.Name).IsUnique(); b.Entity<Employee>().HasIndex(x=>x.EmployeeCode).IsUnique(); b.Entity<Employee>().HasIndex(x=>new{x.DepartmentId,x.IsActive});
  b.Entity<Employee>().HasOne(x=>x.Department).WithMany(x=>x.Employees).HasForeignKey(x=>x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
  b.Entity<LeaveRequest>().HasOne(x=>x.Employee).WithMany().HasForeignKey(x=>x.EmployeeId).OnDelete(DeleteBehavior.Restrict); b.Entity<LeaveRequest>().HasIndex(x=>new{x.Status,x.StartDate});
 }
}
