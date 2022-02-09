using System;

namespace Database.Entities
{
    public class MessageEntity : BaseEntity
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public string Html { get;set; }

    }
}
