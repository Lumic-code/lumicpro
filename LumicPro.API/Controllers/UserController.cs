using LumicPro.Application.Models;
using LumicPro.Core.Entities;
using LumicPro.Core.Enums;
using LumicPro.Core.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LumicPro.API.Controllers
{
     //[ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpPost("add")]
        public IActionResult AddNewUser([FromBody] AddUserDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                AttendanceStatus status;
                if (!Enum.TryParse(model.AttendanceStatus, out status))
                {
                    return BadRequest("Invalid Attendance Status");
                }
                var appUser = new AppUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    AttendanceStatus = model.AttendanceStatus,
                };

                var result = _userRepository.AddNew(appUser);
                return CreatedAtAction(nameof(GetUser), new { Id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("get-user/{Id}")]
        public IActionResult GetUser(string Id)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(Id))
                {
                    return BadRequest("An Id is required!");
                }

                var result = _userRepository.GetById(Id);
                if (result == null)
                {
                    return NotFound("No record found!");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest();
            }
        }

        [HttpPut("update-user/{id}")]
        public IActionResult UpdateUser(string id, [FromBody] UpdateUserDto model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("An Id is required!");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (id != model.Id)
                {
                    return BadRequest("Id mismatch!");
                }
                var appUser = _userRepository.GetById(id);
                if (appUser == null)
                {
                    return NotFound("No record found!");
                }
                appUser.FirstName = model.FirstName;
                appUser.LastName = model.LastName;
                appUser.AttendanceStatus = model.AttendanceStatus;

                var result = _userRepository.Update(appUser);
                if (result == null)
                {
                    return BadRequest("Failed to update user!");
                }
                return Ok(result);
            }
            catch (Exception)
            {

                return BadRequest();
            }

        }

        [HttpGet("get-all-users")]
        public IActionResult GetAll(int page, int perpage)
        {
            var result = _userRepository.GetAll();
            if (result != null && result.Count() > 0)
            {
                var paged = _userRepository.paginate(result.ToList(), page, perpage);
                return Ok(paged);
            }
           
            return Ok(result);
        }

        [HttpDelete("delete-user/{id}")]
        public IActionResult DeleteUser(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("An Id is required!");
                }
                var appUser = _userRepository.GetById(id);
                if (appUser == null)
                {
                    return NotFound("No record found!");
                }
                var result = _userRepository.Delete(appUser);

                if (result == false)
                {
                    return BadRequest("Failed to delete user!");
                }
                return Ok($"User with Id: {appUser.Id} was successfully deleted");
            }
            catch (Exception ex)
            {

                return BadRequest();
            }
        }

        [HttpDelete("delete-all-user/list")]
        public IActionResult DeleteAll([FromBody] DeleteAllUsersDto model)
        {
            try
            {
                if(model.Ids.Count() < 1)
                {
                    return BadRequest("No Ids were provided!");
                }
                var list = new List<AppUser>();

                foreach (var id in model.Ids)
                {
                    var appUser = _userRepository.GetById(id);
                    if (appUser != null)
                    {
                        list.Add(appUser);
                    }
                }

                if (list.Count > 0)
                 {
                    var result = _userRepository.DeleteAll(list);
                     if(result)
                     return Ok("Users deleted successfully!");
                 }


                return BadRequest("Failed to delete users");
            }
            catch (Exception)
            {
                 return BadRequest();
            }

        }
    }
}
