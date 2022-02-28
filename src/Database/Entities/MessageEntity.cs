using System.Collections.Generic;

namespace Database.Entities
{
    public class MessageEntity : BaseEntity
    {
        public string FromName { get; set; }
        public string FromAddress { get; set; }
        public string ToName { get; set; }
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public string Html { get;set; }
        public List<AttachmentEntity> Attachments { get; set; }

    }

}
