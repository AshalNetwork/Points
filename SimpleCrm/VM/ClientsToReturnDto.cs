using System.Reflection.Metadata.Ecma335;

namespace SimpleCrm.VM
{
    public class ClientsToReturnDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UserId { get; set; }
    }
}
