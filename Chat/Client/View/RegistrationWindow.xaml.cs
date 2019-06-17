using System;
using Client.ViewModel;

namespace Client.View
{
    public partial class RegistrationWindow
    {
        public RegistrationWindow(LoginPresenter login)
        {
            InitializeComponent();
            if (DataContext != null)
            {
                ((dynamic)DataContext).loginPresenter = login;
                if (((dynamic)DataContext).CloseAction == null)
                    ((dynamic)DataContext).CloseAction = new Action(Close);
            }
        }

        private void RegistrationControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
