using System;

namespace Model.DTO
{
    public partial class NotificationDto
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Message { get; set; }
        public Guid FkUserId { get; set; }
        public string MessageAR { get; set; }
        public bool IsRead { get; set; }
        public Guid MeetingID { get; set; }
        public UserProfileDto User { get; set; }
    }
}
