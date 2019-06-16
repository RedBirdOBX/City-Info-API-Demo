using System.Diagnostics;

namespace CityInfoAPI.Web.Services
{
    public class LocalMailService : IMailService
    {
        private readonly string _mailTo = Startup.Configuration["mailSettings:mailToAddress"];
        private readonly string _mailFrom = Startup.Configuration["mailSettings:mailFromAddress"];

        public void SendMessage(string subject, string message)
        {
            Debug.Write($"**** Mail from {_mailFrom} to {_mailTo} with LocalMailService. ****");
            Debug.Write($"**** Subject: {subject} ****");
            Debug.Write($"**** Message: {message} ****");
        }
    }
}
