using System;
using System.Collections.Generic;

namespace MeetingPlus.API.Models
{
    public partial class UserConfiguration
    {
        public UserConfiguration()
        {
            User = new HashSet<User>();
        }

        public Guid Id { get; set; }
        public int? LanguageId { get; set; }
        public int? ReminderBeforeMeeting { get; set; }
        public bool? IntegrationWithGoogleCalendar { get; set; }
        public string TimeZone { get; set; }
        public bool NotificationMuted { get; set; }

        public virtual ICollection<User> User { get; set; }
    }
}
