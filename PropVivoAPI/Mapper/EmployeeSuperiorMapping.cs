using PropVivoAPI.Models;

namespace PropVivoAPI.Mapper
{
    public class EmployeeSuperiorMapping
    {
        public int MappingId { get; set; }
        public int EmployeeId { get; set; }
        public int SuperiorId { get; set; }

        public UserMaster Employee { get; set; }
        public UserMaster Superior { get; set; }
    }
}
