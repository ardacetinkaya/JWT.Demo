using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using JWT.API.Model;
using Microsoft.Extensions.Configuration;

namespace JWT.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        public LoginController(IConfiguration config)
        {

            _config = config;
        }
        
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Post([FromBody] LoginDTO credentials)
        {

            //TODO: Check credentials from some user management system

            //So we checked, and let's create a valid token with some Claim
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Application:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = "SomeCustomApp",
                Issuer = "mineplaJWT.api.demo",
                Subject = new ClaimsIdentity(new Claim[]
                {
                    //Add any claim
                    new Claim(ClaimTypes.Name, credentials.Username),
                    new Claim(ClaimTypes.HomePhone,"+90 555 123 45 67")
                }),

                //Expire token after some time
                Expires = DateTime.UtcNow.AddDays(7),

                //Let's also sign token credentials for a security aspect, this is important!!!
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            //So see token info also please check token
            // return Ok(new { TokenInfo = token });

            //Return token in some way
            //to the clients so that they can use it
            //return it with header would be nice
            return Ok(new { Token = tokenString });
        }
    }
}
