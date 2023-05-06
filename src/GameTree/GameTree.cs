namespace connect4;

class GameTree
{
    private StateNode _root;

    private GameTree()
    {
        this._root = new StateNode(new Grid());
        this.GenerateAllGameStates(this._root);
    }

    private StateNode Root
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
    
    public void Minimax(StateNode currentStateNode) {
        // defining whether to use the heuristic evaluation function

        //equivalent to "if useHeuristic parameter is true then use heuristic evaluation function ;
        // else just evaluate the numberOfSticks of a child node
        const evaluationFunction = (StateNode stateNode) => stateNode.playerTurn === 0 ? -1 : 1;


        // Base case: If the game is over (no sticks left)
        // or if the bottom node is reached in case of partially available tree
        if (currentStateNode.isLeaf()) {
            return evaluationFunction(currentStateNode)
        }

        // Maximizing player
        if (currentStateNode.playerTurn === 0) {
            let maxEval = -Infinity;
            for (const childNode of currentStateNode.childStateNodes) {
                if (!childNode.evaluation) {
                    childNode.evaluation = this.minimax(childNode, useHeuristic)
                }
                maxEval = Math.max(maxEval, childNode.evaluation);
            }
            currentStateNode.evaluation = maxEval
            return maxEval;
        } else { // Minimizing player
            let minEval = Infinity;
            for (const childNode of currentStateNode.childStateNodes) {
                if (!childNode.evaluation) {
                    childNode.evaluation = this.minimax(childNode, useHeuristic)
                }
                minEval = Math.min(minEval, childNode.evaluation);
            }
            currentStateNode.evaluation = minEval
            return minEval;
        }
    
    
}

public class StateNode
{
    private Grid _grid;
    private List<StateNode> _children;
    private int _player;

    private readonly Dictionary<int, ConsoleColor> _playerDict = new Dictionary<int, ConsoleColor>(
        new KeyValuePair<int, ConsoleColor>[]
        {
            new KeyValuePair<int, ConsoleColor>(1, ConsoleColor.DarkYellow),
            new KeyValuePair<int, ConsoleColor>(-1, ConsoleColor.DarkRed)
        });

    public StateNode(Grid grid)
    {
        this._grid = grid;
        this._children = new List<StateNode>(0);
        this._player = this._grid.TokenCount() % 2 == 0? 1 : -1;
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
        if(!this._grid.IsFull())
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

}