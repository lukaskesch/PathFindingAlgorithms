using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace PathFindingAlgorithms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            applicationMode = ApplicationMode.NoGrid;
        }

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
            try
            {
                int _MaxX, _MaxY;
                _MaxX = int.Parse(TextBoxMaxX.Text);
                _MaxY = int.Parse(TextBoxMaxY.Text);

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
                throw;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GroupBoxSettings.Visibility = Visibility.Collapsed;
        }

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
            P.Y = Math.Floor((MaxY - P.Y) / dy);
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


    }
}
