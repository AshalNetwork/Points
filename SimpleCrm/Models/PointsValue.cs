using SimpleCrm.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleCrm.Models
{
    public class PointsValue
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Percentage { get; set; }
    }
}
