namespace GameComponents;
    
public class Grid
{
    private Token?[,] _tokens;

    public Token?[,] Tokens
    {
        get { return this._tokens; }
    }

    public Grid()
    {
        this._tokens = new Token[6, 7];
    }

    public Grid(Token?[,] tokens)
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

    public Token? WinnerToken()
    {
        for (var i = 0; i < _tokens.GetLength(0); i++)
        {
            for (var j = 0; j < _tokens.GetLength(1); j++)
            {
                var token = _tokens[i, j];
                if (token != null)
                {
                    var winnerToken = CheckWin(i, j);
                    if (winnerToken != null) return winnerToken;
                }
            }
        }
        return null;
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
                if (counterRight == 4 || counterRight + counterLeft == 5) return this._tokens[i, j];
            }

            if (j + k < this._tokens.GetLength(1) && this._tokens[i, j]?.Color == this._tokens[i, j + k]?.Color)
            {
                counterTop++;
                if (counterTop == 4 || counterTop + counterBottom == 5) return this._tokens[i, j];
            }

            if (i - k >= 0 && this._tokens[i, j]?.Color == this._tokens[i - k, j]?.Color)
            {
                counterLeft++;
                if (counterLeft == 4 || counterRight + counterLeft == 5) return this._tokens[i, j];
            }

            if (j - k >= 0 && this._tokens[i, j]?.Color == this._tokens[i, j - k]?.Color)
            {
                counterBottom++;
                if (counterBottom == 4 || counterTop + counterBottom == 5) return this._tokens[i, j];
            }

            if (i + k < this._tokens.GetLength(0) && j + k < this._tokens.GetLength(1) &&
                this._tokens[i, j]?.Color == this._tokens[i + k, j + k]?.Color)
            {
                counterTopRight++;
                if (counterTopRight == 4 || counterTopRight + counterBottomLeft == 5) return this._tokens[i, j];
            }

            if (j - k >= 0 && i + k < this._tokens.GetLength(0) &&
                this._tokens[i, j]?.Color == this._tokens[i + k, j - k]?.Color)
            {
                counterBottomRight++;
                if (counterBottomRight == 4 || counterBottomRight + counterTopLeft == 5) return this._tokens[i, j];
            }

            if (j - k >= 0 && i - k >= 0 && this._tokens[i, j]?.Color == this._tokens[i - k, j - k]?.Color)
            {
                counterBottomLeft++;
                if (counterBottomLeft == 4 || counterTopRight + counterBottomLeft == 5) return this._tokens[i, j];
            }

            if (i - k >= 0 && j + k < this._tokens.GetLength(1) &&
                this._tokens[i, j]?.Color == this._tokens[i - k, j + k]?.Color)
            {
                counterTopLeft++;
                if (counterTopLeft == 4 || counterBottomRight + counterTopLeft == 5) return this._tokens[i, j];
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
                if (this._tokens[i, j] == null)
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
                    this._tokens[i, j]?.Print(); 
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
        var newTokens = new Token?[_tokens.GetLength(0), _tokens.GetLength(1)];
        for (int i = 0; i < _tokens.GetLength(0); i++)
        {
            for (int j = 0; j < _tokens.GetLength(1); j++)
            {
                newTokens[i, j] = _tokens[i, j];
            }
        }

        return new Grid(newTokens);
    }

    public bool IsSameGridAs(Grid otherGrid)
    {
        for (int i = 0; i < otherGrid.Tokens.GetLength(0); i++)
        {
            for (int j = 0; j < otherGrid.Tokens.GetLength(1); j++)
            {
                if (otherGrid.Tokens[i, j] != null ^ this._tokens[i, j] != null) return false;
                
                if(otherGrid.Tokens[i, j] != null && this._tokens[i, j] != null)
                {
                    if (otherGrid.Tokens[i, j]?.Color != this._tokens[i, j]?.Color) return false;
                }
            }
        }
        return true;
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