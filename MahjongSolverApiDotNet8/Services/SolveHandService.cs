using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MahjongSolverApiDotNet8.Entities;
using MahjongSolverApiDotNet8.Enums;
using Newtonsoft.Json;

namespace MahjongSolverApiDotNet8.Services
{
    public class SolveHandService : ISolveHandService
    {
        public SolveHandResponse SolveMahjongHand(List<int> tiles)
        {
            // invalid number of cards
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

            var tilesCounter = new Dictionary<int, int>();
            var hasWong = false;

            foreach (var tile in tiles)
            {
                if (tile == 49)
                {
                    hasWong = true;
                }
                tilesCounter[tile] = tilesCounter.GetValueOrDefault(tile) + 1;

                if (tilesCounter[tile] > 4)
                {
                    return new SolveHandResponse
                    {
                        StatusCode = Enums.StatusCodesEnum.IncorrectNumberOfCertainTile,
                        IsHandWinning = false,
                        Message = "Certain tile count is greater than 4: " + tile
                    };
                }

            }

            if (hasWong)
            {
                if (tilesCounter[49] == 4)
                {
                    return new SolveHandResponse
                    {
                        StatusCode = Enums.StatusCodesEnum.SuccessIsWinning,
                        IsHandWinning = true,
                        Message = "4 wongs is automatic win"
                    };
                }

            }


            // enough cards to check if hand is winning
            if ((tiles.Count % 3) == 2)
            {

                if (!hasWong)
                {
                    return new SolveHandResponse
                    {
                        StatusCode = Enums.StatusCodesEnum.SuccessIsWinning,
                        IsHandWinning = BackTracking(tilesCounter, false),
                    };
                }
                else
                {
                    tiles.RemoveAll(tile => tile == 49);
                    tilesCounter[49] = 0;
                    var completingTiles = FindTilesToWinNoWong(tiles, tilesCounter);
                    return new SolveHandResponse
                    {
                        StatusCode = Enums.StatusCodesEnum.SuccessIsWinning,
                        IsHandWinning = completingTiles.Count > 0,
                        TilesToWin = completingTiles
                    };
                }

            }
            else // tilesAmount % 3 == 1 or need to find tiles to win
            {
                if (!hasWong)
                {
                    var completingTiles = FindTilesToWinNoWong(tiles, tilesCounter);
                    return new SolveHandResponse
                    {
                        StatusCode = Enums.StatusCodesEnum.SuccessWithTiles,
                        IsHandWinning = completingTiles.Count > 0,
                        TilesToWin = completingTiles
                    };
                }
                else
                {
                    tiles.RemoveAll(tile => tile == 49);
                    var completingTiles = FindTilesToWinWithWong(tiles, tilesCounter);
                    Enums.StatusCodesEnum statusCode;
                    if (completingTiles.Count > 0)
                    {
                        if (completingTiles[0] == 100)
                        {
                            statusCode = Enums.StatusCodesEnum.SuccessWithEveryTile;
                        }
                        else
                        {
                            statusCode = StatusCodesEnum.SuccessWithTiles;
                        }
                    }
                    else
                    {
                        statusCode = StatusCodesEnum.SuccessWithTiles;
                    }

                    return new SolveHandResponse
                    {
                        StatusCode = statusCode,
                        IsHandWinning = completingTiles.Count > 0,
                        TilesToWin = completingTiles.Distinct().ToList()
                    };
                }


            }
        }

        private bool BackTracking(Dictionary<int, int> dictionary, bool got_pair)
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

