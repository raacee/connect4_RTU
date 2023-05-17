using GameComponents;

namespace GameTree;

public class GameTree
{
    private StateNode _root;
    
    public GameTree()
    {
        this._root = new StateNode(new Grid());
        this.GenerateAllGameStates(this._root, 5);
        this.Minimax(this._root);
    }
    
    public GameTree(Grid grid)
    {
        this._root = new StateNode(grid);
        this.GenerateAllGameStates(this._root, 5);
    }

    public StateNode Root
    {
        get { return _root; }
    }

    public void GenerateAllGameStates(StateNode stateNode,int maxDepth = -1, int depth = 0) {
        // checks whether a node has the same state
        if (depth == maxDepth || stateNode.Grid.IsFull())
        {
            return;
        }

        stateNode.PopulateChildren();

        if (!stateNode.IsLeaf()) {
            foreach (var node in stateNode.Children)
            {
                GenerateAllGameStates(node, maxDepth, depth + 1);
            }
        }
    }
    
    private int EvaluationFunction(StateNode stateNode)
    {
        var winnerToken = stateNode.Grid.WinnerToken();
        if (winnerToken != null)
        {
            return int.MaxValue * stateNode.Player;
        }
        if (stateNode.Grid.IsFull())
        {
            return 0;
        }
        return ScoreGrid(stateNode.Grid);
    }

    private int ScoreGrid(Grid grid)
    {
        int totalScore = 0;
        int rows = grid.Tokens.GetLength(0);
        int cols = grid.Tokens.GetLength(1);

        // Check horizontal windows
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols - 3; col++)
            {
                List<Token?> window = new List<Token?>();
                for (int i = 0; i < 4; i++)
                {
                    window.Add(grid.Tokens[row, col + i]);
                }
                totalScore += ScoreWindow(window);
            }
        }

        // Check vertical windows
        for (int row = 0; row < rows - 3; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                List<Token?> window = new List<Token?>();
                for (int i = 0; i < 4; i++)
                {
                    window.Add(grid.Tokens[row + i, col]);
                }
                totalScore += ScoreWindow(window);
            }
        }

        // Check diagonal windows (positive slope)
        for (int row = 0; row < rows - 3; row++)
        {
            for (int col = 0; col < cols - 3; col++)
            {
                List<Token?> window = new List<Token?>();
                for (int i = 0; i < 4; i++)
                {
                    window.Add(grid.Tokens[row + i, col + i]);
                }
                totalScore += ScoreWindow(window);
            }
        }

        // Check diagonal windows (negative slope)
        for (int row = 3; row < rows; row++)
        {
            for (int col = 0; col < cols - 3; col++)
            {
                List<Token?> window = new List<Token?>();
                for (int i = 0; i < 4; i++)
                {
                    window.Add(grid.Tokens[row - i, col + i]);
                }
                totalScore += ScoreWindow(window);
            }
        }

        return totalScore;
    }

    private int ScoreWindow(List<Token?> window)
    {
        int score = 0;
        int aiPiecesCount = window.Count(token => token != null && token.Color == ConsoleColor.DarkYellow);
        int playerPiecesCount = window.Count(token => token != null && token.Color == ConsoleColor.DarkRed);
        int emptyCount = window.Count(token => token == null);

        if (aiPiecesCount == 4)
        {
            score += 100;
        }
        else if (aiPiecesCount == 3 && emptyCount == 1)
        {
            score += 5;
        }
        else if (aiPiecesCount == 2 && emptyCount == 2)
        {
            score += 2;
        }

        if (playerPiecesCount == 4)
        {
            score -= 100;
        }
        else if (playerPiecesCount == 3 && emptyCount == 1)
        {
            score -= 5;
        }
        else if (playerPiecesCount == 2 && emptyCount == 2)
        {
            score -= 2;
        }

        return score;
    }

    private static int NearTokensCount(StateNode stateNode)
    {
        List<int> scores = new List<int>(0);
        var rows = stateNode.Grid.Tokens.GetLength(0);
        var columns = stateNode.Grid.Tokens.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                var token = stateNode.Grid.Tokens[i, j];

                if (token != null)
                {
                    int incrementer = token.Color == ConsoleColor.DarkRed ? -1 : 1;

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
                        if (i + k < rows &&
                            token?.Color == stateNode.Grid.Tokens[i + k, j]?.Color)
                        {
                            counterRight += 2*incrementer;
                        }

                        if (j + k < columns &&
                            token?.Color == stateNode.Grid.Tokens[i, j + k]?.Color)
                        {
                            counterTop += incrementer;
                        }

                        if (i - k >= 0 && token?.Color == stateNode.Grid.Tokens[i - k, j]?.Color)
                        {
                            counterLeft += 2*incrementer;
                        }

                        if (j - k >= 0 && token?.Color == stateNode.Grid.Tokens[i, j - k]?.Color)
                        {
                            counterBottom += incrementer;
                        }

                        if (i + k < rows && j + k < columns &&
                            token?.Color == stateNode.Grid.Tokens[i + k, j + k]?.Color)
                        {
                            counterTopRight += incrementer;
                        }

                        if (j - k >= 0 && i + k < rows &&
                            token?.Color == stateNode.Grid.Tokens[i + k, j - k]?.Color)
                        {
                            counterBottomRight += incrementer;
                        }

                        if (j - k >= 0 && i - k >= 0 &&
                            token?.Color == stateNode.Grid.Tokens[i - k, j - k]?.Color)
                        {
                            counterBottomLeft += incrementer;
                        }

                        if (i - k >= 0 && j + k < columns &&
                            token?.Color == stateNode.Grid.Tokens[i - k, j + k]?.Color)
                        {
                            counterTopLeft += incrementer;
                        }
                    }

                    var sumForToken = 0;
                    var counterArr = new int[]
                    {
                        counterTop,
                        counterBottom,
                        counterRight,
                        counterLeft,
                        counterTopLeft,
                        counterTopRight,
                        counterBottomLeft,
                        counterBottomRight
                    };
                    foreach (var counter in counterArr)
                    {
                        sumForToken += counter;
                    }
                    scores.Add(sumForToken);
                }
            }
        }

        int res = 0;
        foreach (var score in scores)
        {
            res += score;
        }

        return res;
    }
    
    public int Minimax(StateNode currentStateNode) {
        //https://cs.stackexchange.com/questions/13453/trying-to-improve-minimax-heuristic-function-for-connect-four-game-in-js
        //http://www.informatik.uni-trier.de/~fernau/DSL0607/Masterthesis-Viergewinnt.pdf

        if (currentStateNode.IsLeaf())
        {
            return EvaluationFunction(currentStateNode);
        }

        // Maximizing player
        if (currentStateNode.Player == 1)
        {
            int maxEval = int.MinValue;
            foreach (var childNode in currentStateNode.Children) {
                if (childNode.Evaluation == null)
                {
                    childNode.Evaluation = new NodeEvaluation(this.Minimax(childNode));
                }
                maxEval = Math.Max(maxEval, childNode.Evaluation.Value);
            }
            currentStateNode.Evaluation = new NodeEvaluation(maxEval);
            return maxEval;
        } else { // Minimizing player
            var minEval = int.MaxValue;
            foreach (var childNode in currentStateNode.Children) {
                if (childNode.Evaluation == null)
                {
                    childNode.Evaluation = new NodeEvaluation(this.Minimax(childNode));
                }
                minEval = Math.Min(minEval, childNode.Evaluation.Value);
            }
            currentStateNode.Evaluation = new NodeEvaluation(minEval);
            return minEval;
        }
        
    }

    public StateNode? FindBestMove(StateNode node)
    {
        StateNode bestMove = null;
        bool playerIsCPU = node.Player == -1;

        if (playerIsCPU)
        {
            // Minimizing player
            var bestValue = int.MaxValue;
            foreach (var childNode in node.Children)
            {
                if (childNode.Evaluation == null)
                {
                    childNode.Evaluation = new NodeEvaluation(this.Minimax(childNode));
                }
                
                var evaluationValue = childNode.Evaluation.Value;
                if (evaluationValue <= bestValue)
                {
                    bestValue = evaluationValue;
                    bestMove = childNode;
                }
                
            }
        }
        return bestMove;
    }
    
}
public class StateNode
{
    private Grid _grid;
    private List<StateNode> _children;
    private int _player;
    private NodeEvaluation _evaluation;

