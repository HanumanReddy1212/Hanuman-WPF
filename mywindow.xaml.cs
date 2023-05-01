using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using static System.Net.Mime.MediaTypeNames;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class MyRectangle
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string FillColor { get; set; }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public List<MyRectangle> Rectangles { get; set; } = new List<MyRectangle>();

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            if (openFileDialog.ShowDialog() == true)
            {
                SelectedImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }
        private bool isDragging = false;
        private Point startPoint;
        private SolidColorBrush currentColor = Brushes.Red;


        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangle = new Rectangle
            {
                Width = 100,
                Height = 100,
                Fill = currentColor,
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };

            Canvas.SetLeft(rectangle, e.GetPosition(canvas).X);
            Canvas.SetTop(rectangle, e.GetPosition(canvas).Y);

            MyRectangle myRectangle = new MyRectangle
            {
                Left = e.GetPosition(canvas).X,
                Top = e.GetPosition(canvas).Y,
                Width = rectangle.Width,
                Height = rectangle.Height,
                FillColor = currentColor.ToString()
            };

            Rectangles.Add(myRectangle);
            canvas.Children.Add(rectangle);
        }



        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(SelectedImage);
            isDragging = true;
            if (selectedRectangle != null)
            {
                selectedRectangle = null;
            }
        }
        private Rectangle selectedRectangle;

        private bool isResizing = false;
        private Point lastPoint;
        private ResizeDirection resizeDirection;

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPoint = e.GetPosition(SelectedImage);
                double x = Math.Min(startPoint.X, currentPoint.X);
                double y = Math.Min(startPoint.Y, currentPoint.Y);
                double w = Math.Abs(startPoint.X - currentPoint.X);
                double h = Math.Abs(startPoint.Y - currentPoint.Y);
                SelectionRectangle.Margin = new Thickness(x, y, 0, 0);
                SelectionRectangle.Width = w;
                SelectionRectangle.Height = h;
                SelectionRectangle.Visibility = Visibility.Visible;

                if (isResizing)
                {
                    double dx = currentPoint.X - lastPoint.X;
                    double dy = currentPoint.Y - lastPoint.Y;
                    double left = Canvas.GetLeft(selectedRectangle);
                    double top = Canvas.GetTop(selectedRectangle);
                    double width = selectedRectangle.ActualWidth;
                    double height = selectedRectangle.ActualHeight;
                    switch (resizeDirection)
                    {
                        case ResizeDirection.TopLeft:
                            left += dx;
                            top += dy;
                            width -= dx;
                            height -= dy;
                            break;
                        case ResizeDirection.Top:
                            top += dy;
                            height -= dy;
                            break;
                        case ResizeDirection.TopRight:
                            top += dy;
                            width += dx;
                            height -= dy;
                            break;
                        case ResizeDirection.Left:
                            left += dx;
                            width -= dx;
                            break;
                        case ResizeDirection.Right:
                            width += dx;
                            break;
                        case ResizeDirection.BottomLeft:
                            left += dx;
                            width -= dx;
                            height += dy;
                            break;
                        case ResizeDirection.Bottom:
                            height += dy;
                            break;
                        case ResizeDirection.BottomRight:
                            width += dx;
                            height += dy;
                            break;
                        default:
                            break;
                    }
                    if (width > 0 && height > 0)
                    {
                        Canvas.SetLeft(selectedRectangle, left);
                        Canvas.SetTop(selectedRectangle, top);
                        selectedRectangle.Width = width;
                        selectedRectangle.Height = height;
                    }
                    lastPoint = currentPoint;
                    e.Handled = true;
                }
            }
        }


        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            isResizing = false;
            resizeDirection = ResizeDirection.None;
        }
        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedRectangle != null)
            {
                selectedRectangle.Fill = ((Rectangle)sender).Fill;
                selectedRectangle = null;
            }
            else
            {
                selectedRectangle = (Rectangle)sender;
            }
            selectedRectangle = (Rectangle)sender;
            startPoint = e.GetPosition(SelectedImage);
            lastPoint = startPoint;
            isDragging = true;
            isResizing = false;
            resizeDirection = ResizeDirection.None;
            e.Handled = true;
            ((Rectangle)sender).CaptureMouse();
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point endPoint = e.GetPosition(null);
                double left = Canvas.GetLeft((Rectangle)sender);
                double top = Canvas.GetTop((Rectangle)sender);
                left += endPoint.X - startPoint.X;
                top += endPoint.Y - startPoint.Y;
                Canvas.SetLeft((Rectangle)sender, left);
                Canvas.SetTop((Rectangle)sender, top);
                startPoint = endPoint;
            }
        }

        private void Rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            ((Rectangle)sender).ReleaseMouseCapture();
        }


        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            currentColor = button.Background as SolidColorBrush;
            if (selectedRectangle != null)
            {
                selectedRectangle.Fill = button.Background;
                selectedRectangle = null;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (canvas.Children.Count > 0)
            {
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a RenderTargetBitmap with the same size as the image
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)SelectedImage.ActualWidth, (int)SelectedImage.ActualHeight, 96, 96, PixelFormats.Default);

            // Render the contents of the canvas to the RenderTargetBitmap
            renderTargetBitmap.Render(canvas);

            // Create a BitmapEncoder to save the bitmap
            PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
            pngBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            // Save the bitmap to a file
            using (FileStream fileStream = new FileStream("newImage.png", FileMode.Create))
            {
                pngBitmapEncoder.Save(fileStream);
            }
        }


        public enum ResizeDirection
        {
            None,
            TopLeft,
            Top,
            TopRight,
            Right,
            BottomRight,
            Bottom,
            BottomLeft,
            Left
        }

    }
}