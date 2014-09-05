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
using System.ComponentModel;

namespace Random_Polygon
{
    /// <summary>
    /// Interaction logic for InputWindow.xaml
    /// </summary>
    public partial class InputWindow : Window
    {
        public InputWindow()
        {
            InitializeComponent();
            
        }

      

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            
            this.DialogResult = true;
            this.Close();

        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        private void textBox1_Pasting(object sender, DataObjectPastingEventArgs e)
        {

            if (e.DataObject.GetDataPresent(typeof(String)))
            {

                String text = (String)e.DataObject.GetData(typeof(String));

                if (!isNumberic(text))

                { e.CancelCommand(); }

            }

            else { e.CancelCommand(); }

        } 


        private void textBox1_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Space)

                e.Handled = true;
            else if (e.Key == Key.Enter)
            {
                e.Handled = true;
                this.SaveBtn_Click(sender, e);
            }

        } 

        private void textBox1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            if (!isNumberic(e.Text))
            {

                e.Handled = true;

            }

            else

                e.Handled = false;

        }





        //isDigit是否是数字 
        public static bool isNumberic(string _string)
        {

            if (string.IsNullOrEmpty(_string))

                return false;

            foreach (char c in _string)
            {

                if (!char.IsDigit(c))

                    //if(c<'0' c="">'9')//最好的方法,在下面测试数据中再加一个0，然后这种方法效率会搞10毫秒左右

                    return false;

            }

            return true;

        }
 
    }
}
