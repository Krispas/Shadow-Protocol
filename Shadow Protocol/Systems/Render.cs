using System.Collections.Generic;

namespace Shadow_Protocol.Systems;

public class Render
{
    public void RenderGame(Mission mission, GameplayManager gameplay)
    {
        Console.Clear();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("====================================");
        Console.WriteLine($"MISSION: {mission.name}");
        Console.WriteLine("====================================");
        Console.ResetColor();

        //Pridat jake bravy karty mas
        Console.WriteLine($"Dokumenty: {(gameplay.currentMission.documents.hasDocuments ? "ANO" : "NE")}");
        Console.WriteLine($"Cil eliminovan: {(gameplay.currentMission.target.isEliminated ? "ANO" : "NE")}");
        Console.WriteLine($"Detekce: {(gameplay.playerDetected ? "ANO" : "NE")}");
        Console.WriteLine();

        RenderGameplayMap(mission, gameplay);

        Console.WriteLine();
        Console.WriteLine("WASD / sipky = pohyb, E = interakce");
    }

    private void RenderGameplayMap(Mission mission, GameplayManager gameplay)
    {
        int height = mission.layout.Count;
        int width = 0;

        foreach (string row in mission.layout)
        {
            if (row.Length > width)
                width = row.Length;
        }

        char[,] currentMapFrame = new char[height, width];

        for (int y = 0; y < height; y++)
        {
            string row = mission.layout[y];

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

        RenderVisionZones(currentMapFrame, mission);

        currentMapFrame[mission.exit.y, mission.exit.x] = 'X';
        currentMapFrame[gameplay.playerY, gameplay.playerX] = 'P';

        List<string> rightPanel = new List<string>();

        if (mission.legend != null && mission.legend.Count > 0)
        {
            rightPanel.Add("LEGENDA");
            rightPanel.AddRange(mission.legend);
        }

        if (mission.instructions != null && mission.instructions.Count > 0)
        {
            if (rightPanel.Count > 0)
                rightPanel.Add("");

            rightPanel.Add("INSTRUKCE");

            foreach (string instruction in mission.instructions)
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
                            Console.ForegroundColor = ConsoleColor.Cyan;
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

    private void RenderVisionZones(char[,] map, Mission mission)
    {
        foreach (Camera camera in mission.cameras)
        {
            if (IsInsideMap(camera.position.x, camera.position.y, map))
                map[camera.position.y, camera.position.x] = 'C';

            RenderLineOfSight(map, mission, camera.position.x, camera.position.y, camera.direction, camera.range, '!');
        }

        foreach (Enemy enemy in mission.enemies)
        {
            if (IsInsideMap(enemy.position.x, enemy.position.y, map))
                map[enemy.position.y, enemy.position.x] = 'G';

            RenderLineOfSight(map, mission, enemy.position.x, enemy.position.y, enemy.direction, enemy.range, '?');
        }
    }

    private void RenderLineOfSight(char[,] map, Mission mission, int startX, int startY, string direction, int range,
        char symbol)
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


                if (y < 0 || y >= mission.layout.Count)
                    return;

                if (x < 0 || x >= mission.layout[y].Length)
                    return;

                if (mission.layout[y][x] == '#')
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
