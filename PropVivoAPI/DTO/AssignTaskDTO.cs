using PropVivoAPI.Enums;

namespace PropVivoAPI.DTO
{
    public class AssignTaskDTO
    {
        public int TaskAssignId { get; set; }

        public int? UserId { get; set; }

        public int? TaskId { get; set; }

        public DateTime? AssignedAt { get; set; }

        public TaskStatusEnum Status { get; set; }
    }
}
