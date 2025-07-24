using System;
using System.Collections.Generic;

namespace PropVivoAPI.Models;

public partial class QueryResponse
{
    public int QueryResponseId { get; set; }

    public int QueryId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual QueryMaster Query { get; set; } = null!;
}