        private List<int> FindTilesToWinWithWong(List<int> tiles, Dictionary<int, int> tilesCounter)
        {
            var winningTiles = new List<int>();

            var wongAmount = tilesCounter[49];
            tilesCounter[49] = 0;

            if (wongAmount == 1)
            {
                if (BackTracking(tilesCounter, true))
                {
                    Console.WriteLine("any card");
                    return [100];
                }
                var potentialTiles = FindPotentialTilesToWin(tiles, tilesCounter);

                foreach (var potentialTile in potentialTiles)
                {
                    tiles.Add(potentialTile);
                    if (tilesCounter.ContainsKey(potentialTile))
                    {
                        tilesCounter[potentialTile]++;
                    }
                    else
                    {
                        tilesCounter[potentialTile] = 1;
                    }

                    var winningTemp = FindTilesToWinNoWong(tiles, tilesCounter);
                    if (winningTemp.Count > 0) Console.WriteLine(JsonConvert.SerializeObject(winningTemp) + " - " + potentialTile);
                    winningTiles.AddRange(winningTemp);

                    tiles.RemoveAt(tiles.Count - 1);
                    tilesCounter[potentialTile]--;
                }
            }

            if (wongAmount == 2)
            {
                var twoWongCounter = new Dictionary<string, int>();

                var twoWongSet = new HashSet<string>();
                var count = 0;
                var potentialTiles = FindPotentialTilesToWin(tiles, tilesCounter);

                foreach (var potentialTile in potentialTiles)
                {
                    tiles.Add(potentialTile);

                    if (tilesCounter.ContainsKey(potentialTile))
                    {
                        tilesCounter[potentialTile]++;
                    }
                    else
                    {
                        tilesCounter[potentialTile] = 1;
                    }

                    if (BackTracking(tilesCounter, true))
                    {
                        Console.WriteLine("any card");
                        return [100];
                    }


                    var potentialTiles2 = FindPotentialTilesToWin(tiles, tilesCounter);

                    foreach (var potentialTile2 in potentialTiles2)
                    {
                        tiles.Add(potentialTile2);

                        if (tilesCounter.ContainsKey(potentialTile2))
                        {
                            tilesCounter[potentialTile2]++;

                        }
                        else
                        {
                            tilesCounter[potentialTile2] = 1;
                        }

                        List<int> twoWongKeyList = [potentialTile, potentialTile2];
                        twoWongKeyList.Sort();
                        var threeWongKeyString = JsonConvert.SerializeObject(twoWongKeyList);
                        if (twoWongCounter.ContainsKey(threeWongKeyString)) { twoWongCounter[threeWongKeyString]++; }
                        else { twoWongCounter[threeWongKeyString] = 1; }


                        if (!twoWongSet.Contains(threeWongKeyString))
                        {
                            twoWongSet.Add(threeWongKeyString);
                            var winningTemp = FindTilesToWinNoWong(tiles, tilesCounter);
                            count++;
                            if (winningTemp.Count > 0)
                            {
                                Console.WriteLine(JsonConvert.SerializeObject(winningTemp) + " - " + potentialTile + " - " + potentialTile2);
                                winningTiles.AddRange(winningTemp);
                            }
                        }

                        //var winningTemp = FindTilesToWinNoWong(tiles, tilesCounter);
                        //count++;

                        //if (winningTemp.Count > 0)
                        //{
                        //    Console.WriteLine(JsonConvert.SerializeObject(winningTemp) + " - " + potentialTile + " - " + potentialTile2);
                        //    winningTiles.AddRange(winningTemp);
                        //}


                        tiles.RemoveAt(tiles.Count - 1);
                        tilesCounter[potentialTile2]--;
                    }


                    tiles.RemoveAt(tiles.Count - 1);
                    tilesCounter[potentialTile]--;
                }
                Console.WriteLine(count);
            }

            if (wongAmount == 3)
            {
                var threeWongCounter = new Dictionary<string, int>();
                var threeWongSet = new HashSet<string>();
                Console.WriteLine("wong = 3");
                var potentialTiles = FindPotentialTilesToWin(tiles, tilesCounter);
                int count = 0;
                foreach (var potentialTile in potentialTiles)
                {
                    tiles.Add(potentialTile);

                    if (tilesCounter.ContainsKey(potentialTile))
                    {
                        tilesCounter[potentialTile]++;
                    }
                    else
                    {
                        tilesCounter[potentialTile] = 1;
                    }



                    var potentialTiles2 = FindPotentialTilesToWin(tiles, tilesCounter);

                    foreach (var potentialTile2 in potentialTiles2)
                    {
                        tiles.Add(potentialTile2);

                        if (tilesCounter.ContainsKey(potentialTile2))
                        {
                            tilesCounter[potentialTile2]++;

                        }
                        else
                        {
                            tilesCounter[potentialTile2] = 1;
                        }

                        if (BackTracking(tilesCounter, true))
                        {
                            Console.WriteLine("any card");
                            return [100];
                        }

                        var potentialTiles3 = FindPotentialTilesToWin(tiles, tilesCounter);


                        foreach (var potentialTile3 in potentialTiles3)
                        {
                            tiles.Add(potentialTile3);

                            if (tilesCounter.ContainsKey(potentialTile3))
                            {
                                tilesCounter[potentialTile3]++;

                            }
                            else
                            {
                                tilesCounter[potentialTile3] = 1;
                            }

                            List<int> threeWongKeyList = [potentialTile, potentialTile2, potentialTile3];
                            threeWongKeyList.Sort();
                            var threeWongKeyString = JsonConvert.SerializeObject(threeWongKeyList);
                            if (threeWongCounter.ContainsKey(threeWongKeyString)) { threeWongCounter[threeWongKeyString]++; }
                            else { threeWongCounter[threeWongKeyString] = 1; }


                            if (!threeWongSet.Contains(threeWongKeyString))
                            {
                                threeWongSet.Add(threeWongKeyString);
                                var winningTemp = FindTilesToWinNoWong(tiles, tilesCounter);
                                count++;
                                if (winningTemp.Count > 0)
                                {
                                    Console.WriteLine(JsonConvert.SerializeObject(winningTemp) + " - " + potentialTile + " - " + potentialTile2 + " - " + potentialTile3);
                                    winningTiles.AddRange(winningTemp);
                                }
                            }

                            //var winningTemp = FindTilesToWinNoWong(tiles, tilesCounter);
                            //count++;
                            //if (winningTemp.Count > 0)
                            //{
                            //    Console.WriteLine(JsonConvert.SerializeObject(winningTemp) + " - " + potentialTile + " - " + potentialTile2 + " - " + potentialTile3);
                            //    winningTiles.AddRange(winningTemp);
                            //}

                            tiles.RemoveAt(tiles.Count - 1);
                            tilesCounter[potentialTile3]--;

                        }

                        tiles.RemoveAt(tiles.Count - 1);
                        tilesCounter[potentialTile2]--;
                    }


                    tiles.RemoveAt(tiles.Count - 1);
                    tilesCounter[potentialTile]--;
                }

                Console.WriteLine(count);
            }
            winningTiles.Sort();

            return winningTiles;
        }


