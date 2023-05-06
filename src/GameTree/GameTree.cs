using GameComponents;

namespace GameTree;

class GameTree
{
    private StateNode _root;

    private GameTree()
    {
        this._root = new StateNode(new Grid());
        this.GenerateAllGameStates(this._root);
        this.Minimax(this._root);
    }

    public StateNode Root
    {
        get { return _root; }
    }

    public static GameTree GenerateGameTree()
    {
        return new GameTree();
    }
    
    private void GenerateAllGameStates(StateNode stateNode,int maxDepth = -1, int depth = 0) {
        // checks whether a node has the same state
        if (depth == maxDepth || stateNode.Grid.IsFull())
        {
            return;
        }

        stateNode.PopulateChildren();

        if (!stateNode.IsLeaf()) {
            foreach (var node in stateNode.Children)
            {
                this.GenerateAllGameStates(node, maxDepth, depth + 1);
            }
        }
    }
    
    public int Minimax(StateNode currentStateNode) {
        // defining whether to use the heuristic evaluation function

        //equivalent to "if useHeuristic parameter is true then use heuristic evaluation function ;
        // else just evaluate the numberOfSticks of a child node
        Func<StateNode, int> evaluationFunction = delegate(StateNode stateNode)
        {
            var winnerToken = stateNode.Grid.WinnerToken();
            if (winnerToken == null) return 0;
            if (winnerToken.Color == ConsoleColor.DarkYellow) return 1;
            if (winnerToken.Color == ConsoleColor.DarkRed) return -1;
            return 0;
        };


        // Base case: If the game is over (no sticks left)
        // or if the bottom node is reached in case of partially available tree
        if (currentStateNode.IsLeaf())
        {
            return evaluationFunction(currentStateNode);
        }

        // Maximizing player
        if (currentStateNode.Player == 1) {
            int maxEval = int.MinValue;
            foreach (var childNode in currentStateNode.Children) {
                if (childNode.Evaluation == null)
                {
                    childNode.Evaluation.Value = this.Minimax(childNode);
                }
                maxEval = Math.Max(maxEval, childNode.Evaluation.Value);
            }
            currentStateNode.Evaluation.Value = maxEval;
            return maxEval;
        } else { // Minimizing player
            var minEval = int.MaxValue;
            foreach (var childNode in currentStateNode.Children) {
                if (childNode.Evaluation == null)
                {
                    childNode.Evaluation.Value = this.Minimax(childNode);
                }
                minEval = Math.Min(minEval, childNode.Evaluation.Value);
            }
            currentStateNode.Evaluation.Value = minEval;
            return minEval;
        }
    
    
}

    public class StateNode
    {
        private Grid _grid;
        private List<StateNode> _children;
        private int _player;
        public NodeEvaluation Evaluation;

        public int Player => _player;


        private readonly Dictionary<int, ConsoleColor> _playerDict = new Dictionary<int, ConsoleColor>(
            new[]
            {
                new KeyValuePair<int, ConsoleColor>(1, ConsoleColor.DarkYellow),
                new KeyValuePair<int, ConsoleColor>(-1, ConsoleColor.DarkRed)
            });

        public StateNode(Grid grid)
        {
            this._grid = grid;
            this._children = new List<StateNode>(0);
            this._player = this._grid.TokenCount() % 2 == 0 ? 1 : -1;
        }

        public Grid Grid
        {
            get { return _grid; }
        }

        public List<StateNode> Children
        {
            get { return _children; }
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
                        _grid.InsertToken(i, new Token(_playerDict[this._player]));
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
            for (var i0 = 0; i0 < otherNode.Grid.Tokens.GetLength(0); i0++)
            for (var i1 = 0; i1 < otherNode.Grid.Tokens.GetLength(1); i1++)
            {
                if (this._grid.Tokens[i0, i1] != otherNode.Grid.Tokens[i0, i1]) return false;
            }
            return true;
        }

    }
}

class NodeEvaluation
{
    public int Value;

    public NodeEvaluation(int value)
    {
        Value = value;
    }
}