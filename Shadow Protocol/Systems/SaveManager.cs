using System.Text.Json;

namespace Shadow_Protocol.Systems;
public class SaveManager
{
    private string savePath = Path.Combine(
        Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName,
        "save.json"
    ); //Zde byla pomoc od AI abych mohl prepisovat save.json 

    public SaveFile activeSaveFile = new SaveFile();

    public void LoadSaves()
    {
        string json = File.ReadAllText(savePath);
        SaveFile? loaded = JsonSerializer.Deserialize<SaveFile>(
            json,
            new JsonSerializerOptions { IncludeFields = true }
        );
        if (loaded != null)
            activeSaveFile = loaded;
    }

    public void SaveSaves()
    {
        string json = JsonSerializer.Serialize(
            activeSaveFile,
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

        for (int i = 0; i < activeSaveFile.saves.Count; i++)
        {
            if (activeSaveFile.saves[i].saveId == save.saveId)
            {
                activeSaveFile.saves[i] = save;
                SaveSaves();
                return;
            }
        }

        activeSaveFile.saves.Add(save);
        SaveSaves();
    }
    public int GetNextSaveId()
    {
        LoadSaves();

        int highestId = 0;

        foreach (GameSave save in activeSaveFile.saves)
        {
            if (save.saveId > highestId)
                highestId = save.saveId;
        }

        return highestId + 1;
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
    
    public List<CompletedMission> completedMissions = new();
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
    

public class CompletedMission
{
    public int missionId;
    public bool completedWithAgent;
    public bool completeWithSniper;
    public bool completeWithOperator;
}