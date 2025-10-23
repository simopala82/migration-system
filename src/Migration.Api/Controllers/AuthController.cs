using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Migration.DataAccess.Services;

namespace Migration.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IUserNewRepository _userNewRepository;
    private readonly IUserOldRepository _userOldRepository;

    public AuthController(IConfiguration config, IUserNewRepository userNewRepository, IUserOldRepository userOldRepository)
    {
        _config = config;
        _userNewRepository = userNewRepository;
        _userOldRepository = userOldRepository;
    }

    // Endpoint to generate a fake admin token
    [HttpGet("token/admin/{id}")]
    public async Task<IActionResult> GetAdminToken(string id)
    {
        var token = await GetTestTokenClaims(id, "Admin");
        return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) });
    }

    // Endpoint to generate a fake user token
    [HttpGet("token/user/{id}")]
    public async Task<IActionResult> GetUserToken(string id)
    {
        var token = await GetTestTokenClaims(id, "User");
        return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
    
    private async Task<SecurityToken> GetTestTokenClaims(string id, string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>();

        if (Guid.TryParse(id, out var newIdUser))
        {
            var userNew = await _userNewRepository.Get(newIdUser);
            claims.AddRange([
                new Claim(ClaimTypes.NameIdentifier, newIdUser.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, userNew.FullName)
            ]);
        }
        else if (int.TryParse(id, out var legacyUserId))
        {
            var userOld = await _userOldRepository.Get(legacyUserId);
            claims.AddRange([
                new Claim(ClaimTypes.NameIdentifier, legacyUserId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, $"{userOld.FirstName} {userOld.LastName}")
            ]);
        }
        else
            throw new UnauthorizedAccessException();

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

        return token;
    }

}