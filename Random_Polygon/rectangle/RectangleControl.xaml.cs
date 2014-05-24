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

namespace Random_Polygon.rectangle
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class RectangleControl : UserControl, INotifyPropertyChanged
    {
        public RectangleControl()
        {
            InitializeComponent();
            this.ui_condition.DataContext = this.m_condition;
            this.ui_result.DataContext = this;
            this.DataContext = this;
            layer_condaition.ItemsSource = this.ContidationList;

        }
        private Condition m_condition = new Condition(); 
       
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

        // 矩形分层填充控制条件
        ObservableCollection<Condition> m_contidationList = new ObservableCollection<Condition>();
        public ObservableCollection<Condition> ContidationList
        {
            get { return m_contidationList; }
            set { m_contidationList = value; }
        }

        private List<double> m_recover_radio = new List<double>();


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
        private void awesomelyFill(RectangleContainer container,Canvas ui_container, Condition condition)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < condition.IterCount; ++i)
            {
                if (canStopThread)
                {
                    break;
                }
                System.Drawing.Rectangle box = new System.Drawing.Rectangle();
                box.X = rand.Next(container.Width - 1) + 1;
                box.Y = rand.Next(container.Height - 1) + 1;
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

        public void genteraterRun(Condition condition, Canvas ui_containor)
        {
            RectangleContainer container = new RectangleContainer(0,0, condition.CWidth, condition.CHeight);

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

            m_recover_radio.Add(container.getCoverageRatio()*100); 
             
        }

        public void run(object oParameters)
        {
            ThreadParameters parameters = oParameters as ThreadParameters;
            List<Condition> conditionList = parameters.ConditionList.ToList<Condition>();
            sw.Start(); 
            foreach (Condition condition in conditionList)
            {
                try
                {
                    genteraterRun(condition, parameters.Ui_containor);
                }
                catch (System.Exception ex)
                {
                    Debug.Write("Wrong:" + ex.ToString());
                } 
               
            }

            sw.Stop();

            int layer = 1;
            CoverRadio = "";
            foreach(double radio in m_recover_radio)
            {
                CoverRadio += "" + layer + "层:" + radio + "%d" ; 
            }

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
        private void AddPolygon(ExtendedPolygon polygon, Canvas ui_container, Condition condition,RectangleContainer container)
        {

            ExtendedPolygon polygonTemp = new ExtendedPolygon();
            foreach (Point pt in polygon.Points)
            {
                polygonTemp.addPoint(new System.Drawing.Point((int)pt.X,(int)pt.Y));
            }
            polygonTemp.translate(condition.X, condition.Y);

            this.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(
              () =>
              {
                  CostTime = sw.ElapsedMilliseconds.ToString();
                  LogInfo = container.LogInfo;
                  CoverRadio = (container.getCoverageRatio() * 100).ToString();

                  Polygon ui_polygon = createPolygon(polygonTemp.Points);
                  ui_container.Children.Add(ui_polygon);

              }));
        }

        // 开启随机生成多面体
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Condation_Enable = false;
            canStopThread = false;
           
            rect_container.Width = m_condition.BoundaryHeight;
            rect_container.Height = m_condition.BoundaryHeight;
            rect_container.Visibility = Visibility.Visible;
            bg_draw.Children.Clear();
            m_recover_radio.Clear();
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
            ThreadParameters param = new ThreadParameters(ContidationList, bg_draw);
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

        private int getCurrentLayerHeight()
        {
            int height = 0;
            foreach (Condition cd in this.ContidationList)
            {
                height += cd.CHeight;
            }
            return height;
        }
        // 添加分层控制
        private void AddContidation_Click(object sender, RoutedEventArgs e)
        {
            // 判断是否层高是否大于总高 
            int height = m_condition.CHeight + getCurrentLayerHeight();

            if (height > m_condition.BoundaryHeight)
            {
                MessageBox.Show("超出范围", "提醒");
                return;
            }
            boundary_width.IsEnabled = false;
            boundary_height.IsEnabled = false;
            int currentHeight = getCurrentLayerHeight();
            m_condition.Y = currentHeight;
            
            Condition condition = new Condition(m_condition);
            this.ContidationList.Add(condition);
        }

        // 重置分层控制参数
        private void ResetContidation_Click(object sender, RoutedEventArgs e)
        {
            this.ContidationList.Clear();
            m_condition = new Condition();
            boundary_width.IsEnabled = true;
            boundary_height.IsEnabled = true;
        }

        // 删除选中的分层
        private void DeleteContidation_Click(object sender, RoutedEventArgs e)
        {
            int index = layer_condaition.SelectedIndex;
            if (-1 == index)
            {
                return;
            }
            this.ContidationList.RemoveAt(index);
        }
    }

    public class ThreadParameters
    {
        public ThreadParameters(ObservableCollection<Condition> conditionList, System.Windows.Controls.Canvas container)
        {
            this.m_condition_list = conditionList.ToList<Condition>();
            this.m_ui_containor = container;
        }
        private List<Condition> m_condition_list;
        public List<Condition> ConditionList
        {
            get { return m_condition_list; }
            set { m_condition_list = value; }
        }
        private System.Windows.Controls.Canvas m_ui_containor;
        public System.Windows.Controls.Canvas Ui_containor
        {
            get { return m_ui_containor; }
            set { m_ui_containor = value; }
        }
    }

}
