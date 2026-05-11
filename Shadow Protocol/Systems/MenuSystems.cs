namespace Shadow_Protocol.Systems;

public class MenuSystems
{
    private string systemUsername = Environment.UserName;
    
    public int mainMenu()
    {
        string[] options =
        {
            "Nova hra",
            "Nacist hru",
            "Tutorial",
            "Ukončit"
        };

        int selectedIndex = 0;
        Console.CursorVisible = false;

        while (true)
        {
            Console.Clear();
            showMainMenu(options, selectedIndex);

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
    
    public int SaveSelectMenu(SaveManager saveManager)
    {
        saveManager.LoadSaves();

        if (saveManager.activeSaveFile.saves.Count == 0)
        {
            Console.Clear();
            Console.WriteLine("Zadne savy neexistuji.");
            Console.ReadKey(true);
            return -1;
        }

        int selectedIndex = 0;
        Console.CursorVisible = false;

        while (true)
        {
            Console.Clear();

            ShowSaveMenu(saveManager, selectedIndex);

            ConsoleKey key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex--;

                    if (selectedIndex < 0)
                        selectedIndex = saveManager.activeSaveFile.saves.Count - 1;

                    break;

                case ConsoleKey.DownArrow:
                    selectedIndex++;

                    if (selectedIndex >= saveManager.activeSaveFile.saves.Count)
                        selectedIndex = 0;

                    break;

                case ConsoleKey.Enter:
                    Console.Clear();
                    return selectedIndex;
            }
        }
    }

    private void showMainMenu(string[] options, int selectedIndex)
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
    private void ShowSaveMenu(SaveManager saveManager, int selectedIndex)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
====================================
            LOAD SAVE
====================================
");
        Console.ResetColor();

        Console.WriteLine("Vyber save:");
        Console.WriteLine();

        for (int i = 0; i < saveManager.activeSaveFile.saves.Count; i++)
        {
            if (i == selectedIndex)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"> {saveManager.activeSaveFile.saves[i].playerName} <");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"  {saveManager.activeSaveFile.saves[i].playerName}");
            }
        }

        Console.ResetColor();
    }

    public int GameMenu(GameManager gameManager, MissionManager missionManager)
    { 
        int selectedIndex = 0;
        Console.CursorVisible = false;

        while (true)
        {
            Console.Clear();
            List<string> levels = new();

            foreach (Mission mission in missionManager.missions)
            {
                CompletedMission? completedMission = null;

                foreach (CompletedMission saveMission in gameManager.currentSave.completedMissions)
                {
                    if (saveMission.missionId == mission.id)
                    {
                        completedMission = saveMission;
                        break;
                    }
                }

                if (completedMission == null)
                {
                    levels.Add($"{mission.id} - Mise nedokoncena");
                }
                else
                {
                    string completedWith = "";

                    if (completedMission.completedWithAgent)
                        completedWith += "Agent, ";

                    if (completedMission.completeWithSniper)
                        completedWith += "Sniper, ";

                    if (completedMission.completeWithOperator)
                        completedWith += "Operator, ";

                    completedWith = completedWith.TrimEnd(',', ' ');

                    levels.Add($"{mission.id} - Mise dokoncena s {completedWith}");
                }
            }

            ShowGameMenu(gameManager, selectedIndex, levels);

            ConsoleKey key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex--;

                    if (selectedIndex < 0)
                        selectedIndex = gameManager.currentSave.completedMissions.Count - 1;

                    break;

                case ConsoleKey.DownArrow:
                    selectedIndex++;

                    if (selectedIndex >= gameManager.currentSave.completedMissions.Count)
                        selectedIndex = 0;

                    break;

                case ConsoleKey.Enter:
                    Console.Clear();
                    return selectedIndex;
            }
        }
    }

    public void ShowGameMenu(GameManager gameManager, int selectedIndex, List<string> levels)
    {
        Console.WriteLine($"Vítej ve hře {gameManager.currentSave.playerName}!");
        for (int i = 0; i < gameManager.currentSave.completedMissions.Count ; i++)
        {
            if (i == selectedIndex)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($">  <");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($" ");
            }
        }
    }
}