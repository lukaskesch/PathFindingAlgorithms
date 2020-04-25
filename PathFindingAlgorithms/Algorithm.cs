using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PathFindingAlgorithms
{
    public partial class MainWindow : Window
    {
        private void MenuItemAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckData())
            {
                return;
            }

            Prepare(sender);

            Node CurrentNode = StartNode;
            while (CurrentNode != EndNode)
            {
                CurrentNode.Visited = true;

                //Iterate through neighboors
                foreach (Node NeighboorNode in CurrentNode.Neighboors)
                {
                    if (algorithm == Algorithm.AStart)
                        UpdateNeighboorAStar(CurrentNode, NeighboorNode);
                    else if (algorithm == Algorithm.Dijkstra)
                        UpdateNeigboorDijkstra(CurrentNode, NeighboorNode);
                }

                //Pick next best unvisited node
                CurrentNode = UnvisitedNodes.Dequeue();
            }

            MessageBox.Show(EndNode.DistanceToStart.ToString());
        }
        private void UpdateNeighboorAStar(Node Current, Node Neighboor)
        {
            //Check if neighboor has been visited or is an obstacle
            if (Neighboor.Visited || Neighboor.Obstacle)
            {
                return;
            }

            bool BetterScore;
            double DistanceToStart, DistnaceToEnd, GScore;

            //Mark neighboor in canvas

            //Calculate new score
            BetterScore = false;
            DistanceToStart = Current.DistanceToStart + Current.GetDistanceBetween(Neighboor);
            DistnaceToEnd = (Neighboor.GetDistanceBetween(EndNode)) * (1 - Math.Pow(10, -12));
            GScore = DistanceToStart + DistnaceToEnd;

            //May update old score
            if (Neighboor.Score > GScore)
            {
                BetterScore = true;
                Neighboor.DistanceToStart = DistanceToStart;
                Neighboor.EstimatedDistanceToEnd = DistnaceToEnd;
                Neighboor.Score = GScore;
                Neighboor.PriorNode = Current;
            }

            //Check if neighboor has been found yet
            if (!Neighboor.Found)
            {
                Neighboor.Found = true;
                UnvisitedNodes.Enqueue(Neighboor, (float)Neighboor.Score);
            }

            //Update priority if new score is lower
            else if (Neighboor.Found && BetterScore)
            {
                UnvisitedNodes.UpdatePriority(Neighboor, (float)Neighboor.Score);
            }
        }
        private void UpdateNeigboorDijkstra(Node Current, Node Neighboor)
        {

        }
        private bool CheckData()
        {
            //Check
            if (StartNode == null || EndNode == null)
            {
                MessageBox.Show("Start and/or Endpoint is missing", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (StartNode.X == EndNode.X && StartNode.Y == EndNode.Y)
            {
                MessageBox.Show("Start and Endpoint are the same", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            return true;
        }
        private void Prepare(object sender)
        {
            drawingMode = DrawingMode.Nothing;
            applicationMode = ApplicationMode.Algorithm;
            Node.ConvertObstacleListInNodesArray(ObstacleNodes, AllNodes);
            Node.FindAllNeighboors(AllNodes);

            if (((MenuItem)sender).Name == MenuItemAStarAlgorithm.Name)
            {
                algorithm = Algorithm.AStart;
            }
            else if (((MenuItem)sender).Name == MenuItemDijkstraAlgorithm.Name)
            {
                algorithm = Algorithm.Dijkstra;
            }

            foreach (Node node in AllNodes)
            {
                node.DistanceToStart = double.MaxValue;
                node.EstimatedDistanceToEnd = double.MaxValue;
                node.Score = double.MaxValue;
                node.PriorNode = null;
            }

            StartNode.DistanceToStart = 0.0;
        }

    }
}
