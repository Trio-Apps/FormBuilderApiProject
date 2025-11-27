using FormBuilder.Application.DTOS.Auth;
using FormBuilder.Core.IServices.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.ApiProject.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UsersDto>>> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();
            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsersDto>> GetUserById(string id)
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != id && !User.IsInRole("Admin"))
                return Forbid();

            var result = await _userService.GetUserByIdAsync(id);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UsersDto>> CreateUser(CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.CreateUserAsync(createUserDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);

            return CreatedAtAction(nameof(GetUserById), new { id = result.Data.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UsersDto>> UpdateUser(string id, UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != id && !User.IsInRole("Admin"))
                return Forbid();

            var result = await _userService.UpdateUserAsync(id, updateUserDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);

            return NoContent();
        }

        [HttpPatch("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeactivateUser(string id)
        {
            var result = await _userService.DeactivateUserAsync(id);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);

            return Ok(new { message = "User deactivated successfully" });
        }

        [HttpPatch("{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ActivateUser(string id)
        {
            var result = await _userService.ActivateUserAsync(id);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.ErrorMessage);

            return Ok(new { message = "User activated successfully" });
        }
    }
}
