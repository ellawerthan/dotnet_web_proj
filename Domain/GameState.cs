using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class GameState
    {
        public int GameStateId { get; set; }

        public DateTime  LastUpdateDt { get; set; }
        
        public int Width { get; set; } 
        public int Height { get; set; }

        //todo set this up as a B/R thing and make it work.
        public bool MoveByX { get; set; }

        public int HumanPlayerCount { get; set; } 

        [MinLength(2)] [MaxLength(32)] public string Player1Name { get; set; } = default!;
        [MinLength(2)] [MaxLength(32)] public string Player2Name { get; set; } = default!;

        [MinLength(2)] [MaxLength(32)] public string GameName { get; set; } = default!;

        public string BoardStateJson { get; set; } = default!;
    }
}