namespace MahjongSolverApiDotNet8.Entities
{
    public class SolveHandResponse
    {
        public bool IsHandWinning { get; set; }
        public List<string> TilesToWin { get; set; } = new List<string>();

        public string? Message { get; set; }
    }
}
