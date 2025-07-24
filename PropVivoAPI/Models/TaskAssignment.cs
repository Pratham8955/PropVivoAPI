using System;
using System.Collections.Generic;

namespace PropVivoAPI.Models;

public partial class TaskAssignment
{
    public int TaskAssignId { get; set; }

    public int? UserId { get; set; }

    public int? TaskId { get; set; }

    public DateTime? AssignedAt { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<BreakLogTracking> BreakLogTrackings { get; set; } = new List<BreakLogTracking>();

    public virtual ICollection<QueryMaster> QueryMasters { get; set; } = new List<QueryMaster>();

    public virtual TaskMaster? Task { get; set; }

    public virtual ICollection<TaskTimeTracking> TaskTimeTrackings { get; set; } = new List<TaskTimeTracking>();

    public virtual UserMaster? User { get; set; }
}
