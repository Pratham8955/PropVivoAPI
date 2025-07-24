using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CMS.DTOs.GroupMasterDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Plugins;
using PropVivoAPI.Models;

namespace PropVivoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly PropvivoContext _propvivoContext;
        private readonly IConfiguration _configuration;
        public LoginController(PropvivoContext propvivoContext,IConfiguration configuration)
        {
            _propvivoContext = propvivoContext;
            _configuration = configuration;
        }

        [HttpGet("fetchroles")]
        public async Task<ActionResult> FetchRoles()
        {
            var roles = await _propvivoContext.RoleMasters.Select(r =>
            new RoleMasterDTO { RoleId = r.RoleId, RoleName = r.RoleName }).ToListAsync();

            return Ok(new
            {
                success = true,
                message = "roles fetch successfully.",
                Roles = roles,
            });
        }

        [HttpPost]
        public async Task<ActionResult> login([FromBody] LoginUserDTO loginUserDto)
        {
            var user = _propvivoContext.UserMasters.Include(u => u.Role).Where(u => u.Email == loginUserDto.Email).FirstOrDefault();
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginUserDto.Password, user.Password))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid Email and Password"
                });
            }

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                success = true,
                message = "Login Successfull.",
                token = token,
                roleId = user.RoleId,
                redirectUrl = GetRedirectUrl(user.RoleId)
            });
        }
        private string GenerateJwtToken(UserMaster user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim("UserId", user.UserId.ToString()),
            new Claim("RoleName", user.Role.RoleName.ToString()),
            new Claim("RoleId", user.RoleId.ToString()),
        };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetRedirectUrl(int roleId)
        {
            return roleId switch
            {
                1 => "/admin/admindashboard",
                2 => "/faculty/facultydashboard",
                3 => "/student/Studentdashboard",
                _ => "/"
            };
        }
    }
}
