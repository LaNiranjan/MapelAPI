using MapelRestAPI.Entities;
using MapelRestAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MapelRestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserCreationHandler _handler;

        public UsersController(UserCreationHandler handler)
        {
            _handler = handler;
        }

        [HttpPost("bulk-create")]
        public async Task<IActionResult> BulkCreate(List<AppUser> users)
        {
            var result = await _handler.HandleAsync(users);
            return Ok(result);
        }
    }
}
