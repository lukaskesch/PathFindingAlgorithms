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
        Node[,] AllNodes;
        List<Node> ObstacleNodes = new List<Node>();
        Node StartNode = new Node(-1, -1);
        Node EndNode;

        enum Algorithm { AStart, Dijkstra }
        Algorithm algorithm;

        SimplePriorityQueue<Node> UnvisitedNodes = new SimplePriorityQueue<Node>();

        enum CalculationMode { Fast, Slow }
        CalculationMode calculationMode;
        int CalculationDelay;

        double dx, dy;
        int MaxX, MaxY;

        enum ApplicationMode { SetMap, Drawing, Algorithm }
        ApplicationMode applicationMode;

        enum DrawingMode { Nothing, AddObstacle, RemoveObstacle, SetStartpoint, SetEndpoint }
        DrawingMode drawingMode;

        enum EditMode { Preview, Final }
        EditMode editMode;

        Rectangle Rect = new Rectangle();
        Rectangle TempRect = new Rectangle();
        Rectangle StartRect = new Rectangle();
        Rectangle EndRect = new Rectangle();
        int CoordinateLineWidth = 3;

        string FilePath = string.Empty;
    }
}
