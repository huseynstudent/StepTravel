using System;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using StoreApp.Application.Interfaces;

namespace StoreApp.Application.Service;

public class Email : IEmailService
{
    public bool Send(string senderEmail, string appPassword, string receiverEmail, string subject, string body)
    {
        if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(appPassword))
        {
            Console.WriteLine("Email Error: Sender email or App Password is missing in appsettings.json");
            return false;
        }

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Step Travels", senderEmail));
            message.To.Add(new MailboxAddress("User", receiverEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                        <div style='font-family: sans-serif; padding: 20px; border: 1px solid #eee; border-radius: 10px;'>
                            <h2 style='color: #2c3e50;'>{subject}</h2>
                            <div style='font-size: 16px; color: #34495e;'>
                                {body}
                            </div>
                            <p style='margin-top: 20px; font-size: 12px; color: #7f8c8d;'>
                                This is an automated message from Step Travels.
                            </p>
                        </div>"
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate(senderEmail, appPassword);
                client.Send(message);
                client.Disconnect(true);
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Critical Email Error: {ex.Message}");
            if (ex.InnerException != null)
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");

            return false;
        }
    }
}