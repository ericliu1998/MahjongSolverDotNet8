using MahjongSolverApiDotNet8.Entities;
using MahjongSolverApiDotNet8.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MahjongSolverApiDotNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MahjongSolverController : ControllerBase
    {
        private readonly ISolveHandService _solveHand;

        public MahjongSolverController(ISolveHandService solveHand)
        {
            _solveHand = solveHand;
        }


        [HttpPost("SolveHand")]
        public async Task<ActionResult<SolveHandResponse>> SolveHand(SolveHandRequest request)
        {
            Console.WriteLine("solvehand");
            var response = _solveHand.SolveMahjongHand(request.Tiles);

            return Ok(response);
        }
    }
}
