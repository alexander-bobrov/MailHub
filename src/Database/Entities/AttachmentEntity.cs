namespace Database.Entities
{
    public class AttachmentEntity : BaseEntity
    {
        public string ContentId { get; set; }
        public string Filename { get; set; }
        public string ContentType { get; set; }
    }
}
