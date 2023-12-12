using Microsoft.UI.Xaml;

namespace MrCapitalQ.FollowAlong
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MyButton_Click(object sender, RoutedEventArgs e)
        {
            MyButton.Content = "Clicked";
        }
    }
}
