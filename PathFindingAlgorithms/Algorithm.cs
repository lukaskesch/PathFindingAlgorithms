﻿using Priority_Queue;
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
        bool ShowAlgorithmScore, ShowNodes, IsDelay;
        private void MenuItemAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            MenuItemEdit_Click(sender, e);

            if (!CheckData()) return;
            if (!Prepare(sender)) return;

            UnvisitedNodes = new SimplePriorityQueue<Node>();

            stopwatch.Restart();

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
                if (CurrentNode != EndNode && ShowNodes)
                    Rectangles[(int)CurrentNode.X, (int)CurrentNode.Y].Opacity = 0.7;

                //Delay
                if (IsDelay)
                    Wait(Rectangles[(int)CurrentNode.X, (int)CurrentNode.Y], CalculationDelay);
            }

            stopwatch.Stop();

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
                    Rectangles[(int)CurrentNode.X, (int)CurrentNode.Y].Opacity = 0.7;
                    if (IsDelay)
                        Wait(Rectangles[(int)CurrentNode.X, (int)CurrentNode.Y], CalculationDelay / 2);
                }
                CurrentNode = CurrentNode.PriorNode;
            }

            PrintAlgorithmDetails();
        }
        private void UpdateNeighboorAStar(Node Current, Node Neighboor)
        {
            //Check if neighboor has been visited or is an obstacle
            if (Neighboor.IsVisited || Neighboor.IsObstacle)
            {
                return;
            }

            Label label;
            Point P;
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

                if (ShowAlgorithmScore)
                {
                    P = new Point(Neighboor.X, Neighboor.Y);
                    PlotToCanvas(ref P);
                    label = Labels[(int)Neighboor.X, (int)Neighboor.Y];
                    //CanvasPath.Children.Remove(label);
                    label.Content = Math.Round(GScore * 100) / 100;
                    if (IsDelay)
                        Wait(label, 0);
                    Canvas.SetLeft(label, P.X + 0.5 * (dx - label.ActualWidth));
                    Canvas.SetTop(label, P.Y - dy + 0.5 * (dy - label.ActualHeight));
                    //CanvasPath.Children.Add(label);
                }

            }
            else if (algorithm == Algorithm.Dijkstra && Neighboor.DistanceToStart > DistanceToStart)
            {
                BetterScore = true;
                Neighboor.DistanceToStart = DistanceToStart;
                Neighboor.PriorNode = Current;

                if (ShowAlgorithmScore)
                {
                    P = new Point(Neighboor.X, Neighboor.Y);
                    PlotToCanvas(ref P);
                    label = Labels[(int)Neighboor.X, (int)Neighboor.Y];
                    label.Content = Math.Round(DistanceToStart * 100) / 100;
                    if (IsDelay)
                        Wait(label, 0);
                    Canvas.SetLeft(label, P.X + 0.5 * (dx - label.ActualWidth));
                    Canvas.SetTop(label, P.Y - dy + 0.5 * (dy - label.ActualHeight));
                }

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
            if (node != EndNode && ShowNodes)
                Rectangles[(int)node.X, (int)node.Y].Opacity = 0.4;
            if (IsDelay)
                Wait(Rectangles[(int)node.X, (int)node.Y], CalculationDelay);
        }
        private void PrintAlgorithmDetails()
        {
            if (algorithm == Algorithm.AStart)
                LabelAlgorithmName.Content = "A*";
            else if (algorithm == Algorithm.Dijkstra)
                LabelAlgorithmName.Content = "Dijkstra";

            LabelRuntime.Content = stopwatch.ElapsedMilliseconds.ToString() + "ms";

            LabelDistance.Content = EndNode.DistanceToStart.ToString();

            GroupBoxAlgorithm.Visibility = Visibility.Visible;
            Wait(GroupBoxAlgorithm, 0);
            RedrawCanvas();
        }
        private bool CheckData()
        {
            MenuItemAddObstacles.IsChecked = false;
            MenuItemRemoveObstacles.IsChecked = false;

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
        private bool Prepare(object sender)
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



            if (CheckBoxDelayAlgorithm.IsChecked == true)
            {
                try
                {
                    CalculationDelay = int.Parse(TextBoxDelay.Text);
                    IsDelay = true;
                }
                catch (Exception)
                {
                    MessageBox.Show("Delaytime isn't valid");
                    return false;
                }
            }
            else IsDelay = false;

            if (CheckBoxVisualization.IsChecked == true) ShowNodes = true;
            else ShowNodes = false;

            if (CheckBoxShowNodeScore.IsChecked == true) ShowAlgorithmScore = true;
            else ShowAlgorithmScore = false;

            RedrawCanvas();
            return true;
        }

    }
}
