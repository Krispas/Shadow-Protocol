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

        //Pridat jake bravy karty mas
        Console.WriteLine($"Dokumenty: {(gameplay.currentMission.documents.hasDocuments ? "ANO" : "NE")}");
        Console.WriteLine($"Cil eliminovan: {(gameplay.currentMission.target.isEliminated ? "ANO" : "NE")}");
        Console.WriteLine($"Detekce: {(gameplay.playerDetected ? "ANO" : "NE")}");
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
                            
                            Console.ForegroundColor = ConsoleColor.; //nastavit barvu podle barvy karty to same u dvery
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
                        case 'D' :
                            
                            Console.ForegroundColor = ConsoleColor.; //nastavit barvu podle barvy dvery
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
        currentMapFrame[missionForRender.exit.y, missionForRender.exit.x] = 'X';
        currentMapFrame[gameplay.playerY, gameplay.playerX] = 'P';
        foreach (Camera camera in missionForRender.cameras)
        {
            if (IsInsideMap(camera.position.x, camera.position.y, currentMapFrame))
                currentMapFrame[camera.position.y, camera.position.x] = 'C';

            RenderLineOfSight(currentMapFrame, missionForRender, camera.position.x, camera.position.y, camera.direction, camera.range, '!', gameplay);
        }

        foreach (Enemy enemy in missionForRender.enemies)
        {
            if (IsInsideMap(enemy.position.x, enemy.position.y, currentMapFrame))
                currentMapFrame[enemy.position.y, enemy.position.x] = 'G';

            RenderLineOfSight(currentMapFrame, missionForRender, enemy.position.x, enemy.position.y, enemy.direction, enemy.range, '?', gameplay);
        }
    }

    private void RenderLineOfSight(char[,] map, Mission missionForRender, int startX, int startY, string direction, int range, char symbol,GameplayManager gameplay)
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

    private bool IsInsideMap(int x, int y, char[,] map)
    {
        return y >= 0 && y < map.GetLength(0) && x >= 0 && x < map.GetLength(1);
    }
}