        private List<int> FindTilesToWinNoWong(List<int> tiles, Dictionary<int, int> tilesCounter)
        {
            var winningTiles = new List<int>();

            var potentialTilesToWin = FindPotentialTilesToWin(tiles, tilesCounter);

            foreach (var tile in potentialTilesToWin)
            {
                if (tilesCounter.ContainsKey(tile))
                {
                    tilesCounter[tile]++;
                }
                else
                {
                    tilesCounter[tile] = 1;
                }

                if (BackTracking(new Dictionary<int, int>(tilesCounter), false))
                {
                    winningTiles.Add(tile);
                }
                tilesCounter[tile] -= 1;
            }
            winningTiles.Sort();

            return winningTiles;
        }

        private List<int> FindPotentialTilesToWin(List<int> tiles, Dictionary<int, int> tilesCounter)
        {
            HashSet<int> visited = [];

            foreach (var tile in tiles)
            {
                // Only non honor tiles
                if (tile <= 39)
                {
                    if (!visited.Contains(tile - 1) &
                        ((tile % 10) - 1) >= 1 &
                        ((tile % 10) - 1) <= 9)
                    {
                        if (tilesCounter.GetValueOrDefault(tile - 1) < 4)
                        {
                            visited.Add(tile - 1);
                        }
                    }

                    if (!visited.Contains(tile + 1) &
                        ((tile % 10) + 1) >= 1 &
                        ((tile % 10) + 1) <= 9)
                    {
                        if (tilesCounter.GetValueOrDefault(tile + 1) < 4)
                        {
                            visited.Add(tile + 1);
                        }

                    }
                }

                if (!visited.Contains(tile) &
                    (tile % 10) >= 1 &
                    (tile % 10) <= 9 &
                    tile <= 47)
                {

                    if (tilesCounter[tile] < 4)
                    {
                        visited.Add(tile);

                    }
                }
            }


            return visited.ToList();
        }
    }
}
