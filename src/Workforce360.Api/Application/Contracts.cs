namespace Workforce360.Api.Application;
public sealed record EmployeeSummary(int Id,string EmployeeCode,string FullName,string Email,string Department,bool IsActive,DateOnly DateJoined);
public sealed record DepartmentSummary(string Department,int TotalEmployees,int ActiveEmployees);
public sealed record PendingLeaveSummary(int RequestId,string EmployeeName,string Department,DateOnly StartDate,DateOnly EndDate,string Reason,string Status);
public sealed record CreateLeaveRequest(int EmployeeId,DateOnly StartDate,DateOnly EndDate,string Reason);
public interface IEmployeeQueryService { Task<IReadOnlyList<EmployeeSummary>> SearchAsync(string? term,CancellationToken ct); Task<IReadOnlyList<DepartmentSummary>> GetDepartmentSummaryAsync(CancellationToken ct); }
public interface ILeaveQueryService { Task<IReadOnlyList<PendingLeaveSummary>> GetPendingAsync(CancellationToken ct); Task<int> CreateAsync(CreateLeaveRequest request,CancellationToken ct); }
