using System;
using System.Collections.Generic;

namespace PropVivoAPI.Models;

public partial class BreakLogTracking
{
    public int BreakLogId { get; set; }

    public int? TaskAssignId { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public DateTime? TotalTime { get; set; }

    public virtual TaskAssignment? TaskAssign { get; set; }
}
