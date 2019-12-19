using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Threading.Tasks;
using BLL;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Game
{
    public class StartGameModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public StartGameModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty] public GameOptions GameOptions { get; set; } = new GameOptions();

        public void OnGet()
        {
        }

        public async Task<ActionResult> OnPost()
        {
            // validate newgame
            if (ModelState.IsValid)
            {
                // create an empty game in database

                var humanPlayerCount = 0;
                if (GameOptions.ToDoButton == "Human vs Human")
                {
                    humanPlayerCount = 2;
                }
                if (GameOptions.ToDoButton == "AI vs Human")
                {
                    humanPlayerCount = 1;
                }


                var engine = new Engine(_context);
                
                engine.InitializeNewGame(GameOptions.Height, GameOptions.Width);
                var gameState = new GameState()
                {
                    Width = GameOptions.Width,
                    Height = GameOptions.Height,
                   // WinningConditionSequenceLength = GameOptions.WinningCondition,
                    Player1Name = GameOptions.Player1Name,
                    Player2Name = GameOptions.Player2Name,
                    GameName = GameOptions.GameName,
                    HumanPlayerCount = humanPlayerCount,
                    BoardStateJson = engine.GetSerializedGameState()
                };

                _context.GameStates.Add(gameState);
                await _context.SaveChangesAsync();

                return RedirectToPage("./PlayGame", new {gameId = gameState.GameStateId});
            }

            return Page();
        }
    }

    public class GameOptions
    {
        [MinLength(2)] [MaxLength(32)] public string GameName { get; set; } = default!;
        public int Width { get; set; } = 7;
        public int Height { get; set; } = 7;
        public string ToDoButton { get; set; }
        
        [MinLength(2)] [MaxLength(32)] public string Player1Name { get; set; } = "Player1";
        [MinLength(2)] [MaxLength(32)] public string Player2Name { get; set; } = "Player2";
    }
}