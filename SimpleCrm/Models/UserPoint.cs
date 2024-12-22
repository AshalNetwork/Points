using SimpleCrm.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleCrm.Models
{
    public class UserPoint
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public Guid? TaskId  { get; set; }
        public Tasks? Task { get; set; }
        public PointsTypeEnum PointType { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal value { get; set; }
    }
}
