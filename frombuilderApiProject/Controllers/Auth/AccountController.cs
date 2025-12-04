using FormBuilder.Application.Abstractions;
using FormBuilder.Application.Dtos.Auth;
using FormBuilder.Core.DTOS.Auth;
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
            if (request is null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Username and password are required.");

            var token = await _accountService.LoginAsync(request.Username, request.Password, cancellationToken);

            if (token is null)
                return Unauthorized("Invalid username or password.");

            return Ok(new { Token = token });
        }
    }
}
