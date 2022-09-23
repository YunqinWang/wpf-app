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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

using System.Windows.Forms;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

       //set the img file types
        public static readonly List<string> ImageExtensions =
            new List<string> { ".JPG", ".JPEG", ".JPE", ".BMP", ".GIF", ".PNG" };
        
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
                BitmapImage img = new BitmapImage(new Uri(op.FileName));
                imgBox.Source = img;
                imgBox.Stretch = Stretch.Uniform;
            }
        }


        private Point startPoint;
        private Rectangle rect;

        //create a rectangle when mouseDown, set the position and style
        //append the rectangle to the canvas
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("MouseDown");
            startPoint = e.GetPosition(rectCanvas);

            rect = new Rectangle
            {
                Stroke = Brushes.SteelBlue,
                StrokeThickness = 4
            };
            Canvas.SetLeft(rect, startPoint.X);
            Canvas.SetTop(rect, startPoint.Y);
            rectCanvas.Children.Add(rect);
        }

        //track the mousemove to updat the rectangle size based on the cursor position
        private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("MouseMove");
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

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
           
        }

      
    }
}
