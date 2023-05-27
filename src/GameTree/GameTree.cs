using GameComponents;

namespace GameTree;

public class GameTree
{
    public StateNode Root { get; }

    public const int MaxDepth = 5;
    
    public GameTree()
    {
        this.Root = new StateNode(new Grid());
        StateNode.GenerateAllGameStates(this.Root, MaxDepth);
        Minimax(this.Root);
    }
    
    public GameTree(Grid grid)
    {
        this.Root = new StateNode(grid);
        StateNode.GenerateAllGameStates(this.Root, MaxDepth);
        Minimax(this.Root);
    }

    public GameTree(StateNode node)
    {
        this.Root = node;
        StateNode.GenerateAllGameStates(this.Root, MaxDepth);
        Minimax(this.Root);
    }

    private static int EvaluationFunction(StateNode stateNode)
    {
        //TODO : implement function to analyze immediate threats
        var stateNodeWinner = stateNode.Grid.WinnerToken();
        if (stateNodeWinner != null)
        {
            return int.MaxValue * stateNode.Player * -1;
        }
        else if (stateNode.Grid.IsFull())
        {
            return 0;
        }
        
        Tuple<int, int, int, int> coeffs = new Tuple<int, int, int, int>(4, 2, 1, 100000);
        int centerControl = CenterControl(stateNode);
        int maxHeight = MaxHeight(stateNode);
        int nearTokensCount = NearTokensCount(stateNode);

        return (centerControl * coeffs.Item1 - maxHeight * coeffs.Item2 + nearTokensCount * coeffs.Item3) * stateNode.Player * -1;
    }
    

    private static int MaxHeight(StateNode stateNode)
    {
        int maxHeight = 0;
        
        for (int j = 0; j < Grid.Columns; j++)
        {
            int columnHeight = 0;
            for (int i = Grid.Rows - 1; i >= 0; i--)
            {
                if (stateNode.Grid.Tokens[i, j] != null)
                {
                    columnHeight = i;
                }
            }

            if (columnHeight > maxHeight)
            {
                maxHeight = columnHeight;
            }
        }

        return maxHeight;

    }

    private static int CenterControl(StateNode node)
    {
        int res = 0;
        for (int i = 0; i < Grid.Rows; i++)
        {
            for (int j = 2; j < 5; j++)
            {
                if (node.Grid.Tokens[i,j]!=null && node.Grid.Tokens[i, j].Color == Grid.PlayerColorsDict[node.Player])
                {
                    res++;
                }
            }
        }
        return res * node.Player;
    }

    private static int NearTokensCount(StateNode stateNode)
    {
        int res = 0;

        for (int i = 0; i < Grid.Rows; i++)
        {
            for (int j = 0; j < Grid.Columns; j++)
            {
                var token = stateNode.Grid.Tokens[i, j];
                bool[] countersAbility = new bool[] {true, true, true, true, true, true, true, true};

                if (token != null)
                {
                    for (int k = 1; k <= 3; k++)
                    {
                        if (i + k < Grid.Rows 
                            && token.Color == stateNode.Grid.Tokens[i + k, j]?.Color
                            && countersAbility[0])
                        {
                            res++;
                        }
                        else if (i + k < Grid.Rows && token.Color != stateNode.Grid.Tokens[i + k, j]?.Color)
                            countersAbility[0] = false;

                        if (j + k < Grid.Columns &&
                            token.Color == stateNode.Grid.Tokens[i, j + k]?.Color &&
                            countersAbility[1])
                        {
                            res++;
                        }
                        else if (j + k < Grid.Columns && token.Color != stateNode.Grid.Tokens[i, j + k]?.Color)
                            countersAbility[1] = false;

                        if (i - k >= 0 &&
                            token.Color == stateNode.Grid.Tokens[i - k, j]?.Color &&
                            countersAbility[2])
                        {
                            res++;
                        }
                        else if (i - k >= 0 && token.Color != stateNode.Grid.Tokens[i - k, j]?.Color)
                            countersAbility[2] = false;

                        if (j - k >= 0 &&
                            token.Color == stateNode.Grid.Tokens[i, j - k]?.Color &&
                            countersAbility[3])
                        {
                            res++;
                        }
                        else if (j - k >= 0 && token.Color != stateNode.Grid.Tokens[i, j - k]?.Color)
                            countersAbility[3] = false;

                        if (i + k < Grid.Rows && j + k < Grid.Columns &&
                            token.Color == stateNode.Grid.Tokens[i + k, j + k]?.Color &&
                            countersAbility[4])
                        {
                            res++;
                        }
                        else if (i + k < Grid.Rows && j + k < Grid.Columns && token.Color != stateNode.Grid.Tokens[i + k, j + k]?.Color)
                            countersAbility[4] = false;

                        if (j - k >= 0 && i + k < Grid.Rows &&
                            token.Color == stateNode.Grid.Tokens[i + k, j - k]?.Color &&
                            countersAbility[5])
                        {
                            res++;
                        }
                        else if (j - k >= 0 && i + k < Grid.Rows && token.Color != stateNode.Grid.Tokens[i + k, j - k]?.Color)
                            countersAbility[5] = false;

                        if (j - k >= 0 && i - k >= 0 && token.Color ==
                            stateNode.Grid.Tokens[i - k, j - k]?.Color &&
                            countersAbility[6])
                        {
                            res++;
                        }
                        else if (j - k >= 0 && i - k >= 0 && token.Color != stateNode.Grid.Tokens[i - k, j - k]?.Color)
                            countersAbility[6] = false;

                        if (i - k >= 0 && j + k < Grid.Columns &&
                            token.Color == stateNode.Grid.Tokens[i - k, j + k]?.Color &&
                            countersAbility[7])
                        {
                            res++;
                        }
                        else if (i - k >= 0 && j + k < Grid.Columns && token.Color != stateNode.Grid.Tokens[i - k, j + k]?.Color)
                            countersAbility[7] = false;
                    }
                }
            }
        }

        return res;
    }
    
