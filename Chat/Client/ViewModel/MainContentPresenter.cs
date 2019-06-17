using CommandClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Client.Model;
using Client.View;
using Client.View.Others;
using Client.View.TabWindows;
using Client.ViewModel.Others;

namespace Client.ViewModel
{
    public class MainContentPresenter : ObservableObject, IClient
    {
        ClientSendToServer clientSendToServer = ClientSendToServer.Instance;
        ProcessReceivedByte proccesReceiverInformation = ProcessReceivedByte.Instance;
        PrivateMessageWindow privateMessage;
        public Action CloseAction;

        private ObservableCollection<object> tabControlItems = new ObservableCollection<object>();
        public ObservableCollection<object> TabControlItems { get { return tabControlItems; } }
        public int selectedTabControlIndex = 0;
        private readonly ObservableCollection<string> usersConnected = new ObservableCollection<string>();
        private readonly ObservableCollection<string> friendlyUsersConnected = new ObservableCollection<string>();
        private readonly ObservableCollection<string> ignoredUsers = new ObservableCollection<string>();
        private readonly ObservableCollection<string> lobbies = new ObservableCollection<string>();
        private readonly ObservableCollection<string> joinedChannelsList = new ObservableCollection<string>();
        public IEnumerable<string> UsersConnected => usersConnected;
        public IEnumerable<string> FriendlyUsersConnected => friendlyUsersConnected;

        private string selectedUser;
        private string selectedFriendlyUser;

        public MainContentPresenter(Action closeAction)
        {
            User = App.Client;
            CloseAction = closeAction;

            proccesReceiverInformation.ClientLogin += OnClientLogin;
            proccesReceiverInformation.ClientLogout += OnClientLogout;
            proccesReceiverInformation.ClientList += OnClientList;

            proccesReceiverInformation.ClientAddFriend += OnClientAddFriend;
            proccesReceiverInformation.ClientAcceptFriend += OnClientAcceptFriend;
            proccesReceiverInformation.ClientListFriends += OnClientListFriends;
            proccesReceiverInformation.ClientDeleteFriend += OnClientDeleteFriend;
            proccesReceiverInformation.ClientPrivMessage += OnClientPrivMessage;
            proccesReceiverInformation.ClientDenyFriend += (s, e) => MessageBox.Show("User: " + e.clientFriendName + " doesnt accept your ask to be your friend", "Chat: " + User.strName, MessageBoxButton.OK, MessageBoxImage.Information);

            InformServerToSendUserLists informServerToSendUserLists = new InformServerToSendUserLists();

            addTab(new GlobalMessageContent(), "main");
            SelectedTabControlIndex = 0;
        }

        public Model.Client User
        {
            get; set;
        }

        public int SelectedTabControlIndex
        {
            get { return selectedTabControlIndex; }

            set
            {
                selectedTabControlIndex = value;
                RaisePropertyChangedEvent(nameof(SelectedTabControlIndex));
            }
        }

        #region Selected ListBox Commands
        public string SelectedUser
        {
            get { return selectedUser; }

            set
            {
                selectedUser = value;
                RaisePropertyChangedEvent(nameof(SelectedUser));
            }
        }

        public string SelectedFriendlyUser
        {
            get { return selectedFriendlyUser; }

            set
            {
                selectedFriendlyUser = value;
                RaisePropertyChangedEvent(nameof(SelectedFriendlyUser));
            }
        }
        #endregion

        private bool background;
        public bool FriendLoginColor
        {
            get { return background; }
            set
            {
                background = value;
                RaisePropertyChangedEvent(nameof(FriendLoginColor));
            }
        }


        private void addTab<T>(T tabInstance, string tabName, bool channel = false)
        {
            var header = new TextBlock { Text = tabName };

            var tab = new CloseableTabItem();
            tab.SetHeader(header, tabName, ref tabControlItems, channel);
            tab.Content = tabInstance;
            tabControlItems.Add(tab);
        }


        #region ListBox Menu Items Buttons
        public ICommand AddFriendHandleCommand => new DelegateCommand(() =>
        {
            if (usersConnected.Contains(selectedUser) && selectedUser != App.Client.strName)
                clientSendToServer.SendToServer(Command.manageFriend, "Add", selectedUser); // There is send information to server that i add someone to friend list
        });

