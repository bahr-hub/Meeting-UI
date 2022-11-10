using System;
using System.Collections.Generic;

namespace MeetingPlus.API.Models
{
    public partial class Vacation
    {
        public Guid Id { get; set; }
        public Guid FkUserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual User FkUser { get; set; }
        public virtual User UpdatedByNavigation { get; set; }
    }
}
