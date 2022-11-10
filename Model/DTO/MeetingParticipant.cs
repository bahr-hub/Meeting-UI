using System;
using System.Collections.Generic;

namespace Model.DTO
{
    public class MeetingParticipantDto
    {
        public Guid Id { get; set; }
        public Guid MeetingId { get; set; }
        public Guid ParticipantId { get; set; }
        public bool? Response { get; set; }
        public bool? JoinedMeeting { get; set; }
        public DateTime? JoinedMeetingTime { get; set; }
        //public MeetingDto Meeting { get; set; }
        public UserDto Participant { get; set; }
        public bool IsReminded { get; set; }
    }
}
