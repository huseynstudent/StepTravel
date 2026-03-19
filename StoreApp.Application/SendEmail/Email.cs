using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;

namespace StoreApp.Application.Service
{
    public class Email
    {
        public bool Send(string senderEmail, string appPassword, string receiverEmail, string subject, string body)
        {
            try
            {
                var Message = new MimeMessage();
                Message.From.Add(new MailboxAddress("Step Travels", senderEmail));
                Message.To.Add(new MailboxAddress("Receiver", receiverEmail));
                Message.Subject = subject;
                Message.Body = new TextPart("plain") { Text = body };

                using (var Client = new SmtpClient())
                {
                    Client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    try
                    {
                        Client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                        Client.Authenticate(senderEmail, appPassword);
                        Client.Send(Message);
                        Client.Disconnect(true);
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}