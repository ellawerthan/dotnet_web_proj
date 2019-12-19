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
        private bool _playerZeroMove;
        private CellState[,] Board { get; set; }

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

        public MoveResult Move(int posX)
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

            Board[posY, posX] = _playerZeroMove ? CellState.R : CellState.B;

            if (CheckFull())
            {
                return MoveResult.Full;
            }

            if (CheckWin(posY, posX))
            {
                return MoveResult.Won;
            }
            _playerZeroMove = !_playerZeroMove;
            return MoveResult.Success;
        }

        public void RestoreGameStateFromDb(int gameId)
        {
            var state = _ctx.GameStates.First(a => a.GameStateId == gameId);
            Board = JsonConvert.DeserializeObject<CellState[,]>(state.BoardStateJson);
            Height = state.Height;
            Width = state.Width;
        }
        
        private bool CheckWin(int posY, int posX)
        {
            // check diagonally left from last move
            int diagonal_left = 0;
            int tempY = posY;
            int tempX = posX;
            while (tempY >= 0 && tempX >= 0)
            {
                if (Board[posY, posX] == Board[tempY, tempX])
                {
                    diagonal_left++;
                    tempY--;
                    tempX--;
                }
                else
                {
                    break;
                }
            }
            
            // check diagonally right from last move
            int diagonal_right = 0;
            tempY = posY;
            tempX = posX;
            while (tempY >= 0 && tempX < Width)
            {
                if (Board[posY, posX] == Board[tempY, tempX])
                {
                    diagonal_right++;
                    tempY--;
                    tempX++;
                }
                else
                {
                    break;
                }
            }
            
            // check straight down from last move
            int straight_down = 0;
            tempY = posY;
            tempX = posX;
            while (tempY >= 0)
            {
                if (Board[posY, posX] == Board[tempY, tempX])
                {
                    straight_down++;
                    tempY--;
                }
                else
                {
                    break;
                }
            }
            
            return diagonal_left >= 3 || diagonal_right >= 3 || straight_down >= 3;
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