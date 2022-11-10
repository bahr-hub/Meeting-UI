using System;
using System.Collections.Generic;

namespace Model.Models
{
    public partial class Notification
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Message { get; set; }
        public Guid UserId { get; set; }
        public string MessageAR { get; set; }
        public Guid MeetingID { get; set; }
        public bool IsRead { get; set; }
        public virtual User User { get; set; }
        public virtual Meeting Meeting { get; set; }
    }

}
