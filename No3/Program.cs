using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Solution
{
    class Solution
    {
        class Maze
        {
            struct FMVector
            {
                public int x;
                public int y;

                public FMVector(int x, int y)
                {
                    this.x = x;
                    this.y = y;
                }
            }

            struct FMPosition
            {
                public Direct direct;
                public FMVector vector;

                public FMPosition(Direct direct, int row, int col)
                {
                    this.direct = direct;
                    this.vector = new FMVector(row, col);
                }

                public FMPosition(Direct direct, FMVector vector)
                {
                    this.direct = direct;
                    this.vector = vector;
                }
            }

            enum Status
            {
                READY,
                WAITING,
                PROCESSED,

                END,
            }

            enum Direct
            {
                START = -1,

                LEFT,
                RIGHT,
                DOWN,
                UP,

                END,
            }

            private const int PATH = -1;
            private const int EMPTY = 0;

            private int[,] m_maze = null;

            private int m_rowLength = 0;
            private int m_colLength = 0;
            private int m_mazeMax = 0;

            private Direct m_startDirect = Direct.START;

            private FMVector m_startPos, m_endPos;

            public Maze() { }
            public Maze(int[,] maze)
            {
                m_maze = maze;

                m_rowLength = maze.GetLength(0);
                m_colLength = maze.GetLength(1);

                m_mazeMax = m_rowLength * m_colLength;
            }

            public Maze(string direct, int width, string[] map)
            {
                switch (direct)
                {
                    case "WEST": m_startDirect = Direct.LEFT; break;
                    case "EAST": m_startDirect = Direct.RIGHT; break;
                    case "NORTH": m_startDirect = Direct.UP; break;
                    case "SOUTH": m_startDirect = Direct.DOWN; break;
                }

                m_maze = new int[map.Length, width];

                for (int i = 0; i < map.Length; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        switch (map[i][j])
                        {
                            case '.': m_maze[i, j] = 0; break;
                            case '#': m_maze[i, j] = 1; break;
                            case 'T': m_maze[i, j] = 0; m_startPos = new FMVector(i, j); break;
                            case 'G': m_maze[i, j] = 0; m_endPos = new FMVector(i, j); break;
                        }
                    }
                }

                m_rowLength = m_maze.GetLength(0);
                m_colLength = m_maze.GetLength(1);

                m_mazeMax = m_rowLength * m_colLength;
            }

            public string MakeResult()
            {
                int sNode = m_startPos.x * m_colLength + m_startPos.y;
                int eNode = m_endPos.x * m_colLength + m_endPos.y;

                List<FMPosition> postion = null;
                int[,] result = null;

                if (CheckNodeValue(m_maze, sNode, EMPTY) && CheckNodeValue(m_maze, eNode, EMPTY) && Search(sNode, eNode, out postion, out result))
                {
                    // ViewMazeNode(result);

                    System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();

                    for (int i = postion.Count - 1; i >= 0; i--)
                    {
                        switch (postion[i].direct)
                        {
                            case Direct.START: sBuilder.Append(GetDirectStr(m_startDirect, postion[i - 1].direct)); break;
                            default: sBuilder.Append(GetDirectStr(postion[i].direct, (i - 1 < 0) ? Direct.END : postion[i - 1].direct)); break;
                        }
                    }

                    // int lapTime = 0;
                    // string res = sBuilder.ToString();
                    // for (int i = 0; i < res.Length; i++)
                    // {
                    //     if (res[i].Equals('F')) lapTime += 2;
                    //     else lapTime += 1;
                    // }

                    // Console.WriteLine(lapTime);

                    return sBuilder.ToString();
                }

                else
                    return "X";
            }

            private string GetDirectStr(Direct cur, Direct next)
            {
                switch (cur)
                {
                    case Direct.LEFT:
                        if (cur.Equals(next)) return "F";
                        else if (next.Equals(Direct.UP)) return "RF";
                        else if (next.Equals(Direct.DOWN)) return "LF";
                        break;

                    case Direct.RIGHT:
                        if (cur.Equals(next)) return "F";
                        else if (next.Equals(Direct.UP)) return "LF";
                        else if (next.Equals(Direct.DOWN)) return "RF";
                        break;

                    case Direct.UP:
                        if (cur.Equals(next)) return "F";
                        else if (next.Equals(Direct.LEFT)) return "LF";
                        else if (next.Equals(Direct.RIGHT)) return "RF";
                        break;

                    case Direct.DOWN:
                        if (cur.Equals(next)) return "F";
                        else if (next.Equals(Direct.LEFT)) return "RF";
                        else if (next.Equals(Direct.RIGHT)) return "LF";
                        break;
                }

                return "";
            }

            private bool Search(int sNode, int eNode, out List<FMPosition> resultList, out int[,] result)
            {
                resultList = new List<FMPosition>();
                result = null;

                int[] queue = new int[m_mazeMax];
                int[] origin = new int[m_mazeMax];
                FMPosition[] pos = new FMPosition[m_mazeMax];

                int front = 0;
                int rear = 0;

                int cur, left, right, up, down;

                int[,] dummy = MakeDefaultMaze(m_rowLength, m_colLength);

                queue[rear] = sNode;
                origin[rear] = -1;
                pos[rear] = SetPosition(m_colLength, origin[rear], m_startDirect);

                rear++;

                while (front != rear)
                {
                    if (queue[front].Equals(eNode))
                        break;

                    cur = queue[front];

                    left = cur - 1;
                    if (left >= 0 && left / m_colLength == cur / m_colLength)
                    {
                        if (CheckNodeValue(m_maze, left, EMPTY))
                        {
                            if (CheckNodeValue(dummy, left, (int)Status.READY))
                            {
                                queue[rear] = left;
                                origin[rear] = cur;
                                pos[rear] = SetPosition(m_colLength, origin[rear], Direct.LEFT);

                                SetNodeValue(dummy, left, (int)Status.WAITING);

                                rear++;
                            }
                        }
                    }

                    right = cur + 1;
                    if (right < m_mazeMax && right / m_colLength == cur / m_colLength)
                    {
                        if (CheckNodeValue(m_maze, right, EMPTY))
                        {
                            if (CheckNodeValue(dummy, right, (int)Status.READY))
                            {
                                queue[rear] = right;
                                origin[rear] = cur;
                                pos[rear] = SetPosition(m_colLength, origin[rear], Direct.RIGHT);

                                SetNodeValue(dummy, right, (int)Status.WAITING);

                                rear++;
                            }
                        }
                    }

                    up = cur - m_colLength;
                    if (up >= 0)
                    {
                        if (CheckNodeValue(m_maze, up, EMPTY))
                        {
                            if (CheckNodeValue(dummy, up, (int)Status.READY))
                            {
                                queue[rear] = up;
                                origin[rear] = cur;
                                pos[rear] = SetPosition(m_colLength, origin[rear], Direct.UP);

                                SetNodeValue(dummy, up, (int)Status.WAITING);

                                rear++;
                            }
                        }
                    }

                    down = cur + m_colLength;
                    if (down < m_mazeMax)
                    {
                        if (CheckNodeValue(m_maze, down, EMPTY))
                        {
                            if (CheckNodeValue(dummy, down, (int)Status.READY))
                            {
                                queue[rear] = down;
                                origin[rear] = cur;
                                pos[rear] = SetPosition(m_colLength, origin[rear], Direct.DOWN);

                                SetNodeValue(dummy, down, (int)Status.WAITING);

                                rear++;
                            }
                        }
                    }

                    SetNodeValue(dummy, cur, (int)Status.PROCESSED);

                    front++;
                }

                result = CopyMaze(m_maze);

                cur = eNode;
                pos[rear] = SetPosition(m_colLength, cur, Direct.END);

                SetNodeValue(result, cur, PATH);

                int before = -1;
                for (int i = front; i >= 0; i--)
                {
                    if (queue[i] == cur)
                    {
                        before = before.Equals(-1) ? rear : before;
                        resultList.Add(new FMPosition(pos[i].direct, pos[before].vector));

                        // Console.WriteLine(string.Format("{0} = ( {1} , {2} )", pos[i].direct, pos[before].vector.x, pos[before].vector.y));

                        cur = origin[i];

                        if (cur == -1)
                            return true;

                        SetNodeValue(result, cur, PATH);

                        before = i;
                    }
                }

                return false;
            }

            private int GetNodeValue(int[,] maze, int node)
            {
                int col = maze.GetLength(1);
                return maze[node / col, node - node / col * col];
            }

            private bool CheckNodeValue(int[,] maze, int node, int check)
            {
                int col = maze.GetLength(1);
                return maze[node / col, node - node / col * col].Equals(check);
            }

            private void SetNodeValue(int[,] maze, int node, int value)
            {
                int col = maze.GetLength(1);
                maze[node / col, node - node / col * col] = value;
            }

            private FMPosition SetPosition(int col, int node, Direct direct)
            {
                return new FMPosition(direct, node / col, node - node / col * col);
            }

            private int[,] MakeDefaultMaze(int row, int col)
            {
                int[,] maze = new int[row, col];
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                        maze[i, j] = (int)Status.READY;
                }

                return maze;
            }

            private int[,] CopyMaze(int[,] maze)
            {
                int row = maze.GetLength(0);
                int col = maze.GetLength(1);

                int[,] copy = new int[row, col];
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                        copy[i, j] = m_maze[i, j];
                }

                return copy;
            }

            private void ViewMazeNode(int[,] maze)
            {
                for (int i = 0; i < maze.GetLength(0); i++)
                {
                    System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();

                    sBuilder.Append("{ ");
                    for (int j = 0; j < maze.GetLength(1); j++)
                    {
                        sBuilder.Append(maze[i, j].Equals(-1) ? "K" : maze[i, j].ToString());
                        sBuilder.Append(", ");
                    }
                    sBuilder.Append(" }");

                    Console.WriteLine(sBuilder.ToString());
                }

                Console.WriteLine("");
            }
        }

        static string Solve(string D, int W, string[] MAP)
        {
            Maze solve = new Maze(D, W, MAP);
            return solve.MakeResult();
        }

        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(@"/Users/yonghyeonlee/Documents/CSharp/No3/input001.txt");

            string direction = lines[0];
            int width = Convert.ToInt32(lines[1]);
            int height = Convert.ToInt32(lines[2]);
            string[] map = new string[height];

            for (int i = 0; i < height; i++)
                map[i] = lines[i + 3];

            Console.WriteLine(Solve(direction, width, map));
        }
    }
}