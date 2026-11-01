using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;
using Workforce360.Api.Application;
namespace Workforce360.Api.Mcp.Tools;
[McpServerToolType]
[Authorize(Policy="HrAccess")]
public sealed class WorkforceTools(IEmployeeQueryService employees,ILeaveQueryService leaves) {
 [McpServerTool(Name="search_employees")][Description("Search the authorized employee directory by name, employee code, or department. Returns limited non-sensitive summary fields.")]
 public Task<IReadOnlyList<EmployeeSummary>> SearchEmployees([Description("Employee name, code, or department search term.")]string searchTerm,CancellationToken cancellationToken)=>employees.SearchAsync(searchTerm,cancellationToken);
 [McpServerTool(Name="get_department_summary")][Description("Get employee totals and active employee counts grouped by department.")]
 public Task<IReadOnlyList<DepartmentSummary>> GetDepartmentSummary(CancellationToken cancellationToken)=>employees.GetDepartmentSummaryAsync(cancellationToken);
 [McpServerTool(Name="get_pending_leave_requests")][Description("List pending leave requests with employee, department, dates and reason. Does not approve or reject requests.")]
 public Task<IReadOnlyList<PendingLeaveSummary>> GetPendingLeaveRequests(CancellationToken cancellationToken)=>leaves.GetPendingAsync(cancellationToken);
}
