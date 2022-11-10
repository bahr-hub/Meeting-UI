using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DTO
{
    public partial class PrivilegeDto
    {
        public PrivilegeDto()
        {
            ModulePrivilege = new HashSet<ModulePrivilegeDto>();
            RoleModulePrivilege = new HashSet<RoleModulePrivilegeDto>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<ModulePrivilegeDto> ModulePrivilege { get; set; }
        public virtual ICollection<RoleModulePrivilegeDto> RoleModulePrivilege { get; set; }
    }
}
