namespace StoreApp.Application.Interfaces
{
    public interface IEmailService
    {
        bool Send(string senderEmail, string appPassword, string receiverEmail, string subject, string body);
    }
}