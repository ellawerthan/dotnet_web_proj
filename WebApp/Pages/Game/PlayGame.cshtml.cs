using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using BLL;
using DAL;
using Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Game
{
    public class PlayGameModel : PageModel
    {
        private readonly DAL.AppDbContext _context;
        public readonly Engine Engine;
        public int GameId { get; set; }
        
        public PlayGameModel(AppDbContext context)
        {
            _context = context;
            Engine = new Engine(_context);
        }

        public async Task<ActionResult> OnGet(int? gameId, int? col)
        {
            
            
            if (gameId == null) {
                return RedirectToPage("./StartGame");
            }

            GameId = gameId.Value;
            
            var state = _context.GameStates
                .FirstOrDefault(b => b.GameStateId == GameId);

            Engine.RestoreGameStateFromDb(GameId);

            if (col != null)
            {
                var result = Engine.Move(col.Value);
                var board = Engine.GetSerializedGameState();

                if (result == MoveResult.Won)
                {
                    if (state != null) _context.GameStates.Remove(state);
                    _context.SaveChanges();
                    return RedirectToPage("./Winner");
                }
                if (state != null) state.BoardStateJson = board;
                _context.SaveChanges();
            }

            return Page();
        }
    }
}