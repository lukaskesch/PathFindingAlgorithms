using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
            string FileContent = ReadFile();
            if (FileContent == string.Empty)
            {
                MessageBox.Show("File is corrupt", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!LoadFile(FileContent))
            {
                MessageBox.Show("File is corrupt", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MenuItemFileSave.IsEnabled = true;
            MenuItemFileSaveAs.IsEnabled = true;
            MenuItemSettings.IsEnabled = true;
            MenuItemEdit.IsEnabled = true;
            MenuItemAlgorithms.IsEnabled = true;

            MessageBox.Show(FileContent);
        }
        private void MenuItemFileSave_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            CheckIfSaveFileIsPossible(menuItem);

            string Map = string.Empty;
            Map += MaxX.ToString();
            Map += Environment.NewLine + MaxY.ToString();
            Map += Environment.NewLine + StartNode.ToString();
            Map += Environment.NewLine + EndNode.ToString();
            foreach (Node obstacle in ObstacleNodes)
            {
                Map += Environment.NewLine + obstacle.ToString();
            }

            if (menuItem.Name == MenuItemFileSaveAs.Name || FilePath == string.Empty)
            {
                FileSave(menuItem, Map);
            }
            else if (menuItem.Name == MenuItemFileSave.Name)
            {
                FileSave(menuItem, Map);
            }
        }
        private bool CheckIfSaveFileIsPossible(MenuItem menuItem)
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
                string message = "You havn't set any obstacles. Do you still want to save the map?";
                MessageBoxButton messageBoxButton = MessageBoxButton.YesNo;
                MessageBoxImage messageBoxImage = MessageBoxImage.Question;
                MessageBoxResult dialogResult = MessageBox.Show(message, title, messageBoxButton, messageBoxImage);

                if (dialogResult == MessageBoxResult.No)
                {
                    return false;
                }
            }
            if (menuItem.Name == MenuItemFileSave.Name && FilePath == string.Empty)
            {
                //MessageBox.Show("The file hasn't been saved yet." + Environment.NewLine + "In order to save the file for the first time press \"save\".",
                //    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //return false;

            }
            return true;
        }
        private void FileSave(MenuItem menuItem, string FileContent)
        {
            if (menuItem.Name == MenuItemFileSaveAs.Name || FilePath == string.Empty)
            {
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Maps"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Maps");
                }

                SaveFileDialog Dlg = new SaveFileDialog();
                Dlg.Title = "Save Map";
                Dlg.DefaultExt = ".map";
                //Dlg.DefaultExt = ".txt";
                Dlg.Filter = "Maps (.map)|*.map";
                //Dlg.Filter = "Text documents (.txt)|*.txt";
                Dlg.FileName = "Beispiel.map";
                Dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Maps";

                bool? result = Dlg.ShowDialog();
                if (!result == true)
                {
                    return;
                }
                FilePath = Dlg.FileName;
            }
            StreamWriter streamWriter = new StreamWriter(FilePath, false);
            try
            {
                streamWriter.Write(FileContent);
            }
            finally
            {
                streamWriter.Close();
            }
        }
        private string ReadFile()
        {
            OpenFileDialog Dlg = new OpenFileDialog();
            Dlg.Title = "Save Map";
            //Dlg.DefaultExt = ".txt";
            Dlg.DefaultExt = ".map";
            //Dlg.Filter = "Text documents (.txt)|*.txt";
            Dlg.Filter = "Map (.map)|*.map";
            Dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Maps";

            bool? result = Dlg.ShowDialog();

            if (result != true)
            {
                return string.Empty;
            }

            FilePath = Dlg.FileName;
            StreamReader streamReader = new StreamReader(FilePath);

            string FileContent;
            try
            {
                FileContent = streamReader.ReadToEnd();
            }
            finally
            {
                streamReader.Close();
            }
            return FileContent;
        }
        private bool LoadFile(string FileContent)
        {
            return true;
        }
        #endregion

        #region Settings
        private void MenuItemSettings_Click(object sender, RoutedEventArgs e)
        {
            GroupBoxSettings.Visibility = Visibility.Visible;
            MenuItemEdit_Click(sender, e);
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
                MenuItemFileSave.IsEnabled = true;
                MenuItemFileSaveAs.IsEnabled = true;

                if (MaxX != _MaxX || MaxY != _MaxY)
                {
                    MaxX = _MaxX;
                    MaxY = _MaxY;
                    AllNodes = Node.FillNodesArray(MaxX, MaxY);
                    Rectangles = FillRectangeArray();
                    applicationMode = ApplicationMode.Drawing;
                    CanvasPath.Children.Clear();
                    SetScale();
                    DrawCoordinateSystem();
                    UpdateRectangeArray();
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
        private void MenuItemEdit_Click(object sender, RoutedEventArgs e)
        {
            if (applicationMode == ApplicationMode.Algorithm)
            {
                applicationMode = ApplicationMode.Drawing;
                ResetArrayToBeforeAlgorithm();
            }
        }
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
            //Set old color
            Rect.Fill = TempRect.Fill;

            //Set old opacity
            if (TempRect.Fill == Brushes.Gray) Rect.Opacity = TempRect.Opacity;
            else Rect.Opacity = TempRect.Opacity;

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

            //Set new Rect
            Rect = Rectangles[(int)P.X, (int)P.Y];

            //Save prior color
            TempRect.Opacity = 0.1;
            TempRect.Fill = Brushes.Gray;
            if (Rect.Fill == Brushes.Black) { TempRect.Fill = Brushes.Black; TempRect.Opacity = 1; }
            if (Rect.Fill == Brushes.Green) { TempRect.Fill = Brushes.Green; TempRect.Opacity = 1; }
            if (Rect.Fill == Brushes.Blue) { TempRect.Fill = Brushes.Blue; TempRect.Opacity = 1; }
            if (Rect.Fill == Brushes.Red) { TempRect.Fill = Brushes.Red; TempRect.Opacity = 1; };

            //Only allow gray or black background
            if (Rect.Fill != Brushes.Gray && Rect.Fill != Brushes.Black)
            {
                return;
            }
            //For remove obstacle only allow black background
            if (drawingMode == DrawingMode.RemoveObstacle && Rect.Fill == Brushes.Gray)
            {
                return;
            }
            //For add obstacle, add startpoint, or add enpoint only allow gray background
            if (drawingMode != DrawingMode.RemoveObstacle && Rect.Fill == Brushes.Black)
            {
                return;
            }

            //Set Rect
            Rect.Opacity = 0.8;
            SetRectColor(e);


            //Delete obstacle
            if (drawingMode == DrawingMode.RemoveObstacle && editMode == EditMode.Final)
            {
                TempRect.Fill = Brushes.Gray;
                TempRect.Opacity = 0.1;

                AllNodes[(int)P.X, (int)P.Y].IsObstacle = false; ;

                return;
            }

            //Handle Point and Rectange
            if (editMode == EditMode.Final)
            {
                TempRect.Fill = Rect.Fill;
                Rect.Opacity = 1;
                TempRect.Opacity = 1;

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
        private void SetRectColor(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                editMode = EditMode.Final;
            }

            switch (drawingMode)
            {
                case DrawingMode.AddObstacle:
                    Rect.Fill = Brushes.Black;
                    break;
                case DrawingMode.RemoveObstacle:
                    Rect.Fill = Brushes.Red;
                    break;
                case DrawingMode.SetStartpoint:
                    Rect.Fill = Brushes.Green;
                    break;
                case DrawingMode.SetEndpoint:
                    Rect.Fill = Brushes.Blue;
                    break;
                default:
                    break;
            }
        }
        private void HandlePointAndRectangle(Point P)
        {
            Node node = AllNodes[(int)P.X, (int)P.Y];

            switch (drawingMode)
            {
                case DrawingMode.AddObstacle:
                    node.IsObstacle = true;
                    break;
                case DrawingMode.SetStartpoint:
                    StartRect.Fill = Brushes.Gray;
                    StartRect.Opacity = 0.1;
                    StartRect = Rect;
                    StartNode = node;
                    break;
                case DrawingMode.SetEndpoint:
                    EndRect.Fill = Brushes.Gray;
                    EndRect.Opacity = 0.1;
                    EndRect = Rect;
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
                UpdateRectangeArray();
                DrawCoordinateSystem();
            }
        }
        private void UpdateRectangeArray()
        {
            Point P;
            Rectangle rectangle;

            for (int i = 0; i < MaxX; i++)
            {
                for (int j = 0; j < MaxY; j++)
                {
                    P = new Point(i, j);
                    rectangle = Rectangles[i, j];

                    PlotToCanvas(ref P);

                    rectangle.Height = dy - 1.5 * CoordinateLineWidth;
                    rectangle.Width = dx - 1.5 * CoordinateLineWidth;
                    Canvas.SetLeft(rectangle, P.X + CoordinateLineWidth);
                    Canvas.SetTop(rectangle, P.Y - dy + CoordinateLineWidth);

                    CanvasPath.Children.Add(rectangle);
                }
            }
        }
        private void ResetArrayToBeforeAlgorithm()
        {
            foreach (Rectangle rectangle in Rectangles)
            {
                //if ((rectangle.Fill == Brushes.Green || rectangle.Fill == Brushes.Red) && rectangle.Opacity != 1)
                //{
                //    rectangle.Fill = Brushes.Gray;
                //    rectangle.Opacity = 0.1;
                //}
                if (rectangle.Opacity < 1)
                {
                    rectangle.Fill = Brushes.Gray;
                    rectangle.Opacity = 0.1;
                }
            }
        }
        private Rectangle[,] FillRectangeArray()
        {
            Rectangle[,] rectangles = new Rectangle[MaxX, MaxY];

            for (int i = 0; i < MaxX; i++)
            {
                for (int j = 0; j < MaxY; j++)
                {
                    rectangles[i, j] = new Rectangle()
                    {
                        Opacity = 0.1,
                        Fill = Brushes.Gray
                    };
                }
            }

            return rectangles;
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
