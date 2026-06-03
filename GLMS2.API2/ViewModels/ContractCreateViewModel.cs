using GLMS2.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GLMS2.ViewModels
{
    // ViewModel used when creating a new contract from the UI
    public class ContractCreateViewModel
    {
        // Links the contract to a client
        [Required]
        [Display(Name = "Client")]
        public int ClientId { get; set; }

        // Contract start date
        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        // Contract end date
        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        // Initial workflow status
        [Required]
        public ContractStatus Status { get; set; }

        // Describes the level of service provided
        [Required]
        [StringLength(100)]
        [Display(Name = "Service Level")]
        public string ServiceLevel { get; set; } = string.Empty;

        // Determines the contract category
        [Required]
        [Display(Name = "Contract Type")]
        public ContractType ContractType { get; set; }

        // Upload field for signed agreement document
        [Required]
        [Display(Name = "Signed Agreement (PDF only)")]
        public IFormFile? SignedAgreementFile { get; set; }
    }
}