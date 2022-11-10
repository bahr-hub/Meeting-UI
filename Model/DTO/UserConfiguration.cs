using System;

namespace Model.DTO
{
    public partial class UserConfigurationDto
    {
        public Guid Id { get; set; }
        public int? LanguageId { get; set; }
        public int? ReminderBeforeMeeting { get; set; }
        public bool? IntegrationWithGoogleCalendar { get; set; }
        public string TimeZone { get; set; }
    }
}
