using CommandClient;
using System.Collections.Generic;

namespace Server.ClientService.List
{
    class SendOnlineListOfClients : ClientListManager
    {
        public SendOnlineListOfClients(List<Client> clientList, Data send)
        {
            ListOfClientsOnline = clientList;
            Send = send;
        }

        public new void Execute()
        {
 
            Send.cmdCommand = Command.List;
            Send.strName = null;
            Send.strMessage = null;

            //обход коллекции юзеров, которые находятся в данной комнате
            foreach (Client client in ListOfClientsOnline)
            {
                //разделяем звездочкой ники всех юзеров
                Send.strMessage2 += client.strName + "*";
            }
            OnClientList(Send.strMessage2, "");
        }
    }
}
