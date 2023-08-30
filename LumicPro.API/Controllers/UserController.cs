using LumicPro.Application.Models;
using LumicPro.Core.Entities;
using LumicPro.Core.Enums;
using LumicPro.Core.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LumicPro.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("add")]
        public IActionResult AddNewUser([FromBody] AddUserDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var appUser = new AppUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    AttendanceStatus = AttendanceStatus.present,
                };

                var result = _userRepository.AddNew(appUser);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
