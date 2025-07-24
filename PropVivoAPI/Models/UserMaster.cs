using System;
using System.Collections.Generic;

namespace PropVivoAPI.Models;

public partial class UserMaster
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public string Email { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual RoleMaster Role { get; set; } = null!;

    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();

    public virtual ICollection<TaskMaster> TaskMasters { get; set; } = new List<TaskMaster>();
}
