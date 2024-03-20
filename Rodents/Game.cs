namespace Rodents;

public class Game
{
    private readonly Dictionary<GridValue, string> _gridValToEmoji = new()
    {
        { GridValue.Empty, Emojis.Empty },
        { GridValue.Rat, Emojis.Rat },
        { GridValue.Cheese, Emojis.Cheese },
        { GridValue.Wall, Emojis.Wall },
        { GridValue.Cat, Emojis.Cat }
    };

    private ConsoleColor _backgroundColor = ConsoleColor.DarkRed;
    private readonly int _rows = 15, _cols = 15;
    private readonly GameState _gameState;

    public Game()
    {
        _gameState = new GameState(_rows, _cols);
    }

    private void console_KeyDown()
    {
        if (_gameState.GameOver)
        {
            return;
        }
        
        if (!Console.KeyAvailable)
        {
            return;
        }
        
        var key = Console.ReadKey().Key;
        
        if (key == ConsoleKey.UpArrow)
        {
            _gameState.ChangeDirection(Direction.Up); 
        }
        if (key == ConsoleKey.DownArrow)
        {
            _gameState.ChangeDirection(Direction.Down);
        }
        if (key == ConsoleKey.LeftArrow)
        {
            _gameState.ChangeDirection(Direction.Left);
        }
        if (key == ConsoleKey.RightArrow)
        {
            _gameState.ChangeDirection(Direction.Right);
        }
    }

    public async Task Start()
    {
        await GameLoop();
    }

    public void StartScreen()
    {
        Console.SetCursorPosition(1, _rows+1);
        Console.BackgroundColor = _backgroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("PRESS ENTER TO START");
        Console.BackgroundColor = ConsoleColor.White;
    }

    private async Task GameLoop()
    {
        int tick = 0;
        while (!_gameState.GameOver)
        {
            Draw();
            console_KeyDown();
            if (_gameState.CheckGameOver())
            {
                break;
            }
            _gameState.Move();
            
            // Make cat move slower than rats
            if (tick % 5 == 0)
            {
                _gameState.MoveCat();
            }
            _gameState.CheckCatCollision();
            await Task.Delay(175);
            tick++;
        }
    }
    public void Draw()
    {
        Console.SetCursorPosition(1, _rows+1);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Score: {GameState.Score}");
        DrawGrid();
    }

    private void DrawGrid()
    {
        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _cols; c++)
            {
                Console.BackgroundColor = _backgroundColor;
                GridValue gridVal = _gameState.Grid[r, c];
                string emoji = _gridValToEmoji[gridVal];
                // emojis take up more space, c+c solves that (don't touch, don't ask)
                Console.SetCursorPosition(c+c, r);
                Console.Write(emoji);
            }
        }
    }
}