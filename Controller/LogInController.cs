
using Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;


namespace MyProjectController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogIn : ControllerBase
    {
        private  readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _config;
        public LogIn (UserManager<IdentityUser> userManager, 
                      SignInManager<IdentityUser> signInManager,
                      IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

[HttpPost("/login")]
public async Task<IActionResult> Login([FromBody] LogInDto model)
{

    try
    {
        
        var User = await _userManager.FindByNameAsync(model.UserName);
            
            if (User == null) 
            {
                return BadRequest();
            }
            
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

        if (result.Succeeded)
        {

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:secret"]);
            var userRole = await _userManager.GetClaimsAsync(User);
            string role = userRole[1].Value; 

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("role", role.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { 
                            Token = tokenString,
                            id = User.Id,
                            roles = role
                         });
        }

            return NotFound("user does not exist");
    }
    catch (Exception ex)
    {
            return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}

    }
}
