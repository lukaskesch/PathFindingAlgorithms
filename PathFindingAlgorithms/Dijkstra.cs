﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PathFindingAlgorithms
{
    public partial class MainWindow : Window
    {
        private void MenuItemDijkstraAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            if (!PrepareAlgorithm())
            {
                return;
            }
        }
    }
}
