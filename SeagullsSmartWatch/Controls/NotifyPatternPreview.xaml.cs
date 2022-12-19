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
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;

namespace SeagullsSmartWatch
{
    /// <summary>
    /// NotifyPatternPreview.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NotifyPatternPreview : UserControl
    {
        const double NODE_SIZE = 15;
        
        //node
        readonly SolidColorBrush NODE_COLOR = new SolidColorBrush(Colors.DodgerBlue);
        const int NODE_TEXT_WIDTH = 72;
        const int NODE_FONT_SIZE = 13;

        //arrow
        readonly SolidColorBrush ARROW_COLOR = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#201B3E"));
        const int ARROW_THICKNESS = 2;

        //timetext
        readonly SolidColorBrush TIMETEXT_COLOR = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ADFFFFFF")); 
        const int TIMETEXT_FONT_SIZE = 12;
        const int TIMETEXT_WIDTH = 40;

        int nodeCount = 0;
        int selectedIndex = -1;

        Size previewSize = Size.Empty;
        Point firstNodePosition = new Point(200, 28);
        Point centerPosition = new Point(200, 200);

        public List<NotifyPatternData> notifyPatternDatas { get; set; } = null;

        //참고
        //https://onlab94.tistory.com/353

        public NotifyPatternPreview()
        {
            InitializeComponent();
        }
        private void previewCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            previewSize = previewCanvas.RenderSize;
            centerPosition = new Point(previewSize.Width * 0.5, previewSize.Height * 0.5);
            firstNodePosition = new Point(previewSize.Width * 0.5, previewSize.Height * 0.09);

            UpdatePreview();
        }

        private Color GetNodeBorderColor(Color nodeColor)
        {
            if(nodeColor.R > 128)
            {
                if (nodeColor.B < nodeColor.R)
                    return Colors.Blue;
            }

            return Colors.Red;
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
            double angle = 360.0 / nodeCount * nodeNumber;
            Point nodePosition = SeagullMath.GetRotatedPointFromOrigin(firstNodePosition, centerPosition, angle);

            //TextNode
            Point nodeDirection = new Point(nodePosition.X - centerPosition.X, nodePosition.Y - centerPosition.Y);
            nodeDirection = SeagullMath.Normalize(nodeDirection);

            string msg = "";
            string colorString = "#FFFFFF";
            if (notifyPatternDatas.Count > nodeNumber)
            {
                msg = notifyPatternDatas[nodeNumber].Message;
                colorString = notifyPatternDatas[nodeNumber].NotifyTextColor;
            }

            Color color = (Color)ColorConverter.ConvertFromString(colorString);
            Color fontColor = Colors.Black;

            int gray = 128 * 3;
            int totalRGB = color.R + color.G + color.B;
            if (totalRGB <= gray) 
            {
                fontColor = Colors.White;
            }

            //Boder
            Color borderColor = GetNodeBorderColor(color);
            Border border = null;
            if (selectedIndex == nodeNumber)
            {
                border = new Border()
                {
                    BorderBrush = new SolidColorBrush(borderColor),
                    BorderThickness = new Thickness(1),
                };
                previewCanvas.Children.Add(border);
            }


            TextBlock nodeText = new TextBlock()
            {
                Text = msg,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Background = new SolidColorBrush(color),
                TextTrimming = TextTrimming.CharacterEllipsis,
                Foreground = new SolidColorBrush(fontColor),
                FontSize = NODE_FONT_SIZE,
                Width = NODE_TEXT_WIDTH,
                MaxWidth = NODE_TEXT_WIDTH,
            };

            Point textOffset = new Point(-NODE_TEXT_WIDTH * 0.5, -NODE_FONT_SIZE * 0.5); //정중앙으로 만드는 오프셋
            Point circlePositionOffset = new Point(nodeDirection.X * (NODE_TEXT_WIDTH * 0.5) , nodeDirection.Y * (NODE_FONT_SIZE * 0.5)); //원의 바깥쪽으로 향하게 하는 오프셋

            if (border != null)
                border.Child = nodeText;
            else
                previewCanvas.Children.Add(nodeText);
            Canvas.SetLeft(nodeText, nodePosition.X + textOffset.X + circlePositionOffset.X);
            Canvas.SetTop(nodeText, nodePosition.Y + textOffset.Y + circlePositionOffset.Y);

            if (border != null)
            {
                Canvas.SetLeft(border, nodePosition.X + textOffset.X + circlePositionOffset.X);
                Canvas.SetTop(border, nodePosition.Y + textOffset.Y + circlePositionOffset.Y);
            }
        }

