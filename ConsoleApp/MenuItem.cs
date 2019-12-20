using System;

namespace ConsoleApp
{
    public class MenuItem
        {
            private string _command = "";
            private string _title = "";
        
            public string Title
            {
                get => _title;
                set => _title = Validate(value, 1, 100, false);
            }

            public string Command
            {
                get => _command;
                set => _command = Validate(value, 1, 10, true);
            }

            // reference to an method, which returns a string and takes no parameters
            // func<...parameter types..., return type>
            public Func<string>? CommandToExecute { get; set; }
        
            private static string Validate(string item, int minLength, int maxLength, bool toUpper)
            {
                item = item.Trim();
                if (toUpper)
                {
                    item = item.ToUpper();
                }
                if (item.Length < minLength  || item.Length > maxLength)
                {
                    throw new ArgumentException(
                        $"String is not correct length (" +
                        $"{minLength}-{maxLength})! Got " +
                        $"{item.Length} characters.");
                }

                return item;
            }

            public override string ToString()
            {
                return Command + " " + Title;
            }
        }
    }