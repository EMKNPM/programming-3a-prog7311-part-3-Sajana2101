using System.ComponentModel.DataAnnotations;

namespace GLMS2.ViewModels
{
    // ViewModel used when editing an existing service request
    public class ServiceRequestEditViewModel
    {
        // Identifies which service request is being updated
        [Required]
        public int ServiceRequestId { get; set; }

        // Links the service request to a contract
        [Required]
        [Display(Name = "Contract")]
        public int ContractId { get; set; }

        // Description of the requested service
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        // Cost entered in USD
        [Required]
        [Range(0.01, 1000000000)]
        [Display(Name = "Cost (USD)")]
        public decimal CostUSD { get; set; }

        // Exchange rate used for conversion
        [Display(Name = "Exchange Rate (USD to ZAR)")]
        public decimal ExchangeRate { get; set; }

        // Converted cost stored in local currency
        [Display(Name = "Cost (ZAR)")]
        public decimal CostZAR { get; set; }
    }
}