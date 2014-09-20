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
using System.IO;
using System.Xml.Serialization;

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
            this.ui_result.DataContext = this;
            this.DataContext = m_RatioConditionList;
            this.ui_condition.DataContext = m_uiCondition;
        }

        private LadderShapeRationCondition m_uiCondition = new LadderShapeRationCondition();

        private LadderShapeRationConditionList m_RatioConditionList = new LadderShapeRationConditionList();
        public LadderShapeRationConditionList RatioConditionList
        {
            get { return m_RatioConditionList; }
            set { m_RatioConditionList = value; }
        }

        private bool openRatio = false;
        public bool OpenRatio
        {
            get { return openRatio; }
            set
            {
                openRatio = value;
                if (!openRatio)
                {
                    m_uiCondition.ControlRatio.TargetRatio = 1.0;
                }
            }
        }

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
        private void awesomelyFill(LadderShapeContainer container)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < this.m_RatioConditionList.IterCount; ++i)
            {
                if (canStopThread)
                {
                    break;
                }
                LadderShapeRationCondition ratioControl = this.m_RatioConditionList.getMiniRatioControl();
                System.Drawing.Rectangle box = new System.Drawing.Rectangle();
                box.X = rand.Next((int)container.DownLayer - 1) + 1;
                box.Y = rand.Next((int)container.DownLayer - 1) + 1;
                box.Width = (int)ratioControl.Condition.MinRadius * 2;
                box.Height = box.Width;

                ExtendedPolygon polygon = null;
              
                bool bSuccess = false;
                for (int j = 0; j < ratioControl.Condition.MaxRadius * 2; j += ratioControl.Condition.ExpandStep)
                {
                    if (canStopThread)
                    {
                        break;
                    }
                    box.Width += ratioControl.Condition.ExpandStep;
                    box.Height += ratioControl.Condition.ExpandStep;

                    ExtendedPolygon tmpPolygon = randPolygonWithinBox(box, ratioControl.Condition.MaxEdges, ratioControl.Condition.MinAngle, ratioControl.Condition.MaxAngle);
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
                    AddPolygon(polygon, container);
                    ++ratioControl.ControlRatio.Count;
                    this.RatioConditionList.UpdateTotalCount();
                    ratioControl = this.m_RatioConditionList.getMiniRatioControl();
                }
            }
        }


        public void genteraterRun()
        {
            LadderShapeContainer container = new LadderShapeContainer(this.m_RatioConditionList.UpLayer, this.m_RatioConditionList.DownLayer, this.m_RatioConditionList.Height);

            while (true)
            {
                if (canStopThread)
                {
                    break;
                }
                bool bSuccess = false;
                LadderShapeRationCondition ratioControl = this.m_RatioConditionList.getMiniRatioControl();
                ExtendedPolygon polygon = randPolygonWithinBox(container, ratioControl.Condition.MaxEdges, ratioControl.Condition.MinRadius, ratioControl.Condition.MaxRadius, ratioControl.Condition.MinAngle, ratioControl.Condition.MaxAngle);
                bSuccess = container.canSafePut(polygon);
                if (!bSuccess)
                {
                    for (int i = 0; i < 20; ++i)
                    {
                        Random rand = new Random(DateTime.Now.Millisecond);
                        int deltX = ratioControl.Condition.StepX + rand.Next(3);
                        int deltY = ratioControl.Condition.StepY;
                        polygon.translate(deltX, deltY);
                        bSuccess = container.canSafePut(polygon);
                        if (bSuccess)
                        {
                            container.put(polygon);
                            AddPolygon(polygon, container);
                            ++ratioControl.ControlRatio.Count;
                            this.RatioConditionList.UpdateTotalCount();
                            break;
                        }
                    }
                }
                else
                {
                    container.put(polygon);
                    AddPolygon(polygon, container);
                    ++ratioControl.ControlRatio.Count;
                    this.RatioConditionList.UpdateTotalCount();
                }


                this.awesomelyFill(container);

                double radio = container.getCoverageRatio() * 100;
                if (radio > this.m_RatioConditionList.MinCoverRadio)
                {
                   
                    break;
                }
            } 

        }

        public void run()
        {
            sw.Start();         
            genteraterRun();
            sw.Stop();
            Condation_Enable = true;
        }

        private Stopwatch sw = new Stopwatch();

        // 异步函数，保证其他线程能在UI 线程上进行操作
        private void AddPolygon(ExtendedPolygon polygon, LadderShapeContainer container)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(
              () =>
              {
                  CostTime = sw.ElapsedMilliseconds.ToString();
                  LogInfo = container.LogInfo;
                  CoverRadio = (container.getCoverageRatio() * 100).ToString();

                  Polygon ui_polygon = createPolygon(polygon.Points);
                  this.m_RatioConditionList.Add(polygon.Points,polygon.CircleCenter,polygon.Radius);
                  this.bg_draw.Children.Add(ui_polygon);

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
            Button_Click_1(null, null); 
            if (!CheckOpenRatioControl())
            {
                return;
            }

            Condation_Enable = false;
            canStopThread = false;

            rect_container.Width = m_RatioConditionList.DownLayer;
            rect_container.Height = m_RatioConditionList.DownLayer;

            PointCollection pts = new PointCollection();
            LadderShape lshape = new LadderShape(m_RatioConditionList.UpLayer, m_RatioConditionList.DownLayer, m_RatioConditionList.Height);
            foreach (System.Drawing.Point pt in lshape.Points)
            {
                pts.Add(new Point(pt.X, pt.Y));
            }

            rect_container.Points = pts;

            rect_container.Visibility = Visibility.Visible;
            bg_draw.Children.Clear();
            m_RatioConditionList.ClearGeneraterInfo();

            LogInfo = "";
            CoverRadio = "0";
            bg_draw.Children.Add(rect_container);
            if (null == sw)
            {
                sw = new Stopwatch();
            }
            sw.Reset();

            // 工作线程，生成多边形
            m_thread = new Thread(new ThreadStart(run)); 
            m_thread.IsBackground = true;
           
            m_thread.Start();
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
        #region 比率控制
    
        //是否开启物料比率控制
        private bool CheckOpenRatioControl()
        {
            if (!this.OpenRatio)
            {
                this.m_RatioConditionList.RatioConditionList.Clear();
                this.m_RatioConditionList.RatioConditionList.Add(this.m_uiCondition.Clone());

            }

            if (1 == this.m_RatioConditionList.IsTargetOutLimited(0))
            {
                MessageBox.Show("目标比率之和已经超过100%！", "警告");
                return false;
            }
            else if (-1 == this.m_RatioConditionList.IsTargetOutLimited(0))
            {
                MessageBox.Show("目标比率之和不足100%,请调整目标比率！", "警告");
                return false;
            }

            return true;
        }

        //添加物料比率控制
        private void Button_AddClick(object sender, RoutedEventArgs e)
        {

            if (1 == this.m_RatioConditionList.IsTargetOutLimited(this.m_uiCondition.ControlRatio.TargetRatio))
            {
                MessageBox.Show("目标比率之和已经超过100%", "警告");
                return;
            }

            this.m_RatioConditionList.RatioConditionList.Add(this.m_uiCondition.Clone());
        }

        private void Button_DeleteSelectClick(object sender, RoutedEventArgs e)
        {
            LadderShapeRationCondition condition = this.ui_listview.SelectedItem as LadderShapeRationCondition;
            if (condition != null)
            {
                this.m_RatioConditionList.RatioConditionList.Remove(condition);
            }
        }

        private void Button_ClearClick(object sender, RoutedEventArgs e)
        {
            this.ui_listview.SelectedIndex = -1;
            this.m_RatioConditionList.RatioConditionList.Clear();
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.m_RatioConditionList.CostTime = this.CostTime;
                this.m_RatioConditionList.FinalRatio = this.CoverRadio + "%";
                save_tips.Text = "正在保存中……";
                string path = AppDomain.CurrentDomain.BaseDirectory + "laddershape";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string filename = path + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xml";

                using (FileStream stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(LadderShapeRationConditionList));
                    serializer.Serialize(stream, this.m_RatioConditionList);
                }
                save_tips.Text = "保存成功";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("保存失败", "错误", MessageBoxButton.OK);
            }
        }



        private void ctr_slider_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Common.ModifySliderValue(sender as Slider, e);
        }
        private Point m_oldPointX;
        private Point m_oldPointY;

        int fixType = 0;


        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            Window window = Window.GetWindow(bg_draw);

            Point standandOuterLeftPoint = bg_point.TransformToAncestor(window).Transform(new Point(0, 0));
            Point standandOuterRightPoint = bg_point.TransformToAncestor(window).Transform(new Point(bg_point.ActualWidth, bg_point.ActualHeight));


            double x = slOffsetX.Value;
            double y = slOffsetY.Value;
            switch (fixType)
            {
                case 1:
                    x = m_oldPointX.X;
                    if (m_oldPointX.X > slOffsetX.Value)
                        x = slOffsetX.Value;
                    break;
                case 2:
                    x = m_oldPointX.X;
                    if (m_oldPointX.X < slOffsetX.Value)
                        x = slOffsetX.Value;
                    break;
                case 3:
                    y = m_oldPointY.Y;
                    if (m_oldPointY.Y > slOffsetY.Value)
                        y = slOffsetY.Value;
                    break;
                case 4:
                    y = m_oldPointY.Y;
                    if (m_oldPointY.Y < slOffsetY.Value)
                        y = slOffsetY.Value;
                    break;
                case 0:
                    break;

            }

            m_oldPointY = m_oldPointX = bg_draw.Offset;

            Point newOffset = new Point(x, y);
            bg_draw.Offset = newOffset;
            Point standandInnerLeftPoint = bg_draw.TransformToAncestor(window).Transform(new Point(0, 0));
            Point standandInnerRightPoint = bg_draw.TransformToAncestor(window).Transform(new Point(bg_draw.ActualWidth, bg_draw.ActualHeight));


            if (standandOuterLeftPoint.X > standandInnerLeftPoint.X)
            {
                bg_draw.Offset = new Point(m_oldPointX.X, newOffset.Y);
                fixType = 1;

            }
            else if (standandOuterRightPoint.X < standandInnerRightPoint.X)
            {
                bg_draw.Offset = new Point(m_oldPointX.X, newOffset.Y);
                fixType = 2;
            }

            else if (standandOuterLeftPoint.Y > standandInnerLeftPoint.Y)
            {
                bg_draw.Offset = new Point(newOffset.X, m_oldPointY.Y);
                fixType = 3;
            }
            else if (standandOuterRightPoint.Y < standandInnerRightPoint.Y)
            {
                bg_draw.Offset = new Point(newOffset.X, m_oldPointY.Y);
                fixType = 4;
            }
            else
            {
                fixType = 0;
            }


        }


        public void NotifySizeChanged()
        {
            slOffsetX.Value = slOffsetY.Value = 0;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            double oPrecison = 0.0001;
            double iPrecision = 0.0001;
            if (!double.TryParse(outerPrecision.Text, out oPrecison))
            {
                MessageBox.Show("边界精度输入的不是浮点数，请重新输入。");
                return;
            }

            if (!double.TryParse(innerPrecision.Text, out iPrecision))
            {
                MessageBox.Show("内部精度输入的不是浮点数，请重新输入。");
                return;
            }

            Common.OuterPrecision = (float)oPrecison;
            Common.InnerPrecision = (float)iPrecision;
        }
    }


}
