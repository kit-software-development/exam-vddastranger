using CommandClient;
using System.Collections.Generic;

namespace Server.ClientService.List
{
    class SendListOfUsersInChannel : ClientListManager
    {
        public SendListOfUsersInChannel(List<Channel> channelList, Data send, Data received)
        {
            ChannelsList = channelList;
            Send = send;
            Received = received;
        }

        // когда юзер заходит, он отправляет запрос на сервер с целью получить список юзеров, которые находятся в той же комнате
        public new void Execute()
        {
            Channel channel = ChannelGets.getChannelByName(ChannelsList, Received.strMessage2);
            if (channel != null)
            {
                foreach (string userName in channel.Users)
                    Send.strMessage3 += userName + "*";
            }

        }
    }
}
