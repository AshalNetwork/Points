using System.ComponentModel.DataAnnotations;

namespace SimpleCrm.VM
{
    public class AddClientDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Invalid Egyptian mobile number.")]
        public string Phone { get; set; }
        public string? Description { get; set; }
        public Guid ReasonId { get; set; }
        public string? CustomReason { get; set; }
    }
}
