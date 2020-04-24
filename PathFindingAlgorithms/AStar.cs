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
        private void MenuItemAStarAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            if (!PrepareAlgorithm())
            {
                return;
            }

        }
        private bool PrepareAlgorithm()
        {
            //Check
            if (StartNode == null || EndNode == null)
            {
                MessageBox.Show("Start and/or Endpoint is missing");
                return false;
            }
            else if (ObstacleNodes.Count == 0)
            {
                string title = "Continue ?";
                string message = "You havn't set any obstacles. Do you still want to continue the algorithm?";
                MessageBoxButton messageBoxButton = MessageBoxButton.YesNo;
                MessageBoxImage messageBoxImage = MessageBoxImage.Question;
                MessageBoxResult dialogResult = MessageBox.Show(message, title, messageBoxButton, messageBoxImage);

                if (dialogResult == MessageBoxResult.No)
                {
                    return false;
                }
            }

            //Prepare
            drawingMode = DrawingMode.Nothing;
            applicationMode = ApplicationMode.Algorithm;
            Node.ConvertObstacleListInNodesArray(ObstacleNodes, AllNodes);
            Node.FindAllNeighboors(AllNodes);
            return true;
        }

    }
}
