using System.Diagnostics;

namespace CityInfoAPI.Web.Services
{
    public class CloudMailService : IMailService
    {
        private readonly string _mailTo = Startup.Configuration["mailSettings:mailToAddress"];
        private readonly string _mailFrom = Startup.Configuration["mailSettings:mailFromAddress"];

        public void SendMessage(string subject, string message)
        {
            Debug.Write($"Mail from {_mailFrom} to {_mailTo} with CLOUD MailService.");
            Debug.Write($"Subject: {subject}");
            Debug.Write($"Message: {message}");
            // more stuff.....
            // more stuff....
        }
    }
}
