using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
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
        public bool Win { get; set; }
        
        public PlayGameModel(AppDbContext context)
        {
            _context = context;
            Engine = new Engine(_context);
        }
        public async Task<ActionResult> OnGet(int gameId, int col)
        {
            GameId = gameId;
            var state = _context.GameStates
                .FirstOrDefault(b => b.GameStateId == GameId);

            Engine.RestoreGameStateFromDb(GameId);
            if (Win)
            {
                if (state != null)
                {
                    _context.GameStates.Remove(state);
                    await _context.SaveChangesAsync();
                    Win = true;
                    return RedirectToPage("./Winner", new {state.Player1Name, state.Player2Name, state.MoveByB});;
                }

                _context.SaveChanges();
            }
            else
            {
                var result = Engine.Move(col, state != null && state.MoveByB);
                    var board = Engine.GetSerializedGameState();
                    if (state != null) state.BoardStateJson = board;
                    if (state != null) state.MoveByB = !state.MoveByB;
                    if (state != null && state.HumanPlayerCount == 1)
                    {
                        result = Engine.Move(Engine.RandomNumber(0, state.Width - 1), state.MoveByB);
                        board = Engine.GetSerializedGameState();
                        state.BoardStateJson = board;
                        state.MoveByB = !state.MoveByB;
                        _context.SaveChanges();
                    }
                    _context.SaveChanges();
                    if (result == MoveResult.Won)
                    {
                        Win = true;
                    }
            }

            return Page();
        }
    }
}