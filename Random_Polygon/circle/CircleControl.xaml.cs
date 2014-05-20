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
using Random_Polygon.circle;
using System.Windows.Threading;
using System.Diagnostics;
using System.Threading;

namespace Random_Polygon.circle
{
    /// <summary>
    /// Interaction logic for circle_polygon.xaml
    /// </summary>
    public partial class CircleControl : UserControl, INotifyPropertyChanged
    {
        public CircleControl()
        {
            InitializeComponent();
            ui_condition.DataContext = m_circle_condition; 
            this.ui_result.DataContext = this;
            this.DataContext = this;
           
        }

        private Circle_Condition m_circle_condition = new Circle_Condition();
         
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
        public static ExtendedPolygon randPolygonWithinBox(CircleContainer box, int maxEdgeNum, int minRadius, int maxRadius, int minAngle, int maxAngle)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            int edgeNum = 3 + rand.Next(maxEdgeNum - 2);
            Circle_PolygonBuilder pBuilder = new Circle_PolygonBuilder(box);
            return pBuilder.randPolygonWithCircle(edgeNum, minRadius, maxRadius, minAngle, maxAngle);
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
        private void awesomelyFill(CircleContainer container, Canvas ui_container, Circle_Condition condition)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < condition.IterCount; ++i)
            {
                if (canStopThread)
                {
                    break;
                }
                System.Drawing.Rectangle box = new System.Drawing.Rectangle();
                box.X = rand.Next((int)container.Radius*2 - 1) + 1;
                box.Y = rand.Next((int)container.Radius*2 - 1) + 1;
                box.Width = condition.MinRadius * 2;
                box.Height = box.Width;

                ExtendedPolygon polygon = null;
                bool bSuccess = false;
                for (int j = 0; j < condition.MaxRadius * 2; j += condition.ExpandStep)
                {
                    if (canStopThread)
                    {
                        break;
                    }
                    box.Width += condition.ExpandStep;
                    box.Height += condition.ExpandStep;

                    ExtendedPolygon tmpPolygon = randPolygonWithinBox(box, condition.MaxEdges, condition.MinAngle, condition.MaxAngle);
                    bSuccess = container.canSafePut(tmpPolygon);
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
                    container.put(polygon);


                    AddPolygon(polygon, ui_container, condition, container);
                }
            }
        }


        public void genteraterRun(Circle_Condition condition, Canvas ui_containor)
        {
            CircleContainer container = new CircleContainer(condition.Radius, condition.Radius, condition.Radius);

            while (true)
            {
                if (canStopThread)
                {
                    break;
                }
                bool bSuccess = false;
                ExtendedPolygon polygon = randPolygonWithinBox(container, condition.MaxEdges, condition.MinRadius, condition.MaxRadius, condition.MinAngle, condition.MaxAngle);
                bSuccess = container.canSafePut(polygon);
                if (!bSuccess)
                {
                    for (int i = 0; i < 20; ++i)
                    {
                        Random rand = new Random(DateTime.Now.Millisecond);
                        int deltX = condition.StepX + rand.Next(3);
                        int deltY = condition.StepY;
                        polygon.translate(deltX, deltY);
                        bSuccess = container.canSafePut(polygon);
                        if (bSuccess)
                        {
                            container.put(polygon);

                            AddPolygon(polygon, ui_containor, condition, container);

                            break;
                        }
                    }
                }
                else
                {
                    container.put(polygon);

                    AddPolygon(polygon, ui_containor, condition, container);

                }


                this.awesomelyFill(container, ui_containor, condition);

                double radio = container.getCoverageRatio() * 100;
                if (radio > condition.MinCoverRadio)
                {
                    break;
                }
            }

           

        }

        public void run(object parameters)
        {
            RThreadParameters threadParameter = parameters as RThreadParameters;
            genteraterRun(threadParameter.Condition, threadParameter.Ui_containor);

        }
        
        private Stopwatch sw = new Stopwatch();

        // 异步函数，保证其他线程能在UI 线程上进行操作
        private void AddPolygon(ExtendedPolygon polygon, Canvas ui_container, Circle_Condition condition, CircleContainer container)
        { 
            this.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(
              () =>
              {
                  CostTime = sw.ElapsedMilliseconds.ToString();
                  LogInfo = container.LogInfo;
                  CoverRadio = (container.getCoverageRatio() * 100).ToString();

                  Polygon ui_polygon = createPolygon(polygon.Points);
                  ui_container.Children.Add(ui_polygon);

              }));
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


        #endregion

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Condation_Enable = false;
            canStopThread = false;

            rect_container.Width = 2* m_circle_condition.Radius;
            rect_container.Height = 2 * m_circle_condition.Radius;
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
            RThreadParameters param = new RThreadParameters(m_circle_condition, bg_draw);
            thread.Start(param);
        }

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


    public class RThreadParameters
    {
        public RThreadParameters(Circle_Condition condition, System.Windows.Controls.Canvas container)
        {
            this.m_condition = condition;
            this.m_ui_containor = container;
        }
        private Circle_Condition m_condition;
        public Circle_Condition Condition
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
