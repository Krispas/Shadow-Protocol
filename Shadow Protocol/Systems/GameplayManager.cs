namespace Shadow_Protocol.Systems;

public class GameplayManager
{
    private Render render = new Render();

    public Mission? currentMission;
    public int currentAreaID = 0;
    public int playerX;
    public int playerY;
    public bool missionFinished;
    public bool playerDetected;

    public void StartMission(Mission mission)
    {
        currentMission = mission;

        playerX = currentMission.playerStart.x;
        playerY = currentMission.playerStart.y;
        
        missionFinished = false;
        playerDetected = false;

        GameLoop();
    }

    private void GameLoop()
    {
        while (!missionFinished)
        {
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

            CheckMissionState(); //DODELAT
        }
    }

    private void MovePlayer(int dx, int dy)
    {
        int newX = playerX + dx;
        int newY = playerY + dy;

        if (IsWalkable(newX, newY))
        {
            playerX = newX;
            playerY = newY;
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
        if (currentMission.documents.position.x == playerX && currentMission.documents.position.y == playerY)
        {
            currentMission.documents.hasDocuments = true;
        }
        if 
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
        {
        }
    }

    private void ShowMissionCompleted()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Mise splnena, dobra prace.");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Zmackni Enter pro navrat do main menu.");

        while (Console.ReadKey(true).Key != ConsoleKey.Enter)
        {
        }
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

                if (checkX == playerX && checkY == playerY)
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

                if (checkX == playerX && checkY == playerY)
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