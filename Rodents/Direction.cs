namespace Rodents;

public class Direction
{
    public static readonly Direction Left = new Direction(0, -1);
    public static readonly Direction Right = new Direction(0, 1);
    public static readonly Direction Up = new Direction(-1, 0);
    public static readonly Direction Down = new Direction(1, 0);
    private static readonly Random Rnd = new Random();
    public static readonly Direction Random = new Direction(Rnd.Next(-1, 1), Rnd.Next(-1, 1));
    
    public int RowOffset { get; }
    public int ColOffset { get; }

    private Direction(int rowOffset, int colOffset)
    {
        RowOffset = rowOffset;
        ColOffset = colOffset;
    }
}