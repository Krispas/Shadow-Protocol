using System.Text.Json;

namespace Shadow_Protocol.Systems;

public class MissionManager
{
    public List<Mission> missions = new();

    public void LoadMissions(string path)
    {
        string jsonContent = File.ReadAllText(path);

        MissionFile? loadedFile = JsonSerializer.Deserialize<MissionFile>(jsonContent, 
            new JsonSerializerOptions
            {IncludeFields = true}
        );

        if (loadedFile == null)
        {
            Console.WriteLine("Chyba v načítání misí.");
            return;
        }

        missions = loadedFile.missions;
    }

    public Mission? FindMissionById(int missionId)
    {
        foreach (Mission mission in missions)
        {
            if (mission.id == missionId)
                return mission;
        }

        return null;
    }

    public void StartMissionById(int missionId)
    {
        Mission? mission = FindMissionById(missionId);

        if (mission == null)
        {
            Console.Clear();
            Console.WriteLine("Misse nenalezena.");
            return;
        }

        GameplayManager gameplayManager = new GameplayManager();
        gameplayManager.StartMission(mission);
    }
}

public class MissionFile
{
    public List<Mission> missions = new();
}

public class Mission
{
    public int id;
    public string name = "";
    public string type = "";
    public string objective = "";

    public List<string> legend = new();
    public List<string> instructions = new();
    public List<string> layout = new();

    public Position playerStart = new();
    public Position exit = new();

    public List<Enemy> enemies = new();
    public List<Item> items = new();
    public List<Camera> cameras = new();
}

public class Position
{
    public int x;
    public int y;
}

public class Enemy
{
    public string type = "";
    public string name = "";

    public int x;
    public int y;

    public int hp;
    public int attack;

    public string direction = "";
    public int detectionRange;

    public bool isTarget;
}

public class Item
{
    public string type = "";
    public string name = "";

    public int x;
    public int y;
}

public class Camera
{
    public int x;
    public int y;

    public string direction = "";
    public int range;
}