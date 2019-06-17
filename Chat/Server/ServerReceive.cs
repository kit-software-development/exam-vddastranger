using CommandClient;
using Server.ClientService;
using Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server
{
    class ServerReceive : IServerReceive, IChannelList, IClientList, IClient
    {
        public Data Received { get; set; }
        public List<Channel> ChannelsList { get; set; }
        public List<Client> ListOfClientsOnline { get; set; }
        public Client Client { get; set; }

        public ServerReceive(Client client, List<Client> clientList, List<Channel> channelList)
        {
            Client = client;
            ListOfClientsOnline = clientList;
            ChannelsList = channelList;
        }

        DataContext dataContext = new DataContext();

        ClientLogin clientLogin = new ClientLogin();
        ClientRegistration clientRegistration = new ClientRegistration();
        ClientSendActiveCode clientReSendActivCode = new ClientSendActiveCode();
        ClientLogout clientLogout = new ClientLogout();
        ClientMessages clientMessage = new ClientMessages();
        ClientListManager clientListManager = new ClientListManager();
        ClientPrivateMessage clientPrivateMessage = new ClientPrivateMessage();
        ManageClientFriend manageClientFriend = new ManageClientFriend();


        byte[] byteData = new byte[1024];

        public event EventHandler<ClientEventArgs> ClientReceiMessage;
        LoggerToFile servLogger = LoggerToFile.Instance;

        public void startReceiver()
        {
            Client.cSocket.BeginReceive(byteData, 0, byteData.Length, 0, new AsyncCallback(OnReceive), Client);
        }

        private void OnReceive(IAsyncResult ar)
        {
            Client client = (Client)ar.AsyncState;

            //из массива байт в Data объект
            Received = new Data(byteData);
            try
            {
                switch (Received.cmdCommand)
                {
                    case Command.Login:
                        dataContext.SetContext(clientLogin);
                        break;

                    case Command.Registration:
                        dataContext.SetContext(clientRegistration);
                        break;

                    case Command.SendActivationCode:
                        dataContext.SetContext(clientReSendActivCode);
                        break;

                    case Command.Logout:
                        dataContext.SetContext(clientLogout);
                        break;

                    case Command.Message:
                        dataContext.SetContext(clientMessage);
                        break;

                    case Command.privMessage:
                        dataContext.SetContext(clientPrivateMessage);
                        break;

                    case Command.List:
                        dataContext.SetContext(clientListManager);
                        break;

                    case Command.manageFriend:
                        dataContext.SetContext(manageClientFriend);
                        break;

                    default:
                        throw new ArgumentException("Wrong Package command from client");
                }
                dataContext.Load(client, Received, ListOfClientsOnline, ChannelsList);
                dataContext.Execute();
                dataContext.Response();

                ReceivedMessage(client, Received, byteData);
            }
            catch (Exception ex)
            {
                clientLogout.Send.cmdCommand = Command.Logout;
                clientLogout.Send.strName = Client.strName;
                clientLogout.Load(client, Received, ListOfClientsOnline, ChannelsList);
                clientLogout.Execute();
                string exMessage = "client: " + client.strName + " " + ex.Message;
                Console.WriteLine(exMessage);
                servLogger.Log(exMessage);

                clientLogout.Response();
            }
        }

        private void ReceivedMessage(Client conClient, Data msgReceived, byte[] byteData)
        {
            if (msgReceived.cmdCommand != Command.Logout)
                conClient.cSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnReceive), conClient);
            else if (msgReceived.strMessage != null)
                OnClientReceiMessage((int)msgReceived.cmdCommand, msgReceived.strName, msgReceived.strMessage, msgReceived.strMessage);
        }
        protected virtual void OnClientReceiMessage(int command, string cName, string cMessage, string cFriendName)
        {
            ClientReceiMessage?.Invoke(this, new ClientEventArgs() { clientCommand = command, clientName = cName, clientMessageReciv = cMessage, clientFriendName = cFriendName });// tu jeszcze nie wiem
        }
    }
}
