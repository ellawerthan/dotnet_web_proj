using Microsoft.AspNetCore.Mvc.RazorPages;
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

namespace WebApp.Pages.Game
{
    public class Winner : PlayGameModel
    {
        public string P1name;
        public string P2name;
        public bool Winning_move;
        public Winner(AppDbContext context) : base(context)
        {
        }

        public async Task<ActionResult> OnGet(string p1name, string p2name, bool winning_move)
        {
            P1name = p1name;
            P2name = p2name;
            Winning_move = winning_move;
            return Page();
        }
    }

    class WinnerImpl : Winner
    {
        public WinnerImpl(AppDbContext context) : base(context)
        {
        }
    }
}