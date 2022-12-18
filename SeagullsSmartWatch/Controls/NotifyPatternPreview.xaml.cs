using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace SeagullsSmartWatch
{
    /// <summary>
    /// NotifyPatternPreview.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NotifyPatternPreview : UserControl
    {
        const double NODE_SIZE = 15;
        readonly SolidColorBrush NODE_COLOR = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1E90FF"));// Brushes.DodgerBlue;
        readonly SolidColorBrush ARROW_COLOR = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1E90FF"));// Brushes.DodgerBlue;

        int nodeCount = 0;

        Size previewSize = Size.Empty;
        Point firstNodePosition = new Point(130, 10);
        double nodeGap = 24;

        //참고
        //https://onlab94.tistory.com/353

        public NotifyPatternPreview()
        {
            InitializeComponent();
        }
        private void previewCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            previewSize = previewCanvas.RenderSize;
            //centerPosition = new Point(previewSize.Width * 0.5, previewSize.Height * 0.5);
            //firstNodePosition = new Point(previewSize.Width * 0.5, previewSize.Height * 0.1);

            UpdatePreview();
        }

        private void CreateStartNode()
        {
            double x = 0;
            double y = 0;

            // Create the rectangle
            Rectangle rec = new Rectangle()
            {
                Width = NODE_SIZE,
                Height = NODE_SIZE,
                Fill = Brushes.Green,
                Stroke = Brushes.Red,
                StrokeThickness = 2,
            };
            // Add to a canvas for example
            previewCanvas.Children.Add(rec);
            Canvas.SetLeft(rec, firstNodePosition.X - NODE_SIZE * 0.5);
            Canvas.SetTop(rec, firstNodePosition.Y - NODE_SIZE * 0.5);
        }

        private void CreateNode(int nodeNumber)
        {

            //Rectangle node = new Rectangle
            //{
            //    Fill = Brushes.DodgerBlue,
            //    Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#555599")),
            //    StrokeThickness = 2,
            //    Height = NODE_SIZE,
            //    Width = NODE_SIZE,
            //};

            //previewCanvas.Children.Add(node);
            //Canvas.SetLeft(node, nodePosition.X + originOffset.X);
            //Canvas.SetTop(node, nodePosition.Y + originOffset.Y);


            //Text
            //double textAngle = (currentAangle + nextAngle) * 0.5;
            //Point textPosition = new Point(firstNodePosition.X, firstNodePosition.Y - 25);
            //textPosition = SeagullMath.GetRotatedPointFromOrigin(firstNodePosition, centerPosition, textAngle);
            Point nodePosition = new Point(firstNodePosition.X, firstNodePosition.Y + nodeGap * nodeNumber);

            const int TEXT_WIDTH = 140;
            const int FONT_SIZE = 12;

            TextBlock timeText = new TextBlock()
            {
                Text = "보통 속도입니다",
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Left,
                Background = new SolidColorBrush(Colors.DodgerBlue),
                TextTrimming = TextTrimming.CharacterEllipsis,
                FontSize = FONT_SIZE,
                Width = TEXT_WIDTH,
                MaxWidth = TEXT_WIDTH,
            };

            Point textOffset = new Point(TEXT_WIDTH * 0.5, 0.0);

            previewCanvas.Children.Add(timeText);
            Canvas.SetLeft(timeText, nodePosition.X);
            Canvas.SetTop(timeText, nodePosition.Y);
        }

        //private void CreateArrow(int currentNode)
        //{
        //    double currentAangle = 360.0 / nodeCount * currentNode;
        //    double nextAngle = 360.0 / nodeCount * (currentNode + 1);

        //    Point startPosition = SeagullMath.GetRotatedPointFromOrigin(firstNodePosition, centerPosition, currentAangle);
        //    Point finishPosition = SeagullMath.GetRotatedPointFromOrigin(firstNodePosition, centerPosition, nextAngle);

        //    string arrowData = "M " + startPosition.X.ToString() + " " + startPosition.Y.ToString() + " " +
        //                       finishPosition.X.ToString() + " " + finishPosition.Y.ToString();

        //    Path startArrow = new Path()
        //    {r
        //        Data = Geometry.Parse(arrowData),
        //        Stroke = Brushes.Black,
        //    };
        //    previewCanvas.Children.Add(startArrow);

        //    //Path startArrowHead = new Path()
        //    //{
        //    //    Data = Geometry.Parse("M 130 40 140 50 130 60"),
        //    //    Stroke = Brushes.Black,
        //    //};
        //    //previewCanvas.Children.Add(startArrowHead);

        //    //Text
        //    Point originOffset = new Point(-NODE_SIZE * 0.5, -NODE_SIZE * 0.5);

        //    double textAngle = (currentAangle + nextAngle) * 0.5;
        //    Point textPosition = new Point(firstNodePosition.X, firstNodePosition.Y - 25);
        //    textPosition = SeagullMath.GetRotatedPointFromOrigin(firstNodePosition, centerPosition, textAngle);

        //    TextBlock timeText = new TextBlock()
        //    {
        //        Text = "10초",
        //        TextAlignment = TextAlignment.Center,
        //    };

        //    previewCanvas.Children.Add(timeText);
        //    Canvas.SetLeft(timeText, textPosition.X + originOffset.X);
        //    Canvas.SetTop(timeText, textPosition.Y + originOffset.Y);
        //}

        private void CreateStartArrow()
        {
            const double startTextX = 10;
            const double startTextY = 10;
            TextBlock startText = new TextBlock()
            {
                Text = "시작",
            };

            previewCanvas.Children.Add(startText);
            Canvas.SetLeft(startText, startTextX);
            Canvas.SetTop(startText, startTextY);

            Path startArrow = new Path()
            {
                Data = Geometry.Parse("M 20 30 120 30"),
                Stroke = Brushes.Black,
            };
            previewCanvas.Children.Add(startArrow);

            Path startArrowHead = new Path()
            {
                Data = Geometry.Parse("M 110 20 120 30 110 40"),
                Stroke = Brushes.Black,
            };
            previewCanvas.Children.Add(startArrowHead);
        }

        private void CreateFinishArrow()
        {

        }

        public void UpdatePreview()
        {
            previewCanvas.Children.Clear();
            CreateStartArrow();

            nodeCount = 14;
            //for (int i = 0; i < nodeCount; i++)
            //{
            //    CreateArrow(i);

            //}
            for (int i = 0; i < nodeCount; i++)
            {
                CreateNode(i);
            }
        }

    }
}
