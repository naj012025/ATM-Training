using AtmApi.DTO;
using AtmApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AtmApi.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("Login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        LoginResponse? response = await authService.LoginAsync(request);

        if (response is null)
            return Unauthorized(new { message = "invalid Credentials" });


        return Ok(response);
    }
}
