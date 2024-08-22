using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {


        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            UserManager = userManager;
            TokenRepository = tokenRepository;
        }

        public UserManager<IdentityUser> UserManager { get; }
        public ITokenRepository TokenRepository { get; }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto RRD)
        {
            var identityUser = new IdentityUser
            {
                UserName = RRD.Username,
                Email = RRD.Username
            };
            var identityResult = await UserManager.CreateAsync(identityUser, RRD.Password);

            if (identityResult.Succeeded)
            {
                // if user was created successfully, we need to assign him a role if specified.
                if (RRD.Roles != null && RRD.Roles.Any())
                {
                    identityResult = await UserManager.AddToRolesAsync(identityUser, RRD.Roles);
                    if (identityResult.Succeeded)
                    {
                        return Ok("Registered Successfully! Please login.");
                    }
                    
                }
            }
            return BadRequest("Something went wrong!");
        }



        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto LRD)
        {
            var user = await UserManager.FindByEmailAsync(LRD.Username);
            if (user != null)
            {
                var checkPasswordResult = await UserManager.CheckPasswordAsync(user, LRD.Password);
                if (checkPasswordResult)
                {
                    //we need to get the roles for this user
                    var roles = await UserManager.GetRolesAsync(user);
                    if (roles != null)
                    {
                        // create a token
                        var jwtToken = TokenRepository.CreateJWTToken(user, roles.ToList());

                        //create response dto
                        var response = new LoginResponseDto
                        {
                            JwtToken = jwtToken,
                        };
                        return Ok(response);
                    }
                    
                    
                }
            }
            
            return BadRequest("Username or Password Incorrect!");
        }
    }
}
