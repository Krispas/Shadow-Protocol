using System.Text.Json;

namespace Shadow_Protocol.Systems;

public class MissionManager
{
    public List<Mission> missions = new();

    public void LoadMissions(string path)
    {
        string jsonContent = File.ReadAllText(path);

        MissionFile? loadedFile = JsonSerializer.Deserialize<MissionFile>(jsonContent, new JsonSerializerOptions {IncludeFields = true});

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
        Mission? foundMission = FindMissionById(missionId);

        if (foundMission == null)
        {
            Console.Clear();
            Console.WriteLine("Misse nenalezena.");
            return;
        }

        GameplayManager gameplayManager = new GameplayManager();
        gameplayManager.StartMission(foundMission);
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
    public string type;
    public string overallObjective = "";

    public List<string> legend = new();
    public List<string> instructions = new();
    public List<Layouts> layouts = new();

    public Position playerStart = new();
    public Position exit = new();
    
    public Target target = new();
    public Documents documents = new();
    public List<Keycard> Keycards = new();
    public List<Door> Doors = new();
    
    public List<Enemy> enemies = new();
    public List<Camera> cameras = new();
}

public class Layouts
{
    public string areaName = "";
    public int areaID;
    public string areaObjective = "";
    public List<string> layout = new();
}

public class Position
{
    public int x;
    public int y;
}

public class Enemy
{
    public string type;
    public string name;
    public bool isAlive = true;
    
    public Position position;

    public int hp;
    public int attack;
    
    public string direction;
    public int range;
}
public class Camera
{
    public Position position;

    public string direction;
    public int range;
}

public class Target
{
    public bool isEliminated = false;
    public string name;
    public Position position;
}

public class Documents
{
    public bool hasDocuments = false;
    public Position position;
}
public class Keycard
{
    public bool hasKeycard = false;
    public string color;
    public Position position;
}

public class Door
{
    public Position position;
    public string color;
}