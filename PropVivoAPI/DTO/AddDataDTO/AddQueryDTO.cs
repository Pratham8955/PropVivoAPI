namespace PropVivoAPI.DTO.AddDataDTO
{
    public class AddQueryDTO
    {
        public int TaskAssignId { get; set; }

        public string Subject { get; set; }
        public string Description { get; set; } 

        public IFormFile IssueAttachment { get; set; } 

        public string Status { get; set; } 

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
