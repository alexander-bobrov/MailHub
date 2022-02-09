using System;

namespace Database.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}