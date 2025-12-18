using FormBuilder.Application.Abstractions;
using FormBuilder.Application.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IaccountService _accountService;

        public AccountController(IaccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _accountService.LoginAsync(request.Username, request.Password, cancellationToken);

            if (!response.Success)
                return Unauthorized(new { response.ErrorMessage });

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _accountService.RefreshTokenAsync(request.RefreshToken, cancellationToken);

            if (!response.Success)
                return Unauthorized(new { response.ErrorMessage });

            return Ok(response);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accountService.LogoutAsync(request.RefreshToken, cancellationToken);

            if (!result)
                return BadRequest(new { message = "Invalid refresh token." });

            return Ok(new { message = "Logged out successfully." });
        }

        [HttpPost("revoke-all-tokens/{userId}")]
        [Authorize(Roles = "Administration")]
        public async Task<IActionResult> RevokeAllUserTokens(int userId, CancellationToken cancellationToken)
        {
            if (userId <= 0)
                return BadRequest(new { message = "Invalid user ID." });

            var result = await _accountService.RevokeAllUserTokensAsync(userId, cancellationToken);

            if (!result)
                return BadRequest(new { message = "Failed to revoke tokens." });

            return Ok(new { message = "All user tokens revoked successfully." });
        }

        [HttpGet("current-user")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized(new { message = "Invalid user token." });

            var userInfo = await _accountService.GetCurrentUserAsync(userId, cancellationToken);

            if (userInfo == null)
                return NotFound(new { message = "User not found." });

            return Ok(userInfo);
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized(new { message = "Invalid user token." });

            var result = await _accountService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword, cancellationToken);

            if (!result)
                return BadRequest(new { message = "Current password is incorrect or user not found." });

            return Ok(new { message = "Password changed successfully. All existing sessions have been revoked." });
        }
    }
}
