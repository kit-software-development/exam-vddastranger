using System.Windows.Controls;

namespace Client.View
{
    /// <summary>
    /// Interaction logic for MainContent.xaml
    /// </summary>
    public partial class MainContent : UserControl
    {
        public MainContent()
        {
            //MainContentPresenter presenter = new MainContentPresenter();
            //DataContext = presenter;
            // DataContext is added in MainProgramWindow.xaml file line:20
            InitializeComponent();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
