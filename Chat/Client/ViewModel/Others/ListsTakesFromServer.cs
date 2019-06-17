using CommandClient;
using System;

namespace Client.ViewModel.Others
{
    class InformServerToSendUserLists
    {
        ClientSendToServer clientSendToServer = ClientSendToServer.Instance;

        private System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        int second = 0;

        public InformServerToSendUserLists()
        {
            dispatcherTimer.Tick += new EventHandler(SendToServerAsk);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void SendToServerAsk(object sender, EventArgs e)
        {
            if (second == 0)
                clientSendToServer.SendToServer(Command.List);
            else if (second == 1)
                clientSendToServer.SendToServer(Command.List, "IgnoredUsers"); 
            else if (second == 2)
                clientSendToServer.SendToServer(Command.List, "ChannelsJoined");
            else if (second == 3)
                clientSendToServer.SendToServer(Command.List, "Friends"); 
            else if (second == 4)
                clientSendToServer.SendToServer(Command.List, "Channel"); 
            if (second > 4)
                dispatcherTimer.Stop();
            second = second + 1;
        }
    }
}
