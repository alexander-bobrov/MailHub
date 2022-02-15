namespace MailHub.Services.MessageService.Models
{
    public class Attachment
    {
        public string ContentId { get; set; }
        public string Filename { get; set; }
        public string ContentType { get; set; }

        //some fields has been cut since they not needed for now
    }
}
