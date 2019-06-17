using CommandClient;
using Server.Interfaces;
using System.Collections.Generic;

namespace Server.ResponseMessages
{
    class SendMessageToSomeone : Respond, IServerSend, IServerReceive
    {
        List<Client> ListOfClientsOnline;

        public Data Send { get; set; }
        public Data Received { get; set; }

        public SendMessageToSomeone(List<Client> clientList, Data send)
        {
            Send = send;
            ListOfClientsOnline = clientList;
        }

        public void ResponseToSomeone()
        {
            foreach (Client cInfo in ListOfClientsOnline)
            {
                if (cInfo.strName == Received.strMessage2 || Received.strName == cInfo.strName)
                    Response(Send.ToByte(), cInfo);
            }
        }
    }
}
