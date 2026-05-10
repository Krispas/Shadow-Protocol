using System.Runtime.InteropServices;

namespace Shadow_Protocol.Systems;

public class Render
{
    public void RenderGame(Mission missionForRender, GameplayManager gameplay)
    {
        Console.Clear();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("====================================");
        Console.WriteLine($"MISSION: {missionForRender.name}");
        Console.WriteLine("====================================");
        Console.ResetColor();

        Console.WriteLine("Karty co máš u sebe :");
        foreach (string color in gameplay.colorsOfKeycardsOwned)
        {
            Console.WriteLine(color);
        }
        Console.WriteLine();

        RenderGameplayMap(missionForRender, gameplay);

        Console.WriteLine();
        Console.WriteLine("WASD / sipky = pohyb, E = interakce");
    }

    private void RenderGameplayMap(Mission missionForRender, GameplayManager gameplay)
    {
        int height = missionForRender.layouts[gameplay.currentAreaID].layout.Count;
        int width = 0;

        foreach (string row in missionForRender.layouts[gameplay.currentAreaID].layout)
        {
            if (row.Length > width)
                width = row.Length;
        }

        char[,] currentMapFrame = new char[height, width];

        for (int y = 0; y < height; y++)
        {
            string row = missionForRender.layouts[gameplay.currentAreaID].layout[y];

            for (int x = 0; x < width; x++)
            {
                if (x < row.Length)
                {
                    currentMapFrame[y, x] = row[x];
                }
                else
                {
                    currentMapFrame[y, x] = ' ';
                }
            }
        }

        RenderObjects(currentMapFrame, missionForRender, gameplay);

        List<string> rightPanel = new List<string>();

        if (missionForRender.legend != null && missionForRender.legend.Count > 0)
        {
            rightPanel.Add("LEGENDA");
            rightPanel.AddRange(missionForRender.legend);
        }

        if (missionForRender.instructions != null && missionForRender.instructions.Count > 0)
        {
            if (rightPanel.Count > 0)
                rightPanel.Add("");

            rightPanel.Add("INSTRUKCE");

            foreach (string instruction in missionForRender.instructions)
            {
                rightPanel.Add("- " + instruction);
            }
        }

        int totalLines = Math.Max(height, rightPanel.Count);

        for (int y = 0; y < totalLines; y++)
        {
            if (y < height)
            {
                for (int x = 0; x < width; x++)
                {
                    char tile = currentMapFrame[y, x];

                    switch (tile)
                    {
                        case '#':
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            break;
                        case 'P':
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case 'K':
                            Console.ForegroundColor = GetKeycardColor(x, y, missionForRender, gameplay);
                            break;
                        case '=':
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case 'C':
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        case 'G':
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            break;
                        case 'T':
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            break;
                        case 'X':
                            Console.ForegroundColor = ConsoleColor.Blue;
                            break;
                        case 'D':
                            Console.ForegroundColor = GetDoorColor(x, y, missionForRender, gameplay);
                            break;
                        case '!':
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        case '?':
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                    }

                    Console.Write(tile);
                }
            }
            else
            {
                Console.Write(new string(' ', width));
            }

            Console.ResetColor();
            Console.Write("     ");

            if (y < rightPanel.Count)
            {
                if (rightPanel[y] == "LEGENDA" || rightPanel[y] == "INSTRUKCE")
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(rightPanel[y]);
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(rightPanel[y]);
                }
            }

            Console.WriteLine();
        }

