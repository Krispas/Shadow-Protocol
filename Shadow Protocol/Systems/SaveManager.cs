using System.Text.Json;

namespace Shadow_Protocol.Systems;

public class SaveManager
{
    private string savePath = Path.Combine(AppContext.BaseDirectory, "save.json"); //Zde byla pomoc od AI

    public SaveFile saveFile = new SaveFile();

    public void LoadSaves()
    {
        if (!File.Exists(savePath))
        {
            saveFile = new SaveFile();
            SaveSaves();
            return;
        }

        string json = File.ReadAllText(savePath);
        SaveFile? loaded = JsonSerializer.Deserialize<SaveFile>(
            json,
            new JsonSerializerOptions { IncludeFields = true }
        );
        if (loaded != null)
            saveFile = loaded;
    }

    public void SaveSaves()
    {
        string json = JsonSerializer.Serialize(
            saveFile,
            new JsonSerializerOptions
            {
                IncludeFields = true,
                WriteIndented = true
            }
        );

        File.WriteAllText(savePath, json);
    }
    
    public void SaveGame(GameSave save)
    {
        LoadSaves();

        for (int i = 0; i < saveFile.saves.Count; i++)
        {
            if (saveFile.saves[i].saveId == save.saveId)
            {
                saveFile.saves[i] = save;
                SaveSaves();
                return;
            }
        }

        saveFile.saves.Add(save);
        SaveSaves();
    }
}

public class SaveFile
{
    public List<GameSave> saves = new();
}

public class GameSave
{
    public int saveId;
    public string playerName = "";
    public Agent agent = new();
    public Sniper sniper = new();
    public Operator operator_ = new();
    
    public List<CompletedMissionSave> completedMissions = new();
}

public class Agent
{
    public int level;
}

public class Sniper
{
    public int level;
}

public class Operator
{
    public int level;
}
    

public class CompletedMissionSave
{
    public int missionId;
    public bool completedWithAgent;
    public bool completeWithSniper;
    public bool completeWithOperator;
}