using LumicPro.Application.Models;
using LumicPro.Core.Entities;
using LumicPro.Core.Repository;
using LumicPro.Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LumicPro.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, IUserRepository userRepository,
            UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var res = new ResponseObject<string>
            {
                 StatusCode = 400
            };
            //throw new Exception("This app must stop here!!!"); 

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // var user = _userRepository.GetByEmail(model.Email);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                if (user.EmailConfirmed)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                    if (result.Succeeded)
                    {
                        var jwt = new Utils(_configuration);
                        var token = jwt.GenerateJWT(user);
                        res.StatusCode = 200;
                        res.DisplayMessage = "Login Successful!";
                        res.Data = token;
                        return Ok(res);
                    }
                    res.DisplayMessage = "Invalid credentials for password!";
                    return BadRequest(res);
                }
                    res.DisplayMessage = "Email not confirmed yet!";
                    return BadRequest(res);
            }
            res.DisplayMessage = "Invalid credential for email";
            return BadRequest(res);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var res = new ResponseObject<string>
            {
                StatusCode = 400
            };
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var link = Url.Action("ResetPassword", new { email = model.Email, token});
                _logger.LogWarning(link);
                res.DisplayMessage = "Confirm token generated and sent to your email";
                res.StatusCode = 200;
                return Ok(res);
            }
            res.StatusCode = 404;
            res.DisplayMessage = $"user with email: {model.Email} was not found!";
            return NotFound(user);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var res = new ResponseObject<string>
            {
                StatusCode = 400
            };

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            };

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                if (result.Succeeded)
                {
                    res.DisplayMessage = "password reset was successful";
                    res.StatusCode = 200;
                    return Ok(res);
                }
                foreach(var err in result.Errors)
                {
                    res.DisplayMessage ="Password reset failed!";
                    res.ErrorMessages.Add(err.Description);
                    return BadRequest(res);

                }
            }
            res.DisplayMessage = $"User with email: {model.Email} was not found!";
            return NotFound();


        }

        [HttpPost("log-out")]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Ok( new ResponseObject<string>() { StatusCode = 200, DisplayMessage = "LogOut Successful!"});
        }
    }


}

