using System;
using System.Collections.Generic;

namespace PropVivoAPI.Models;

public partial class UserMaster
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public string? Email { get; set; }

    public string? Name { get; set; }

    public string? Password { get; set; }

    public virtual RoleMaster? Role { get; set; }

    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();

    public virtual ICollection<TaskMaster> TaskMasters { get; set; } = new List<TaskMaster>();
}
