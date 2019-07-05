using System.Diagnostics;

namespace CityInfoAPI.Web.Services
{
    /// <summary>
    /// cloud mail service such as gmail
    /// </summary>
    public class CloudMailService : IMailService
    {
        private readonly string _mailTo = Startup.Configuration["mailSettings:mailToAddress"];
        private readonly string _mailFrom = Startup.Configuration["mailSettings:mailFromAddress"];

        /// <summary>
        /// method to send message via cloud service
        /// </summary>
        /// <param name="subject">subject of message</param>
        /// <param name="message">message itself</param>
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
