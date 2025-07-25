using PropVivoAPI.Enums;

namespace PropVivoAPI.DTO
{
    public class QueryDTO
    {
        public int QueryId { get; set; }

        public int TaskAssignId { get; set; }

        public string Subject { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string IssueAttachment { get; set; } = null!;

        public QueryStatusEnum Status { get; set; }


        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
