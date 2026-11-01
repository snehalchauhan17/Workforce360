using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Workforce360.Api.Infrastructure;
namespace Workforce360.Api.AI;
public sealed record ChatRequest(string Message);
[ApiController][Route("api/ai")][Authorize(Policy="HrAccess")]
public sealed class AIChatController(WorkforceAiService ai):ControllerBase {
 [HttpPost("chat")]public async Task<IActionResult> Chat(ChatRequest request,CancellationToken ct){if(string.IsNullOrWhiteSpace(request.Message)||request.Message.Length>4000)return BadRequest(new{message="Message required; maximum 4000 characters."});var bearer=Request.Headers.Authorization.ToString();var answer=await ai.AskAsync(request.Message,bearer,ct);return Ok(new{answer});}
}
