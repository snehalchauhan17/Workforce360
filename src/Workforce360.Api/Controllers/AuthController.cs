using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
namespace Workforce360.Api.Controllers;
public sealed record DemoLoginRequest(string Email,string Password);
[ApiController][Route("api/auth")]
public sealed class AuthController(IConfiguration config,IWebHostEnvironment env):ControllerBase {
 [HttpPost("demo-login")] public IActionResult DemoLogin(DemoLoginRequest request) {
  if(!env.IsDevelopment())return NotFound();var email=config["Demo:Email"]??"hr.admin@workforce360.local";var password=config["Demo:Password"];
  if(string.IsNullOrWhiteSpace(password))return Problem("Set Demo:Password using dotnet user-secrets.");
  if(!string.Equals(request.Email,email,StringComparison.OrdinalIgnoreCase)||request.Password!=password)return Unauthorized(new{message="Invalid demo credentials."});
  var keyText=config["Jwt:SigningKey"];if(string.IsNullOrWhiteSpace(keyText))keyText=Convert.ToBase64String(Encoding.UTF8.GetBytes("LOCAL-ONLY-CHANGE-THIS-DEVELOPMENT-KEY-32-BYTES-MINIMUM"));
  var claims=new[]{new Claim(JwtRegisteredClaimNames.Sub,request.Email),new Claim(ClaimTypes.Name,"Workforce360 Demo HR Admin"),new Claim(ClaimTypes.Role,"HRAdmin")};
  var token=new JwtSecurityToken(issuer:config["Jwt:Issuer"]??"Workforce360",audience:config["Jwt:Audience"]??"Workforce360Client",claims:claims,expires:DateTime.UtcNow.AddHours(1),signingCredentials:new SigningCredentials(new SymmetricSecurityKey(Convert.FromBase64String(keyText)),SecurityAlgorithms.HmacSha256));
  return Ok(new{accessToken=new JwtSecurityTokenHandler().WriteToken(token),tokenType="Bearer",expiresInSeconds=3600});
 }
}