        Console.ResetColor();
    }

    private void RenderObjects(char[,] currentMapFrame, Mission missionForRender, GameplayManager gameplay)
    {
        if (IsInsideCurrentMap(missionForRender.exit.x, missionForRender.exit.y, currentMapFrame, missionForRender.exit.areaID, gameplay))
            currentMapFrame[missionForRender.exit.y, missionForRender.exit.x] = 'X';
        if (missionForRender.documents.position != null)
        {
            if (IsInsideCurrentMap(missionForRender.documents.position.x, missionForRender.documents.position.y,
                    currentMapFrame, missionForRender.documents.position.areaID, gameplay) && !missionForRender.documents.hasDocuments)
                currentMapFrame[missionForRender.documents.position.y, missionForRender.documents.position.x] = '=';
        }

        if (missionForRender.target.position != null)
        {
            if (IsInsideCurrentMap(missionForRender.target.position.x, missionForRender.target.position.y,
                    currentMapFrame, missionForRender.target.position.areaID, gameplay) && !missionForRender.target.isEliminated)
                currentMapFrame[missionForRender.target.position.y, missionForRender.target.position.x] = 'T';
        }

        if (missionForRender.doors != null)
        {
            foreach (Door door in missionForRender.doors)
            {
                if (IsInsideCurrentMap(door.position.x, door.position.y, currentMapFrame, door.position.areaID,
                        gameplay))
                    currentMapFrame[door.position.y, door.position.x] = 'D';
                if (IsInsideCurrentMap(door.leadsToPosition.x, door.leadsToPosition.y, currentMapFrame, door.leadsToPosition.areaID,
                        gameplay))
                    currentMapFrame[door.leadsToPosition.y, door.leadsToPosition.x] = 'D';
            }
        }
        
        if (missionForRender.keycards != null)
        {
            foreach (Keycard keycard in missionForRender.keycards)
            {
                if (IsInsideCurrentMap(keycard.position.x, keycard.position.y, currentMapFrame, keycard.position.areaID, gameplay) && !gameplay.colorsOfKeycardsOwned.Contains(keycard.color))
                    currentMapFrame[keycard.position.y, keycard.position.x] = 'K';
            }
        }

        if (missionForRender.cameras != null)
        {
            foreach (Camera camera in missionForRender.cameras)
            {
                if (IsInsideCurrentMap(camera.position.x, camera.position.y, currentMapFrame, camera.position.areaID,
                        gameplay) && camera.isActive)
                {
                    currentMapFrame[camera.position.y, camera.position.x] = 'C';

                    RenderLineOfSight(currentMapFrame, missionForRender, camera.position.x, camera.position.y,
                        camera.direction,
                        camera.range, '!', gameplay);
                }
            }
        }

        if (missionForRender.enemies != null)
        {
            foreach (Enemy enemy in missionForRender.enemies)
            {
                if (IsInsideCurrentMap(enemy.position.x, enemy.position.y, currentMapFrame, enemy.position.areaID,
                        gameplay) && enemy.isAlive)
                {currentMapFrame[enemy.position.y, enemy.position.x] = 'G';

                    RenderLineOfSight(currentMapFrame, missionForRender, enemy.position.x, enemy.position.y,
                    enemy.direction,
                    enemy.range, '?', gameplay);
                }
                
            }
        }
        if (IsInsideCurrentMap(gameplay.characterX, gameplay.characterY, currentMapFrame, gameplay.characterArenaID, gameplay))
            currentMapFrame[gameplay.characterY, gameplay.characterX] = 'P';
    }

    private void RenderLineOfSight(char[,] map, Mission missionForRender, int startX, int startY, string direction,
        int range, char symbol, GameplayManager gameplay)
    {
        for (int i = 1; i <= range; i++)
        {
            int maxWidth = Math.Min(i / 2, 10);
            for (int offset = -maxWidth; offset <= maxWidth; offset++)
            {
                int x = startX;
                int y = startY;

                switch (direction)
                {
                    case "up":
                        y -= i;
                        x += offset;
                        break;

                    case "down":
                        y += i;
                        x += offset;
                        break;

                    case "left":
                        x -= i;
                        y += offset;
                        break;

                    case "right":
                        x += i;
                        y += offset;
                        break;
                }


                if (y < 0 || y >= missionForRender.layouts[gameplay.currentAreaID].layout.Count)
                    return;

                if (x < 0 || x >= missionForRender.layouts[gameplay.currentAreaID].layout[y].Length)
                    return;

                if (missionForRender.layouts[gameplay.currentAreaID].layout[y][x] == '#')
                    return;

                if (map[y, x] == '.')
                    map[y, x] = symbol;
            }
        }
    }

    private bool IsInsideCurrentMap(int x, int y, char[,] currentMapFrame, int objectsArenaID, GameplayManager gameplay)
    {
        return y >= 0 && y < currentMapFrame.GetLength(0) && x >= 0 && x < currentMapFrame.GetLength(1) &&
               gameplay.currentAreaID == objectsArenaID;
    }

    private ConsoleColor GetKeycardColor(int x, int y, Mission missionForRender, GameplayManager gameplay) // Na tutu funkci bylo vyuzito AI 
    {
        foreach (Keycard keycard in missionForRender.keycards)
        {
            if (keycard.position.x == x && keycard.position.y == y && keycard.position.areaID == gameplay.currentAreaID)
                return Enum.Parse<ConsoleColor>(keycard.color, true); //Na funkci Enum.Parse priso AI
        }
        return ConsoleColor.White ;
    }
    private ConsoleColor GetDoorColor(int x, int y, Mission missionForRender, GameplayManager gameplay) // Na tutu funkci bylo vyuzito AI 
    {
        foreach (Door door in missionForRender.doors)
        {
            if (door.color == null)
                return ConsoleColor.White;
            if (door.position.x == x && door.position.y == y && door.position.areaID == gameplay.currentAreaID)
                return Enum.Parse<ConsoleColor>(door.color, true); //Na funkci Enum.Parse priso AI
            if (door.leadsToPosition.x == x && door.leadsToPosition.y == y && door.leadsToPosition.areaID == gameplay.currentAreaID)
                return Enum.Parse<ConsoleColor>(door.color, true); //Na funkci Enum.Parse priso AI
        }
        return ConsoleColor.White ;
    }
}

