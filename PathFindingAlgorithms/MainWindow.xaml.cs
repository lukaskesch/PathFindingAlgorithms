﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PathFindingAlgorithms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Window
        public MainWindow()
        {
            InitializeComponent();
            applicationMode = ApplicationMode.NoGrid;
            editMode = EditMode.Nothing;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GroupBoxSettings.Visibility = Visibility.Collapsed;
            MenuItemEdit.IsEnabled = false;
            MenuItemSettings.IsEnabled = false;
            MenuItemAlgorithms.IsEnabled = false;
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RedrawCanvas();
        }
        #endregion


        #region File
        private void MenuItemFileCreate_Click(object sender, RoutedEventArgs e)
        {
            MenuItemSettings.IsEnabled = true;
            GroupBoxSettings.Visibility = Visibility.Visible;
        }
        private void MenuItemFileLoad_Click(object sender, RoutedEventArgs e)
        {



            MenuItemSettings.IsEnabled = true;
            MenuItemEdit.IsEnabled = true;
            MenuItemAlgorithms.IsEnabled = true;
        }
        private void MenuItemFileSave_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Settings
        private void MenuItemSettings_Click(object sender, RoutedEventArgs e)
        {
            GroupBoxSettings.Visibility = Visibility.Visible;
        }
        private void CheckBoxVisualization_Checked(object sender, RoutedEventArgs e)
        {
            if (StackPanelDelay != null)
                StackPanelDelay.Visibility = Visibility.Visible;
        }
        private void CheckBoxVisualization_Unchecked(object sender, RoutedEventArgs e)
        {
            StackPanelDelay.Visibility = Visibility.Hidden;
        }
        private void ButtonApplySettings_Click(object sender, RoutedEventArgs e)
        {
            GroupBoxSettings.Visibility = Visibility.Collapsed;
            Wait(GroupBoxSettings, 1);

            try
            {
                int _MaxX, _MaxY;
                _MaxX = int.Parse(TextBoxMaxX.Text);
                _MaxY = int.Parse(TextBoxMaxY.Text);

                MenuItemEdit.IsEnabled = true;
                MenuItemAlgorithms.IsEnabled = true;

                if (MaxX != _MaxX || MaxY != _MaxY)
                {
                    MaxX = _MaxX;
                    MaxY = _MaxY;
                    AllNodes = Node.FillNodesArray(MaxX, MaxY);
                    SetScale();
                    DrawCoordinateSystem();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Coordinates are invalid");
            }

            if (CheckBoxVisualization.IsChecked == true)
            {
                try
                {
                    CalculationDelay = int.Parse(TextBoxDelay.Text);
                    calculationMode = CalculationMode.Slow;
                }
                catch (Exception)
                {
                    MessageBox.Show("Delaytime isn't valid");
                }
            }
            else
            {
                calculationMode = CalculationMode.Fast;
            }
        }
        #endregion


        #region MenuItemsEdit
        private void MenuItemAddObstacles_Checked(object sender, RoutedEventArgs e)
        {
            MenuItemRemoveObstacles.IsChecked = false;
            editMode = EditMode.AddObstacle;
        }
        private void MenuItemObstacles_Unchecked(object sender, RoutedEventArgs e)
        {
            editMode = EditMode.Nothing;
        }
        private void MenuItemRemoveObstacles_Checked(object sender, RoutedEventArgs e)
        {
            MenuItemAddObstacles.IsChecked = false;
            editMode = EditMode.RemoveObstacle;
        }
        private void MenuItemSetStartpoint_Click(object sender, RoutedEventArgs e)
        {
            MenuItemAddObstacles.IsChecked = false;
            MenuItemRemoveObstacles.IsChecked = false;
            editMode = EditMode.SetStartpoint;
        }
        private void MenuItemSetEndpoint_Click(object sender, RoutedEventArgs e)
        {
            MenuItemAddObstacles.IsChecked = false;
            MenuItemRemoveObstacles.IsChecked = false;
            editMode = EditMode.SetEndpoint;
        }
        #endregion


        #region Canvas
        private void CanvasPath_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (applicationMode != ApplicationMode.Drawing || editMode == EditMode.Nothing)
            {
                return;
            }
        }
        private void CanvasPath_MouseMove(object sender, MouseEventArgs e)
        {
            if (applicationMode != ApplicationMode.Drawing || editMode == EditMode.Nothing)
            {
                return;
            }
        }
        #endregion

        #region Drawing
        private void SetScale()
        {
            dx = CanvasPath.ActualWidth / MaxX;
            dy = CanvasPath.ActualHeight / MaxY;
        }
        private void PlotToCanvas(ref Point P)
        {
            P.X *= dx;
            P.Y = (MaxY - P.Y) * dy;
        }
        private void PlotToCoordinateSystem(ref Point P)
        {
            P.X = Math.Floor(P.X / dx);
            P.Y = Math.Floor(MaxY - (P.Y / dy));
        }
        private void DrawCoordinateSystem()
        {
            Point P1, P2;

            P1 = new Point(0, 0);
            P2 = new Point(0, MaxY);
            for (int i = 0; i < MaxX + 1; i++)
            {
                P1.X = i;
                P2.X = i;
                DrawLine(P1, P2);
            }

            P1 = new Point(0, 0);
            P2 = new Point(MaxX, 0);
            for (int i = 0; i < MaxY + 1; i++)
            {
                P1.Y = i;
                P2.Y = i;
                DrawLine(P1, P2);
            }
        }
        private void DrawLine(Point P1, Point P2)
        {
            PlotToCanvas(ref P1);
            PlotToCanvas(ref P2);

            Line line = new Line()
            {
                X1 = P1.X,
                Y1 = P1.Y,
                X2 = P2.X,
                Y2 = P2.Y,
                Stroke = Brushes.Gray,
                StrokeThickness = 3
            };
            CanvasPath.Children.Add(line);
        }
        private void RedrawCanvas()
        {
            if (MaxX != 0)
            {
                CanvasPath.Children.Clear();
                SetScale();
                DrawCoordinateSystem();
            }
        }
        private void Wait(UIElement element, int CalculationDelay)
        {
            Action DummyAction = DoNothing;
            Thread.Sleep(CalculationDelay);
            element.Dispatcher.Invoke(DispatcherPriority.Input, DummyAction);
        }
        private void DoNothing()
        { }

        #endregion


    }
}
