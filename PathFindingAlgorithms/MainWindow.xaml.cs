using System;
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
            applicationMode = ApplicationMode.SetMap;
            drawingMode = DrawingMode.Nothing;
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
                    applicationMode = ApplicationMode.Drawing;
                    CanvasPath.Children.Clear();
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
            drawingMode = DrawingMode.AddObstacle;
        }
        private void MenuItemObstacles_Unchecked(object sender, RoutedEventArgs e)
        {
            drawingMode = DrawingMode.Nothing;
        }
        private void MenuItemRemoveObstacles_Checked(object sender, RoutedEventArgs e)
        {
            MenuItemAddObstacles.IsChecked = false;
            drawingMode = DrawingMode.RemoveObstacle;
        }
        private void MenuItemSetStartpoint_Click(object sender, RoutedEventArgs e)
        {
            MenuItemAddObstacles.IsChecked = false;
            MenuItemRemoveObstacles.IsChecked = false;
            drawingMode = DrawingMode.SetStartpoint;
        }
        private void MenuItemSetEndpoint_Click(object sender, RoutedEventArgs e)
        {
            MenuItemAddObstacles.IsChecked = false;
            MenuItemRemoveObstacles.IsChecked = false;
            drawingMode = DrawingMode.SetEndpoint;
        }
        #endregion


        #region Canvas
        private void CanvasPath_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            editMode = EditMode.Final;
            EditPoint(e);
        }
        private void CanvasPath_MouseMove(object sender, MouseEventArgs e)
        {
            editMode = EditMode.Preview;
            EditPoint(e);
        }
        private void EditPoint(MouseEventArgs e)
        {
            CanvasPath.Children.Remove(TempRect);

            //Check if drawing is necessary
            if (applicationMode != ApplicationMode.Drawing || drawingMode == DrawingMode.Nothing)
            {
                return;
            }

            //Convert mouseposition
            Point P = e.GetPosition(CanvasPath);
            PlotToCoordinateSystem(ref P);

            //Check if mouseposition is in bound
            if (P.X < 0 || P.X >= MaxX || P.Y < 0 || P.Y >= MaxY)
            {
                return;
            }

            //Convert back to canvas
            PlotToCanvas(ref P);

            //Prepare rectangles
            Rect = new Rectangle();
            SetRectParameters(P, e);
            CanvasPath.Children.Add(TempRect);

            //Delete obstacle
            if (drawingMode == DrawingMode.RemoveObstacle && editMode == EditMode.Final)
            {
                //Löschen
            }

            //Handle Point and Rectange
            if (editMode == EditMode.Final)
            {
                HandlePointAndRectangle(P);

                //Label lable = new Label()
                //{
                //    Content = "5",
                //    FontSize = 10,
                //    Foreground = Brushes.Red
                //    //Height = 50,
                //    //Width = 50
                //};
                //Canvas.SetLeft(lable, P.X + 0.5 * CoordinateLineWidth);
                //Canvas.SetTop(lable, P.Y - dy + 0.5 * CoordinateLineWidth);
                //CanvasPath.Children.Add(lable);
            }
        }
        private void SetRectParameters(Point P, MouseEventArgs e)
        {
            TempRect.Height = dy - 1 * CoordinateLineWidth;
            TempRect.Width = dx - 1 * CoordinateLineWidth;
            Canvas.SetLeft(TempRect, P.X + 0.5 * CoordinateLineWidth);
            Canvas.SetTop(TempRect, P.Y - dy + 0.5 * CoordinateLineWidth);
            TempRect.Fill = Brushes.Black;
            TempRect.Opacity = 0.6;

            Rect.Height = TempRect.Height;
            Rect.Width = TempRect.Width;
            Canvas.SetLeft(Rect, P.X + 0.5 * CoordinateLineWidth);
            Canvas.SetTop(Rect, P.Y - dy + 0.5 * CoordinateLineWidth);
            Rect.Fill = Brushes.Black;


            if (e.LeftButton == MouseButtonState.Pressed)
            {
                editMode = EditMode.Final;
            }

            switch (drawingMode)
            {
                case DrawingMode.RemoveObstacle:
                    TempRect.Fill = Brushes.Red;
                    break;
                case DrawingMode.SetStartpoint:
                    TempRect.Fill = Brushes.Green;
                    Rect.Fill = Brushes.Green;
                    break;
                case DrawingMode.SetEndpoint:
                    TempRect.Fill = Brushes.Blue;
                    Rect.Fill = Brushes.Blue;
                    break;
                default:
                    break;
            }
        }
        private void HandlePointAndRectangle(Point P)
        {
            PlotToCoordinateSystem(ref P);
            Node node = new Node(P.X, P.Y);

            switch (drawingMode)
            {
                case DrawingMode.Nothing:
                    break;
                case DrawingMode.AddObstacle:
                    if (!Node.CheckIfObstacleNodeAlreadyExist(ObstacleNodes, node))
                    {
                        ObstacleNodes.Add(node);
                        CanvasPath.Children.Add(Rect);
                    }
                    break;
                case DrawingMode.RemoveObstacle:
                    if (Node.CheckIfObstacleNodeAlreadyExist(ObstacleNodes, node))
                    {
                        //Vermutlich eigene Methode schreiben
                        ObstacleNodes.Remove(node);
                    }
                    break;
                case DrawingMode.SetStartpoint:
                    if (!CanvasPath.Children.Contains(StartRect))
                    {
                        StartRect = Rect;
                        CanvasPath.Children.Add(StartRect);
                    }
                    else
                    {
                        CanvasPath.Children.Remove(StartRect);
                        StartRect = Rect;
                        CanvasPath.Children.Add(StartRect);
                    }
                    StartNode = node;
                    break;
                case DrawingMode.SetEndpoint:
                    if (!CanvasPath.Children.Contains(EndRect))
                    {
                        EndRect = Rect;
                        CanvasPath.Children.Add(EndRect);
                    }
                    else
                    {
                        CanvasPath.Children.Remove(EndRect);
                        EndRect = Rect;
                        CanvasPath.Children.Add(EndRect);
                    }
                    EndNode = node;
                    break;
                default:
                    break;
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
                StrokeThickness = CoordinateLineWidth
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
