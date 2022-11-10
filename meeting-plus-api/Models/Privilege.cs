using System;
using System.Collections.Generic;

namespace MeetingPlus.API.Models
{
    public partial class Privilege
    {
        public Privilege()
        {
            ModulePrivilege = new HashSet<ModulePrivilege>();
            RoleModulePrivilege = new HashSet<RoleModulePrivilege>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<ModulePrivilege> ModulePrivilege { get; set; }
        public virtual ICollection<RoleModulePrivilege> RoleModulePrivilege { get; set; }
    }
}
