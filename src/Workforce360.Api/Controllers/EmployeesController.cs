using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Workforce360.Api.Application;
namespace Workforce360.Api.Controllers;
[ApiController][Route("api/employees")][Authorize(Policy="HrAccess")]
public sealed class EmployeesController(IEmployeeQueryService service):ControllerBase {
 [HttpGet]public Task<IReadOnlyList<EmployeeSummary>> Search([FromQuery]string? q,CancellationToken ct)=>service.SearchAsync(q,ct);
 [HttpGet("department-summary")]public Task<IReadOnlyList<DepartmentSummary>> Summary(CancellationToken ct)=>service.GetDepartmentSummaryAsync(ct);
}
