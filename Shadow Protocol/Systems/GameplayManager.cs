namespace Shadow_Protocol.Systems;

public class GameplayManager
{
    private Render render = new Render();

    public Mission currentMission = null;

    public int playerX;
    public int playerY;

    public bool hasKeycard = false;
    public bool hasDocuments = false;
    public bool targetEliminated = false;
    public bool missionFinished = false;
    public bool playerDetected = false;

    public void StartMission(Mission mission)
    {
        currentMission = mission;

        playerX = mission.playerStart.x;
        playerY = mission.playerStart.y;

        hasKeycard = false;
        hasDocuments = false;
        targetEliminated = false;
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

            CheckMissionState();
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
        if (y < 0 || y >= currentMission.layout.Count)
            return false;

        if (x < 0 || x >= currentMission.layout[y].Length)
            return false;

        return currentMission.layout[y][x] != '#';
    }

    private void Interact()
    {
        foreach (Item item in currentMission.items)
        {
            if (item.x == playerX && item.y == playerY)
            {
                if (item.type == "keycard")
                    hasKeycard = true;

                if (item.type == "documents")
                    hasDocuments = true;
            }
        }

        foreach (Enemy enemy in currentMission.enemies)
        {
            if (enemy.x == playerX && enemy.y == playerY && enemy.isTarget)
            {
                targetEliminated = true;
            }
        }
    }

    private void CheckMissionState()
    {
        if (playerX == currentMission.exit.x &&
            playerY == currentMission.exit.y &&
            hasKeycard &&
            hasDocuments &&
            targetEliminated)
        {
            missionFinished = true;
            ShowMissionCompleted();
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
            int checkX = camera.x;
            int checkY = camera.y;

            switch (camera.direction)
            {
                case "up":
                    checkY -= i;
                    break;
                case "down":
                    checkY += i;
                    break;
                case "left":
                    checkX -= i;
                    break;
                case "right":
                    checkX += i;
                    break;
            }

            if (!IsInsideMap(checkX, checkY))
                return false;

            if (currentMission.layout[checkY][checkX] == '#')
                return false;

            if (checkX == playerX && checkY == playerY)
                return true;
        }

        return false;
    }

    private bool CanEnemySeePlayer(Enemy enemy)
    {
        for (int i = 1; i <= enemy.detectionRange; i++)
        {
            int checkX = enemy.x;
            int checkY = enemy.y;

            switch (enemy.direction)
            {
                case "up":
                    checkY -= i;
                    break;
                case "down":
                    checkY += i;
                    break;
                case "left":
                    checkX -= i;
                    break;
                case "right":
                    checkX += i;
                    break;
            }

            if (!IsInsideMap(checkX, checkY))
                return false;

            if (currentMission.layout[checkY][checkX] == '#')
                return false;

            if (checkX == playerX && checkY == playerY)
                return true;
        }

        return false;
    }

    private bool IsInsideMap(int x, int y)
    {
        if (y < 0 || y >= currentMission.layout.Count)
            return false;

        if (x < 0 || x >= currentMission.layout[y].Length)
            return false;

        return true;
    }
}