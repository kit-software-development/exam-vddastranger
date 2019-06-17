using CommandClient;
using Server.Interfaces;
using System.Collections.Generic;

namespace Server.ResponseMessages
{
    class SendMessageToAll : Respond, IServerSend, IClient
    {
        private List<Client> ListOfClientsOnline;

        public Data Send { get; set; }
        public Client Client { get; set; }

        public SendMessageToAll(Client client, Data send, List<Client> clientList)
        {
            Send = send;
            ListOfClientsOnline = clientList;
            Client = client;
        }

        public void ResponseToAll()
        {
            foreach (Client cInfo in ListOfClientsOnline)
            {
                if (!cInfo.ignoredUsers.Contains(Client.strName)) // игнорируются такие методы, как login,logout,msg,msg
                    Response(Send.ToByte(), cInfo);
            }
        }
    }
}
