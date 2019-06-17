using CommandClient;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Client.Model;
using Client.View.Others;
using Client.ViewModel.Others;

namespace Client.ViewModel
{
    public class CloseableTabItem : TabItem
    {
        string tabControlName;
        ProcessReceivedByte getMessageFromServer = ProcessReceivedByte.Instance;
        ClientSendToServer clientSendToServer = ClientSendToServer.Instance;
        ObservableCollection<object> tb;

        bool isChannel = false;

        public void SetHeader(UIElement header, string name, ref ObservableCollection<object> tabControlItems, bool channel = false)
        {
            tabControlName = name;
            isChannel = channel;
            var dockPanel = new DockPanel();
            dockPanel.Children.Add(header);

            tb = tabControlItems;

            var closeButton = new TabCloseButton();
            closeButton.Click +=
                (sender, e) =>
                {
                    if (tabControlName != "main")
                    {
                        tb.Remove(this);
                        if (isChannel)
                            clientSendToServer.SendToServer(Command.leaveChannel, tabControlName);
                    }
                    else
                        MessageBox.Show("can't close main tab!", "Chat: " + App.Client.strName, MessageBoxButton.OK, MessageBoxImage.Error);
                };
            dockPanel.Children.Add(closeButton);

            Header = dockPanel;
        }

        private void removeTab()
        {
            tb.Remove(this);
        }
    }
}
