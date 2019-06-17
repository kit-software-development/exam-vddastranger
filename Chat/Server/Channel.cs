using System;
using System.Collections.Generic;

namespace Server
{
    public class Channel
    {
        string channelName;
        List<string> users = new List<string>(); // Использование для отправки списка пользователей, когда новый пользователь входит на этот канал
        Int64 founderiD;
        Int64 channelId;

        public Channel(Int64 ChannelId, string name, Int64 creatorId)
        {
            channelId = ChannelId;
            channelName = name;
            founderiD = creatorId;
        }

        public List<string> Users
        {
            get
            {
                return users;
            }

            set
            {
                users = value;
            }
        }

        public string ChannelName
        {
            get
            {
                return channelName;
            }

            set
            {
                channelName = value;
            }
        }

        public Int64 FounderiD
        {
            get
            {
                return founderiD;
            }

            set
            {
                founderiD = value;
            }
        }

        public Int64 ChannelId
        {
            get
            {
                return channelId;
            }

            set
            {
                channelId = value;
            }
        }
    }
}
