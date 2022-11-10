using System;
using System.Collections.Generic;

namespace Model.Models
{
    public partial class ModulePrivilege
    {
        public int FkPrivilegeId { get; set; }
        public int FkModuleId { get; set; }

        public virtual Module FkModule { get; set; }
        public virtual Privilege FkPrivilege { get; set; }
    }
}
