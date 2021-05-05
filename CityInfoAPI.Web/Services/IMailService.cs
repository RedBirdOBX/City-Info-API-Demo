namespace CityInfoAPI.Web.Services
{
    /// <summary>
    /// contract for mail service - all implementations must look like this
    /// </summary>
    public interface IMailService
    {
        /// <summary>
        /// method to send message
        /// </summary>
        /// <param name="subject">the subject of the message</param>
        /// <param name="message">the message itself</param>
        void SendMessage(string subject, string message);
    }
}
