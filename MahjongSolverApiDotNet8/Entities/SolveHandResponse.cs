using MahjongSolverApiDotNet8.Enums;

namespace MahjongSolverApiDotNet8.Entities
{
    public class SolveHandResponse
    {
        public required StatusCodesEnum StatusCode { get; set; }
        public bool IsHandWinning { get; set; }
        public List<int> TilesToWin { get; set; } = [];

        public string? Message { get; set; }

        public string? TimeElasped { get; set; }

    }
}
