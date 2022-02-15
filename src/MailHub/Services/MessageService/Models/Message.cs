namespace MailHub.Services.MessageService.Models
{
    public class Message
    {
        public Person From { get; set; }
        public Person To { get; set; }
        public string Text { get; set; }
        public string Html { get; set; }
        public string Subject { get; set; }
        public Attachment[] Attachments { get; set; }

        //some fields has been cut since they not needed for now
    }
}
