namespace GameComponents;
    
public class Grid
{
    public const int Rows = 6;
    public const int Columns = 7;
    
    public static Dictionary<int, ConsoleColor> PlayerColorsDict = new()
    {
        {1, ConsoleColor.DarkYellow},
        {-1, ConsoleColor.DarkRed}
    };

    public Token?[,] Tokens { get; }

    public Grid()
    {
        this.Tokens = new Token[Rows, Columns];
    }

    private Grid(Token?[,] tokens)
    {
        this.Tokens = tokens;
    }

    public int[] InsertToken(int column, Token token)
    {
        if (this.Tokens[0, column] != null)
        {
            throw new ArgumentException("Tried inserting a token in a full column");
        }

        for (int i = Rows - 1; i >= 0; i--)
        {
            if (this.Tokens[i, column] == null)
            {
                this.Tokens[i, column] = token;
                return new[] {i, column};
            }
        }
        return Array.Empty<int>();
    }

    public int TokenCount()
    {
        int tokens = 0;

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                if (this.Tokens[i, j] != null)
                {
                    tokens++;
                }
            }
        }

        return tokens;
    }

    public Token? WinnerToken()
    {
        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                var token = Tokens[i, j];
                if (token != null)
                {
                    var winnerToken = this.CheckWin(i, j);
                    if (winnerToken != null) return winnerToken;
                }
            }
        }
        return null;
    }

    public Token? CheckWin(int i, int j)
    {
        int counterTop = 0;
        int counterBottom = 0;
        int counterRight = 0;
        int counterLeft = 0;

        int counterTopLeft = 0;
        int counterTopRight = 0;
        int counterBottomLeft = 0;
        int counterBottomRight = 0;
        
        // False if a counter have been interrupted
        bool[] countersAbility = new bool[] {true, true, true, true, true, true, true, true};

        for (int k = 1; k <= 3; k++)
        {
            if (i + k < Rows && this.Tokens[i, j]?.Color == this.Tokens[i + k, j]?.Color && countersAbility[0])
            {
                counterBottom++;
                if (counterBottom == 3 || counterTop + counterBottom == 3) return this.Tokens[i, j];
            }
            else countersAbility[0] = false;

            if (j + k < Columns && this.Tokens[i, j]?.Color == this.Tokens[i, j + k]?.Color && countersAbility[1])
            {
                counterRight++;
                if (counterRight == 3 || counterRight + counterLeft == 3) return this.Tokens[i, j];
            }
            else countersAbility[1] = false;

            if (i - k >= 0 && this.Tokens[i, j]?.Color == this.Tokens[i - k, j]?.Color && countersAbility[2])
            {
                counterTop++;
                if (counterTop == 3 || counterTop + counterBottom == 3) return this.Tokens[i, j];
            }
            else countersAbility[2] = false;

            if (j - k >= 0 && this.Tokens[i, j]?.Color == this.Tokens[i, j - k]?.Color && countersAbility[3])
            {
                counterLeft++;
                if (counterLeft == 3 || counterLeft + counterRight == 3) return this.Tokens[i, j];
            }
            else countersAbility[3] = false;

            if (i + k < Rows && j + k < Columns &&
                this.Tokens[i, j]?.Color == this.Tokens[i + k, j + k]?.Color && countersAbility[4])
            {
                counterBottomRight++;
                if (counterBottomRight == 3 || counterTopLeft + counterBottomRight == 3) return this.Tokens[i, j];
            }
            else countersAbility[4] = false;

            if (j - k >= 0 && i + k < Rows &&
                this.Tokens[i, j]?.Color == this.Tokens[i + k, j - k]?.Color && countersAbility[5])
            {
                counterBottomLeft++;
                if (counterBottomLeft == 3 || counterBottomLeft + counterTopRight == 3) return this.Tokens[i, j];
            }
            else countersAbility[5] = false;

            if (j - k >= 0 && i - k >= 0 && this.Tokens[i, j]?.Color == this.Tokens[i - k, j - k]?.Color &&
                countersAbility[6])
            {
                counterTopLeft++;
                if (counterTopLeft == 3 || counterTopLeft + counterBottomRight == 3) return this.Tokens[i, j];
            }
            else countersAbility[6] = false;
            
            if (i - k >= 0 && j + k < Columns &&
                this.Tokens[i, j]?.Color == this.Tokens[i - k, j + k]?.Color && countersAbility[7])
            {
                counterTopRight++;
                if (counterTopRight == 3 || counterTopRight + counterBottomLeft == 3) return this.Tokens[i, j];
            }
            else countersAbility[7] = false;
        }
        return null;
    }

    public bool IsFull()
    {
        for (int j = 0; j < Columns; j++)
        {
            if (this.Tokens[0, j] == null)
            {
                return false;
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

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                Console.Write("║  ");
                if (this.Tokens[i, j] == null)
                {
                    Console.Write(' ');
                }
                else
                {
                    this.Tokens[i, j]?.Print(); 
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
        var newTokens = new Token?[Tokens.GetLength(0), Tokens.GetLength(1)];
        for (int i = 0; i < Tokens.GetLength(0); i++)
        {
            for (int j = 0; j < Tokens.GetLength(1); j++)
            {
                newTokens[i, j] = Tokens[i, j];
            }
        }

        return new Grid(newTokens);
    }

    public bool IsSameGridAs(Grid otherGrid)
    {
        if (otherGrid.Tokens.GetLength(0) != Rows ||
            otherGrid.Tokens.GetLength(1) != Columns)
        {
            throw new Exception("The two grids have different dimensions");
        }
        for (int i = 0; i < otherGrid.Tokens.GetLength(0); i++)
        {
            for (int j = 0; j < otherGrid.Tokens.GetLength(1); j++)
            {
                if (otherGrid.Tokens[i, j] != null ^ this.Tokens[i, j] != null) return false;
                if (otherGrid.Tokens[i, j] == null && this.Tokens[i, j] == null) continue;
                if (otherGrid.Tokens[i, j] == this.Tokens[i, j]) continue;
                if (otherGrid.Tokens[i, j]?.Color == this.Tokens[i, j]?.Color) continue;
                if (otherGrid.Tokens[i, j]?.Color != this.Tokens[i, j]?.Color) return false;
            }
        }
        return true;
    }
}

public class Token
{
    public ConsoleColor Color { get; }

    public Token(ConsoleColor color)
    {
        Color = color;
    }

    public void Print()
    {
        var previousForegroundColor = Console.ForegroundColor;
        Console.ForegroundColor = Color;
        Console.Write('⬤');
        Console.ForegroundColor = previousForegroundColor;
    }
}