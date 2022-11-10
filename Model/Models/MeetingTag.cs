using System;
using System.Collections.Generic;

namespace Model.Models
{
    public partial class MeetingTag
    {
        public Guid MeetingId { get; set; }
        public int TagId { get; set; }
        public Guid Id { get; set; }

        public virtual Meeting Meeting { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
