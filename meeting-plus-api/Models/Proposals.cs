using System;
using System.Collections.Generic;

namespace MeetingPlus.API.Models
{
    public partial class Proposals
    {
        public Guid Id { get; set; }
        public Guid ProposedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ProposedStartTime { get; set; }
        public DateTime ProposedEndTime { get; set; }
        public int ProposalState { get; set; }
        public Guid MeetingId { get; set; }

        public virtual Meeting Meeting { get; set; }
        public virtual User ProposedByNavigation { get; set; }
    }
}
