using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Random_Polygon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

      

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ui_CadFileGenerater_Parent.IsSelected)
            {
                ui_CadFileGenerater.Initialize();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ui_CadFileGenerater.Close();
        }

    }
}
