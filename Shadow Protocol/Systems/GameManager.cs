using System.Security;

namespace Shadow_Protocol.Systems;

public class GameManager
{
    SaveManager? saveManager = new();
    public GameplayManager gameplay = new();
    public GameSave? currentSave;
    public string? characterName;
    public void CreateNewGame()
    {
        Console.WriteLine("Welcome to Shadow Protocol!");
        Console.WriteLine("You will first need to create a character.");
        Console.WriteLine("Enter your character name: ");
        characterName = Console.ReadLine().Trim();
        currentSave.playerName = characterName;
        saveManager.SaveGame(currentSave);
    }
}