namespace CityInfo.API.Services
{
    public class LocalMailService
    {
        private string _from = "admin@test.com";
        private string _to = "recipient@test.com";

        public void Send(string subject, string body)
        {
            Console.WriteLine($"Mail is send from {_from} to {_to} with {nameof(LocalMailService)}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Body: {body}");
        }

    }
}
