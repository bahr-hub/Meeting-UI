using System;
using System.ComponentModel.DataAnnotations;

namespace Model.DTO
{
    public partial class MeetingTopicDto 
    {
        public Guid Id { get; set; }
       // [Required]
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

        public UserDto Presenter { get; set; }
    }
}
