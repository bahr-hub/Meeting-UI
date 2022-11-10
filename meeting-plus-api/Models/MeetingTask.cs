using System;
using System.Collections.Generic;

namespace MeetingPlus.API.Models
{
    public partial class MeetingTask
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid MeetingId { get; set; }
        public Guid AssigneeId { get; set; }
        public int Status { get; set; }
        public DateTime DueDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual User Assignee { get; set; }
        public virtual User CreatedByNavigation { get; set; }
        public virtual Meeting Meeting { get; set; }
        public virtual User UpdatedByNavigation { get; set; }
    }
}
