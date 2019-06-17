﻿using CommandClient;
using Server.ClientService.List;
using Server.Interfaces;
using System;
using System.Collections.Generic;

namespace Server.ClientService
{
    class ClientListManager : ServerResponds, IBuildResponse //класс, отвечающий за списки клиентов, скомпанованные по определенному признаку
    {
        protected DataBaseManager db = DataBaseManager.Instance;

        public event EventHandler<ClientEventArgs> ClientListEvent;
        protected List<Channel> ChannelsList;
        protected List<Client> ListOfClientsOnline;

        public void Load(Client client, Data receive, List<Client> clientList = null, List<Channel> channelList = null)
        {
            Received = receive;
            Client = client;
            ListOfClientsOnline = clientList;
            ChannelsList = channelList;
        }

        public void Execute()
        {
            prepareResponse();
            if (Received.strMessage == "Friends")
            {
                SendFriendsList sendFriends = new SendFriendsList(Client, Send);
                sendFriends.Execute();
            }

            else if (Received.strMessage == "ChannelUsers")
            {
                SendListOfUsersInChannel sendListOfUsersInChannel = new SendListOfUsersInChannel(ChannelsList, Send, Received);
                sendListOfUsersInChannel.Execute();
            }
            else
            {
                SendOnlineListOfClients sendOnlineUsers = new SendOnlineListOfClients(ListOfClientsOnline, Send);
                sendOnlineUsers.Execute();
            }
        }

        public override void Response()
        {
            if (Send.strMessage2 != null || Send.strMessage3 != null)
                base.Response();
        }

        protected string foreachInQuery(List<string> query)
        {
            string returnItems = null;
            foreach (var item in query)
            {
                returnItems += item + "*";
            }
            return returnItems;
        }

        protected virtual void OnClientList(string cMessage, string cMessage2)
        {
            ClientListEvent?.Invoke(this, new ClientEventArgs() { clientMessageToSend = cMessage, clientMessageTwoToSend = cMessage2 });
        }
    }
}
