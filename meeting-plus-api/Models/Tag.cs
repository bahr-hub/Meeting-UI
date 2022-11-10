using System;
using System.Collections.Generic;

namespace MeetingPlus.API.Models
{
    public partial class Tag
    {
        public Tag()
        {
            MeetingTag = new HashSet<MeetingTag>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual User UpdatedByNavigation { get; set; }
        public virtual ICollection<MeetingTag> MeetingTag { get; set; }
    }
}
