using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
namespace Workforce360.Api.Infrastructure;
public sealed class WorkforceAiService(IChatClient chatClient,IConfiguration config) {
 public async Task<string> AskAsync(string message,string authorization,CancellationToken ct) {
  var endpoint=config["Mcp:Endpoint"]??throw new InvalidOperationException("Mcp:Endpoint missing.");
  var transport=new HttpClientTransport(new HttpClientTransportOptions{Endpoint=new Uri(endpoint),TransportMode=HttpTransportMode.StreamableHttp,AdditionalHeaders=new Dictionary<string,string>{{"Authorization",authorization}},ConnectionTimeout=TimeSpan.FromSeconds(20)});
  await using var mcp=await McpClient.CreateAsync(transport,cancellationToken:ct);
  var tools=await mcp.ListToolsAsync(cancellationToken:ct);
  var history=new List<ChatMessage>{
   new(ChatRole.System,"You are Workforce360's HR operations assistant. Use tools for current employee, department and pending leave data. Never invent records or counts. You cannot approve or reject leave. Never request or reveal credentials, bank details, payroll values or sensitive information. Summarize only authorized tool results."),
   new(ChatRole.User,message)};
  var response=await chatClient.GetResponseAsync(history,new ChatOptions{Tools=[..tools]},ct);
  return response.Text??"I could not generate a response.";
 }
}
