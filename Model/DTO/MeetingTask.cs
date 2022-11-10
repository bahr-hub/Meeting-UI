using System;
using System.ComponentModel.DataAnnotations;

namespace Model.DTO
{
    public partial class MeetingTaskDto
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public Guid MeetingId { get; set; }
        [Required]
        public Guid AssigneeId { get; set; }
        public int Status { get; set; }
        [Required]
        public DateTime DueDate { get; set; }

        public UserDto Assignee { get; set; }
        public Guid? RelatedTaskId { get; set; }
                                           //public MeetingDto Meeting { get; set; }
    }
}
