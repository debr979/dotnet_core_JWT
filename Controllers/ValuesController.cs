using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace WebApplication4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public IConfiguration _config { get; }
        public ValuesController(IConfiguration configuration)
        {
            _config = configuration;
        }

        [HttpGet]
        public IActionResult TestGet()
        {
            Models.RegInfo reg = new Models.RegInfo();
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.NameId,"doug"),
                new Claim(ClaimTypes.Role,"AdminRole")
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SignKey"]));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                    issuer: _config["JWT:Issuer"],
                    audience: _config["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(7),
                    signingCredentials: cred
                    );
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }


        [Route("Login")]
        [HttpPost]
        public IActionResult TestPost([FromBody]Models.RegInfo reg)
        {

            if (reg.name == "doug" && reg.pswd == "doug")
            {
                var claims = new[]{
                    new Claim(ClaimTypes.Name,reg.name),
                    new Claim(ClaimTypes.Role,"AdminRole")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SignKey"]));
                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: _config["JWT:Issuer"],
                    audience: _config["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: cred
                    );
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
                ;
            }

            return BadRequest("Could not verify name & pswd");
        }
    }

}
