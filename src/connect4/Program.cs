namespace connect4;

static class Program
{
    static void Main(string[] args)
    {
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
        
        Console.Clear();

        switch (playerSelectInput)
        {
            case "1":
                Grid grid = new Grid();
                Token? winnerToken = null;
                while (true)
                {
                    playerEntry:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    grid.DisplayGrid();
                    Console.WriteLine("Press a key to select a column to insert a token");
                    string? key = Console.ReadLine();

                    int columnNumber = 0;
                    bool parseSuccessful = int.TryParse(key, out columnNumber);
                    columnNumber--;

                    if (!parseSuccessful || columnNumber >= grid.Tokens.GetLength(1) || columnNumber < 0)
                    {
                        Console.ResetColor();
                        Console.Clear();
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error, invalid number entered");
                        Console.WriteLine("Press a key to continue");
                        Console.ReadLine();
                        Console.Clear();
                        goto playerEntry;
                    }

                    try
                    {
                        winnerToken = grid.InsertToken(columnNumber, new Token(playerColorsDict[player]));
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

                    if (winnerToken != null)
                    {
                        Console.WriteLine(winnerToken.Color.ToString() + " Wins !");
                        Console.WriteLine("Press a key to go main menu");
                        Console.ReadLine();
                        Console.Clear();
                        goto gameStart;
                    }

                    player *= -1;
                    Console.Clear();
                }

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

public class Grid
{
    private Token[,] _tokens;

    public Token[,] Tokens
    {
        get { return this._tokens; }
    }

    public Grid()
    {
        this._tokens = new Token[6, 7];
    }

    public Grid(Token[,] tokens)
    {
        this._tokens = tokens;
    }

    public Token? InsertToken(int column, Token token)
    {
        if (this._tokens[0, column] != null)
        {
            throw new ArgumentException("This column is already full");
        }

        for (int i = this._tokens.GetLength(0) - 1; i >= 0; i--)
        {
            if (this._tokens[i, column] == null)
            {
                this._tokens[i, column] = token;
                return this.CheckWin(i, column);
            }
        }

        return null;
    }

    public int TokenCount()
    {
        int tokens = 0;

        for (int i = 0; i < this._tokens.GetLength(0); i++)
        {
            for (int j = 0; j < this._tokens.GetLength(1); j++)
            {
                if (this._tokens[i, j] != null)
                {
                    tokens++;
                }
            }
        }

        return tokens;
    }

    private Token? CheckWin(int i, int j)
    {
        int counterTop = 0;
        int counterBottom = 0;
        int counterRight = 0;
        int counterLeft = 0;

        int counterTopLeft = 0;
        int counterTopRight = 0;
        int counterBottomLeft = 0;
        int counterBottomRight = 0;

        for (int k = 0; k < 4; k++)
        {
            if (i + k < this._tokens.GetLength(0) && this._tokens[i, j]?.Color == this._tokens[i + k, j]?.Color)
            {
                counterRight++;
                if (counterRight == 4 || counterRight + counterLeft == 5) return this._tokens[i,j];
            }

            if (j + k < this._tokens.GetLength(1) && this._tokens[i, j]?.Color == this._tokens[i, j + k]?.Color)
            {
                counterTop++;
                if (counterTop == 4 || counterTop + counterBottom == 5) return this._tokens[i,j];
            }

            if (i - k >= 0 && this._tokens[i, j]?.Color == this._tokens[i - k, j]?.Color)
            {
                counterLeft++;
                if (counterLeft == 4 || counterRight + counterLeft == 5) return this._tokens[i,j];
            }

            if (j - k >= 0 && this._tokens[i, j]?.Color == this._tokens[i, j - k]?.Color)
            {
                counterBottom++;
                if (counterBottom == 4 || counterTop + counterBottom == 5) return this._tokens[i,j];
            }

            if (i + k < this._tokens.GetLength(0) && j + k < this._tokens.GetLength(1) &&
                this._tokens[i, j]?.Color == this._tokens[i + k, j + k]?.Color)
            {
                counterTopRight++;
                if (counterTopRight == 4 || counterTopRight + counterBottomLeft == 5) return this._tokens[i,j];
            }

            if (j - k >= 0 && i + k < this._tokens.GetLength(0) &&
                this._tokens[i, j]?.Color == this._tokens[i + k, j - k]?.Color)
            {
                counterBottomRight++;
                if (counterBottomRight == 4 || counterBottomRight + counterTopLeft == 5) return this._tokens[i,j];
            }

            if (j - k >= 0 && i - k >= 0 && this._tokens[i, j]?.Color == this._tokens[i - k, j - k]?.Color)
            {
                counterBottomLeft++;
                if (counterBottomLeft == 4 || counterTopRight + counterBottomLeft == 5) return this._tokens[i,j];
            }

            if (i - k >= 0 && j + k < this._tokens.GetLength(1) &&
                this._tokens[i, j]?.Color == this._tokens[i - k, j + k]?.Color)
            {
                counterTopLeft++;
                if (counterTopLeft == 4 || counterBottomRight + counterTopLeft == 5) return this._tokens[i,j];
            }
        }

        return null;
    }

    public bool IsFull()
    {
        for (int i = 0; i < this._tokens.GetLength(0); i++)
        {
            for (int j = 0; j < this._tokens.GetLength(1); j++)
            {
                if (this._tokens[i,j] == null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void DisplayGrid(){
    {
        Console.Write("╔═════");
        for (int i = 0; i < 6; i++)
        {
            Console.Write("╦═════");
        }
        Console.WriteLine("╗");

        for (int i = 0; i < this._tokens.GetLength(0); i++)
        {
            for (int j = 0; j < this._tokens.GetLength(1); j++)
            {
                Console.Write("║  ");
                if (this._tokens[i, j] == null)
                {
                    Console.Write(' ');
                }
                else
                {
                    this._tokens[i, j].Print(); 
                }
                Console.Write("  ");
            }
            Console.WriteLine("║");
            if (i < 5)
            {
                Console.Write("╠═════");
                for (int n = 0; n < 6; n++)
                {
                    Console.Write("╬═════");
                }
                Console.WriteLine("╣");
            }
            else
            {
                Console.Write("╚═════");
                for (int n = 0; n < 6; n++)
                {
                    Console.Write("╩═════");
                }
                Console.WriteLine("╝");
            }
        }
        
       
            
        Console.Write("|  1  ");
        for (int n = 2; n <= 7; n++)
        {
            Console.Write("|  " + n + "  ");
        }
        Console.WriteLine("|");
    }}

    public Grid Clone()
    {
        var newTokens = new Token[_tokens.GetLength(0), _tokens.GetLength(1)];
        for (int i = 0; i < _tokens.GetLength(0); i++)
        {
            for (int j = 0; j < _tokens.GetLength(1); j++)
            {
                newTokens[i, j] = _tokens[i, j];
            }
        }

        return new Grid(newTokens);
    }
}

public class Token
{
    readonly ConsoleColor _color;

    public ConsoleColor Color
    {
        get { return this._color; }
    }

    public Token(ConsoleColor color)
    {
        _color = color;
    }
    
    public void Print()
    {
        Console.ForegroundColor = _color;
        Console.Write('⬤');
        Console.ForegroundColor = ConsoleColor.Gray;
    }
}