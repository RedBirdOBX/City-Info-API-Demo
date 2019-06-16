namespace CityInfoAPI.Web.Services
{
    public interface IMailService
    {
        void SendMessage(string subject, string message);
    }
}