        private void CreateArrow(int currentNode)
        {
            double additionalAngle = 5;
            double currentAangle = 360.0 / nodeCount * currentNode + additionalAngle;
            double nextAngle = 360.0 / nodeCount * (currentNode + 1) - additionalAngle;

            //Arrow Line
            Point startPosition = SeagullMath.GetRotatedPointFromOrigin(firstNodePosition, centerPosition, currentAangle);
            Point finishPosition = SeagullMath.GetRotatedPointFromOrigin(firstNodePosition, centerPosition, nextAngle);

            string arrowData = "M " + startPosition.X.ToString() + " " + startPosition.Y.ToString() + " " +
                               finishPosition.X.ToString() + " " + finishPosition.Y.ToString();

            Path startArrow = new Path()
            {
                Data = Geometry.Parse(arrowData),
                Stroke = ARROW_COLOR,
                StrokeThickness = ARROW_THICKNESS,
            };
            previewCanvas.Children.Add(startArrow);

            //Arrow Head
            Point arrowDirection = new Point(startPosition.X - finishPosition.X, startPosition.Y - finishPosition.Y);
            arrowDirection = SeagullMath.Normalize(arrowDirection);

            const double HEAD_LENGTH = 15;
            Point arrowHead = new Point(arrowDirection.X * HEAD_LENGTH + finishPosition.X, arrowDirection.Y * HEAD_LENGTH + finishPosition.Y);
            Point arrowHeadLeft = SeagullMath.GetRotatedPointFromOrigin(arrowHead, finishPosition, 30);
            Point arrowHeadRight = SeagullMath.GetRotatedPointFromOrigin(arrowHead, finishPosition, -30);


            string arrowHeadData = "M " + arrowHeadLeft.X.ToString() + " " + arrowHeadLeft.Y.ToString() + " " +
                               finishPosition.X.ToString() + " " + finishPosition.Y.ToString() + " " +
                               arrowHeadRight.X.ToString() + " " + arrowHeadRight.Y.ToString();

            Path startArrowHead = new Path()
            {
                Data = Geometry.Parse(arrowHeadData),
                Stroke = ARROW_COLOR,
                StrokeThickness = ARROW_THICKNESS,
            };
            previewCanvas.Children.Add(startArrowHead);

            //Boder
            Border border = null;
            if (selectedIndex - 1 == currentNode ||
                (currentNode == nodeCount -1 && selectedIndex == 0)) //첫번째 패턴을 선택했을경우 맨마지막 시간도 붉은색 경계를 넣어준다
            {
                border = new Border()
                {
                    BorderBrush = new SolidColorBrush(Colors.Red),
                    BorderThickness = new Thickness(1),
                };
                previewCanvas.Children.Add(border);
            }

            //Time
            double textAngle = (currentAangle + nextAngle) * 0.5;
            Point textPosition = new Point(firstNodePosition.X, firstNodePosition.Y - 25);
            textPosition = SeagullMath.GetRotatedPointFromOrigin(firstNodePosition, centerPosition, textAngle);

            //TimeText
            int time = 0;
            if (notifyPatternDatas.Count > currentNode + 1)
                time = notifyPatternDatas[currentNode + 1].Time;
            else
                time = notifyPatternDatas[0].Time;

            TextBlock timeText = new TextBlock()
            {
                Text = time.ToString() + "초",
                FontSize = TIMETEXT_FONT_SIZE,
                Background = TIMETEXT_COLOR,
                TextAlignment = TextAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis,
                Width = TIMETEXT_WIDTH,
                MaxWidth = TIMETEXT_WIDTH,
            };
            Point textDirection = new Point(textPosition.X - centerPosition.X, textPosition.Y - centerPosition.Y);
            textDirection = SeagullMath.Normalize(textDirection);

            const double OFFSET_SCALE = 1.5;
            Point textOffset = new Point(-TIMETEXT_WIDTH * 0.5, -TIMETEXT_FONT_SIZE * 0.5); //정중앙으로 만드는 오프셋
            Point circlePositionOffset = new Point(-textDirection.X * (TIMETEXT_WIDTH * 0.5) * OFFSET_SCALE, -textDirection.Y * (TIMETEXT_FONT_SIZE * 0.5) * OFFSET_SCALE); //원의 안쪽으로 향하게 하는 오프셋

            if (border != null)
                border.Child = timeText;
            else
                previewCanvas.Children.Add(timeText);
            Canvas.SetLeft(timeText, textPosition.X + textOffset.X + circlePositionOffset.X);
            Canvas.SetTop(timeText, textPosition.Y + textOffset.Y + circlePositionOffset.Y);

            if (border != null)
            {
                Canvas.SetLeft(border, textPosition.X + textOffset.X + circlePositionOffset.X);
                Canvas.SetTop(border, textPosition.Y + textOffset.Y + circlePositionOffset.Y);
            }
        }

