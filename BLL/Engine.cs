using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Versioning;
using DAL;
using Domain;
using Newtonsoft.Json;

namespace BLL
{
    public class Engine
    {
/*
        private bool _playerZeroMove;
*/
        private static CellState[,] Board { get; set; }

        private readonly AppDbContext _ctx;

        public int Width { get; private set; }
        public int Height { get; private set; }
        
        public Engine(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public void InitializeNewGame(int height, int width)
        {
            Board = new CellState[height, width];
        }

        public string GetSerializedGameState()
        {
            return JsonConvert.SerializeObject(Board);
        }



        public CellState GetBoardCellValue(int y, int x)
        {
            return Board[y, x];
        }

        public MoveResult Move(int posX, bool blueMove)
        {
            if (posX >= Width)
            {
                return MoveResult.Fail;
            }
            var posY = 0;
            if (Board[posY, posX] != CellState.Empty)
            {
                return MoveResult.Fail;
            }

            while (posY + 1 < Height && Board[posY + 1, posX] == CellState.Empty)
            {
                posY++;
            }

            Board[posY, posX] = blueMove ? CellState.R : CellState.B;

            if (CheckFull())
            {
                return MoveResult.Full;
            }

            if (CheckWin())
            {
                return MoveResult.Won;
            }
            return MoveResult.Success;
        }


        public void RestoreGameStateFromDb(int gameId)
        {
            var state = _ctx.GameStates.First(a => a.GameStateId == gameId);
            Board = JsonConvert.DeserializeObject<CellState[,]>(state.BoardStateJson);
            Height = state.Height;
            Width = state.Width;
        }
        
        private bool CheckWin()
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    if (Board[y, x] == CellState.Empty) continue;
                    if ((x - 1 >= 0 && Board[y, x - 1] == Board[y, x]) ||
                        (x + 1 < Width && Board[y, x + 1] == Board[y, x]))
                    {
                        var connectCount = 1;
                        var tempX = x - 1;
                        while (tempX >= 0 && Board[y, tempX] == Board[y, x])
                        {
                            connectCount++;
                            tempX--;
                        }

                        tempX = x + 1;
                        while (tempX < Width && Board[y, tempX] == Board[y, x])
                        {
                            connectCount++;
                            tempX++;
                        }
                        if (connectCount >= 4)
                        {
                            return true;
                        }
                    }

                    if ((y - 1 >= 0 && Board[y - 1, x] == Board[y, x]) ||
                        (y + 1 < Height && Board[y + 1, x] == Board[y, x]))
                    {
                        var connectCount = 1;
                        var tempY = y - 1;
                        while (tempY >= 0 && Board[tempY, x] == Board[y, x])
                        {
                            connectCount++;
                            tempY--;
                        }

                        tempY = y + 1;
                        while (tempY < Height && Board[tempY, x] == Board[y, x])
                        {
                            connectCount++;
                            tempY++;
                        }
                        if (connectCount >= 4)
                        {
                            return true;
                        }
                    }

                    if ((y - 1 < 0 || x - 1 < 0 || Board[y - 1, x - 1] != Board[y, x]) &&
                        (y + 1 >= Height || x + 1 >= Width || Board[y + 1, x + 1] != Board[y, x]))
                        continue;
                    {
                        var connectCount = 1;
                        var tempY = y - 1;
                        var tempX = x - 1;
                        while (tempY >= 0 && tempX >= 0 && Board[tempY, tempX] == Board[y, x])
                        {
                            connectCount++;
                            tempY--;
                            tempX--;
                        }

                        tempY = y + 1;
                        tempX = x + 1;
                        while (tempY < Height && tempX < Width && Board[tempY, tempX] == Board[y, x])
                        {
                            connectCount++;
                            tempY++;
                            tempX++;
                        }

                        if (connectCount >= 4)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        
        private bool CheckFull()
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    if (Board[y, x] == CellState.Empty)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        
        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

    }

    public enum CellState
    {
        Empty,
        R,
        B
    }
    public enum MoveResult
    {
        Full,
        Won,
        Success,
        Fail
    }
}