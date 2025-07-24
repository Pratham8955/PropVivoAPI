namespace PropVivoAPI.DTO
{
    public class TaskMasterDTO
    {
        public int TaskId { get; set; }

        public int? CreatedBy { get; set; }

        public string? TaskTitle { get; set; }

        public string? Description { get; set; }

        public string? EstimatedHrs { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
