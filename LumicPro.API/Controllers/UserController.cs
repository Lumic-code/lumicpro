using AutoMapper;
using LumicPro.Application.Models;
using LumicPro.Core.Entities;
using LumicPro.Core.Enums;
using LumicPro.Core.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LumicPro.API.Controllers
{

    //[ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "regular")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepository userRepository, UserManager<AppUser> userManager, IMapper mapper, ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("add")]
        public async Task<IActionResult> AddNewUser([FromBody] AddUserDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userExists = await _userManager.FindByEmailAsync(model.Email);
                if(userExists != null)
                {
                    return BadRequest("Email already exists!");
                }

                AttendanceStatus status;
                if (!Enum.TryParse(model.AttendanceStatus, out status))
                {
                    return BadRequest("Invalid Attendance Status");
                }
             

                var mappedUser = _mapper.Map<AppUser>(model);
                var result = await _userManager.CreateAsync(mappedUser, model.Password);
                if(result.Succeeded)
                {
                    var token = _userManager.GenerateEmailConfirmationTokenAsync(mappedUser);
                    _logger.LogWarning($"Token: {token}");
                    //return CreatedAtAction(nameof(GetUser), new { Id = mappedUser.Id }, mappedUser);
                     var userFromDb = _mapper.Map<UserResponse>(mappedUser);

                    await _userManager.AddToRoleAsync(mappedUser, "Regular");

                    var res = new ResponseObject<UserResponse>()
                    {
                        StatusCode = 200,
                        DisplayMessage = $"User with Id {userFromDb.Id} was successfully added!",
                        Data = userFromDb,
                     };

                      return Ok(res);
                }

                foreach(var err in result.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }

                return BadRequest();
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
                var mappedUser = _mapper.Map<UserResponse>(result);
                var res = new ResponseObject<UserResponse>()
                {
                    StatusCode = 200,
                    DisplayMessage = $"User with Id {mappedUser.Id} was successfully found!",
                    Data = mappedUser
                };

                return Ok(res);
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
