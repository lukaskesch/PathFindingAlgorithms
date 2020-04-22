using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PathFindingAlgorithms
{
    public partial class MainWindow : Window
    {
        Node[,] AllNodes;
        List<Node> ObstacleNodes;
        Node StartNode;
        Node EndNode;

        enum CalculationMode { Fast, Slow }
        CalculationMode calculationMode;
        int CalculationDelay;

        double dx, dy;
        int MaxX, MaxY;

        enum ApplicationMode { NoGrid, ReadyForDrawing, FinishedAlgorithm }
        ApplicationMode applicationMode;

        enum DrawingMode { Nothing, AddObstacle, RemoveObstacle, SetStartpoint, SetEndpoint }
        DrawingMode drawingMode;

        enum EditMode { Preview, Final }
        EditMode editMode;
    }
}
