# Workforce360 — Enterprise workforce platform with AI + MCP

A portfolio MVP using .NET 10, ASP.NET Core, SQL Server, EF Core, Angular, OpenAI and the official MCP C# SDK.

## Included
- Employee directory, department analytics, pending leave list and leave request creation.
- JWT-protected API and Development-only demo login.
- MCP Streamable HTTP endpoint hosted inside the same ASP.NET Core API (`/mcp`).
- MCP tools: `search_employees`, `get_department_summary`, `get_pending_leave_requests`.
- OpenAI integration via `Microsoft.Extensions.AI`; MCP client discovers tools and passes them to the model.
- Angular chat UI, xUnit tests and Docker Compose.

This is a learning/portfolio MVP, not a production HR/payroll system. It uses fictional seed data and does not expose salary, bank, or similarly sensitive fields to AI.

## Requirements
.NET 10 SDK, SQL Server or Docker, Node.js 20.19+/22.12+, OpenAI API key with API billing enabled.

## Configure secrets
From `src/Workforce360.Api`:
```bash
dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "YOUR_OPENAI_API_KEY"
dotnet user-secrets set "Demo:Password" "A-Strong-Local-Only-Password"
dotnet user-secrets set "Jwt:SigningKey" "YOUR_BASE64_RANDOM_KEY"
```
Generate key using `openssl rand -base64 48`. Never commit secrets.

## Run SQL Server
```bash
docker compose up -d sql
```
The compose file uses local-only credentials; change them before any shared deployment.

## Run API
```bash
cd src/Workforce360.Api
dotnet restore
dotnet run
```
The launch profile uses `http://localhost:5080` and `https://localhost:7080`. Swagger: `/swagger`.
In Development only, the app uses `EnsureCreated` and seeds fictional demo records. Replace this with EF Core migrations for production.

## Demo login (Development only)
`POST http://localhost:5080/api/auth/demo-login`
```json
{"email":"hr.admin@workforce360.local","password":"YOUR_CONFIGURED_DEMO_PASSWORD"}
```
Use the returned access token as `Authorization: Bearer <token>`. This demo login is not a production identity system; replace it with ASP.NET Core Identity or OIDC/Entra ID.

## MCP
MCP endpoint: `http://localhost:5080/mcp` (Streamable HTTP). It requires an HRAdmin or HRManager JWT. Test with [MCP Inspector](https://github.com/modelcontextprotocol/inspector), selecting Streamable HTTP and providing the bearer token.

The AI service connects to this endpoint using the caller's JWT, calls `McpClient.ListToolsAsync()`, and passes the discovered `McpClientTool` functions to OpenAI through `Microsoft.Extensions.AI`. `UseFunctionInvocation()` executes model-selected tool calls.

## Test AI
`POST /api/ai/chat` with bearer token:
```json
{"message":"How many active employees are in each department?"}
```
The internal MCP URL is `Mcp:Endpoint` in appsettings. If you change the API port, update it.

## Angular UI
```bash
cd frontend/workforce360-ui
npm install
npm start
```
Open `http://localhost:4200`. Sign in using the demo email and configured password. The API enables a narrowly scoped CORS policy for this local Angular origin; replace it with your deployed frontend origin in other environments.

## Tests
```bash
dotnet test tests/Workforce360.Tests/Workforce360.Tests.csproj
```

## Security and next steps
- MCP and AI routes require HR roles; AI-to-MCP forwards the authenticated caller's JWT.
- MCP tools call application services, never arbitrary SQL.
- Tool inputs are untrusted and must be validated server-side.
- Add per-user/team/tenant data scoping before introducing non-HR users.
- Add rate limits, structured audit logs, API integration tests, MCP authorization tests and secret management before production.
- Before write tools (approval/payroll changes), add human confirmation, idempotency, audit trails and explicit authorization.
- Not included: full identity provider, payroll engine, attendance ingestion, RAG, persistent chat history, leave approval write-tools, production CI/CD.
