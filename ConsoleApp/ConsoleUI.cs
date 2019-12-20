using System.ComponentModel;
using System.Net.WebSockets;
using BLL;
using Domain;
using Newtonsoft.Json;
using static BLL.Engine;

namespace ConsoleApp
{
    using System;


    public class ConsoleUI
    {
        private static readonly string _boardSeparator = "===================================";
        private static readonly string _verticalSeparator = "|";
        private static readonly string _horizontalSeparator = "-";
        private static readonly string _centerSeparator = "+";
        
        public static void PrintBoard(GameState game)
        {
            CellState[,] board = JsonConvert.DeserializeObject<CellState[,]>(game.BoardStateJson);
            Console.WriteLine(_boardSeparator);
            Console.WriteLine();
            for (int yIndex = 0; yIndex < game.Height; yIndex++)
            {
                var line = "   ";
                line = line + _verticalSeparator;
                for (int xIndex = 0; xIndex < game.Width; xIndex++)
                {
                    
                    line = line + " " + GetSingleState(board[yIndex, xIndex]) + " ";
                    if (xIndex < game.Width)
                    {
                        line = line + _verticalSeparator;
                    }
                }
                
                Console.WriteLine(line);

                if (yIndex < game.Height)
                {
                    line = "   ";
                    line = line + _horizontalSeparator;
                    for (int xIndex = 0; xIndex < game.Width; xIndex++)
                    {
                        line = line + _horizontalSeparator + _horizontalSeparator + _horizontalSeparator;
                        if (xIndex < game.Width - 1)
                        {
                            line = line +_centerSeparator;
                        }
                    }

                    line = line + _horizontalSeparator;
                    Console.WriteLine(line);
                }
            }
            var rowIndex = "   ";
            var xVal = 0;
            while (xVal < game.Width)
            {
                rowIndex = rowIndex + $"  {xVal + 1} ";
                xVal++;
            }
            Console.WriteLine(rowIndex);
            Console.WriteLine();
            Console.WriteLine(_boardSeparator);
        }

        public static string GetSingleState(CellState state)
        {
            switch (state)
            {
                case CellState.Empty:
                    return " ";
                case CellState.R:
                    return "R";
                case CellState.B:
                    return "B";
                default:
                    throw new InvalidEnumArgumentException("Unknown enum option!");
            }
            
        }
    }
}