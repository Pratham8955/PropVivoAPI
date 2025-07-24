using System;
using System.Collections.Generic;

namespace PropVivoAPI.Models;

public partial class RoleMaster
{
    public int RoleId { get; set; }

    public string? RoleName { get; set; }

    public virtual ICollection<UserMaster> UserMasters { get; set; } = new List<UserMaster>();
}
