using System.Collections.Generic;
using System.Threading.Tasks;
using BLL;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SQLitePCL;

namespace WebApp.Pages.Game
{
    public class Load : PageModel
    {
        public static DAL.AppDbContext _context;
        public static Engine Engine;
        
        public Load(AppDbContext context)
        {
            _context = context;
            Engine = new Engine(_context);
        }

        [BindProperty] public Selection Selection { get; set; } = new Selection();
        

        public async Task<ActionResult> OnPost(Selection selection)
        {
            GameState gameStart = new GameState();
            if (ModelState.IsValid)
            {
                foreach (var game in _context.GameStates)
                {
                    if (game.GameName == selection.GameName)
                    {
                        return RedirectToPage("./PlayGame", new {gameId = game.GameStateId});
                    }
                }
            }

            return Page();
        }
    }

    public class GameList
    {
    }
    public class Selection
    {
        public string GameName { get; set; }
    }
}