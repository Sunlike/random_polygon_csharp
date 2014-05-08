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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Threading;

namespace Random_Polygon
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window, INotifyPropertyChanged
    {
        public Window1()
        {
            InitializeComponent();
            this.ui_condition.DataContext = this.m_condition;
            this.ui_result.DataContext = this;

        }
        private Condition m_condition = new Condition();

        private RectangleContainer m_container = null;
        private string costTime = "0";
        public string CostTime
        {
            get { return costTime; }
            set { costTime = value; OnPropertyChanged("CostTime"); }
        }

        private string logInfo = "";
        public string LogInfo
        {
            get { return logInfo; }
            set { logInfo = value; OnPropertyChanged("LogInfo"); }
        }
        private string coverRadio = "0";
        public string CoverRadio
        {
            get { return coverRadio; }
            set { coverRadio = value; OnPropertyChanged("CoverRadio"); }
        }
        public static ExtendedPolygon randPolygonWithinBox(System.Drawing.Rectangle box, int maxEdgeNum, int minAngle, int maxAngle)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            int edgeNum = 3 + rand.Next(maxEdgeNum - 2);
            ExtendedPolygonBuilder pBuilder = new ExtendedPolygonBuilder();
            return pBuilder.buildPolygon(box, edgeNum, minAngle, maxAngle);
        }
        public static ExtendedPolygon randPolygonWithinBox(RectangleContainer box, int maxEdgeNum, int minRadius, int maxRadius, int minAngle, int maxAngle)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            int edgeNum = 3 + rand.Next(maxEdgeNum - 2);
            ExtendedPolygonBuilder pBuilder = new ExtendedPolygonBuilder(box);
            return pBuilder.buildPolygon(edgeNum, minRadius, maxRadius, minAngle, maxAngle);
        }

        // TODO(Rye): 1.   Randomly run points, pick those that are not in any of the polygons in container
        //              2.    For each point, change it into random boxes with a small unit bound.
        //              2.1.  Then increase there bounds by a small random step independently,
        //              3.    Go through the small boxes in 2.1, throw out those intersect with exist polygons
        //              3.1   For each of the rest boxes, randomly run polygons within
        //              3.2   Put the generated polygons into container
        //              4     Repeat for reasonable times
        private void awesomelyFill(Condition condition)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < condition.IterCount; ++i)
            {
                System.Drawing.Rectangle box = new System.Drawing.Rectangle();
                box.X = rand.Next(m_container.Width - 1) + 1;
                box.Y = rand.Next(m_container.Height- 1) + 1;
                box.Width = m_condition.MinRadius * 2;
                box.Height = box.Width;

                ExtendedPolygon polygon = null;
                bool bSuccess = false;
                for (int j = 0; j < condition.MaxRadius * 2; j += condition.ExpandStep)
                {
                    box.Width += condition.ExpandStep;
                    box.Height += condition.ExpandStep;

                    ExtendedPolygon tmpPolygon = randPolygonWithinBox(box, condition.MaxEdges, condition.MinAngle, condition.MaxAngle);
                    bSuccess = m_container.canSafePut(tmpPolygon);
                    if (bSuccess)
                    {
                        polygon = tmpPolygon;
                    }
                    else
                    {
                        break;
                    } 
                }

                if (polygon != null)
                {
                    m_container.put(polygon);

                    AddPolygon(polygon);
                    //double radio = m_container.getCoverageRatio() * 100;
                    //if (radio > m_condition.MinCoverRadio)
                    //{
                    //    return;
                    //}
                } 
            } 
        }

        public void run(Condition condition)
        {
            sw.Start();
           // try
            //{
           
            while (true)
            {
                
                   bool bSuccess = false;
                   ExtendedPolygon polygon = randPolygonWithinBox(m_container, condition.MaxEdges, condition.MinRadius, condition.MaxRadius, condition.MinAngle, condition.MaxAngle);
                   bSuccess = m_container.canSafePut(polygon);
                   if (!bSuccess)
                   {
                       for (int i = 0; i < 20; ++i)
                       {
                           Random rand = new Random(DateTime.Now.Millisecond);
                           int deltX = condition.StepX + rand.Next(3);
                           int deltY = condition.StepY;
                           polygon.translate(deltX, deltY);
                           bSuccess = m_container.canSafePut(polygon);
                           if (bSuccess)
                           {
                               m_container.put(polygon);
                               AddPolygon(polygon);
                               break;
                           }
                       }
                   }
                   else
                   {
                       m_container.put(polygon);
                       AddPolygon(polygon);
                   }


                   this.awesomelyFill(condition);
                    
                   double radio = m_container.getCoverageRatio()*100;
                   if (radio > condition.MinCoverRadio)
                   {
                       break;
                   }                     
            }
            //}
           // catch (System.Exception ex)
           // {
           //     Debug.WriteLine(ex.ToString());     
           // }
            sw.Stop();
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(
                () => 
                {
                CostTime = sw.ElapsedMilliseconds.ToString();
                LogInfo = m_container.LogInfo; 
                CoverRadio = (m_container.getCoverageRatio() * 100).ToString();
                }));
        }

        private Stopwatch sw = null;
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            m_container = new RectangleContainer(0, 0, m_condition.CWidth, m_condition.CHeight);
            rect_container.Width = m_condition.CWidth;
            rect_container.Height = m_condition.CHeight;
            rect_container.Visibility = Visibility.Visible;
            bg_draw.Children.Clear();
            LogInfo = "";
            CoverRadio = "0";
            bg_draw.Children.Add(rect_container);
            if (null == sw)
            {
                sw = new Stopwatch();
            }
            
           //Thread thread = new Thread( run);
           //thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
           //thread.Start();

            run(m_condition); 
          
            
        }

        // 异步函数，保证其他线程能在UI 线程上进行操作
        private void AddPolygon(ExtendedPolygon polygon)
        {
            sw.Stop();
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(
                () => 
                {
                    bg_draw.Children.Add(polygon.MPolygon); 
                }));
           
           sw.Start(); 
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