    //https://cs.stackexchange.com/questions/13453/trying-to-improve-minimax-heuristic-function-for-connect-four-game-in-js
    //http://www.informatik.uni-trier.de/~fernau/DSL0607/Masterthesis-Viergewinnt.pd
    public static int Minimax(StateNode currentStateNode) {
        
        if (currentStateNode.IsEndNode() || currentStateNode.IsLeaf())
        {
            return EvaluationFunction(currentStateNode);
        }

        // Maximizing player
        if (currentStateNode.Player == 1)
        {
            int maxEval = int.MinValue;
            foreach (var childNode in currentStateNode.Children)
            {
                if (childNode == null) continue;
                if (childNode.Evaluation == null)
                {
                    childNode.Evaluation = new NodeEvaluation(Minimax(childNode));
                }
                maxEval = Math.Max(maxEval, childNode.Evaluation.Value);
            }
            currentStateNode.Evaluation = new NodeEvaluation(maxEval);
            return maxEval;
        } 
        else { // Minimizing player
            var minEval = int.MaxValue;
            foreach (var childNode in currentStateNode.Children)
            {
                if (childNode == null) continue;
                if (childNode.Evaluation == null)
                {
                    childNode.Evaluation = new NodeEvaluation(Minimax(childNode));
                }
                minEval = Math.Min(minEval, childNode.Evaluation.Value);
            }
            currentStateNode.Evaluation = new NodeEvaluation(minEval);
            return minEval;
        }
        
    }

    public StateNode? FindBestMove(StateNode stateNode)
    {
        StateNode? bestMove = null;
        if (!stateNode.IsEndNode())
        {
            bool playerIsCpu = stateNode.Player == -1;

            if (playerIsCpu)
            {
                // Minimizing player
                var bestValue = int.MaxValue;
                foreach (var childNode in stateNode.Children)
                {
                    if (childNode == null) continue;
                    if (childNode.Evaluation == null)
                    {
                        childNode.Evaluation = new NodeEvaluation(Minimax(childNode));
                    }

                    var evaluationValue = childNode.Evaluation.Value;
                    if (evaluationValue < bestValue)
                    {
                        bestValue = evaluationValue;
                        bestMove = childNode;
                    }
                }
                if(bestMove == null){
                    foreach (var child in stateNode.Children)
                    {
                        if (child != null)
                        {
                            return child;
                        }
                    }
                }
            }
        }
        return bestMove;
    }
    
}
public class StateNode
{
    public Grid Grid { get; }
    public int Player { get; }
    public StateNode?[]? Children { get; private set; }
    public NodeEvaluation? Evaluation { get; set; }

    private static readonly Dictionary<int, ConsoleColor> PlayerDict = new(
        new[]
        {
            new KeyValuePair<int, ConsoleColor>(1, ConsoleColor.DarkYellow),
            new KeyValuePair<int, ConsoleColor>(-1, ConsoleColor.DarkRed)
        }
    );

    public StateNode(Grid grid)
    {
        this.Grid = grid;
        this.Children = null;
        this.Player = this.Grid.TokenCount() % 2 == 0 ? 1 : -1;
        this.Evaluation = null;
    }

    private void PopulateChildren()
    {
        this.Children = new StateNode[Grid.Columns];

        for (int i = 0; i < Grid.Columns; i++)
        {
            var gridCopy = Grid.Clone();
            try
            {
                gridCopy.InsertToken(i, new Token(PlayerDict[this.Player]));
            }
            catch (ArgumentException argumentException)
            {
                if (argumentException.Message == "Tried inserting a token in a full column")
                {
                    this.Children[i] = null;
                    continue;
                }
                else throw;
            }

            Children[i] = new StateNode(gridCopy);
        }
    }
    
    public static void GenerateAllGameStates(StateNode stateNode, int maxDepth = -1, int depth = 0) {
        // checks whether a node has the same state
        if (maxDepth >= 0 && depth >= maxDepth)
        {
            return;
        }
        if (!stateNode.IsEndNode()) {
            stateNode.PopulateChildren();
            foreach (var node in stateNode.Children)
            {
                if(node != null)
                {
                    GenerateAllGameStates(node, maxDepth, depth + 1);
                }
            }
        }
        else
        {
            stateNode.Children = null;
        }
    }

        public bool IsLeaf()
    {
        return this.Children == null;
    }

    public bool SameStateAs(StateNode otherNode)
    {
        return this.Grid.IsSameGridAs(otherNode.Grid);
    }

    public bool IsEndNode()
    {
        return this.Grid.IsFull() || this.Grid.WinnerToken() != null;
    }

}

public class NodeEvaluation
{
    public int Value { get; }

    public NodeEvaluation(int value)
    {
        Value = value;
    }
}