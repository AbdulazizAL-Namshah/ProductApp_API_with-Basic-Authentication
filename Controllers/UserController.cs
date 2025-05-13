using ProductApp_API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductApp_API.Controllers;
[ApiController]
[Route("[Controller]")]
public class UserController(JwtOptions jwtOptions,ApplicationDbContext dbContext):ControllerBase
{

    [HttpPost]
    [Route("auth")]
    public ActionResult<string> AuthenticationUser(AuthenticationRequest request)
    {
        var user = dbContext.Set<User>().FirstOrDefault(x => x.Name == request.UserName && x.Password == request.Password);
            if (user == null)
               return Unauthorized();
            var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = jwtOptions.Issuer,
            Audience = jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)), SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new(ClaimTypes.Name,user.Name),
                new(ClaimTypes.Role,"Admin"),
                new(ClaimTypes.Role,"SuperOnly"),
                new("UserType","Employee"),
                new("DateOfBirth","1990-01-02")
            })
        };
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(securityToken);
        return Ok(accessToken);
    }





}



