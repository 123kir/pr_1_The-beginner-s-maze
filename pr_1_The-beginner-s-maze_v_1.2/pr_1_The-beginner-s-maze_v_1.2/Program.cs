using System;
using System.Collections.Generic;

class Program
{
    static int width = 21;  
    static int height = 21; 

    static char[,] maze = new char[height, width];

    static int playerX = 1; 
    static int playerY = 1;

    static List<(int x, int y)> pathToExit = new List<(int, int)>(); 
    

    static void Main(string[] args)
    {
        GenerateMaze();
        AddStartAndFinish();
        while (true)
        {
            DisplayMaze();
            MovePlayer();
        }
    }

    static void GenerateMaze()
    {
        Random rand = new Random();

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                maze[y, x] = '#';

        maze[1, 1] = ' ';

        CreatePath(1, 1);
    }

    static void CreatePath(int x, int y)
    {
        int[] directions = { 0, 1, 2, 3 };
        ShuffleArray(directions);

        foreach (int dir in directions)
        {
            int nx = x, ny = y;

            switch (dir)
            {
                case 0: nx += 2; break; 
                case 1: nx -= 2; break; 
                case 2: ny += 2; break; 
                case 3: ny -= 2; break; 
            }

            if (IsInBounds(nx, ny) && maze[ny, nx] == '#')
            {
                maze[ny, nx] = ' ';
                maze[y + (ny - y) / 2, x + (nx - x) / 2] = ' ';
                CreatePath(nx, ny);
            }
        }
    }

    static void AddStartAndFinish()
    {
        maze[1, 1] = 'S';

        maze[height - 2, width - 2] = 'F';
    }

    static void MovePlayer()
    {
        ConsoleKeyInfo key = Console.ReadKey(true); 

        int newX = playerX;
        int newY = playerY;

        switch (key.Key)
        {
            case ConsoleKey.W: newY--; break; 
            case ConsoleKey.S: newY++; break;
            case ConsoleKey.A: newX--; break; 
            case ConsoleKey.D: newX++; break; 
            case ConsoleKey.P: ShowPath(); return; 
            case ConsoleKey.C: ClearPath(); return; 
            default: return; 
        }

        if (IsInBounds(newX, newY) && maze[newY, newX] != '#')
        {
            maze[playerY, playerX] = ' ';
            
            playerX = newX;
            playerY = newY;

            if (maze[playerY, playerX] == 'F')
            {
                DisplayMaze();
                Console.WriteLine("Вы выиграли!");
                Environment.Exit(0); 
            }

            // Установка новой позиции игрока
            maze[playerY, playerX] = 'S';
        }
    }

    static void ShowPath()
    {
        pathToExit.Clear(); 

        var found = FindPath(playerX, playerY);

        if (found)
        {
            foreach (var (x, y) in pathToExit)
            {
                if (maze[y, x] != 'F' && maze[y, x] != 'S' && maze[y, x] != '#')
                {
                    maze[y, x] = '*'; 
                }
            }
            Console.WriteLine("Путь к выходу найден!");
        }
        else
        {
            Console.WriteLine("Путь к 'F' не найден!");
        }

        Console.ReadKey(); 
    }

    static void ClearPath()
    {
        foreach (var (x, y) in pathToExit)
        {
            if (maze[y, x] == '*')
            {
                maze[y, x] = ' '; 
            }
        }
        pathToExit.Clear();
        Console.WriteLine("Путь очищен.");
        Console.ReadKey(); 
    }

    // Функция поиска пути к выходу (BFS - поиск в ширину)
    static bool FindPath(int startX, int startY)
    {
        // Очередь для хранения ячеек, которые нужно посетить
        Queue<(int x, int y)> queue = new Queue<(int x, int y)>();
        // Словарь для хранения информации о том, как мы попали в каждую ячейку
        Dictionary<(int x, int y), (int x, int y)> cameFrom = new Dictionary<(int x, int y), (int x, int y)>();

        queue.Enqueue((startX, startY));
        cameFrom[(startX, startY)] = (startX, startY);

        while (queue.Count > 0)
        {
            (int x, int y) current = queue.Dequeue();

            if (maze[current.y, current.x] == 'F')
            {
                ReconstructPath(cameFrom, current);
                return true;
            }

            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };

            for (int i = 0; i < 4; i++)
            {
                int nx = current.x + dx[i];
                int ny = current.y + dy[i];

                if (IsInBounds(nx, ny) && maze[ny, nx] != '#' && !cameFrom.ContainsKey((nx, ny)))
                {
                    queue.Enqueue((nx, ny));
                    cameFrom[(nx, ny)] = current;
                }
            }
        }

        return false;
    }

    static void ReconstructPath(Dictionary<(int x, int y), (int x, int y)> cameFrom, (int x, int y) current)
    {
        pathToExit.Clear();

        while (current != cameFrom[current])
        {
            pathToExit.Add(current);
            current = cameFrom[current];
        }
        pathToExit.Add(current);
        pathToExit.Reverse();
    }

    static void DisplayMaze()
    {
        Console.Clear();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (maze[y, x] == 'S')
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write('S');
                }
                else if (maze[y, x] == 'F')
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write('F');
                }
                else if (maze[y, x] == '*')
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write('*');
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(maze[y, x]);
                }
            }
            Console.WriteLine();
        }
    }

    static bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    // Использует алгоритм Фишера-Йетса для перемешивания
    static void ShuffleArray<T>(T[] array)
    {
        Random rng = new Random();
        int n = array.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}
