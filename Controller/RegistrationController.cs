
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace MyProjectController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterUser : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        
        public RegisterUser(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("/register/{role}")]
        public async Task<IActionResult> Register([FromBody] RegisterUSerDto model, [FromRoute] string role)
        {
            try
            {
                var user = new IdentityUser { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                  {                      
                        
                        Claim[] userCliams = 
                            [
                                new Claim(ClaimTypes.Email, model.Email),
                                new Claim(ClaimTypes.Role, role)
                            ]; 
                        
                         await _userManager.AddClaimsAsync(user, userCliams);
                        return Ok("User registered successfully!");
                        
                }
    
                else
                {
                    throw new Exception("account was not created");   
                }

            }
            catch (Exception ex)
            {
                
                    return BadRequest(ex.Message);
            }
            
        }

    }
}
