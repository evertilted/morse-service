using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using morse_service.Database;

namespace morse_service.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger, IDbContextFactory<MSDBContext> contextFactory)
        {
            _logger = logger;
        }
    }
}
