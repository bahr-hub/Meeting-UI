using System;
using System.Collections.Generic;

namespace Model.Models
{
    public partial class Location
    {
        public Location()
        {
            Meeting = new HashSet<Meeting>();
            User = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual User UpdatedByNavigation { get; set; }
        public virtual ICollection<Meeting> Meeting { get; set; }
        public virtual ICollection<User> User { get; set; }
    }
}
