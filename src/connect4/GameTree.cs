namespace connect4;

class GameTree
{
    private StateNode _root;

    private GameTree()
    {
        this._root = new StateNode(new Grid());
    }

    private StateNode Root
    {
        get { return _root; }
    }

    public static GameTree GenerateGameTree()
    {
        return new GameTree();
    }
}

class StateNode
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

}