// Controllers/AuthController.cs
using FormBuilder.API.DTOs;
using FormBuilder.API.DTOs.FormBuilder.API.DTOs;
using FormBuilder.API.Models;
using FormBuilder.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FormBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegister)
        {
            try
            {
                _logger.LogInformation("Register endpoint called for user: {Username}", userRegister.Username);

                if (!ModelState.IsValid)
                {
                    return BadRequest(SingleApiResponse.ErrorResult("Invalid input data"));
                }

                var origin = Request.Headers["Origin"].ToString();
                var result = await _authService.RegisterAsync(userRegister, origin);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Register endpoint for user: {Username}", userRegister.Username);
                return StatusCode(500, SingleApiResponse.ErrorResult("An error occurred during registration"));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            try
            {
                _logger.LogInformation("Login endpoint called for user: {Email}", userLoginDto.email);

                if (!ModelState.IsValid)
                {
                    return BadRequest(SingleApiResponse.ErrorResult("Invalid input data"));
                }

                var result = await _authService.Login(userLoginDto);

                if (result.Success)
                {
                    return Ok(result);
                }

                return Unauthorized(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Login endpoint for user: {Email}", userLoginDto.email);
                return StatusCode(500, SingleApiResponse.ErrorResult("An error occurred during login"));
            }
        }

        //[HttpPost("refresh-token")]
        //public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenRequest)
        //{
        //    try
        //    {
        //        _logger.LogInformation("RefreshToken endpoint called");

        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(SingleApiResponse.ErrorResult("Invalid input data"));
        //        }

        //        var result = await _authService.RefreshTokenAsync(refreshTokenRequest.RefreshToken);

        //        if (result.Success)
        //        {
        //            return Ok(result);
        //        }

        //        return Unauthorized(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error in RefreshToken endpoint");
        //        return StatusCode(500, SingleApiResponse.ErrorResult("An error occurred while refreshing token"));
        //    }
        //}

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                _logger.LogInformation("Logout endpoint called");

                // Get user ID from claims
                var userIdClaim = User.FindFirst("uid")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(SingleApiResponse.ErrorResult("Invalid token"));
                }

                var result = await _authService.LogoutAsync(userId);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Logout endpoint");
                return StatusCode(500, SingleApiResponse.ErrorResult("An error occurred during logout"));
            }
        }

        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            try
            {
                _logger.LogInformation("GetCurrentUser endpoint called");

                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized(SingleApiResponse.ErrorResult("User not authenticated"));
                }

                var userInfo = new UserInfoDto
                {
                    UserId = int.Parse(User.FindFirst("uid")?.Value ?? "0"),
                    Username = User.FindFirst(ClaimTypes.Name)?.Value ?? "",
                    Email = User.FindFirst(ClaimTypes.Email)?.Value ?? "",
                    Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
                };

                return Ok(SingleApiResponse.SuccessResult(userInfo, "User information retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCurrentUser endpoint");
                return StatusCode(500, SingleApiResponse.ErrorResult("An error occurred while retrieving user information"));
            }
        }
    }
}