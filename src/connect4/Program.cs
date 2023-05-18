using GameComponents;
using GameTree;

namespace connect4;

static class Program
{
    static void Main()
    {
        Console.Clear();
        
        string title = "\n\t\t\t\t\t\t\t\t\t\t\t\t  \n" +
                       "\t\t▄▄▄▄▄▄▄  ▄▄▄▄▄▄▄▄▄ ▄▄    ▄▄  ▄▄    ▄▄  ▄▄▄▄▄▄▄  ▄▄▄▄▄▄▄▄  ▄▄▄▄▄▄▄▄  ▄   ▄▄▄  " + "\t\t  \n" +
                       "\t\t█      █ █       █ █  █  █ █ █  █  █ █ █      █ █       █ █      █ █ █ █   █" + "\t\t  \n" +
                       "\t\t█      █ █   ▄   █ █   █▄█ █ █   █▄█ █ █    ▄▄█ █       █ █▄    ▄█ █ █▄█   █" + "\t\t  \n" +
                       "\t\t█     ▄▄ █  █ █  █ █       █ █       █ █   █▄▄▄ █     ▄▄█   █   █  █       █" + "\t\t  \n" +
                       "\t\t█    █   █  █▄█  █ █  ▄    █ █  ▄    █ █    ▄▄█ █    █      █   █  █▄▄▄    █" + "\t\t  \n" +
                       "\t\t█    █▄▄ █       █ █ █ █   █ █ █ █   █ █   █▄▄▄ █    █▄▄    █   █      █   █" + "\t\t  \n" +
                       "\t\t█▄▄▄▄▄▄█ █▄▄▄▄▄▄▄█ █▄█  █▄▄█ █▄█  █▄▄█ █▄▄▄▄▄▄█ █▄▄▄▄▄▄▄█   █▄▄▄█      █▄▄▄█" + "\t\t  \n";
        
        string playerSelectStr =
            "    Enter 1 to play locally against an other player\tEnter 2 to play against the computer\n\n\t\t\t\t\tEnter Q to leave the game ";

        gameStart:
        
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine(title);
        Console.WriteLine("\t"+playerSelectStr);

        var playerSelectInput = Console.ReadKey();

        int player = 1;
        Dictionary<int, ConsoleColor> playerColorsDict = new Dictionary<int, ConsoleColor>();
        playerColorsDict.Add(1,ConsoleColor.DarkYellow);
        playerColorsDict.Add(-1,ConsoleColor.DarkRed);
        
        var selectCursors = new[]
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

        switch (playerSelectInput.Key)
        {
            case ConsoleKey.D1:
                Grid grid = new Grid();
                Token? winnerToken = null;
                while (winnerToken == null && !grid.IsFull())
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
                        else if (key.Key == ConsoleKey.P)
                        {
                            Console.Clear();    
                            Console.WriteLine("Game is paused. Press P to resume. Press Q to go to main menu.");
                            while(true)
                            {
                                var resumeKey = Console.ReadKey();
                                if (resumeKey.Key == ConsoleKey.P)
                                {
                                    Console.Clear();
                                    goto playerEntry;
                                }
                                else if (resumeKey.Key == ConsoleKey.Q)
                                {
                                    Console.Clear();
                                    goto gameStart;
                                }
                            }
                            
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

                    if (grid.IsFull())
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Game over, the grid is full ! The game is a draw");
                        Console.WriteLine("Press a key to go main menu");
                        Console.ReadLine();
                        Console.Clear();
                        goto gameStart;
                    }

                    player *= -1;
                    
                }
                break;

            case ConsoleKey.D2:
                Grid cpuGrid = new Grid();
                GameTree.GameTree gt = new GameTree.GameTree(cpuGrid);
                StateNode currentNode = gt.Root;
                Token? winnerTokenCPU = null;

                while (!cpuGrid.IsFull() && winnerTokenCPU == null)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Gray;
                    cpuGrid.DisplayGrid();

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
                            cpuGrid.DisplayGrid();
                            Console.WriteLine(selectCursors[cursorPosition]);
                            Console.WriteLine("Press an arrow key to select a column to insert a token");
                            Console.WriteLine("Or press Return/Enter to insert a token");
                        }
                        else if (key.Key == ConsoleKey.RightArrow && cursorPosition < 6)
                        {
                            cursorPosition++;
                            Console.Clear();
                            cpuGrid.DisplayGrid();
                            Console.WriteLine(selectCursors[cursorPosition]);
                            Console.WriteLine("Press an arrow key to select a column to insert a token");
                            Console.WriteLine("Or press Return/Enter to insert a token");
                        }
                        else if (key.Key == ConsoleKey.P)
                        {
                            Console.Clear();    
                            Console.WriteLine("Game is paused. Press P to resume");
                            while(true)
                            {
                                var resumeKey = Console.ReadKey();
                                if (resumeKey.Key == ConsoleKey.P)
                                {
                                    Console.Clear();
                                    break;
                                }
                            }
                            
                        }
                        else if (key.Key == ConsoleKey.Enter)
                        {
                            break;
                        }
                    }

                    winnerTokenCPU = cpuGrid.InsertToken(cursorPosition,new Token(playerColorsDict[player]));
                    
                    if (winnerTokenCPU != null)
                    {
                        Console.ForegroundColor = winnerTokenCPU.Color;
                        Console.WriteLine(winnerTokenCPU.Color + " Wins !");
                        Console.WriteLine("Press a key to go main menu");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    }

                    foreach (var childNode in currentNode.Children)
                    {
                        if (childNode == null) continue;
                        if (childNode.SameStateAs(currentNode))
                        {
                            Console.WriteLine("SAME STATE CHILD FOUND");
                            currentNode = childNode;
                            if (!currentNode.EndNode() &&
                                (currentNode.Children.Length == 0 || currentNode.Children == null))
                            {
                                gt.GenerateAllGameStates(currentNode, 5);
                            }
                            break;
                        }
                    }
                    
                    cpuGrid = currentNode.Grid;
                    Console.WriteLine("AFTER PLAYER TURN GRID IS :");
                    cpuGrid.DisplayGrid();
                    
                    Thread.Sleep(1000);
                    
                    var bestNextNode = gt.FindBestMove(currentNode);
                    
                    Console.WriteLine(bestNextNode is null); // TODO : Writes True on first iteration
                    
                    currentNode = bestNextNode;

                    if (!currentNode.EndNode() &&
                        (currentNode.Children.Length == 0 || currentNode.Children == null))
                    {
                        gt.GenerateAllGameStates(currentNode, 5);
                    }
                    
                    cpuGrid = currentNode.Grid;

                    Console.WriteLine("AFTER CPU TURN GRID IS :");

                    winnerTokenCPU = cpuGrid.WinnerToken();
                    if (winnerTokenCPU != null)
                    {
                        Console.ForegroundColor = winnerTokenCPU.Color;
                        Console.WriteLine(winnerTokenCPU.Color.ToString() + " Wins !");
                        Console.WriteLine("Press a key to go main menu");
                        Console.ReadLine();
                        Console.Clear();
                        goto gameStart;
                    }

                    if (cpuGrid.IsFull())
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Game over, the grid is full ! The game is a draw");
                        Console.WriteLine("Press a key to go main menu");
                        Console.ReadLine();
                        Console.Clear();
                        goto gameStart;
                    }

                    
                }
                break;
            
            case ConsoleKey.Q:
                return;
            
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