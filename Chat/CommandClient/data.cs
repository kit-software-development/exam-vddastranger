using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandClient
{
    //команды для взаимодействия клиента с сервером
    public enum Command
    {
        NoCommand,      
        Login,          
        Logout,         //выход
        Message,        //послать сообщение всем клиентам
        List,           //получить список всех юзеров, друзей, каналов
        privMessage,    //приватные сообщения от друзей

        Registration,   
        SendActivationCode,    //послать код активации клиенту на ящик

        changePassword, 
        lostPassword,   

        createChannel,  //создание комнаты юзером
        joinChannel,    //присоединиться к комнате
        exitChannel,    //выйти из комнаты
        deleteChannel,  //удалить комнату
        editChannel,    //изменить комнату 
        leaveChannel,   //покинуть комнату
        enterChannel,   //информировать юзером о подключении нового 
        kickUserChannel,//кик из комнаты
        banUserChannel, //бан

        manageFriend,   //удалить/добавить/принять приглашение
        ignoreUser,     

        kick,           //кик с сервера
        ban,            //бан на сервере
        sendFile
    }

    //структура данных, с помощью которой сервер и клиент взаимодействуют друг с другом
    public class Data
    {
        public string strName;      
        public string strMessage;   //имя, под которым клиент регистрируется в roomMessageOne
        public string strMessage2;  
        public string strMessage3;  
        public string strMessage4;  
        public byte[] strFileMsg;
        public Command cmdCommand;  //тип команды (login, logout, send message, и тд)

        //конструктор
        public Data()
        {
            cmdCommand = Command.NoCommand;
            strName = null;
            strMessage = null;
            strMessage2 = null;
            strMessage3 = null;
            strMessage4 = null;
            strFileMsg = null;
        }

        //конвертер из битов в объект типа Data
        public Data(byte[] data)
        {
            //первые четыпе бита содержат команду
            cmdCommand = (Command)BitConverter.ToInt32(data, 0);

            //следующие четыре - длина имени
            int nameLen = BitConverter.ToInt32(data, 4);

            //следующие четыре - длина сообщения
            int strMessageLen = BitConverter.ToInt32(data, 8);

            // -//-
            int strMessage2Len = BitConverter.ToInt32(data, 12);

            int strMessage3Len = BitConverter.ToInt32(data, 16);

            int strMessage4Len = BitConverter.ToInt32(data, 20);

            int strFileMsgLen = BitConverter.ToInt32(data, 24);

            if (nameLen > 0)
                strName = Encoding.UTF8.GetString(data, 28, nameLen);
            else
                strName = null;

            if (strMessageLen > 0)
                strMessage = Encoding.UTF8.GetString(data, 28 + nameLen, strMessageLen);
            else
                strMessage = null;

            if (strMessage2Len > 0)
                strMessage2 = Encoding.UTF8.GetString(data, 28 + nameLen + strMessageLen, strMessage2Len);
            else
                strMessage2 = null;

            if (strMessage3Len > 0)
                strMessage3 = Encoding.UTF8.GetString(data, 28 + nameLen + strMessageLen + strMessage2Len, strMessage3Len);
            else
                strMessage3 = null;

            if (strMessage4Len > 0)
                strMessage4 = Encoding.UTF8.GetString(data, 28 + nameLen + strMessageLen + strMessage2Len + strMessage3Len, strMessage4Len);
            else
                strMessage4 = null;

            if (strFileMsgLen > 0)
                strFileMsg = data.Skip(28 + nameLen + strMessageLen + strMessage2Len + strMessage3Len + strMessage4Len).Take(strFileMsgLen).ToArray();
            else
                strFileMsg = null;
        }

        //конвертер из объекта типа Data в массив битов
        public byte[] ToByte()
        {
            List<byte> result = new List<byte>();

            //первые четыре бита для команды
            result.AddRange(BitConverter.GetBytes((int)cmdCommand));

            //добавляем длину имени
            if (strName != null)
                result.AddRange(BitConverter.GetBytes(strName.Length));
            else
                result.AddRange(BitConverter.GetBytes(0));

            //длина сообщения
            if (strMessage != null)
                result.AddRange(BitConverter.GetBytes(strMessage.Length));
            else
                result.AddRange(BitConverter.GetBytes(0));

            //длина сообщения 2
            if (strMessage2 != null)
                result.AddRange(BitConverter.GetBytes(strMessage2.Length));
            else
                result.AddRange(BitConverter.GetBytes(0));

            //длина сообщения 3
            if (strMessage3 != null)
                result.AddRange(BitConverter.GetBytes(strMessage3.Length));
            else
                result.AddRange(BitConverter.GetBytes(0));

            //длина сообщения 4
            if (strMessage4 != null)
                result.AddRange(BitConverter.GetBytes(strMessage4.Length));
            else
                result.AddRange(BitConverter.GetBytes(0));

            if (strFileMsg != null)
                result.AddRange(BitConverter.GetBytes(strFileMsg.Length));
            else
                result.AddRange(BitConverter.GetBytes(0));

            if (strName != null)
                result.AddRange(Encoding.UTF8.GetBytes(strName));

            if (strMessage != null)
                result.AddRange(Encoding.UTF8.GetBytes(strMessage));

            //и напоследок добавляем текст сообщения в наш массив битов
            if (strMessage2 != null)
                result.AddRange(Encoding.UTF8.GetBytes(strMessage2));

            if (strMessage3 != null)
                result.AddRange(Encoding.UTF8.GetBytes(strMessage3));

            if (strMessage4 != null)
                result.AddRange(Encoding.UTF8.GetBytes(strMessage4));

            if (strFileMsg != null)
                result.AddRange(strFileMsg);

            return result.ToArray();
        }
    }
}
