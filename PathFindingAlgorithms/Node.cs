using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFindingAlgorithms
{
    class Node
    {
        //Object
        int X, Y;

        bool Visited;
        bool Obstacle;

        Node PriorNode;
        List<Node> Neighboors;

        public Node(int _X, int _Y)
        {
            X = _X;
            Y = _Y;
            Visited = false;
            Obstacle = false;
            PriorNode = null;
            Neighboors = new List<Node>();
        }


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
        public static void FindAllNeighboors(Node[,] AllNodes, List<Node> ObstacleNodes)
        {
            for (int i = 0; i < MaxX; i++)
            {
                for (int j = 0; j < MaxY; j++)
                {
                    if (i > 0 && !ObstacleNodes.Contains(AllNodes[i - 1, j]))
                    {
                        AllNodes[i, j].Neighboors.Add(AllNodes[i - 1, j]);
                    }
                    if (i < MaxX - 1 && !ObstacleNodes.Contains(AllNodes[i + 1, j]))
                    {
                        AllNodes[i, j].Neighboors.Add(AllNodes[i + 1, j]);
                    }
                }
            }
        }
    }
}
