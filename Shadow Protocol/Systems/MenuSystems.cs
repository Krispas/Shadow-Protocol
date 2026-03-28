namespace Shadow_Protocol.Systems;

public class MenuSystems
{
    private string systemUsername = Environment.UserName;

    public int MainMenu()
    {
        string[] options =
        {
            "Nova hra",
            "Nacist hru",
            "Tutorial"
        };

        int selectedIndex = 0;
        Console.CursorVisible = false;

        while (true)
        {
            Console.Clear();
            DrawMainMenu(options, selectedIndex);

            ConsoleKey key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex--;
                    if (selectedIndex < 0)
                        selectedIndex = options.Length - 1;
                    break;

                case ConsoleKey.DownArrow:
                    selectedIndex++;
                    if (selectedIndex >= options.Length)
                        selectedIndex = 0;
                    break;

                case ConsoleKey.Enter:
                    Console.Clear();
                    return selectedIndex;
            }
        }
    }

    private void DrawMainMenu(string[] options, int selectedIndex)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
====================================
        SHADOW PROTOCOL
====================================
");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("> ACCESS: GRANTED");
        Console.ResetColor();

        Console.WriteLine($"Welcome, {systemUsername}!");
        Console.WriteLine();
        Console.WriteLine("Pouzij sipky nahoru a dolu, Enter pro potvrzeni.");
        Console.WriteLine();

        for (int i = 0; i < options.Length; i++)
        {
            if (i == selectedIndex)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"> {options[i]} <");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"  {options[i]}");
            }
        }

        Console.ResetColor();
    }
}