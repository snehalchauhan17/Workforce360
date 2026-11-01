using System.Text;
using OpenAI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.AI;
using ModelContextProtocol.AspNetCore;
using ModelContextProtocol.Server;
using Microsoft.OpenApi;
using Workforce360.Api.Application;
using Workforce360.Api.Data;
using Workforce360.Api.Infrastructure;
using Workforce360.Api.Mcp.Tools;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
}); 
builder.Services.AddCors(options => options.AddPolicy("AngularDev", policy =>
    policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddDbContext<WorkforceDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("WorkforceDb")));
builder.Services.AddScoped<IEmployeeQueryService, EmployeeQueryService>();
builder.Services.AddScoped<ILeaveQueryService, LeaveQueryService>();
builder.Services.AddScoped<WorkforceAiService>();

var keyText = builder.Configuration["Jwt:SigningKey"];
if (string.IsNullOrWhiteSpace(keyText)) {
    if (!builder.Environment.IsDevelopment()) throw new InvalidOperationException("Configure Jwt:SigningKey.");
    keyText = Convert.ToBase64String(Encoding.UTF8.GetBytes("LOCAL-ONLY-CHANGE-THIS-DEVELOPMENT-KEY-32-BYTES-MINIMUM"));
}
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o => {
    o.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer=true, ValidIssuer=builder.Configuration["Jwt:Issuer"] ?? "Workforce360",
        ValidateAudience=true, ValidAudience=builder.Configuration["Jwt:Audience"] ?? "Workforce360Client",
        ValidateIssuerSigningKey=true, IssuerSigningKey=new SymmetricSecurityKey(Convert.FromBase64String(keyText)),
        ValidateLifetime=true, ClockSkew=TimeSpan.FromSeconds(30),
        NameClaimType="name", RoleClaimType="role"
    };
});
builder.Services.AddAuthorization(o => o.AddPolicy("HrAccess", p => p.RequireAuthenticatedUser().RequireRole("HRAdmin","HRManager")));

var openAiKey = builder.Configuration["OpenAI:ApiKey"];
if (string.IsNullOrWhiteSpace(openAiKey)) throw new InvalidOperationException("Configure OpenAI:ApiKey using dotnet user-secrets.");
var model = builder.Configuration["OpenAI:Model"] ?? "gpt-4.1-mini";
builder.Services.AddSingleton<IChatClient>(_ => new ChatClientBuilder(new OpenAIClient(openAiKey).GetChatClient(model).AsIChatClient()).UseFunctionInvocation().Build());
builder.Services.AddMcpServer().WithHttpTransport(o => o.SessionMode = HttpServerSessionMode.Stateless).WithToolsFromAssembly();

var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); await DemoDataSeeder.SeedAsync(app.Services); }
if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseCors("AngularDev");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapMcp("/mcp").RequireAuthorization("HrAccess");
app.Run();
public partial class Program;
