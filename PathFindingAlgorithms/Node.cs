using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace PathFindingAlgorithms
{
    public class Node //: IComparable<Node>
    {
        //Object
        public readonly int Index;
        public readonly double X, Y;

        public double DistanceToStart;
        public double EstimatedDistanceToEnd;
        public double Score;

        public bool Found;
        public bool Visited;
        public bool Obstacle;

        public Node PriorNode;
        public readonly List<Node> Neighboors;

        public Node(double _X, double _Y)
        {
            X = _X;
            Y = _Y;
            PriorNode = null;
            Neighboors = new List<Node>();
            Index = -1;
        }

        public double GetDistanceBetween(Node node)
        {
            return Math.Sqrt(Math.Pow(this.X - node.X, 2) + Math.Pow(this.Y - node.Y, 2));
        }
        public override string ToString()
        {
            return "(" + X + ";" + Y + ")";
        }
        //public int CompareTo(Node other)
        //{
        //    if (this.Score < other.Score) return -1;
        //    else if (this.Score > other.Score) return 1;
        //    else return 0;
        //}

        static int MaxX, MaxY;
        public static Node[,] FillNodesArray(int _MaxX, int _MaxY)
        {
            MaxX = _MaxX;
            MaxY = _MaxY;
            Node[,] Nodes = new Node[MaxX, MaxY];

            for (int i = 0; i < MaxX; i++)
            {
                for (int j = 0; j < MaxY; j++)
                {
                    Nodes[i, j] = new Node(i, j);
                }
            }
            return Nodes;
        }
        public static void FindAllNeighboors(Node[,] AllNodes)
        {
            for (int i = 0; i < MaxX; i++)
            {
                for (int j = 0; j < MaxY; j++)
                {
                    if (i > 0 && !AllNodes[i - 1, j].Obstacle)
                    {
                        AllNodes[i, j].Neighboors.Add(AllNodes[i - 1, j]);
                    }
                    if (i < MaxX - 1 && !AllNodes[i + 1, j].Obstacle)
                    {
                        AllNodes[i, j].Neighboors.Add(AllNodes[i + 1, j]);
                    }
                    if (j > 0 && !AllNodes[i, j - 1].Obstacle)
                    {
                        AllNodes[i, j].Neighboors.Add(AllNodes[i, j - 1]);
                    }
                    if (j < MaxY - 1 && !AllNodes[i, j + 1].Obstacle)
                    {
                        AllNodes[i, j].Neighboors.Add(AllNodes[i, j + 1]);
                    }
                }
            }
        }
        public static bool CheckIfObstacleNodeAlreadyExist(List<Node> list, Node node1)
        {
            foreach (Node node2 in list)
            {
                if (node1.X == node2.X && node1.Y == node2.Y)
                {
                    return true;
                }
            }
            return false;
        }
        public static void ConvertObstacleListInNodesArray(List<Node> Obstacles, Node[,] Nodes)
        {
            foreach (Node node1 in Obstacles)
            {
                foreach (Node node2 in Nodes)
                {
                    if (node1.X == node2.X && node1.Y == node2.Y)
                    {
                        node2.Obstacle = true;
                    }
                    else
                    {
                        node2.Obstacle = false;
                    }
                }
            }
        }
    }
}
