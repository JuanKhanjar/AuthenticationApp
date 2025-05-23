using AuthenticationApp.Application.DTOs;
using AuthenticationApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AuthenticationApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// إنشاء حساب جديد
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("بيانات غير صالحة");

            var result = await _authService.RegisterAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// تسجيل الدخول
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }

        /// <summary>
        /// تجديد التوكن (مستقبلاً)
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromQuery] string token, [FromQuery] string refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(token, refreshToken);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        /// <summary>
        /// تأكيد البريد الإلكتروني بعد التسجيل
        /// </summary>
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return BadRequest("المعرف أو التوكن غير صالح");

            var result = await _authService.ConfirmEmailAsync(userId, WebUtility.UrlDecode(token));

            return result
                ? Ok("تم تأكيد البريد بنجاح ✅")
                : BadRequest("الرابط غير صالح أو انتهت صلاحيته ❌");
        }
    }
}
