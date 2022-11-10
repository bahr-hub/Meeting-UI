using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Model.DTO
{
    public partial class MeetingDto
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int LocationId { get; set; }
        [Required]
        public int? ProjectId { get; set; }
        public int Status { get; set; }
        [Required]
        public DateTime From { get; set; }
        [Required]
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
        
        [Required]
        public ICollection<MeetingTopicDto> MeetingTopic { get; set; }
        public ICollection<MeetingTaskDto> MeetingTask { get; set; }
        public ICollection<MeetingTagDto> MeetingTag { get; set; }
        [Required]
        public ICollection<MeetingParticipantDto> MeetingParticipant { get; set; }

        public UserDto CreatedByNavigation { get; set; }
        public LocationDto Location { get; set; }
        public ProjectDto Project { get; set; }
        public UserDto UpdatedByNavigation { get; set; }

        public bool Started
        {
            get
            {
                return this.StartedAt != null;
            }
        }
        public bool NotEnded
        {
            get
            {
                return this.EndedAt == null;
            }
        }

        public dynamic Days;
        public dynamic Hours;
        public dynamic Minutes;
        public dynamic Seconds;
    }
    public class MeetingListSections
    {
        public List<MeetingDto> PreviousMeetings { get; set; }
        public List<MeetingDto> CurrentMeetings { get; set; }
        public List<MeetingDto> UpcomingMeetings { get; set; }
    }
}
