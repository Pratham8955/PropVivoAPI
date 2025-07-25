using System;
using System.Collections.Generic;
using PropVivoAPI.Enums;

namespace PropVivoAPI.Models;

public partial class QueryMaster
{
    public int QueryId { get; set; }

    public int TaskAssignId { get; set; }

    public string Subject { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string IssueAttachment { get; set; } = null!;

    public String Status { get; set; } 

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<QueryResponse> QueryResponses { get; set; } = new List<QueryResponse>();

    public virtual TaskAssignment TaskAssign { get; set; } = null!;
}
