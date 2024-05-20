using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MahjongSolverApiDotNet8.Entities;

namespace MahjongSolverApiDotNet8.Services
{
    public class SolveHandService : ISolveHandService
    {
        public SolveHandResponse SolveMahjongHand(List<int> tiles)
        {
            if (tiles.Count > 14 | tiles.Count <= 2)
            {
                return new SolveHandResponse
                {
                    StatusCode = Enums.StatusCodesEnum.IncorrectNumberOfTilesInHand,
                    IsHandWinning = false,
                    Message = "Number of tiles must between 2 and 14: " + tiles.Count.ToString()
                };
            }
            if (tiles.Count % 3 == 0)
            {
                return new SolveHandResponse
                {
                    StatusCode = Enums.StatusCodesEnum.IncorrectNumberOfTilesInHand,
                    IsHandWinning = false,
                    Message = "Incorrect number of tiles: " + tiles.Count.ToString()
                };
            }
            else
            {
                var tilesCounter = new Dictionary<int, int>();
                var hasWong = false;

                foreach (var tile in tiles)
                {
                    if (tile == 49)
                    {
                        hasWong = true;
                    }
                    tilesCounter[tile] = tilesCounter.GetValueOrDefault(tile) + 1;
                }

                if ((tiles.Count % 3) == 2)
                {
                    foreach (var item in tilesCounter)
                    {
                        if (item.Value > 4)
                        {
                            return new SolveHandResponse
                            {
                                StatusCode = Enums.StatusCodesEnum.IncorrectNumberOfCertainTile,
                                IsHandWinning = false,
                                Message = "Certain tile count is greater than 4: " + item.Key
                            };
                        }
                    }
                    return new SolveHandResponse
                    {
                        StatusCode = Enums.StatusCodesEnum.SuccessIsWinning,
                        IsHandWinning = BackTracking(tilesCounter, false),
                    };
                }
                else // tilesAmount % 3 == 1
                {

                    HashSet<int> visited = [];
                    List<int> winningTiles = [];
                    foreach (var tile in tiles)
                    {
                        // Only non honor tiles
                        if (tile <= 39)
                        {
                            if (!visited.Contains(tile - 1) &
                                ((tile % 10) - 1) >= 1 &
                                ((tile % 10) - 1) <= 9)
                            {
                                visited.Add(tile - 1);
                                if (tilesCounter.TryGetValue(tile - 1, out var value))
                                {
                                    if (value < 4)
                                    {
                                        tilesCounter[tile - 1] += 1;
                                        //Console.WriteLine(BackTracking(new Dictionary<int, int>(tilesCounter), false));
                                        if (BackTracking(new Dictionary<int, int>(tilesCounter), false))
                                        {
                                            winningTiles.Add((tile - 1));
                                            //winningTiles.Add(string.Join(",", tilesCounter.Values.ToArray()));

                                        }
                                        tilesCounter[tile - 1] -= 1;

                                    }

                                }
                            }

                            if (!visited.Contains(tile + 1) &
                                ((tile % 10) + 1) >= 1 &
                                ((tile % 10) + 1) <= 9)
                            {
                                visited.Add(tile + 1);
                                if (tilesCounter.TryGetValue(tile + 1, out var value))
                                {
                                    if (value < 4)
                                    {
                                        tilesCounter[tile + 1] += 1;
                                        //Console.WriteLine(BackTracking(new Dictionary<int, int>(tilesCounter), false));
                                        if (BackTracking(new Dictionary<int, int>(tilesCounter), false))
                                        {
                                            winningTiles.Add((tile + 1));
                                            //winningTiles.Add(string.Join(",", tilesCounter.Values.ToArray()));
                                            // add the current tile
                                        }
                                        tilesCounter[tile + 1] -= 1;

                                    }
                                }

                            }
                        }

                        if (!visited.Contains(tile) &
                            (tile % 10) >= 1 &
                            (tile % 10) <= 9)
                        {
                            visited.Add(tile);

                            if (tilesCounter[tile] < 4)
                            {
                                tilesCounter[tile] += 1;
                                //Console.WriteLine(BackTracking(new Dictionary<int, int>(tilesCounter), false));
                                if (BackTracking(new Dictionary<int, int>(tilesCounter), false))
                                {

                                    winningTiles.Add(tile);
                                    //winningTiles.Add()

                                }

                                tilesCounter[tile] -= 1;

                            }
                        }
                    }

                    return new SolveHandResponse
                    {
                        StatusCode = Enums.StatusCodesEnum.SuccessWithTiles,
                        IsHandWinning = winningTiles.Count > 0,
                        TilesToWin = winningTiles,
                    };
                }

            }
        }

        public bool BackTracking(Dictionary<int, int> dictionary, bool got_pair)
        {
            if (dictionary.Sum(x => x.Value) == 0)
            {
                return got_pair;
            }

            for (int i = 0; i <= 47; i++)
            {
                // Form Triplet
                if (dictionary.GetValueOrDefault(i) >= 3)
                {
                    dictionary[i] -= 3;
                    if (BackTracking(dictionary, got_pair))
                    {
                        return true;
                    }
                    dictionary[i] += 3;
                }

                // Form Pair
                if (dictionary.GetValueOrDefault(i) == 2 & got_pair == false)
                {
                    dictionary[i] -= 2;
                    if (BackTracking(dictionary, true))
                    {
                        return true;
                    }
                    dictionary[i] += 2;
                }

                // Form consecutive set for non honor tiles
                if (dictionary.GetValueOrDefault(i) > 0 & i < 40)
                {
                    if (
                        dictionary.GetValueOrDefault(i + 1) >= 1 &
                        (i % 10) + 1 <= 9 &
                        dictionary.GetValueOrDefault(i + 2) >= 1 &
                        (i % 10) + 2 <= 9
                        )
                    {
                        dictionary[i] -= 1;
                        dictionary[i + 1] -= 1;
                        dictionary[i + 2] -= 1;

                        if (BackTracking(dictionary, got_pair))
                        {
                            return true;
                        }

                        dictionary[i] += 1;
                        dictionary[i + 1] += 1;
                        dictionary[i + 2] += 1;
                    }
                }
            }

            return false;
        }

        public List<int> FindTilesToWin(List<int> tiles, bool hasWong)
        {
            var ret = new List<int>();

            if (!hasWong)
            {

            }

            return ret;
        }

        //public List<int> FindTilesToWinHelper() { 
        //}
    }
}
