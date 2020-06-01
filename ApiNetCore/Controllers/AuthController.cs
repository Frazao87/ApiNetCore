using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiNetCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ApiNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        public AuthController(IConfiguration configuration)
        {
            _config = configuration;
        }

        [HttpPost]
        public IActionResult Login([FromBody] User loginDetails)
        {
            bool result = UserValidate(loginDetails);
            if (result)
            {
                var tokenString = JwtTokenGenerate();
                return Ok(new { token = tokenString });
            }
            else
            {
                return Unauthorized();
            }
        }
        private bool UserValidate(User loginDetails)
        {
            if (loginDetails.UserName == "apiowner" && loginDetails.Password == "123")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string JwtTokenGenerate()
        {
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expiry = DateTime.Now.AddMinutes(60);

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials
                (securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken
                (issuer: issuer,
                audience: audience,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            var stringToken = tokenHandler.WriteToken(token);

            return stringToken;
        }
    }
}
