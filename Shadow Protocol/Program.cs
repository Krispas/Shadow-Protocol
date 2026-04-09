using Shadow_Protocol;
using Shadow_Protocol.Systems;

class Program
{
    static void Main()
    {
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
                    break;

                case 1:
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