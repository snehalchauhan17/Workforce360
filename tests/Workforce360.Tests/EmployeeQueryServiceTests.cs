using Microsoft.EntityFrameworkCore;
using Workforce360.Api.Data;
using Workforce360.Api.Domain;
using Workforce360.Api.Infrastructure;
using Xunit;
namespace Workforce360.Tests;
public sealed class EmployeeQueryServiceTests {
 [Fact]public async Task SearchAsync_FiltersAndProjectsDepartment(){var options=new DbContextOptionsBuilder<WorkforceDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;await using var db=new WorkforceDbContext(options);var d=new Department{Name="Engineering"};db.Departments.Add(d);db.Employees.Add(new Employee{EmployeeCode="WF-1",FullName="Aarav Mehta",Email="aarav@example.test",Department=d,IsActive=true,DateJoined=new DateOnly(2024,1,1)});await db.SaveChangesAsync();var result=await new EmployeeQueryService(db).SearchAsync("Aarav",CancellationToken.None);Assert.Single(result);Assert.Equal("Engineering",result[0].Department);}
 [Fact]public async Task DepartmentSummary_CountsActiveEmployees(){var options=new DbContextOptionsBuilder<WorkforceDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;await using var db=new WorkforceDbContext(options);var d=new Department{Name="Engineering"};db.Departments.Add(d);db.Employees.AddRange(new Employee{EmployeeCode="WF-1",FullName="One",Email="one@example.test",Department=d,IsActive=true,DateJoined=new DateOnly(2024,1,1)},new Employee{EmployeeCode="WF-2",FullName="Two",Email="two@example.test",Department=d,IsActive=false,DateJoined=new DateOnly(2024,1,1)});await db.SaveChangesAsync();var result=await new EmployeeQueryService(db).GetDepartmentSummaryAsync(CancellationToken.None);Assert.Single(result);Assert.Equal(2,result[0].TotalEmployees);Assert.Equal(1,result[0].ActiveEmployees);}
}
