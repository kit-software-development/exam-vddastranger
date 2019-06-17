using CommandClient;
using Server.Interfaces;
using Server.ResponseMessages;
using System;
using System.Collections.Generic;

namespace Server
{
    class ClientLogin : ServerResponds, IClient, IBuildResponse //класс, отвечающий за аутентификацию клиента
    {
        public event EventHandler<ClientEventArgs> clientLoginEvent;

        DataBaseManager db = DataBaseManager.Instance;
        EmailSender emailSender = new EmailSender();

        private List<Client> ListOfClientsOnline;

        public void Load(Client client, Data receive, List<Client> clientList = null, List<Channel> channelsList = null)
        {
            Received = receive;
            Client = client;
            ListOfClientsOnline = clientList;
        }

        public void Execute()
        {
            prepareResponse();

            string userName = Received.strName;
            string userPassword = Received.strMessage;

            string[] query = GetUserInformationFromDB(userName, userPassword);
            if (query == null || userName != query[3])
                Send.strMessage = "Wrong login or password";
            else if (query[0] != "")
            {
                Send.cmdCommand = Command.SendActivationCode;
                Send.strMessage = "You must activate your account first.";
            }
            else
            {               
                    ClientSuccesfullyLogIn(query);
                    userEmailNotification(query[1]);                
            }
        }

        private void userEmailNotification(string email)
        {
            string loginNotyfiUser = Received.strMessage2;
            if (loginNotyfiUser == "1") // User wants to be notyficated when login into account
            {
                emailSender.SetProperties(Client.strName, email, "Gold Chat: Login Notyfication", "You have login: " + DateTime.Now.ToString("dd:MM On HH:mm:ss") + " To Gold Chat Account.");
                emailSender.SendEmail();
            }
        }

        private string[] GetUserInformationFromDB(string userName, string userPassword)
        {
            db.bind(new string[] { "@userName", userName, "@password", userPassword });
            db.manySelect("SELECT register_id, email, id_user, login, permission FROM users WHERE login = @userName AND password = @password");
            return db.tableToRow();
        }

        private void ClientSuccesfullyLogIn(string[] query)
        {
            SetClientProperties(query);

            Send.strMessage = "You are succesfully Log in";
            Send.strMessage2 = Client.permission.ToString();
            OnClientLogin(Client.strName, Client.addr.Address.ToString(), Client.addr.Port.ToString()); // Server OnClientLogin occur only when succes program.cs -> OnClientLogin
        }

        private void SetClientProperties(string[] query)
        {
            Client.id = Int64.Parse(query[2]);
            Client.permission = Int16.Parse(query[4]);
            Client.strName = query[3];
            Client.enterChannels = new List<string>(); // Init of channels whitch i joined
            Client.ignoredUsers = new List<string>(); // Init of ignored users
        }

        public override void Response()
        {
            base.Response();

            ResponseToAll();
        }

        private void ResponseToAll()
        {
            if (Send.strMessage == "You are succesfully Log in") // клиент успешно залогинился и все юзеры получают сообщение
            {
                Send.strMessage = "<<<" + Received.strName + " has joined the room>>>";
                SendMessageToAll sendMsgToAll = new SendMessageToAll(Client, Send, ListOfClientsOnline);
                sendMsgToAll.ResponseToAll();
            }
        }

        protected virtual void OnClientLogin(string cName, string cIpadress, string cPort)
        {
            clientLoginEvent?.Invoke(this, new ClientEventArgs { clientName = cName, clientIpAdress = cIpadress, clientPort = cPort });
        }
    }
}
