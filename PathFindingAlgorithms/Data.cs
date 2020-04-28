using Priority_Queue;
using PriorityQueues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace PathFindingAlgorithms
{
    public partial class MainWindow : Window
    {
        //Nodes
        Node[,] AllNodes;
        List<Node> ObstacleNodes = new List<Node>();
        Node StartNode = new Node(-1, -1);
        Node EndNode;

        //Algorithm
        enum Algorithm { AStart, Dijkstra }
        Algorithm algorithm;
        enum CalculationMode { Fast, Slow }
        CalculationMode calculationMode;
        int CalculationDelay;
        SimplePriorityQueue<Node> UnvisitedNodes;


        //Scaling
        double dx, dy;
        int MaxX, MaxY;

        //Application
        enum ApplicationMode { SetMap, Drawing, Algorithm }
        ApplicationMode applicationMode;

        //Drawing
        enum DrawingMode { Nothing, AddObstacle, RemoveObstacle, SetStartpoint, SetEndpoint }
        DrawingMode drawingMode;
        enum EditMode { Preview, Final }
        EditMode editMode;
        Rectangle[,] Rectangles;
        Rectangle TempRect = new Rectangle();
        Rectangle Rect = new Rectangle();
        Rectangle StartRect = new Rectangle();
        Rectangle EndRect = new Rectangle();
        int CoordinateLineWidth = 1;

        //File
        string FilePath = string.Empty;
    }
}
