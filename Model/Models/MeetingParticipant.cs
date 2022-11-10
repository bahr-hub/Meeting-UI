using System;
using System.Collections.Generic;

namespace Model.Models
{
    public partial class MeetingParticipant
    {
        public Guid Id { get; set; }
        public Guid MeetingId { get; set; }
        public Guid ParticipantId { get; set; }
        public bool? Response { get; set; }
        public bool? JoinedMeeting { get; set; }
        public DateTime? JoinedMeetingTime { get; set; }
        public bool IsReminded { get; set; }

        public virtual Meeting Meeting { get; set; }
        public virtual User Participant { get; set; }
    }
}
