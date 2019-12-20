using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BLL;
using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Newtonsoft.Json;
using WebApp.Pages.Game;

namespace ConsoleApp
{
    public class Program
    
    {

        public static DbContextOptions<AppDbContext> DbOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=/Users/ellawerthan/Downloads/hwdemo-master/WebApp/app.db").Options;
        public static readonly AppDbContext Context = new AppDbContext(DbOptions);
        public static readonly Engine Engine = new Engine(Context);
        public static void Main(string[] args)
        {
           /* using(var ctx = new AppDbContext())
                foreach (var game in ctx.SavedGames)
                {
                    Console.WriteLine(game.GameName);
                }
            */
           Console.Clear();
            
            Console.WriteLine("Hello Game!");

            var gameMenu = new Menu(1)
            {
                Title   = "Start a new game of Connect 4",
                MenuItems = new List<MenuItem>()
                {
                    new MenuItem()
                    {
                        Command = "1",
                        Title = "Human against Human",
                        CommandToExecute = NewHumanGame
                    },
                    new MenuItem()
                    {
                        Command = "2",
                        Title = "Human against Machine",
                        CommandToExecute = NewAIGame
                    }
                }
            };

            var menu0 = new Menu(0)
            {
                Title   = "Connect 4 Main Menu",
                MenuItems = new List<MenuItem>()
                {
                    new MenuItem()
                    {
                        Command = "N",
                        Title = "Start a new game",
                        CommandToExecute = gameMenu.Run
                    },
                    new MenuItem()
                    {
                        Command = "L",
                        Title = "Load a saved game",
                        CommandToExecute = LoadGameSt
                    }
                }
            };
            menu0.Run();
        }

        private static string NewHumanGame()
        {
            NewGame(2);
            return "Game Started";
        }

        private static string NewAIGame()
        {
            NewGame(1);
            return "Game Started";
        }

        private static string LoadGameSt()
        {
            /*CellState[,] board;
            do
            {
                board = LoadGame();
            } while (board.GetUpperBound(0) == -1);
            var rows = board.GetUpperBound(0) - board.GetLowerBound(0) + 1;
            var columns = board.GetUpperBound(1) - board.GetLowerBound(1) + 1;
            var settings = new GameSettings {BoardHeight = rows, BoardWidth = columns};
            if (board.GetUpperBound(0) - board.GetLowerBound(0) == 0)
            {
                return "Cancelled";
            }
            //TestGame(new Game(settings, board));*/
            LoadGame();
            return "Game Started!";
        }

        private static void NewGame(int humanCount)
        {
            //get width
            int width = 0;
            while (width <= 3)
            {
                Console.WriteLine("Give a width for your game (4 or higher): ");
                int.TryParse(Console.ReadLine()?.Trim(), out width);
                if (width <= 3)
                {
                    Console.WriteLine("Too small!");
                }
            }
            
            //get height
            int height = 0;
            while (height <= 3)
            {
                Console.WriteLine("Give a height for your game (4 or higher): ");
                int.TryParse(Console.ReadLine()?.Trim(), out height);
                if (height <= 3)
                {
                    Console.WriteLine("Too small!");
                }
            }
            
            //todo get p1 name
            var p1name = "p1name";
            //todo get p2 name or fill with "Computer" for AI game
            string p2name;
            if (humanCount == 2)
            {
                p2name = "p2name";
            }
            else
            {
                p2name = "computer";
            }
            
            Engine.InitializeNewGame(height,width);
            var gameState = new GameState()
            {
                Width = width,
                Height = height,
                Player1Name = p1name,
                Player2Name = p2name,
                HumanPlayerCount = humanCount,
                BoardStateJson = Engine.GetSerializedGameState()
            };
            PlayGame(gameState);
        }
// todo troubleshoot playgame valid moves in console
        private static void PlayGame(GameState game)
        {
            var defaultPrompt = "Give me column value!";
            string prompt = defaultPrompt;
            MoveResult status;
            do
            {
                Console.Clear();
                Console.WriteLine("Type 'S' to save game");
                Console.WriteLine(game.BoardStateJson);
                ConsoleUI.PrintBoard(game);
                
                var userXint = -1;
                do
                {
                    Console.WriteLine(prompt);
                    Console.Write(">");
                    prompt = defaultPrompt;
                    var userX = Console.ReadLine();
                    
                    if (!int.TryParse(userX, out userXint))
                    {
                        Console.WriteLine($"{userX} is not a number!");
                    }
                    
                } while (userXint <= 0);
                status = Engine.Move(userXint - 1, game.MoveByB);
                if (status == MoveResult.Fail)
                {
                    prompt = "Invalid Move! Please try again:";
                }
            } while (status != MoveResult.Won && status != MoveResult.Full);


            ConsoleUI.PrintBoard(game);
            Console.WriteLine("Game Over!");
        }
//todo troubleshoot the not null issues
        private static void SaveGame(GameState game)
        {
            var saved = false;
            do
            {
                Console.Write("Please enter a name for your game: ");
                var gameName = Console.ReadLine();
                var exists = false;
                foreach (var exisingGame in Context.GameStates)
                {
                    if (exisingGame.GameName == gameName)
                    {
                        exists = true;
                    }
                }
                if (exists)
                {
                    Console.Write("This game already exists!");
                    string? overwrite = "";
                    do
                    {
                        Console.Write(" Would you like to overwrite {0}? Y/N", gameName);
                        overwrite = Console.ReadLine()?.Trim().ToUpper();
                        if (overwrite == "Y")
                        {
                            var oldGame = Context.GameStates.FirstOrDefault(p => p.GameName == gameName);
                            Context.GameStates.Remove(oldGame ?? throw new ArgumentException());
                            Context.GameStates.Add(game);
                            Context.SaveChanges();
                            saved = true;
                            Console.WriteLine("Your Game is Saved!");
                        }
                        else if (overwrite == "N")
                        {
                            break;
                        }
                    } while (overwrite != "Y" && overwrite != "N");
                }
                else
                {
                    Context.GameStates.Add(game);
                    Context.SaveChanges();
                    saved = true;
                    Console.WriteLine("Your Game is Saved!");
                }
            } while (!saved);
        }
        
        public static void LoadGame()
        {
            string gameName = "";
                do
                {
                    var gamesList = new List<string>();
                    var x = 1;
                    foreach (GameState game in Context.GameStates)
                    {
                        gamesList.Add(game.GameName);
                        Console.WriteLine(x + " - " + game.GameName);
                        gamesList[x - 1] = game.GameName;
                        x++;
                    }
                    Console.WriteLine("Please choose a game from the selection above!");
                    int.TryParse(Console.ReadLine()?.Trim(), out var selection);
                    var gamesArray = gamesList.ToArray();
                    if (selection > gamesArray.Length || selection <= 0)
                    {
                        Console.WriteLine("Invalid Choice!");
                    }
                    else
                    {
                        gameName = gamesArray[selection - 1];
                    }
                } while (gameName == "");

                var loadGame = Context.GameStates.FirstOrDefault(p => p.GameName == gameName);
                PlayGame(loadGame);
        }
    }
    
}
