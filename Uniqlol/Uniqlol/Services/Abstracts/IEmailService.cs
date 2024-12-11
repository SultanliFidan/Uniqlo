namespace Uniqlol.Services.Abstracts
{
    public interface IEmailService
    {
            void SendEmailConfirmation(string reciever, string name, string token);
            void ResetPassword(string reciever, string name, string token);
        
    }
}
