using System;
using System.Collections.Generic;

namespace Model.Models
{
    public partial class UserProfile
    {
        public UserProfile()
        {
            User = new HashSet<User>();
        }

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CountryCode { get; set; }
        public int? Gender { get; set; }
        public string Address { get; set; }
        public string Photo { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual User UpdatedByNavigation { get; set; }
        public virtual ICollection<User> User { get; set; }
    }
}
