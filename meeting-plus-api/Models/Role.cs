using System;
using System.Collections.Generic;

namespace MeetingPlus.API.Models
{
    public partial class Role
    {
        public Role()
        {
            RoleModulePrivilege = new HashSet<RoleModulePrivilege>();
            UserRole = new HashSet<UserRole>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual User UpdatedByNavigation { get; set; }
        public virtual ICollection<RoleModulePrivilege> RoleModulePrivilege { get; set; }
        public virtual ICollection<UserRole> UserRole { get; set; }
    }
}
