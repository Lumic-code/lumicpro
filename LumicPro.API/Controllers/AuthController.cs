﻿using LumicPro.Application.Models;
using LumicPro.Core.Entities;
using LumicPro.Core.Repository;
using LumicPro.Infrastructure.Security;
using Microsoft.AspNetCore.Http;
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
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, IUserRepository userRepository, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto model)
        {
           
                 //throw new Exception("This app must stop here!!!"); 

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = _userRepository.GetByEmail(model.Email);
                if (user != null)
                {
                    if (model.Password == user.PasswordHash)
                    {
                        var jwt = new Utils(_configuration);
                        var token = jwt.GenerateJWT(user);
                        return Ok(token);
                    }
                    return BadRequest("Invalid credential for password");
                }
                return BadRequest("Invalid credential for email");
               
            }
          
           
        }

        
    }

