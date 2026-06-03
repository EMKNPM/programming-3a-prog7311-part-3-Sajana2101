using GLMS2.Enums;
using GLMS2.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GLMS2.Models
{
    public class Contract
    {
        [Key]
        public int ContractId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client? Client { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public ContractStatus Status { get; set; }

        [Required]
        [StringLength(100)]
        public string ServiceLevel { get; set; } = string.Empty;

        [Required]
        public ContractType ContractType { get; set; }

        [StringLength(255)]
        public string? SignedAgreementFilePath { get; set; }

        [NotMapped]
        public bool IsActiveForServiceRequest =>
            Status == ContractStatus.Active &&
            StartDate.Date <= DateTime.Today &&
            EndDate.Date >= DateTime.Today;

        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}