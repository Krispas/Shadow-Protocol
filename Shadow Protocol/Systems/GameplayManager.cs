namespace Shadow_Protocol.Systems;

public class GameplayManager
{
    private Render render = new Render();

    public Mission currentMission;
    public int currentAreaID = 0;
    public int characterX;
    public int characterY;
    public int characterArenaID;
    public bool missionFinished;
    public bool playerDetected;
    public List<string> colorsOfKeycardsOwned = new() ;

    public void StartMission(Mission mission)
    {
        currentMission = mission;

        characterX = currentMission.missionCharacter.position.x;
        characterY = currentMission.missionCharacter.position.y;
        characterArenaID = currentMission.missionCharacter.position.areaID;
        currentAreaID = characterArenaID;
        
        missionFinished = false;
        playerDetected = false;

        GameLoop();
    }

    private void GameLoop()
    {
        while (!missionFinished)
        {
            characterArenaID = currentAreaID;
            playerDetected = IsPlayerDetected();

            render.RenderGame(currentMission, this);

            if (playerDetected)
            {
                ShowMissionFailed();
                return;
            }

            ConsoleKey key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    MovePlayer(0, -1);
                    break;

                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    MovePlayer(0, 1);
                    break;

                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    MovePlayer(-1, 0);
                    break;

                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    MovePlayer(1, 0);
                    break;

                case ConsoleKey.E:
                    Interact();
                    break;
            }
        }
    }

    private void MovePlayer(int dx, int dy)
    {
        int newX = characterX + dx;
        int newY = characterY + dy;

        if (IsWalkable(newX, newY))
        {
            characterX = newX;
            characterY = newY;
        }
    }

    private bool IsWalkable(int x, int y)
    {
        if (y < 0 || y >= currentMission.layouts[currentAreaID].layout.Count)
            return false;

        if (x < 0 || x >= currentMission.layouts[currentAreaID].layout[y].Length)
            return false;

        return currentMission.layouts[currentAreaID].layout[y][x] != '#';
    }

    private void Interact()
    {
        if (currentMission.documents.position.x == characterX && currentMission.documents.position.y == characterY && currentMission.documents.position.areaID == characterArenaID)
        {
            currentMission.documents.hasDocuments = true;
        }

        foreach (Keycard keycard in currentMission.keycards)
        {
            if (keycard.position.x == characterX && keycard.position.y == characterY && keycard.position.areaID == characterArenaID && !colorsOfKeycardsOwned.Contains(keycard.color))
            {
                colorsOfKeycardsOwned.Add(keycard.color);
            }
        }

        foreach (Door door in currentMission.doors)
        {
            if (door.position.x == characterX && door.position.y == characterY &&
                door.position.areaID == characterArenaID && door.color == null)
            {
                currentAreaID = door.leadsToPosition.areaID;
                characterX = door.leadsToPosition.x;
                characterY = door.leadsToPosition.y;
            }
            if (door.position.x == characterX && door.position.y == characterY && door.position.areaID == characterArenaID && colorsOfKeycardsOwned.Contains(door.color))
            {
                currentAreaID = door.leadsToPosition.areaID;
                characterX = door.leadsToPosition.x;
                characterY = door.leadsToPosition.y;
            }
            if (door.leadsToPosition.x == characterX && door.leadsToPosition.y == characterY &&
                door.leadsToPosition.areaID == characterArenaID && door.color == null)
            {
                currentAreaID = door.position.areaID;
                characterX = door.position.x;
                characterY = door.position.y;
            }
            if (door.leadsToPosition.x == characterX && door.leadsToPosition.y == characterY && door.leadsToPosition.areaID == characterArenaID && colorsOfKeycardsOwned.Contains(door.color))
            {
                currentAreaID = door.position.areaID;
                characterX = door.position.x;
                characterY = door.position.y;
            }

            if (currentMission.exit.x == characterX && currentMission.exit.y == characterY &&
                currentMission.exit.areaID == characterArenaID)
            {
                if (missionFinished | currentMission.documents.hasDocuments)
                {
                    ShowMissionCompleted();
                    return;
                }
                ShowMissionFailed();
            }
        }
        
    }
    
    private void ShowMissionFailed()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Mise selhalo, skill issue!.");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Zmackni Enter pro navrat do main menu.");

        while (Console.ReadKey(true).Key != ConsoleKey.Enter)
        {}
        missionFinished = true;
    }

    private void ShowMissionCompleted()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Mise splnena, dobra prace.");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Zmackni Enter pro navrat do menu.");

        while (Console.ReadKey(true).Key != ConsoleKey.Enter)
        {
        }
        missionFinished = true;
    }

    private bool IsPlayerDetected()
    {
        foreach (Camera camera in currentMission.cameras)
        {
            if (CanCameraSeePlayer(camera))
                return true;
        }

        foreach (Enemy enemy in currentMission.enemies)
        {
            if (CanEnemySeePlayer(enemy))
                return true;
        }

        return false;
    }

    private bool CanCameraSeePlayer(Camera camera)
    {
        for (int i = 1; i <= camera.range; i++)
        {
            int maxWidth = Math.Min(i / 2, 10);
            for (int offset = -maxWidth; offset <= maxWidth; offset++)
            {
                int checkX = camera.position.x;
                int checkY = camera.position.y;

                switch (camera.direction)
                {
                    case "up":
                        checkY -= i;
                        checkX += offset;
                        break;
                    case "down":
                        checkY += i;
                        checkX += offset;
                        break;
                    case "left":
                        checkX -= i;
                        checkY += offset;
                        break;
                    case "right":
                        checkX += i;
                        checkY += offset;
                        break;
                }

                if (!IsInsideMap(checkX, checkY))
                    return false;

                if (currentMission.layouts[currentAreaID].layout[checkY][checkX] == '#')
                    return false;

                if (checkX == characterX && checkY == characterY)
                    return true;
            }
        }

        return false;
    }

    private bool CanEnemySeePlayer(Enemy enemy)
    {
        for (int i = 1; i <= enemy.range; i++)
        {
            int maxWidth = Math.Min(i / 2, 10);
            for (int offset = -maxWidth; offset <= maxWidth; offset++)
            {
                int checkX = enemy.position.x;
                int checkY = enemy.position.y;

                switch (enemy.direction)
                {
                    case "up":
                        checkY -= i;
                        checkX += offset;
                        break;
                    case "down":
                        checkY += i;
                        checkX += offset;
                        break;
                    case "left":
                        checkX -= i;
                        checkY += offset;
                        break;
                    case "right":
                        checkX += i;
                        checkY += offset;
                        break;
                }

                if (!IsInsideMap(checkX, checkY))
                    return false;

                if (currentMission.layouts[currentAreaID].layout[checkY][checkX] == '#')
                    return false;

                if (checkX == characterX && checkY == characterY)
                    return true;
            }
        }

        return false;
    }

    private bool IsInsideMap(int x, int y)
    {
        if (y < 0 || y >= currentMission.layouts[currentAreaID].layout.Count)
            return false;

        if (x < 0 || x >= currentMission.layouts[currentAreaID].layout[y].Length)
            return false;

        return true;
    }
}