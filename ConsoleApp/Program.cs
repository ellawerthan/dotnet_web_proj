using System;
using BLL;
using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello HW Demo!");

            var dbOption = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=/Users/akaver/Development/temp/CSharpHWDemoDayTime/WebApp/app.db").Options;
            var ctx = new AppDbContext(dbOption);

            var engine = new Engine(ctx);

            var done = false;
            do
            {
                var descr = GetDescription();
                if (descr != null)
                {
                    var number = GetNumber();
                    if (number != null)
                    {
                        //engine.AddNewCalculation(descr, number.Value);
                    }
                    else
                    {
                        done = true;
                    }
                }
                else
                {
                    done = true;
                }

            } while (!done);  


            foreach (var calculation in ctx.Calculations)
            {
                Console.WriteLine($"{calculation.Description} {calculation.Number} Sum: {calculation.Sum}");
            }
        }


        private static string? GetDescription()
        {
            while (true)
            {
                Console.Write("Description (min 2 chars, empty to exit):");
                var descr = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(descr))
                {
                    return null;
                }

                descr = descr.Trim();

                if (descr.Length < 2)
                {
                    Console.WriteLine("Too short!");
                }
                else
                {
                    return descr;
                }
            }
        }

        private static int? GetNumber()
        {
            while (true)
            {
                Console.Write("Give me a integer (empty to exit):");
                var numstr = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(numstr))
                {
                    return null;
                }

                numstr = numstr.Trim();

                if (int.TryParse(numstr, out var number))
                {
                    return number;
                }

                Console.WriteLine("This isn't a number!");
            }
        }
    }
}