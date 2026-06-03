using GLMS2.Enums;

namespace GLMS2.API2.Dtos
{
    public class ContractResponseDto
    {
        public int ContractId { get; set; }

        public int ClientId { get; set; }

        public string? ClientName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public ContractStatus Status { get; set; }

        public string ServiceLevel { get; set; } = string.Empty;

        public ContractType ContractType { get; set; }

        public string? SignedAgreementFilePath { get; set; }
    }
}