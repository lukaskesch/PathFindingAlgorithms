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

        enum CalculationMode { Fast, Slow }
        CalculationMode calculationMode;
        int CalculationDelay;

        double dx, dy;
        int MaxX, MaxY;

        enum ApplicationMode { NoGrid, Drawing, FinishedAlgorithm }
        ApplicationMode applicationMode;

        enum EditMode { Nothing, AddObstacle, RemoveObstacle, SetStartpoint, SetEndpoint }
        EditMode editMode;
    }
}
