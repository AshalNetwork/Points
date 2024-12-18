using SimpleCrm.Abstraction;
using SimpleCrm.Enums;

namespace SimpleCrm.Models
{
    public class ValidationCode:BaseEntity
    {
        public string UserId { get; set; }


        public string Code { get; set; } = null!;

        public DateTime GeneratedDate { get; set; }

        public DateTime ExpirationDate { get; set; }

    }
}
