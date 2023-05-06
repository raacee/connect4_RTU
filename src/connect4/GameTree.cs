namespace connect4;

class GameTree
{
    private StateNode _root;

    public StateNode Root
    {
        get { return _root; }
    }
}

class StateNode
{
    private Grid _grid;
    private List<StateNode> _children = new List(0);
    public Grid Grid
    {
        get { return _grid; }
    }
    public List<StateNode> Children
    {
        get { return _children; }
    }

    public void populateChildren()
    {
        
    }

}