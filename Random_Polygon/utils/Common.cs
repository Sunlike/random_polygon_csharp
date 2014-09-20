using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace Random_Polygon
{
    public partial class Common
    {
        public static void ModifySliderValue(Slider sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                InputWindow input = new InputWindow();

                Point pt = e.GetPosition(Application.Current.MainWindow);
                input.Left = pt.X - input.Width / 2 + Application.Current.MainWindow.Left;
                input.Top = pt.Y + 30 + +Application.Current.MainWindow.Top;
                input.tb_input.Text = ((int)sender.Value).ToString();
                if (true == input.ShowDialog())
                {
                    int result = 0;
                    if (!int.TryParse(input.tb_input.Text, out result))
                    {
                        result = 0;
                    }
                    sender.Value = result > sender.Maximum ? sender.Maximum : result < sender.Minimum ? sender.Minimum : result;
                }
            }
        }
    }
}
