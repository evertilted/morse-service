using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using morse_service.Database;
using morse_service.Interfaces.Services;

namespace morse_service.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [Authorize]
        [HttpGet(Name = "Friends")]
        public IActionResult Friends([FromHeader]int userId)
        { 
            return Ok(_userService.GetFriends(userId));
        }
    }
}
