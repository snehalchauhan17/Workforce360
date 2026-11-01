using Microsoft.EntityFrameworkCore;
using Workforce360.Api.Application;
using Workforce360.Api.Data;
namespace Workforce360.Api.Infrastructure;
public sealed class EmployeeQueryService(WorkforceDbContext db):IEmployeeQueryService {
 public async Task<IReadOnlyList<EmployeeSummary>> SearchAsync(string? term,CancellationToken ct) {
  var q=db.Employees.AsNoTracking().Include(x=>x.Department).AsQueryable();
  if(!string.IsNullOrWhiteSpace(term)){var s=term.Trim();q=q.Where(x=>x.FullName.Contains(s)||x.EmployeeCode.Contains(s)||x.Department.Name.Contains(s));}
  return await q.OrderBy(x=>x.FullName).Take(50).Select(x=>new EmployeeSummary(x.Id,x.EmployeeCode,x.FullName,x.Email,x.Department.Name,x.IsActive,x.DateJoined)).ToListAsync(ct);
 }
 public async Task<IReadOnlyList<DepartmentSummary>> GetDepartmentSummaryAsync(CancellationToken ct)=>await db.Employees.AsNoTracking().GroupBy(x=>x.Department.Name).Select(g=>new DepartmentSummary(g.Key,g.Count(),g.Count(x=>x.IsActive))).OrderBy(x=>x.Department).ToListAsync(ct);
}
