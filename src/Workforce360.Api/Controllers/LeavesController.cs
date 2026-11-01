using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Workforce360.Api.Application;
namespace Workforce360.Api.Controllers;
[ApiController][Route("api/leaves")][Authorize(Policy="HrAccess")]
public sealed class LeavesController(ILeaveQueryService service):ControllerBase {
 [HttpGet("pending")]public Task<IReadOnlyList<PendingLeaveSummary>> Pending(CancellationToken ct)=>service.GetPendingAsync(ct);
 [HttpPost]public async Task<IActionResult> Create(CreateLeaveRequest request,CancellationToken ct){try{var id=await service.CreateAsync(request,ct);return Created($"/api/leaves/{id}",new{id});}catch(ArgumentException ex){return BadRequest(new{message=ex.Message});}catch(KeyNotFoundException ex){return NotFound(new{message=ex.Message});}}
}
