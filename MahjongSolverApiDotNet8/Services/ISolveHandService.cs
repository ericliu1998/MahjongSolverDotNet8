using MahjongSolverApiDotNet8.Entities;

namespace MahjongSolverApiDotNet8.Services
{
    public interface ISolveHandService
    {
        SolveHandResponse SolveMahjongHand(List<int> tiles);

        bool BackTracking(Dictionary<int, int> dictionary, bool got_pair);
    }
}
