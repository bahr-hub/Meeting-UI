using System;
using System.Collections.Generic;

namespace Model.DTO
{
    public class SystemConfigurationDto
    {
        public int Id { get; set; }
        public int AuthenticationMode { get; set; }
        public bool IntegrationWithGoogleCalendar { get; set; }
        public int MaxMeetingTime { get; set; }
        public int StartOfWorkDays { get; set; }
        public int EndOfWorkDays { get; set; }
        public string TimeZone { get; set; }
    }
}
