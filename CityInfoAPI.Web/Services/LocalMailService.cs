using System.Diagnostics;

namespace CityInfoAPI.Web.Services
{
    /// <summary>
    /// a local, smtp mail service
    /// </summary>
    public class LocalMailService : IMailService
    {
        private readonly string _mailTo = Startup.Configuration["mailSettings:mailToAddress"];
        private readonly string _mailFrom = Startup.Configuration["mailSettings:mailFromAddress"];

        /// <summary>
        /// method to send message from local smtp service
        /// </summary>
        /// <param name="subject">subject of message</param>
        /// <param name="message">message itself</param>
        public void SendMessage(string subject, string message)
        {
            Debug.Write($"**** Mail from {_mailFrom} to {_mailTo} with LocalMailService. ****");
            Debug.Write($"**** Subject: {subject} ****");
            Debug.Write($"**** Message: {message} ****");
        }
    }
}
