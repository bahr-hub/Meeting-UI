using System;
using System.Collections.Generic;

namespace Model.Models
{
    public partial class Meeting
    {
        public Meeting()
        {
            MeetingParticipant = new HashSet<MeetingParticipant>();
            MeetingTag = new HashSet<MeetingTag>();
            MeetingTask = new HashSet<MeetingTask>();
            MeetingTopic = new HashSet<MeetingTopic>();
            Proposals = new HashSet<Proposals>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int LocationId { get; set; }
        public int? ProjectId { get; set; }
        public int Status { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public DateTime? PostponedTo { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string ExternalToken { get; set; }
        public Guid? PreviousMeetingID { get; set; }
        public virtual User CreatedByNavigation { get; set; }
        public virtual Location Location { get; set; }
        public virtual Project Project { get; set; }
        public virtual User UpdatedByNavigation { get; set; }
        public virtual ICollection<MeetingParticipant> MeetingParticipant { get; set; }
        public virtual ICollection<MeetingTag> MeetingTag { get; set; }
        public virtual ICollection<MeetingTask> MeetingTask { get; set; }
        public virtual ICollection<MeetingTopic> MeetingTopic { get; set; }
        public virtual ICollection<Proposals> Proposals { get; set; }
    }
}
