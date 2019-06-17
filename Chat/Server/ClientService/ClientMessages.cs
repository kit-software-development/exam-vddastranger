﻿using CommandClient;
using Server.Interfaces;
using Server.ResponseMessages;
using System;
using System.Collections.Generic;

namespace Server.ClientService
{
    class ClientMessages : ServerResponds, IBuildResponse //класс, отвечающий за сообщения
    {
        public event EventHandler<ClientEventArgs> ClientMessage;
        public event EventHandler<ClientEventArgs> ClientChannelMessage;

        private List<Client> ListOfClientsOnline;

        public void Load(Client client, Data receive, List<Client> clientList = null, List<Channel> channelList = null)
        {
            Client = client;
            Received = receive;
            ListOfClientsOnline = clientList;
        }

        public void Execute()
        {
            prepareResponse();
            Send.strMessage = Received.strName + ": " + Received.strMessage;
            OnClientMessage(Send.strMessage, Received.strName + ": " + Received.strMessage);
        }

        public override void Response()
        {
            if (Received.strMessage2 == null)
            {
                SendMessageToAll sendToAll = new SendMessageToAll(Client, Send, ListOfClientsOnline);
                sendToAll.ResponseToAll();
            }
            else
            {
                string channelName = Received.strMessage2;
                SendMessageToChannel sendToChannel = new SendMessageToChannel(Send, ListOfClientsOnline, channelName);
                sendToChannel.ResponseToChannel();
                OnClientChannelMessage(Send.strMessage, Received.strName + ": " + Received.strMessage + " On:" + channelName);
            }
        }

        protected virtual void OnClientMessage(string cMessageToSend, string cMessageRecev)
        {
            ClientMessage?.Invoke(this, new ClientEventArgs() { clientMessageToSend = cMessageToSend, clientMessageReciv = cMessageRecev });
        }

        protected virtual void OnClientChannelMessage(string cMessageToSend, string cMessageRecev)
        {
            ClientChannelMessage?.Invoke(this, new ClientEventArgs() { clientMessageToSend = cMessageToSend, clientMessageReciv = cMessageRecev });
        }
    }
}
