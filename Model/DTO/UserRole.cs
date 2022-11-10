using System;

namespace Model.DTO
{
    public partial class UserRoleDto 
    {
        public Guid FkUserId { get; set; }
        public int FkRoleId { get; set; }
    }
}
