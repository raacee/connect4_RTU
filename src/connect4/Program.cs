using GameComponents;

namespace connect4;

static class Program
{
    static void Main()
    {
        Console.Clear();
        string title = "\n\t\t\t\t\t\t\t\t\t\t\t\t  \n" +
                       "\t\t▄▄▄▄▄▄▄  ▄▄▄▄▄▄▄▄▄  ▄▄    ▄▄  ▄▄    ▄▄  ▄▄▄▄▄▄▄  ▄▄▄▄▄▄▄▄  ▄▄▄▄▄▄▄▄   ▄   ▄▄▄  " + "\t\t  \n" +
                       "\t\t█      █ █       █  █  █  █ █ █  █  █ █ █      █ █       █ █      █  █ █ █   █" + "\t\t  \n" +
                       "\t\t█      █ █   ▄   █  █   █▄█ █ █   █▄█ █ █    ▄▄█ █       █ █▄    ▄█  █ █▄█   █" + "\t\t  \n" +
                       "\t\t█     ▄▄ █  █ █  █  █       █ █       █ █   █▄▄▄ █     ▄▄█  █   █    █       █" + "\t\t  \n" +
                       "\t\t█    █   █  █▄█  █  █  ▄    █ █  ▄    █ █    ▄▄█ █    █     █   █    █▄▄▄    █" + "\t\t  \n" +
                       "\t\t█    █▄▄ █       █  █ █ █   █ █ █ █   █ █   █▄▄▄ █    █▄▄   █   █        █   █" + "\t\t  \n" +
                       "\t\t█▄▄▄▄▄▄█ █▄▄▄▄▄▄▄█  █▄█  █▄▄█ █▄█  █▄▄█ █▄▄▄▄▄▄█ █▄▄▄▄▄▄▄█  █▄▄▄█        █▄▄▄█" + "\t\t  \n";
                     

        gameStart:
        
        Console.ForegroundColor = ConsoleColor.DarkRed;

        Console.WriteLine(title);

        string playerSelectStr =
            "    Enter 1 to play locally against an other player\tEnter 2 to play against the computer\t  ";
        
        Console.WriteLine("\t"+playerSelectStr);

        string? playerSelectInput = Console.ReadLine();

        int player = 1;
        Dictionary<int, ConsoleColor> playerColorsDict = new Dictionary<int, ConsoleColor>();
        playerColorsDict.Add(1,ConsoleColor.DarkYellow);
        playerColorsDict.Add(-1,ConsoleColor.DarkRed);
        var selectCursors = new string[7]
        {
            "   ^                                       ",
            "         ^                                 ",
            "               ^                           ",
            "                     ^                     ",
            "                           ^               ",
            "                                 ^         ",
            "                                       ^   "
        };
        
        Console.Clear();

        switch (playerSelectInput)
        {
            case "1":
                Grid grid = new Grid();
                Token? winnerToken = null;
                while (winnerToken == null)
                {
                    Console.Clear();
                    
                    playerEntry:
                    
                    Console.ForegroundColor = ConsoleColor.Gray;
                    grid.DisplayGrid();

                    int cursorPosition = 0;
                    Console.WriteLine(selectCursors[cursorPosition]);
                    
                    
                    Console.WriteLine("Press an arrow key to select a column to insert a token");
                    Console.WriteLine("Or press Return/Enter to insert a token");
                    while (true)
                    {
                        var key = Console.ReadKey();
                        if (key.Key == ConsoleKey.LeftArrow && cursorPosition > 0)
                        {
                            cursorPosition--;
                            Console.Clear();
                            grid.DisplayGrid();
                            Console.WriteLine(selectCursors[cursorPosition]);
                            Console.WriteLine("Press an arrow key to select a column to insert a token");
                            Console.WriteLine("Or press Return/Enter to insert a token");
                        }
                        else if (key.Key == ConsoleKey.RightArrow && cursorPosition < 6)
                        {
                            cursorPosition++;
                            Console.Clear();
                            grid.DisplayGrid();
                            Console.WriteLine(selectCursors[cursorPosition]);
                            Console.WriteLine("Press an arrow key to select a column to insert a token");
                            Console.WriteLine("Or press Return/Enter to insert a token");
                        }
                        else if (key.Key == ConsoleKey.Enter)
                        {
                            break;
                        }
                    }
                    
                    try
                    {
                        winnerToken = grid.InsertToken(cursorPosition, new Token(playerColorsDict[player]));
                    }
                    catch (Exception e)
                    {
                        Console.ResetColor();
                        Console.Clear();
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(e.Message);
                        Console.ReadLine();
                        Console.ResetColor();
                        Console.Clear();
                        goto playerEntry;
                    }
                    
                    Console.Clear();
                    grid.DisplayGrid();

                    if (winnerToken != null)
                    {
                        Console.ForegroundColor = winnerToken.Color;
                        Console.WriteLine(winnerToken.Color.ToString() + " Wins !");
                        Console.WriteLine("Press a key to go main menu");
                        Console.ReadLine();
                        Console.Clear();
                        goto gameStart;
                    }

                    player *= -1;
                    
                }
                break;

            case "2":
                Console.ResetColor();
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                throw new NotImplementedException();
            default:
                Console.ResetColor();
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error, invalid number entered");
                Console.WriteLine("Press a key to continue");
                Console.ReadLine();
                Console.ResetColor();
                Console.Clear();
                goto gameStart;
        }
    }
}