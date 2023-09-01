using AutoMapper;
using LumicPro.Application.Models;
using LumicPro.Core.Entities;
using LumicPro.Core.Enums;
using LumicPro.Core.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LumicPro.API.Controllers
{
     //[ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoleController : ControllerBase
    {
       
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, IMapper mapper)
        {
            
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("add")]
        public async Task<IActionResult> AddRole([FromBody] AddRoleDto model)
        {
            var res = new ResponseObject<List<string>>();
            if (model.Roles != null && model.Roles.Count > 0)
            {
                 foreach (var role in model.Roles)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role)); 
                }

                res.StatusCode = 200;
                res.DisplayMessage = "Roles Successfully Added!";
                return Ok(res);
            }
            res.StatusCode = 400;
            res.DisplayMessage = "Null or empty entry!";
            return BadRequest(res);
        }

       
    }
}
