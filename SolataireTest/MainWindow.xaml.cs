using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DraggableLabelDemo
{
    public partial class MainWindow : Window
    {
        private bool isDragging = false;
        private Point mouseOffset;

        public MainWindow()
        {
            InitializeComponent();
        }
        //hi
        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                isDragging = true;
                // Store the offset between mouse position and label's top-left corner
                mouseOffset = e.GetPosition(MyLabel);
                MyLabel.CaptureMouse();
            }
        }

        private void Label_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                // Get mouse position relative to the parent container
                Point position = e.GetPosition(MainCanvas);

                // Move label so that the mouse stays at the same offset
                Canvas.SetLeft(MyLabel, position.X - mouseOffset.X);
                Canvas.SetTop(MyLabel, position.Y - mouseOffset.Y);
            }
        }

        private void Label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            MyLabel.ReleaseMouseCapture();
        }
    }
}
