using System.Windows.Controls;
using Client.ViewModel.TabWindows;

namespace Client.View.TabWindows
{

    public partial class GlobalMessageContent : UserControl
    {
        public GlobalMessageContent()
        {
            DataContext = new GlobalMessageContentPresenter();
            InitializeComponent();
        }
    }
}
