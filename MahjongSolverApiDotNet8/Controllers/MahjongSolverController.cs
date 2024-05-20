using System.Diagnostics;
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
            var timer = new Stopwatch();
            timer.Start();

            Console.WriteLine("solvehand");
            var response = _solveHand.SolveMahjongHand(request.Tiles);
            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            response.TimeElasped = timeTaken.ToString(@"m\:ss\.fff");
            return Ok(response);
        }
    }
}
