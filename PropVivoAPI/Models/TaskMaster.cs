using System;
using System.Collections.Generic;

namespace PropVivoAPI.Models;

public partial class TaskMaster
{
    public int TaskId { get; set; }

    public int CreatedBy { get; set; }

    public string TaskTitle { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string EstimatedHrs { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual UserMaster CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();
}
