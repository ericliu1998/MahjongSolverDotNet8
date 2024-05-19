using MahjongSolverApiDotNet8.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MahjongSolverApiDotNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : Controller
    {
        [HttpGet]
        public async Task<ActionResult<Health>> GetHealth()
        {
            var health = new Health()
            {
                Status = "Ok",
                Time = DateTime.Now,
            };

            return Ok(health);
        }
    }
}
