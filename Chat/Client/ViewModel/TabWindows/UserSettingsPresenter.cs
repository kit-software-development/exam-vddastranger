using CommandClient;
using System.Security;
using System.Windows;
using System.Windows.Input;
using Client.Model;
using Client.ViewModel.Others;

namespace Client.ViewModel.TabWindows
{
    public class UserSettingsPresenter : ObservableObject
    {
        ClientSendToServer clientSendToServer = ClientSendToServer.Instance;
        ProcessReceivedByte proccesReceiverInformation = ProcessReceivedByte.Instance;

       

        Configuration<Settings> config = new Configuration<Settings>();
        Settings userSettings = new Settings();

        private bool loginNotyfiIsChecked;

        public bool LoginNotyfiIsChecked
        {
            get
            {
                return loginNotyfiIsChecked;
            }
            set
            {
                if (loginNotyfiIsChecked == value)
                    return;
                loginNotyfiIsChecked = value;
                RaisePropertyChangedEvent(nameof(LoginNotyfiIsChecked));
            }
        }
    }
}
