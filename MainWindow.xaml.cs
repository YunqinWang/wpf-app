using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// start from loading an image
    /// 1.draw multiple rectangles
    /// 2.resize the rectangle
    /// 3.recolor the rectnagle
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // action state, once a btn is clicked,
        enum Action
        {
            NotSet,
            Draw,
            ReColor,
            Resize,
        }
        // the currentAction will be set to the corresponding state
        Action currentAction = new Action();

        // update btn color if is activated/deactivated
        SolidColorBrush notActivate = Brushes.LightGray;
        SolidColorBrush activate = Brushes.Khaki;

        //set the img file types
        public static readonly List<string> ImageExtensions =
            new List<string> { ".JPG", ".JPEG", ".JPE", ".BMP", ".GIF", ".PNG" };

        //start point of the mousedown position
        private Point startPoint;

        //an array to store all created rectangles
        Rectangle[] rectList = new Rectangle[100];

        //total number of rectangles in the array
        //if the number >100, need to refactor
        private int numberOfRect = 0;

        //selected rectangle after mousedown
        private Rectangle selected = new Rectangle();

        //loaded img
        BitmapImage img = null;

        //when the "load image" btn is clicked, open a window to choose file
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";

            //show the file in the imgBox
            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                img = new BitmapImage(new Uri(op.FileName));
                imgBox.Source = img;
                imgBox.Stretch = Stretch.Uniform;
            }
        }

        //check which action state is at and call functions
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (currentAction)
            {
                case Action.NotSet:
                    break;
                case Action.Draw:
                    drawRect(e);
                    break;
                case Action.Resize:
                    resizeRect(e);
                    break;
                case Action.ReColor:
                    changeColor(e);
                    break;
                default:
                    return;
            } 
        }

        // check which action state is at and call functions
        private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            switch (currentAction)
            {
                case Action.NotSet:
                    break;
                case Action.Draw:
                    drawRectMove(e);
                    break;
                case Action.Resize:
                    resizeMove(e);
                    break;
                case Action.ReColor:
                    break;
                default:
                    return;
            }
        }

        // start draw rectangle when mousedown
        private void drawRect(MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(rectCanvas);
            
            numberOfRect++;
            rectList[numberOfRect - 1] = new Rectangle
            {
                Fill = Brushes.Magenta,
                Opacity = 0.4,
            };
            Rectangle rect = rectList[numberOfRect - 1];
            Canvas.SetLeft(rect, startPoint.X);
            Canvas.SetTop(rect, startPoint.Y);
            rect.MouseDown += new MouseButtonEventHandler(Canvas_MouseDown);
            rectCanvas.Children.Add(rect);
        }

        // update the newly drawn rectangle based on the cursor position
        private void drawRectMove(System.Windows.Input.MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(numberOfRect);
            if (numberOfRect == 0) return;
            Rectangle rect = rectList[numberOfRect - 1];
            if (e.LeftButton == MouseButtonState.Released || rect == null)
                return;

            var pos = e.GetPosition(rectCanvas);

            var x = Math.Min(pos.X, startPoint.X);
            var y = Math.Min(pos.Y, startPoint.Y);

            var w = Math.Max(pos.X, startPoint.X) - x;
            var h = Math.Max(pos.Y, startPoint.Y) - y;

            rect.Width = w;
            rect.Height = h;

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
        }

        // select the rectangle on cavas
        private void selelctRect(MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("selelctRect");
            Point cursor = e.GetPosition(rectCanvas);
            for(int i = 0; i <numberOfRect; i++)
            {
                if (cursor.X >= Canvas.GetLeft(rectList[i]) &&

                   cursor.Y >= Canvas.GetTop(rectList[i])
                   )
                {
                    System.Diagnostics.Debug.WriteLine("inside");
                    selected = rectList[i];
                    rectList[i].Stroke = Brushes.DarkRed;
                    rectList[i].StrokeThickness = 2;
                    return;
                }
            }
        }

        //resize the rect, get initial rectangle size when mouse is clicked
        private void resizeRect(MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(rectCanvas);
            selelctRect(e);
            Rectangle oldRect = new Rectangle();
            startPoint = e.GetPosition(selected);

            selected.Fill = Brushes.Green;
            selected.MouseDown += new MouseButtonEventHandler(Canvas_MouseDown);
        }

        //resize the rect, update the rectangle size when moving the mouse
        private void resizeMove(System.Windows.Input.MouseEventArgs e)
        {
            if (selected == null)
                return;
            if (e.LeftButton == MouseButtonState.Released)
            {
                selected.Fill = Brushes.Magenta;
                return;
            }

            var pos = e.GetPosition(rectCanvas);

            var originX = e.GetPosition(selected).X;
            var originY = e.GetPosition(selected).Y;

            var x = pos.X;
            var y = pos.Y;
            double newX,newY, w, h;
            //downdize
            if (pos.X > originX && pos.X < e.GetPosition(selected).X+selected.Width)
            {
                newX = originX + Math.Abs((pos.X - originX));
                newY = originY + Math.Abs((pos.Y - originY));
                w = Math.Max(20, selected.Width - Math.Abs((newX - originX)));
                h = Math.Max(20, selected.Height - Math.Abs((newY - originY)));
            }
            else // Upsize
            {
                newX = originX - Math.Abs((pos.X - originX));
                newY = originY - Math.Abs((pos.Y - originY));
                w = selected.Width + Math.Abs((newX - originX));
                h = selected.Height + Math.Abs((newY - originY));
            }
            
            selected.Width = w;
            selected.Height = h;

            Canvas.SetLeft(selected, x);
            Canvas.SetTop(selected, y);
        }
        
        // click a rectangle on canvas, pick a new color for the fill
        private void changeColor(MouseButtonEventArgs e)
        {
            selected = null;
            startPoint = e.GetPosition(rectCanvas);
            selelctRect(e);
            if (selected != null) {
                ColorDialog colorDialog = new ColorDialog();
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    System.Drawing.Color ColPicker = colorDialog.Color;
                    selected.Fill = new SolidColorBrush(Color.FromRgb(ColPicker.R, 
                        ColPicker.G, ColPicker.B));
                    System.Diagnostics.Debug.WriteLine(selected.Fill);
                }
            }
        }

      
        //set draw rectangle btn color
        private void btnDraw_Click(object sender, RoutedEventArgs e) {
            currentAction = Action.Draw;
            btnDraw.Background = activate;

            btnAdd.Background = notActivate;
            btnResize.Background = notActivate;
            btnRecolor.Background = notActivate;
        }

        //set resize btn color
        private void btnResize_Click(object sender, RoutedEventArgs e)
        {
            currentAction = Action.Resize;
            btnResize.Background = activate;

            btnAdd.Background = notActivate;
            btnDraw.Background = notActivate;
            btnRecolor.Background = notActivate;

        }

        //set recolor btn color
        private void btnRecolor_Click(object sender, RoutedEventArgs e)
        {
            currentAction = Action.ReColor;
            btnRecolor.Background = activate;

            btnAdd.Background = notActivate;
            btnDraw.Background = notActivate;
            btnResize.Background = notActivate;
        }
    }
}
