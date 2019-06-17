using CommandClient;
using Server.Interfaces;
using Server.ResponseMessages;
using System;
using System.Collections.Generic;

namespace Server.ClientService
{
    class ManageClientFriend : ServerResponds, IBuildResponse //класс, отвечающий за операции над friend-листом
    {
        public event EventHandler<ClientEventArgs> ClientAddFriend;
        public event EventHandler<ClientEventArgs> ClientDeleteFriend;

        DataBaseManager db = DataBaseManager.Instance;

        SendMessageToNick sendToNick;

        private List<Client> ListOfClientsOnline;

        string FriendName;

        public void Load(Client client, Data receive, List<Client> clientList = null, List<Channel> channelList = null)
        {
            Client = client;
            Received = receive;
            ListOfClientsOnline = clientList;
            Send.strName = Received.strMessage2;
            sendToNick = new SendMessageToNick(ListOfClientsOnline, Send);
        }

        public void Execute()
        {
            prepareResponse();
            string type = Received.strMessage;
            FriendName = Received.strMessage2;


            Int64 friend_id = GetFriendIdFromDB();
            if (friend_id > 0)
            {
                if (type == "Yes")
                {
                    bool InserClientToDb = AddFriendToDb(Client.id, friend_id); // добавление друга в бд
                    bool InserFriendToDb = AddFriendToDb(friend_id, Client.id);

                    if (InserClientToDb && InserFriendToDb)
                    {
                        OnClientAddFriend(Client.strName, FriendName);
                        Send.strMessage = "Yes";

                        ResponseToNick();
                    }
                    else
                    {
                        Send.strMessage = "No";
                        Send.strMessage2 = FriendName;
                    }
                }
                else if (type == "Delete")
                {
                    bool userDeleteFriend = DeleteFriendToDb(Client.id, friend_id);
                    bool friendDeleteUser = DeleteFriendToDb(friend_id, Client.id);
                    if (userDeleteFriend && friendDeleteUser)
                    {
                        Send.strMessage = "Delete";
                        ResponseToNick();
                        OnClientDeleteFriend(Client.strName, FriendName);
                    }
                }
                else if (type == "Add")
                {
                    ResponseToNick();
                }
                else if (type == "No")
                {
                    Send.strMessage = "No";
                    ResponseToNick();
                }
            }
            else
                Send.strMessage = "There is no friend that you want to add.";
        }

        private Int64 GetFriendIdFromDB()
        {
            db.bind("FriendName", FriendName);
            return Int64.Parse(db.singleSelect("SELECT id_user FROM users WHERE login = @FriendName"));
        }

        private void ResponseToNick()
        {
            sendToNick.Send = Send;
            sendToNick.Send.strMessage2 = Client.strName;
            sendToNick.Send.strName = FriendName;
            sendToNick.ResponseToNick();
        }

        public override void Response()
        {
            if (Send.strMessage == "No" || Send.strMessage == "Yes" || Send.strMessage == "Delete" || Send.strMessage == "There is no friend that you want to add.")
                base.Response();
        }

        // Used in ManageUserFriend function
        private bool AddFriendToDb(Int64 clientId, Int64 friendId)
        {
            db.bind(new string[] { "idUser", clientId.ToString(), "idFriend", friendId.ToString() });

            if (db.executeNonQuery("INSERT INTO user_friend (id_user, id_friend) " + "VALUES (@idUser, @idFriend)") > 0)
                return true;
            else
                return false;
        }

        private bool DeleteFriendToDb(Int64 clientId, Int64 friendId)
        {
            db.bind(new string[] { "idUser", clientId.ToString(), "idFriend", friendId.ToString() });

            if (db.executeNonQuery("DELETE FROM user_friend WHERE id_user = @idUser AND id_friend = @idFriend") > 0)
                return true;
            else
                return false;
        }

        protected virtual void OnClientAddFriend(string ClientName, string ClientFriendName)
        {
            ClientAddFriend?.Invoke(this, new ClientEventArgs() { clientName = ClientName, clientFriendName = ClientFriendName });
        }

        protected virtual void OnClientDeleteFriend(string ClientName, string ClientFriendName)
        {
            ClientDeleteFriend?.Invoke(this, new ClientEventArgs() { clientName = ClientName, clientFriendName = ClientFriendName });
        }
    }
}
