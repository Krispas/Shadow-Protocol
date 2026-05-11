using System.Security;

namespace Shadow_Protocol.Systems;

public class GameManager
{
    MenuSystems menuSystems = new();
    MissionManager missionManager = new();
    SaveManager? saveManager = new();
    public GameplayManager gameplay = new();
    public GameSave? currentSave = new();
    public string? characterName;
    public void CreateNewGame()
    {
        Console.WriteLine("Welcome to Shadow Protocol!");
        Console.WriteLine("You will first need to create a character.");
        Console.WriteLine("Enter your character name: ");
        characterName = Console.ReadLine().Trim();
        currentSave.playerName = characterName;
        currentSave.saveId = saveManager.GetNextSaveId();
        saveManager.SaveGame(currentSave);
        Console.WriteLine("Prošel jsi tutoriálem?");
        Console.WriteLine("A = Ano | N = Ne");
        ConsoleKey answer = Console.ReadKey(true).Key;
        switch (answer)
        {
            case ConsoleKey.A: Console.WriteLine("Very well, Mužeš se tedy Přesunout do hlavní meny hry!");
                Console.WriteLine("Enter pro pokračování ");
                while (Console.ReadKey(true).Key != ConsoleKey.Enter)
                {}
                missionManager.StartMissionById(menuSystems.GameMenu(this, missionManager));
                break;
            case ConsoleKey.N: Console.WriteLine("Tak to si ho musíš dát!");
                Console.WriteLine("Enter pro pokračování ");
                while (Console.ReadKey(true).Key != ConsoleKey.Enter)
                {}
                missionManager.StartMissionById(0);
                break;
        }
    }
}