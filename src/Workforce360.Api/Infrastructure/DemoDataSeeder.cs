using Microsoft.EntityFrameworkCore;
using Workforce360.Api.Data;
using Workforce360.Api.Domain;
namespace Workforce360.Api.Infrastructure;
public static class DemoDataSeeder {
 public static async Task SeedAsync(IServiceProvider services) {
  await using var scope=services.CreateAsyncScope();var db=scope.ServiceProvider.GetRequiredService<WorkforceDbContext>();await db.Database.EnsureCreatedAsync();if(await db.Departments.AnyAsync())return;
  var eng=new Department{Name="Engineering"};var hr=new Department{Name="Human Resources"};var fin=new Department{Name="Finance"};db.Departments.AddRange(eng,hr,fin);await db.SaveChangesAsync();
  db.Employees.AddRange(new Employee{EmployeeCode="WF-1001",FullName="Aarav Mehta",Email="aarav@example.test",DepartmentId=eng.Id,IsActive=true,DateJoined=new DateOnly(2023,4,10)},new Employee{EmployeeCode="WF-1002",FullName="Diya Shah",Email="diya@example.test",DepartmentId=eng.Id,IsActive=true,DateJoined=new DateOnly(2024,1,15)},new Employee{EmployeeCode="WF-1003",FullName="Kabir Patel",Email="kabir@example.test",DepartmentId=eng.Id,IsActive=false,DateJoined=new DateOnly(2022,8,1)},new Employee{EmployeeCode="WF-2001",FullName="Anaya Desai",Email="anaya@example.test",DepartmentId=hr.Id,IsActive=true,DateJoined=new DateOnly(2023,6,5)},new Employee{EmployeeCode="WF-3001",FullName="Vivaan Joshi",Email="vivaan@example.test",DepartmentId=fin.Id,IsActive=true,DateJoined=new DateOnly(2024,3,20)});await db.SaveChangesAsync();
  var employee=await db.Employees.FirstAsync();db.LeaveRequests.Add(new LeaveRequest{EmployeeId=employee.Id,StartDate=DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),EndDate=DateOnly.FromDateTime(DateTime.UtcNow.AddDays(12)),Reason="Personal leave"});await db.SaveChangesAsync();
 }
}
