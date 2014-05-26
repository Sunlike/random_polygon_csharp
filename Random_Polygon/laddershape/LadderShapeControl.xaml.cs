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

namespace Random_Polygon.laddershape
{
    /// <summary>
    /// Interaction logic for laddershape_polygon.xaml
    /// </summary>
    public partial class LadderShapeControl : UserControl, INotifyPropertyChanged
    {
        public LadderShapeControl()
        {
            InitializeComponent();
            ui_condition.DataContext = m_ladder_condition;
            this.ui_result.DataContext = this;
            this.DataContext = this;
            layer_condaition.DataContext = this.m_ladder_condition.RatioControlList;
        }

        private LadderShap_Condition m_ladder_condition = new LadderShap_Condition();

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
        public static ExtendedPolygon randPolygonWithinBox(System.Drawing.Rectangle box, int edgeNum, int minAngle, int maxAngle)
        {
            
            ExtendedPolygonBuilder pBuilder = new ExtendedPolygonBuilder();
            return pBuilder.buildPolygon(box, edgeNum, minAngle, maxAngle);
        }
        public static ExtendedPolygon randPolygonWithinBox(LadderShapeContainer box, int edgeNum, int minRadius, int maxRadius, int minAngle, int maxAngle)
        {   
            LadderShape_PolygonBuilder pBuilder = new LadderShape_PolygonBuilder(box);
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
        private void awesomelyFill(LadderShapeContainer container, Canvas ui_container, ref LadderShap_Condition condition)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < condition.IterCount; ++i)
            {
                if (canStopThread)
                {
                    break;
                }
                System.Drawing.Rectangle box = new System.Drawing.Rectangle();
                box.X = rand.Next((int)container.DownLayer - 1) + 1;
                box.Y = rand.Next((int)container.DownLayer - 1) + 1;
                box.Width = (int)condition.MinRadius * 2;
                box.Height = box.Width;

                ExtendedPolygon polygon = null;
                RatioControl ratioControl = condition.RatioControlList.getMinRatio();
                bool bSuccess = false;
                for (int j = 0; j < condition.MaxRadius * 2; j += condition.ExpandStep)
                {
                    if (canStopThread)
                    {
                        break;
                    }
                    box.Width += condition.ExpandStep;
                    box.Height += condition.ExpandStep;

                    ExtendedPolygon tmpPolygon = randPolygonWithinBox(box,  ratioControl.Key, condition.MinAngle, condition.MaxAngle);
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
                    condition.RatioControlList.UpdateCount(ratioControl.Key);
                    condition.RatioControlList.UpdateTotalCount(); 
                    ratioControl = condition.RatioControlList.getMinRatio();
                }
            }
        }


        public void genteraterRun(LadderShap_Condition condition, Canvas ui_containor)
        {
            LadderShapeContainer container = new LadderShapeContainer(condition.UpLayer, condition.DownLayer, condition.Height);

            while (true)
            {
                if (canStopThread)
                {
                    break;
                }
                bool bSuccess = false;
                RatioControl ratioControl = condition.RatioControlList.getMinRatio();
                ExtendedPolygon polygon = randPolygonWithinBox(container, ratioControl.Key, condition.MinRadius, condition.MaxRadius, condition.MinAngle, condition.MaxAngle);
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
                            condition.RatioControlList.UpdateCount(ratioControl.Key);
                            condition.RatioControlList.UpdateTotalCount(); 
                            break;
                        }
                    }
                }
                else
                {
                    container.put(polygon);
                    AddPolygon(polygon, ui_containor, condition, container);
                    condition.RatioControlList.UpdateCount(ratioControl.Key);
                    condition.RatioControlList.UpdateTotalCount(); 
                }


                this.awesomelyFill(container, ui_containor, ref condition);

                double radio = container.getCoverageRatio() * 100;
                if (radio > condition.MinCoverRadio)
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(
                                () =>
                                {
                                    this.m_ladder_condition.RatioControlList = condition.RatioControlList;

                                }));
                    break;
                }
            } 

        }

        public void run(object parameters)
        {
            sw.Start();
            LThreadParameters threadParameter = parameters as LThreadParameters;
            genteraterRun(threadParameter.Condition, threadParameter.Ui_containor);
            sw.Stop();
            Condation_Enable = true;
        }

        private Stopwatch sw = new Stopwatch();

        // 异步函数，保证其他线程能在UI 线程上进行操作
        private void AddPolygon(ExtendedPolygon polygon, Canvas ui_container, LadderShap_Condition condition, LadderShapeContainer container)
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


        #region  Button Event

        private Thread m_thread = null;
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Condation_Enable = false;
            canStopThread = false;

            rect_container.Width = m_ladder_condition.DownLayer;
            rect_container.Height = m_ladder_condition.DownLayer;

            PointCollection pts = new PointCollection();
            LadderShape lshape = new LadderShape(m_ladder_condition.UpLayer, m_ladder_condition.DownLayer, m_ladder_condition.Height);
            foreach (System.Drawing.Point pt in lshape.Points)
            {
                pts.Add(new Point(pt.X, pt.Y));
            }

            rect_container.Points = pts;

            rect_container.Visibility = Visibility.Visible;
            bg_draw.Children.Clear();
            m_ladder_condition.RatioControlList.ClearGeneraterInfo();

            LogInfo = "";
            CoverRadio = "0";
            bg_draw.Children.Add(rect_container);
            if (null == sw)
            {
                sw = new Stopwatch();
            }
            sw.Reset();

            // 工作线程，生成多边形
            m_thread = new Thread(new ParameterizedThreadStart(run));

            m_thread.IsBackground = true;
            LThreadParameters param = new LThreadParameters(m_ladder_condition, bg_draw);
            m_thread.Start(param);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            canStopThread = true;
            if (m_thread != null && m_thread.IsAlive)
            {
                m_thread.Abort(1000);
            }

            Condation_Enable = true;
            Debug.WriteLine("canStopThread = true");
        }

        #endregion
    }


    public class LThreadParameters
    {
        public LThreadParameters(LadderShap_Condition condition, System.Windows.Controls.Canvas container)
        {
            this.m_condition = condition;
            this.m_ui_containor = container;
        }
        private LadderShap_Condition m_condition;
        public LadderShap_Condition Condition
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
