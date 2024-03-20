// See https://aka.ms/new-console-template for more information

using Rodents;

class Program
{
    static async Task Main()
    {
        var game = new Game();
        game.StartScreen();
        while (true)
        { 
            var key = Console.ReadKey().Key;
            if (key == ConsoleKey.Enter)
            {
                Console.Clear();
                break;
            }
        }
        
        while (true)
        {
            
            await game.Start();
        }
    }
}