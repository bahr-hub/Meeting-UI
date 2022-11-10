using System;
using System.Collections.Generic;

namespace Model.Models
{
    public partial class MeetingTopic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid PresenterId { get; set; }
        public int Duration { get; set; }
        public Guid FkMeetingId { get; set; }
        public bool IsDeleted { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsClosed { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual Meeting FkMeeting { get; set; }
        public virtual User Presenter { get; set; }
        public virtual User UpdatedByNavigation { get; set; }
    }
}
