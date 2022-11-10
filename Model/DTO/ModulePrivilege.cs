using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DTO
{
    public partial class ModulePrivilegeDto
    {
        public int FkPrivilegeId { get; set; }
        public int FkModuleId { get; set; }

        public virtual ModuleDto FkModule { get; set; }
        public virtual PrivilegeDto FkPrivilege { get; set; }
    }
}
