using System;
using System.Collections.Generic;

namespace PropVivoAPI.Models;

public partial class QueryMaster
{
    public int QueryId { get; set; }

    public int? TaskAssignId { get; set; }

    public string? Subject { get; set; }

    public string? Description { get; set; }

    public string? IssueAttachment { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<QueryResponse> QueryResponses { get; set; } = new List<QueryResponse>();

    public virtual TaskAssignment? TaskAssign { get; set; }
}
