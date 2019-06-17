using Server.Interfaces;
using System;
using System.Net.Mail;

namespace Server
{
    public class EmailSenderEventArgs : EventArgs
    {
        public string Subject { get; set; }
        public string ToEmail { get; set; }
        public string EmailMessage { get; set; }
        public string UserNameEmail { get; set; }
    }

    public class EmailSender : ISendEmail
    {
        public event EventHandler<EmailSenderEventArgs> EmailSended;
        public event EventHandler<EmailSenderEventArgs> FailEmailSended;

        string UserNameEmail { get; set; }
        string ToEmail { get; set; }
        string Subject { get; set; }
        string EmailMessage { get; set; }

        public void SetProperties(string userName, string toEmail, string subject, string emailMessage)
        {
            UserNameEmail = userName;
            ToEmail = toEmail;
            Subject = subject;
            EmailMessage = emailMessage;
        }

        public bool SendEmail()
        {
            string fromAddress = "taktigrynerychat@yandex.ru";

            MailMessage message = new MailMessage(fromAddress, ToEmail, Subject, EmailMessage);
            message.IsBodyHtml = true;
            message.Priority = MailPriority.High;
            SmtpClient emailClient = new SmtpClient("smtp.yandex.ru", 587);
            emailClient.EnableSsl = true;
            emailClient.Credentials = new System.Net.NetworkCredential(fromAddress, "Ces7niNuw");

            try
            {
                //Send the Email
                emailClient.Send(message);
                OnEmailSended(UserNameEmail, Subject, ToEmail, EmailMessage);
                return true;
            }
            catch
            {
                OnFailEmailSended(UserNameEmail, Subject, ToEmail, EmailMessage);
                return false;
            }
        }

        protected virtual void OnEmailSended(string userName, string toEmail, string subject, string emailMessage)
        {
            EmailSended?.Invoke(this, new EmailSenderEventArgs() { UserNameEmail = userName, ToEmail = toEmail, Subject = subject, EmailMessage = emailMessage });
        }

        protected virtual void OnFailEmailSended(string userName, string toEmail, string subject, string emailMessage)
        {
            FailEmailSended?.Invoke(this, new EmailSenderEventArgs() { UserNameEmail = userName, ToEmail = toEmail, Subject = subject, EmailMessage = emailMessage });
        }
    }
}
