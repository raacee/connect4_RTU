using GameComponents;
using GameTree;

namespace connect4;

static class Program
{
    private static readonly string[] SelectCursors = 
    {
        "   ^                                       ",
        "         ^                                 ",
        "               ^                           ",
        "                     ^                     ",
        "                           ^               ",
        "                                 ^         ",
        "                                       ^   "
    };

    private static void DisplayGridPlayer(Grid grid, int col)
    {
        grid.DisplayGrid();
        Console.WriteLine(SelectCursors[col]);
        Console.WriteLine("Press an arrow key to select a column to insert a token");
        Console.WriteLine("Or press Return/Enter to insert a token");
    }

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

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Gray;

        switch (playerSelectInput.Key)
        {
            case ConsoleKey.D1:
                Grid grid = new Grid();
                Token? winnerToken = null;
                int player = 1;
                
                while (winnerToken == null && !grid.IsFull())
                {
                    Console.Clear();
                    
                    playerEntry:
                    
                    Console.ForegroundColor = ConsoleColor.Gray;
                    grid.DisplayGrid();

                    int cursorPosition = 0;
                    Console.WriteLine(SelectCursors[cursorPosition]);
                    
                    
                    Console.WriteLine("Press an arrow key to select a column to insert a token");
                    Console.WriteLine("Or press Return/Enter to insert a token");
                    while (true)
                    {
                        var key = Console.ReadKey();
                        if (key.Key == ConsoleKey.LeftArrow && cursorPosition > 0)
                        {
                            cursorPosition--;
                            Console.Clear();
                            DisplayGridPlayer(grid, cursorPosition);
                        }
                        else if (key.Key == ConsoleKey.RightArrow && cursorPosition < 6)
                        {
                            cursorPosition++;
                            Console.Clear();
                            DisplayGridPlayer(grid, cursorPosition);
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
                        var newTokenCoord = grid.InsertToken(cursorPosition, new Token(Grid.PlayerColorsDict[player]));
                        winnerToken = grid.CheckWin(newTokenCoord[0], newTokenCoord[1]);
                    }
                    catch (ArgumentException argumentException)
                    {
                        Console.ResetColor();
                        Console.Clear();
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(argumentException.Message);
                        Console.WriteLine("Press a key to retry");
                        Console.ReadKey();
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
                GameTree.GameTree gt = new GameTree.GameTree(new Grid());
                StateNode currentNode = gt.Root;
                winnerToken = null;

                while (!currentNode.Grid.IsFull() && winnerToken == null)
                {
                    playerSelect:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    
                    Console.Clear();
                    int cursorPosition = 0;
                    DisplayGridPlayer(currentNode.Grid, cursorPosition);

                    //Player choosing column
                    while (true)
                    {
                        var key = Console.ReadKey();
                        if (key.Key == ConsoleKey.LeftArrow && cursorPosition > 0)
                        {
                            cursorPosition--;
                        }
                        else if (key.Key == ConsoleKey.RightArrow && cursorPosition < 6)
                        {
                            cursorPosition++;
                        }
                        else if (key.Key == ConsoleKey.P)
                        {
                            Console.Clear();    
                            Console.WriteLine("Game is paused. Press P to resume. Press Q to quit to main menu");
                            while(true)
                            {
                                var resumeKey = Console.ReadKey();
                                if (resumeKey.Key == ConsoleKey.P)
                                {
                                    Console.Clear();
                                    break;
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
                        Console.Clear();
                        DisplayGridPlayer(currentNode.Grid, cursorPosition);
                    }
                    
                    try
                    {
                        var newTokenCoord = currentNode.Grid.InsertToken(cursorPosition, new Token(Grid.PlayerColorsDict[currentNode.Player]));
                        winnerToken = currentNode.Grid.CheckWin(newTokenCoord[0], newTokenCoord[1]);
                        
                        if (winnerToken != null)
                        {
                            Console.Clear();
                            currentNode.Grid.DisplayGrid();
                            Console.ForegroundColor = winnerToken.Color;
                            Console.WriteLine(winnerToken.Color + " Wins !");
                            Console.WriteLine("Press a key to go main menu");
                            Console.ReadKey();
                            Console.Clear();
                            goto gameStart;
                        }

                        currentNode = currentNode.Children[cursorPosition];
                    }
                    
                    catch(ArgumentException argumentException)
                    {
                        if (argumentException.Message == "Tried inserting a token in a full column")
                        {
                            Console.ResetColor();
                            Console.Clear();
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(argumentException.Message);
                            Console.WriteLine("Press a key to retry");
                            Console.ReadKey();
                            Console.ResetColor();
                            Console.Clear();
                            
                            goto playerSelect;
                        }
                        else throw;
                    }
                    
                    currentNode.Grid.DisplayGrid();

                    if (currentNode.IsLeaf() && !currentNode.IsEndNode())
                    {
                        StateNode.GenerateAllGameStates(currentNode, GameTree.GameTree.MaxDepth);
                    }
                    
                    GameTree.GameTree.Minimax(currentNode);
                    var bestMove = GameTree.GameTree.FindBestMove(currentNode);

                    if(bestMove != null)
                    {
                        currentNode = bestMove;
                    }
                    else
                    {
                        throw new SystemException("Best move is null.");
                    }
                    
                    if (currentNode.IsLeaf() && !currentNode.IsEndNode())
                    {
                        StateNode.GenerateAllGameStates(currentNode, GameTree.GameTree.MaxDepth);
                        GameTree.GameTree.Minimax(currentNode);
                    }

                    Console.WriteLine("CPU has played :");

                    winnerToken = currentNode.Grid.WinnerToken();
                    if (winnerToken != null)
                    {
                        currentNode.Grid.DisplayGrid();
                        Console.ForegroundColor = winnerToken.Color;
                        Console.WriteLine(winnerToken.Color.ToString() + " Wins !");
                        Console.WriteLine("Press a key to go main menu");
                        Console.ReadLine();
                        Console.Clear();
                        goto gameStart;
                    }

                    if (currentNode.Grid.IsFull())
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
                Console.ReadKey();
                Console.ResetColor();
                Console.Clear();
                goto gameStart;
        }
    }
}