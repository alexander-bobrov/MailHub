namespace MailHub.Services.MessageService.Models
{
    public class Message
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Text { get; set; }
        public string Html { get; set; }
        public string Subject { get; set; }
    }
}
