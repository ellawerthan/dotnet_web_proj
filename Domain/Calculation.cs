using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Calculation
    {
        public int CalculationId { get; set; }

        [MaxLength(128)] [MinLength(2)] public string Description { get; set; } = default!;

        public int Number { get; set; }

        public int Sum { get; set; }
    }
}