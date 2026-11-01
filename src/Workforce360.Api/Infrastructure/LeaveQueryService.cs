using Microsoft.EntityFrameworkCore;
using Workforce360.Api.Application;
using Workforce360.Api.Data;
using Workforce360.Api.Domain;
namespace Workforce360.Api.Infrastructure;
public sealed class LeaveQueryService(WorkforceDbContext db):ILeaveQueryService {
 public async Task<IReadOnlyList<PendingLeaveSummary>> GetPendingAsync(CancellationToken ct)=>await db.LeaveRequests.AsNoTracking().Where(x=>x.Status==LeaveStatus.Pending).Include(x=>x.Employee).ThenInclude(x=>x.Department).OrderBy(x=>x.StartDate).Take(100).Select(x=>new PendingLeaveSummary(x.Id,x.Employee.FullName,x.Employee.Department.Name,x.StartDate,x.EndDate,x.Reason,x.Status.ToString())).ToListAsync(ct);
 public async Task<int> CreateAsync(CreateLeaveRequest r,CancellationToken ct) {
  if(r.EmployeeId<=0)throw new ArgumentException("EmployeeId must be positive.");
  if(r.StartDate<DateOnly.FromDateTime(DateTime.UtcNow))throw new ArgumentException("Leave start date cannot be in the past.");
  if(r.EndDate<r.StartDate)throw new ArgumentException("End date must be on or after start date.");
  if(string.IsNullOrWhiteSpace(r.Reason)||r.Reason.Length>500)throw new ArgumentException("Reason is required and must be <= 500 characters.");
  if(!await db.Employees.AnyAsync(x=>x.Id==r.EmployeeId&&x.IsActive,ct))throw new KeyNotFoundException("Active employee not found.");
  var entity=new LeaveRequest{EmployeeId=r.EmployeeId,StartDate=r.StartDate,EndDate=r.EndDate,Reason=r.Reason.Trim()};db.LeaveRequests.Add(entity);await db.SaveChangesAsync(ct);return entity.Id;
 }
}
