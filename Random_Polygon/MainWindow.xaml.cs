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
using System.Collections.ObjectModel;

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
            this.DataContext = this;

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

        private bool m_condation_enable = true;
        public bool Condation_Enable
        {
            get { return m_condation_enable; }
            set { m_condation_enable = value; OnPropertyChanged("Condation_Enable"); }
        }

        private bool canStopThread = false;


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

        #region 随机生成多边形的方法接口

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
        #endregion
        
        #region 随机生成多边形的算法

        // TODO(Rye): 1.   Randomly run points, pick those that are not in any of the polygons in container
        //              2.    For each point, change it into random boxes with a small unit bound.
        //              2.1.  Then increase there bounds by a small random step independently,
        //              3.    Go through the small boxes in 2.1, throw out those intersect with exist polygons
        //              3.1   For each of the rest boxes, randomly run polygons within
        //              3.2   Put the generated polygons into container
        //              4     Repeat for reasonable times
        private void awesomelyFill(ThreadParameters parameters)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < parameters.Condition.IterCount; ++i)
            {
                if (canStopThread)
                {
                    break;
                }
                System.Drawing.Rectangle box = new System.Drawing.Rectangle();
                box.X = rand.Next(m_container.Width - 1) + 1;
                box.Y = rand.Next(m_container.Height - 1) + 1;
                box.Width = m_condition.MinRadius * 2;
                box.Height = box.Width;

                ExtendedPolygon polygon = null;
                bool bSuccess = false;
                for (int j = 0; j < parameters.Condition.MaxRadius * 2; j += parameters.Condition.ExpandStep)
                {
                    if (canStopThread)
                    {
                        break;
                    }
                    box.Width += parameters.Condition.ExpandStep;
                    box.Height += parameters.Condition.ExpandStep;

                    ExtendedPolygon tmpPolygon = randPolygonWithinBox(box, parameters.Condition.MaxEdges, parameters.Condition.MinAngle, parameters.Condition.MaxAngle);
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
                    AddPolygon(polygon, parameters); 
                }
            }
        }

        public void run(object oParameters)
        {
            ThreadParameters parameters = oParameters as ThreadParameters;
            Condition condition = parameters.Condition;

            sw.Start();
            
            while (true)
            {
                if (canStopThread)
                {
                    break;
                }
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
                            AddPolygon(polygon, parameters);

                            break;
                        }
                    }
                }
                else
                {
                    m_container.put(polygon);
                    AddPolygon(polygon, parameters);

                }


                this.awesomelyFill(parameters);

                double radio = m_container.getCoverageRatio() * 100;
                if (radio > condition.MinCoverRadio)
                {
                    break;
                }
            }
           
            sw.Stop();
            Condation_Enable = true;

        }
        #endregion

        private Stopwatch sw = null; 
       

        private Brush getRandomBrush()
        {
            Random rand = new Random();

            byte r = Convert.ToByte(rand.Next(255));
            byte g = Convert.ToByte(rand.Next(255));
            byte b = Convert.ToByte(rand.Next(255));
            SolidColorBrush br = new SolidColorBrush();
            br.Color = Color.FromRgb(r, g, b);
            return br;
        }

        // 创建ui多边形
        private Polygon createPolygon(List<Point> pts)
        {
            Polygon polygon = new Polygon();
            foreach (Point pt in pts)
            {
                polygon.Points.Add(pt);
            }

            polygon.Stroke = getRandomBrush(); ;
            polygon.Fill = System.Windows.Media.Brushes.LightSeaGreen;
            polygon.StrokeThickness = 1;
            return polygon;
        }
        // 异步函数，保证其他线程能在UI 线程上进行操作
        private void AddPolygon(ExtendedPolygon polygon, ThreadParameters parameters)
        {
            Canvas container = parameters.Ui_containor;
            this.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(
              () =>
              {
                  CostTime = sw.ElapsedMilliseconds.ToString();
                  LogInfo = m_container.LogInfo;
                  CoverRadio = (m_container.getCoverageRatio() * 100).ToString();

                  Polygon ui_polygon = createPolygon(polygon.Points);
                  bg_draw.Children.Add(ui_polygon); 

              }));


        }
 
        // 开启随机生成多面体
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Condation_Enable = false;
            canStopThread = false;
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

            // 工作线程，生成多边形
            Thread thread = new Thread(new ParameterizedThreadStart(run));
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            //thread.IsBackground = true;
            ThreadParameters param = new ThreadParameters(m_condition, bg_draw);
            thread.Start(param); 
        }
        // 终止线程
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(
             () =>
             {
                 canStopThread = true;
                 Debug.WriteLine("canStopThread = true");
             }));
        }
    }

    class ThreadParameters
    {
        public ThreadParameters(Condition condition, System.Windows.Controls.Canvas container)
        {
            this.m_condition = condition;
            this.m_ui_containor = container;
        }
        private Condition m_condition;
        public Random_Polygon.Condition Condition
        {
            get { return m_condition; }
            set { m_condition = value; }
        }
        private System.Windows.Controls.Canvas m_ui_containor;
        public System.Windows.Controls.Canvas Ui_containor
        {
            get { return m_ui_containor; }
            set { m_ui_containor = value; }
        }
    }

}
