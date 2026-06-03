using System.ComponentModel.DataAnnotations;
using GLMS2.Enums;
using Microsoft.AspNetCore.Http;

namespace GLMS2.ViewModels
{
    // ViewModel used when editing an existing contract
    public class ContractEditViewModel
    {
        // Identifies which contract is being updated
        [Required]
        public int ContractId { get; set; }

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

        // Current workflow status of the contract
        [Required]
        public ContractStatus Status { get; set; }

        // Describes the level of service provided
        [Required]
        [StringLength(100)]
        [Display(Name = "Service Level")]
        public string ServiceLevel { get; set; } = string.Empty;

        // Specifies the contract category
        [Required]
        [Display(Name = "Contract Type")]
        public ContractType ContractType { get; set; }

        // Optional new PDF file to replace the existing agreement
        [Display(Name = "Replace Signed Agreement (PDF only)")]
        public IFormFile? SignedAgreementFile { get; set; }

        // Stores the path of the previously uploaded file
        public string? ExistingSignedAgreementFilePath { get; set; }
    }
}