        private void PrivateMessage(string userName) //костыли конечно, но что поделать, сообщения в базе не хранятся
        {
            if (privateMessage == null)
            {
                privateMessage = new PrivateMessageWindow(userName);
                privateMessage.Show();
            }
            else MessageBox.Show("You allready private talk with " + userName, "Chat: " + App.Client.strName, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public ICommand PrivateMsgToUserCommand => new DelegateCommand(() =>
        {
            if (usersConnected.Contains(selectedUser) && selectedUser != App.Client.strName)
                PrivateMessage(selectedUser);
        });


        public ICommand PrivateMsgToFriendCommand => new DelegateCommand(() =>
        {
            string friendName = selectedFriendlyUser;

            if (friendlyUsersConnected.Contains(friendName) && usersConnected.Contains(friendName)) // Now if friend is in our friend list + his online(exists in clientList) 
                PrivateMessage(friendName);
            else MessageBox.Show("Your friend " + friendName + " is offline", "Chat: " + App.Client.strName, MessageBoxButton.OK, MessageBoxImage.Error);
        });

       
        public ICommand DeleteFriendCommand => new DelegateCommand(() =>
        {
            string friendName = selectedFriendlyUser;
            if (friendlyUsersConnected.Contains(friendName) && friendName != App.Client.strName)
                clientSendToServer.SendToServer(Command.manageFriend, "Delete", friendName);
        });

        private int GetLastTabControlElement()
        {
            int index = 0;
            foreach (var closeTableItem in tabControlItems)
            {
                index++;
            }
            return index;
        }

        #endregion


        #region Selected Lists Headers
        public string AddFriendHeaderCommand
        {
            get { return "add to friend list"; }
        }

        

        public string DeleteFriendHeaderCommand
        {
            get { return "delete from friend list"; }
        }

        public string PrivMsgHeaderCommand
        {
            get { return "send private message"; }
        }

        #endregion

        private void OnClientLogin(object sender, ClientEventArgs e)
        {
            if (!usersConnected.Contains(e.clientLoginName))
            {
                usersConnected.Add(e.clientLoginName);
                if (friendlyUsersConnected.Contains(e.clientLoginName))
                {
                    FriendLoginColor = true;
                }
            }
        }

        private void OnClientLogout(object sender, ClientEventArgs e)
        {
            usersConnected.Remove(e.clientLogoutMessage);
        }

        private void OnClientList(object sender, ClientEventArgs e)
        {
            if (e.clientListMessage != null)
            {
                string[] splitNicks = e.clientListMessage.Split('*').Where(value => value != "").ToArray();
                foreach (string nick in splitNicks)
                {
                    if (!usersConnected.Contains(nick))
                        usersConnected.Add(nick);
                }
            }
        }

        private void OnClientPrivMessage(object sender, ClientEventArgs e)
        {
            if (privateMessage == null)
            {
                privateMessage = new PrivateMessageWindow(e.clientFriendName, e.clientPrivMessage);
                privateMessage.Show();
            }
        }

        private void OnClientListChannel(object sender, ClientEventArgs e)
        {
            string[] splitChannels = e.clientListChannelsMessage.Split('*').Where(value => value != "").ToArray();
            foreach (string channel in splitChannels)
            {
                if (!lobbies.Contains(channel))
                    lobbies.Add(channel);
            }
        }

        private void OnClientAddFriend(object sender, ClientEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("User: " + e.clientFriendName + " want to be your friend. Accept?", "Chat: " + User.strName, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                clientSendToServer.SendToServer(Command.manageFriend, "No", e.clientFriendName);
            else
                clientSendToServer.SendToServer(Command.manageFriend, "Yes", e.clientFriendName);
        }

        private void OnClientAcceptFriend(object sender, ClientEventArgs e)
        {
            friendlyUsersConnected.Add(e.clientFriendName == User.strName ? e.clientName : e.clientFriendName);

            MessageBox.Show("You are now friend with: " + (e.clientFriendName == User.strName ? e.clientName : e.clientFriendName), "Chat: " + User.strName, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnClientListFriends(object sender, ClientEventArgs e)
        {
            string[] listFriends = e.clientListFriendsMessage.Split('*').Where(value => value != "").ToArray(); ;
            foreach (string friendName in listFriends)
            {
                if (!friendlyUsersConnected.Contains(friendName))
                {
                    friendlyUsersConnected.Add(friendName);
                    if (!usersConnected.Contains(friendName))
                        FriendLoginColor = false;
                }
            }
        }

        private void OnClientDeleteFriend(object sender, ClientEventArgs e)
        {
            friendlyUsersConnected.Remove(e.clientFriendName == User.strName ? e.clientName : e.clientFriendName);
        }

    }
}
