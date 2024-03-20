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
    private readonly LinkedList<Position> _catPositions = new LinkedList<Position>();
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

        for (int c = 1; c <= 2; c++)
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

        // In case the board is full
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

        // In case the board is full
        if (empty.Count == 0)
        {
            return;
        }

        Position pos = empty[_random.Next(empty.Count)];
        //Position pos = new Position(10, 10);
        _catPositions.AddFirst(pos);
        Grid[pos.Row, pos.Col] = GridValue.Cat;
    }

    public bool CheckGameOver()
    {
        if (_ratPositions.Count == 0)
        {
            Console.WriteLine("Game over");
            GameOver = true; 
            return true;
        }
        return false;
    }
    
    public Position CatPosition()
    {
        return _catPositions.First.Value;
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
        _ratPositions.AddFirst(pos);
        Grid[pos.Row, pos.Col] = GridValue.Cat;
    }
    
    private void RemoveOldCat()
    {
        Position oldCat = _catPositions.Last.Value;
        Grid[oldCat.Row, oldCat.Col] = GridValue.Empty;
        _catPositions.RemoveLast();
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
    
    private GridValue WillHit(Position newHeadPos)
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
        Position newCatPosition = CatPosition();
        GridValue hit = WillHit(newCatPosition);
        
        foreach (var catPosition in _catPositions)
        {
            if (ratPosition.Col > catPosition.Col)
            { 
                //newCatPosition.Translate(Direction.Right);
                AddAnotherCat(newCatPosition.Translate(Direction.Right));
            }
            
        }
    }

    public void Move()
    {
        Position newHeadPos = HeadPosition().Translate(Dir);
        GridValue hit = WillHit(newHeadPos);
        
        if (hit == GridValue.Empty)
        {
            RemoveTail();
            AddHead(newHeadPos);
        }
        
        else if (hit == GridValue.Cat)
        {
            RemoveTail();
        }
        
        else if (hit == GridValue.Cheese)
        {
            AddHead(newHeadPos);
            Score++;
            AddCheese();
            AddCat();
        }
    }
}