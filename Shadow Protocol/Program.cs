using Shadow_Protocol;
using Shadow_Protocol.Systems;

class Program
{
    static void Main()
    {
        SaveManager saveManager = new SaveManager();
        GameManager gameManager = new GameManager();
        MenuSystems menuSystems = new MenuSystems();
        MissionManager missionManager = new MissionManager();

        string path = Path.Combine(AppContext.BaseDirectory, "missions.json"); 
        missionManager.LoadMissions(path);

        bool running = true;

        while (running)
        {
            int choice = menuSystems.mainMenu();

            switch (choice)
            {
                case 0:
                    gameManager.CreateNewGame();
                    break;

                case 1:
                    int selectedSaveIndex = menuSystems.SaveSelectMenu(saveManager);

                    if (selectedSaveIndex != -1)
                    {
                        GameSave loadedSave = saveManager.activeSaveFile.saves[selectedSaveIndex];

                        gameManager.currentSave = loadedSave;

                        Console.WriteLine($"Nacten save s hracem: {loadedSave.playerName}");
                        Console.ReadKey(true);
                    }

                    break;


                case 2:
                    missionManager.StartMissionById(0);
                    break;

                case 3:
                    running = false;
                    break;
            }
        }
    }
}