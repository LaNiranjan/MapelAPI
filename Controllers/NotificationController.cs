using MapelRestAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MapelRestAPI.Controllers
{
    //[Authorize] // ensure only valid Entra users can access
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly UserInvitaionHandler _handler;

        public NotificationController(UserInvitaionHandler handler)
        {
            _handler = handler;
        }

        [HttpPost("user-invite")]
        public async Task<IActionResult> UserInvitation([FromBody] string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
                return BadRequest("Email address is required.");

            var result = await _handler.HandleAsync(userEmail);
            return Ok(result);
        }
    }
}
