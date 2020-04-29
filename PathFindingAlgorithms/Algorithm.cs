using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PathFindingAlgorithms
{
    public partial class MainWindow : Window
    {
        private void MenuItemAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            MenuItemEdit_Click(sender, e);

            if (!CheckData())
            {
                return;
            }

            Prepare(sender);
            UnvisitedNodes = new SimplePriorityQueue<Node>();

            //Algorithm
            Node CurrentNode = StartNode;
            while (CurrentNode != EndNode)
            {
                CurrentNode.IsVisited = true;

                //Iterate through neighboors
                foreach (Node NeighboorNode in CurrentNode.Neighboors)
                {
                    if (NeighboorNode.IsObstacle)
                        continue;
                    UpdateNeighboorAStar(CurrentNode, NeighboorNode);
                }

                //Pick next best unvisited node
                try { CurrentNode = UnvisitedNodes.Dequeue(); }
                catch (Exception)
                {
                    MessageBox.Show("No solution found. Make sure the endpoint is reachable.", "No Solution", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                //Mark it in canvas
                if (CurrentNode != EndNode)
                    Rectangles[(int)CurrentNode.X, (int)CurrentNode.Y].Opacity = 0.7;
                if (calculationMode == CalculationMode.Slow)
                    Wait(Rectangles[(int)CurrentNode.X, (int)CurrentNode.Y], CalculationDelay);
            }

            //Final Path
            CurrentNode = EndNode.PriorNode;
            while (true)
            {
                if (CurrentNode == StartNode)
                {
                    break;
                }
                else
                {
                    Rectangles[(int)CurrentNode.X, (int)CurrentNode.Y].Fill = Brushes.Green;
                    if (calculationMode == CalculationMode.Slow)
                        Wait(Rectangles[(int)CurrentNode.X, (int)CurrentNode.Y], CalculationDelay / 2);
                }
                CurrentNode = CurrentNode.PriorNode;
            }
            MessageBox.Show(EndNode.DistanceToStart.ToString());
        }
        private void UpdateNeighboorAStar(Node Current, Node Neighboor)
        {
            //Check if neighboor has been visited or is an obstacle
            if (Neighboor.IsVisited || Neighboor.IsObstacle)
            {
                return;
            }

            bool BetterScore;
            double DistanceToStart, DistnaceToEnd, GScore;

            //Calculate new score
            BetterScore = false;
            DistanceToStart = Current.DistanceToStart + Current.GetDistanceBetween(Neighboor);
            DistnaceToEnd = (Neighboor.GetDistanceBetween(EndNode)) * (1 - Math.Pow(10, -12));
            GScore = DistanceToStart + DistnaceToEnd;

            //May update old score
            if (algorithm == Algorithm.AStart && Neighboor.Score > GScore)
            {
                BetterScore = true;
                Neighboor.DistanceToStart = DistanceToStart;
                Neighboor.EstimatedDistanceToEnd = DistnaceToEnd;
                Neighboor.Score = GScore;
                Neighboor.PriorNode = Current;

                Labels[(int)Neighboor.X, (int)Neighboor.Y].Content = Math.Round(GScore * 100) / 100;
            }
            else if (algorithm == Algorithm.Dijkstra && Neighboor.DistanceToStart > DistanceToStart)
            {
                BetterScore = true;
                Neighboor.DistanceToStart = DistanceToStart;
                Neighboor.PriorNode = Current;

                Labels[(int)Neighboor.X, (int)Neighboor.Y].Content = DistanceToStart;
            }

            //Check if neighboor has been found yet
            if (!Neighboor.IsFound)
            {
                Neighboor.IsFound = true;
                if (algorithm == Algorithm.AStart)
                    UnvisitedNodes.Enqueue(Neighboor, (float)Neighboor.Score);
                else if (algorithm == Algorithm.Dijkstra)
                    UnvisitedNodes.Enqueue(Neighboor, (float)Neighboor.DistanceToStart);

                //Mark node in canvas
                MarkNodeInCanvas(Neighboor);
            }

            //Update priority if new score is lower
            else if (Neighboor.IsFound && BetterScore)
            {
                if (algorithm == Algorithm.AStart)
                    UnvisitedNodes.UpdatePriority(Neighboor, (float)Neighboor.Score);
                else if (algorithm == Algorithm.Dijkstra)
                    UnvisitedNodes.UpdatePriority(Neighboor, (float)Neighboor.DistanceToStart);
            }
        }
        private void MarkNodeInCanvas(Node node)
        {
            if (node != EndNode)
                Rectangles[(int)node.X, (int)node.Y].Opacity = 0.4;
            if (calculationMode == CalculationMode.Slow)
                Wait(Rectangles[(int)node.X, (int)node.Y], CalculationDelay);
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
            int NumberOfObstacles = 0;
            foreach (Node node in AllNodes)
            {
                if (node.IsObstacle)
                    NumberOfObstacles++;
            }
            if (NumberOfObstacles == 0)
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
                node.IsFound = false;
                node.IsVisited = false;
                node.DistanceToStart = double.MaxValue;
                node.EstimatedDistanceToEnd = double.MaxValue;
                node.Score = double.MaxValue;
                node.PriorNode = null;
            }

            StartNode.DistanceToStart = 0.0;
        }

    }
}