    public int Player => _player;

    private readonly Dictionary<int, ConsoleColor> _playerDict = new(
        new[]
        {
            new KeyValuePair<int, ConsoleColor>(1, ConsoleColor.DarkYellow),
            new KeyValuePair<int, ConsoleColor>(-1, ConsoleColor.DarkRed)
        }
    );

    public Grid Grid =>  _grid;

    public List<StateNode> Children => _children;

    public NodeEvaluation Evaluation
    {
        get => this._evaluation;
        set => this._evaluation = value;
    }
    
    public StateNode(Grid grid)
    {
        this._grid = grid;
        this._children = new List<StateNode>(0);
        this._player = this._grid.TokenCount() % 2 == 0 ? 1 : -1;
        this._evaluation = this._player == 1 ? new NodeEvaluation(int.MaxValue) : new NodeEvaluation(int.MinValue);
    }

    public void PopulateChildren()
    {
        if (!this._grid.IsFull())
        {
            for (int i = 0; i < _grid.Tokens.GetLength(1); i++)
            {
                var gridCopy = _grid.Clone();
                try
                {
                    gridCopy.InsertToken(i, new Token(_playerDict[this._player]));
                }
                catch (ArgumentException)
                {
                    continue;
                }

                _children.Add(new StateNode(gridCopy));
            }
        }
    }

    public bool IsLeaf()
    {
        return this._grid.IsFull();
    }

    public bool SameStateAs(StateNode otherNode)
    {
        return this.Grid.IsSameGridAs(otherNode.Grid);
    }

    public StateNode GetChild(int column)
    {
        return this.Children[column-1];
    }

    public bool EndNode()
    {
        return this.Grid.IsFull() || this.Grid.WinnerToken() != null;
    }

}
public class NodeEvaluation
{
    public int Value;

    public NodeEvaluation(int value)
    {
        Value = value;
    }
}