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

    public int? SuperiorId { get; set; }

    public virtual ICollection<UserMaster> InverseSuperior { get; set; } = new List<UserMaster>();

    public virtual RoleMaster Role { get; set; } = null!;

    public virtual UserMaster? Superior { get; set; }

    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();

    public virtual ICollection<TaskMaster> TaskMasters { get; set; } = new List<TaskMaster>();
}
