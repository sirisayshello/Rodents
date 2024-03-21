namespace Rodents;

public class GameState
{
    public int Rows { get; }
    public int Cols { get; }
    public GridValue[,] Grid { get; }
    public Direction Dir { get; private set; }
    public static int Score { get; private set; }
    public bool GameOver { get; private set; }

    private readonly LinkedList<Position> _ratPositions = new LinkedList<Position>();
    private readonly List<Position> _catPositions = new List<Position>();
    private readonly Random _random = new Random();

    public GameState(int rows, int cols)
    {
        Rows = rows;
        Cols = cols;
        Grid = new GridValue[rows, cols];
        Dir = Direction.Right;
        Score = 0;
        
        AddWall();
        AddRat();
        AddCheese();
        AddCat();
    }

    private void AddRat()
    {
        int r = Rows / 2;

        for (int c = 1; c <= 3; c++)
        {
            Grid[r, c] = GridValue.Rat;
            _ratPositions.AddFirst(new Position(r, c));
        }
    }
    

    private void AddWall()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                if (r == 0 || c == 0 || r == Rows-1 || c == Cols-1)
                {
                    Grid[r, c] = GridValue.Wall;
                }
                
            }
        }
    }

    private IEnumerable<Position> EmptyPositions()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                if (Grid[r, c] == GridValue.Empty)
                {
                    yield return new Position(r, c);
                }
            }
        }
    }

    private void AddCheese()
    {
        List<Position> empty = new List<Position>(EmptyPositions());

        // In case the board is full, wow
        if (empty.Count == 0)
        {
            return;
        }

        Position pos = empty[_random.Next(empty.Count)];
        Grid[pos.Row, pos.Col] = GridValue.Cheese;
    }
    
    private void AddCat()
    {
        List<Position> empty = new List<Position>(EmptyPositions());

        // In case the board is full, probably not gonna happen
        if (empty.Count == 0)
        {
            return;
        }

        Position pos = empty[_random.Next(empty.Count)];
        _catPositions.Add(pos);
        Grid[pos.Row, pos.Col] = GridValue.Cat;
    }

    public bool CheckGameOver()
    {
        if (_ratPositions.Count > 0)
        {
            return false;
        }
        Console.WriteLine("Game over");
        GameOver = true; 
        return true;
    }
    
    public Position HeadPosition()
    {
        return _ratPositions.First.Value;
    }

    public Position TailPosition()
    {
        return _ratPositions.Last.Value;
    }
    
    private void AddAnotherCat(Position pos)
    {
        Grid[pos.Row, pos.Col] = GridValue.Cat;
    }
    
    private void RemoveOldCat(Position pos)
    {
        Grid[pos.Row, pos.Col] = GridValue.Empty;
    }

    private void AddHead(Position pos)
    {
        _ratPositions.AddFirst(pos);
        Grid[pos.Row, pos.Col] = GridValue.Rat;
    }

    private void RemoveTail()
    {
        Position tail = _ratPositions.Last.Value;
        Grid[tail.Row, tail.Col] = GridValue.Empty;
        _ratPositions.RemoveLast();
    }

    public void ChangeDirection(Direction dir)
    {
        Dir = dir;
    }
    
    private GridValue NextMove(Position newHeadPos)
    {
        if (newHeadPos == TailPosition())
        {
            return GridValue.Empty;
        }
        
        return Grid[newHeadPos.Row, newHeadPos.Col];
    }

    public void MoveCat()
    {
        Position ratPosition = HeadPosition();

        for (int i = 0; i < _catPositions.Count; i++)
        {
            Position catPosition = _catPositions[i];
            Position newCatPosition = catPosition;
            RemoveOldCat(catPosition);
            
            if (ratPosition.Col > catPosition.Col)
            { 
                newCatPosition = catPosition.Translate(Direction.Right);
            }
            else if (ratPosition.Row > catPosition.Row)
            {
                newCatPosition = catPosition.Translate(Direction.Down);
            }
            else if (ratPosition.Col < catPosition.Col)
            {
                newCatPosition = catPosition.Translate(Direction.Left);
            }
            else if (ratPosition.Row < catPosition.Row)
            {
                newCatPosition = catPosition.Translate(Direction.Up);
            }
            
            
            GridValue nextMove = NextMove(newCatPosition);
            if (nextMove == GridValue.Cheese)
            {
                AddCheese();
            }
            // If cat next to cat, make them scatter randomly
            if (nextMove == GridValue.Cat)
            {
                newCatPosition = newCatPosition.Translate(Direction.Random);
            }
            AddAnotherCat(newCatPosition);
            _catPositions[i] = newCatPosition;
        }
    }

    public void CheckCatCollision()
    {
        foreach (Position catPosition in _catPositions)
        {
            foreach (Position ratPosition in _ratPositions)
            {
                
                if (ratPosition.Col == catPosition.Col && ratPosition.Row == catPosition.Row)
                {
                    RemoveTail();
                    break;
                }
            }
        }
    }

    public void Move()
    {
        Position newHeadPos = HeadPosition().Translate(Dir);
        GridValue nextMove = NextMove(newHeadPos);
        
        if (nextMove == GridValue.Empty)
        {
            RemoveTail();
            AddHead(newHeadPos);
        }
        
        else if (nextMove == GridValue.Cheese)
        {
            AddHead(newHeadPos);
            Score++;
            AddCheese();
            // Add cat every other cheese, for fun 
            if (Score % 2 == 0)
            {
                AddCat();
            }
        }
    }
}