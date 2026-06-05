using GLMS2.Enums;

namespace GLMS2.API2.Dtos
{
    public class ServiceRequestResponseDto
    {
        public int ServiceRequestId { get; set; }

        public int ContractId { get; set; }

        public string? ClientName { get; set; }

        public ContractStatus? ContractStatus { get; set; }

        public string Description { get; set; } = string.Empty;

        public decimal CostUSD { get; set; }

        public decimal CostZAR { get; set; }

        public ServiceRequestStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}