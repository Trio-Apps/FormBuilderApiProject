using FormBuilder.Application.Abstractions;
using FormBuilder.Application.Dtos.Auth;
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
    }
}
