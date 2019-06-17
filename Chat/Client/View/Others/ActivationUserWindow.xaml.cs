using System;
using System.Windows;

namespace Client.View.Others
{
    /// <summary>
    /// Interaction logic for ActivationUserWindow.xaml
    /// </summary>
    public partial class ActivationUserWindow : Window
    {
        public ActivationUserWindow()
        {
            InitializeComponent();
            if (DataContext != null)
            {
                if (((dynamic)DataContext).CloseAction == null)
                    ((dynamic)DataContext).CloseAction = new Action(Close);
            }
        }
    }
}