        private void CreateStartArrow()
        {
            const double startTextX = 20;
            const double startTextY = 25;
            //시작 메시지
            TextBlock startText = new TextBlock()
            {
                Text = "시작",
                Background = new SolidColorBrush(Colors.Yellow),
                FontWeight = FontWeights.Bold,
            };

            previewCanvas.Children.Add(startText);
            Canvas.SetLeft(startText, startTextX);
            Canvas.SetTop(startText, startTextY);

            //Arrow Line
            Path startArrow = new Path()
            {
                Data = Geometry.Parse("M 30 20 Q 105 -10 200 20"),
                Stroke = ARROW_COLOR,
                StrokeThickness = ARROW_THICKNESS,
            };
            previewCanvas.Children.Add(startArrow);

            //Arrow Head
            Path startArrowHead = new Path()
            {
                Data = Geometry.Parse("M 195 9 200 20 190 25"),
                Stroke = ARROW_COLOR,
                StrokeThickness = ARROW_THICKNESS,
            };
            previewCanvas.Children.Add(startArrowHead);

            //Boder
            Border border = null;
            if (selectedIndex == 0)
            {
                border = new Border()
                {
                    BorderBrush = new SolidColorBrush(Colors.Red),
                    BorderThickness = new Thickness(1),
                };
                previewCanvas.Children.Add(border);
                Canvas.SetLeft(border, 85);
                Canvas.SetTop(border, 5);
            }

            //TimeText
            int time = 0;
            if (notifyPatternDatas.Count > 0)
                time = notifyPatternDatas[0].Time;

            TextBlock timeText = new TextBlock()
            {
                Text = time.ToString() + "초",
                FontSize = TIMETEXT_FONT_SIZE,
                Background = TIMETEXT_COLOR,
                TextAlignment = TextAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis,
                Width = TIMETEXT_WIDTH,
                MaxWidth = TIMETEXT_WIDTH,
            };
            if (border != null)
                border.Child = timeText;
            else
                previewCanvas.Children.Add(timeText);

            Canvas.SetLeft(timeText, 85);
            Canvas.SetTop(timeText, 5);
        }

        private void CreateLoopArrow()
        {
            const double startTextX = 200;
            const double startTextY = 50;

            //Arrow Line
            Path startArrow = new Path()
            {
                Data = Geometry.Parse("M 220 50 A 50 50 0 1 0 260 50"),
                Stroke = ARROW_COLOR,
                StrokeThickness = ARROW_THICKNESS,
            };
            previewCanvas.Children.Add(startArrow);

            //Arrow Head
            Path startArrowHead = new Path()
            {
                Data = Geometry.Parse("M 206 46 220 50 216 67"),
                Stroke = ARROW_COLOR,
                StrokeThickness = ARROW_THICKNESS,
            };
            previewCanvas.Children.Add(startArrowHead);

            //Boder
            Border border = null;
            if (selectedIndex == 0)
            {
                border = new Border()
                {
                    BorderBrush = new SolidColorBrush(Colors.Red),
                    BorderThickness = new Thickness(1),
                };
                previewCanvas.Children.Add(border);
                Canvas.SetLeft(border, 220);
                Canvas.SetTop(border, 128);
            }

            //TimeText
            int time = 0;
            if (notifyPatternDatas.Count > 0)
                time = notifyPatternDatas[0].Time;

            TextBlock timeText = new TextBlock()
            {
                Text = time.ToString() + "초",
                FontSize = TIMETEXT_FONT_SIZE,
                Background = TIMETEXT_COLOR,
                TextAlignment = TextAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis,
                Width = TIMETEXT_WIDTH,
                MaxWidth = TIMETEXT_WIDTH,
            };
            if (border != null)
                border.Child = timeText;
            else
                previewCanvas.Children.Add(timeText);

            Canvas.SetLeft(timeText, 220);
            Canvas.SetTop(timeText, 128);
        }

        public void UpdatePreview(int _selectedIdx = -1)
        {
            previewCanvas.Children.Clear();
            selectedIndex = _selectedIdx;

            if (notifyPatternDatas == null)
                return;

            CreateStartArrow();

            nodeCount = notifyPatternDatas.Count;
            for (int i = 0; i < nodeCount; i++)
                CreateNode(i);

            if (nodeCount == 1)
            {
                CreateLoopArrow();
                return;
            }

            for (int i = 0; i < nodeCount; i++)
                CreateArrow(i);
        }

    }
}
