using GLMS2.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GLMS2.Models
{
    public class ServiceRequest
    {
        [Key]
        public int ServiceRequestId { get; set; }

        [Required]
        public int ContractId { get; set; }

        [ForeignKey(nameof(ContractId))]
        public Contract? Contract { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostUSD { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostZAR { get; set; }

        [Required]
        public ServiceRequestStatus Status { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}