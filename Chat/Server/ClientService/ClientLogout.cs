using CommandClient;
using Server.Interfaces;
using Server.ResponseMessages;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server.ClientService
{
    class ClientLogout : ServerResponds, IBuildResponse //в принципе не используется пока, но можно прикрутить в будущем
    {
        private List<Client> ListOfClientsOnline;
        private List<Channel> ChannelsList;

        public event EventHandler<ClientEventArgs> ClientLogoutEvent;

        DataBaseManager db = DataBaseManager.Instance;

        public void Load(Client client, Data receive, List<Client> clientList = null, List<Channel> channelList = null)
        {
            Client = client;
            Received = receive;
            ChannelsList = channelList;
            ListOfClientsOnline = clientList;
        }

        public void Execute()
        {
            prepareResponse();
            int nIndex = 0;
            foreach (Client clientInList in ListOfClientsOnline)
            {
                if (Client.cSocket == clientInList.cSocket)
                {
                    ListOfClientsOnline.RemoveAt(nIndex);
                    break;
                }
                ++nIndex;

                foreach (Channel ch in ChannelsList) 
                {
                    if (ch.Users.Contains(Client.strName))
                        ch.Users.Remove(Client.strName);
                }
            }

            Send.strMessage = Client.strName;
            OnClientLogout(Client.strName, Client.cSocket);

            Client.cSocket.Close();
        }

        public override void Response()
        {
            SendMessageToAll sendToAll = new SendMessageToAll(Client, Send, ListOfClientsOnline);
            sendToAll.ResponseToAll();

            OnClientSendMessage(Send.strMessage);
        }

        protected virtual void OnClientLogout(string cName, Socket socket)
        {
            ClientLogoutEvent?.Invoke(this, new ClientEventArgs() { clientName = cName, clientSocket = socket });
        }
    }
}
