namespace CityInfo.API.Services
{
    public class LocalMailService : IMailService
    {
        private string _from = string.Empty;
        private string _to = string.Empty;

        public LocalMailService(IConfiguration configuration)
        {
            _from = configuration["MailSettings:MailToAddress"];
            _to = configuration["MailSettings:MailFromAddress"];

        }

        public void Send(string subject, string body)
        {
            Console.WriteLine($"Mail is send from {_from} to {_to} with {nameof(LocalMailService)}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Body: {body}");
        }

    }
}
