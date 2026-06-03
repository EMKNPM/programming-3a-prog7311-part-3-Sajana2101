using GLMS2.Enums;
using System.ComponentModel.DataAnnotations;

namespace GLMS2.API2.Dtos
{
    public class UpdateContractStatusRequest
    {
        [Required]
        [EnumDataType(typeof(ContractStatus))]
        public ContractStatus Status { get; set; }
    }
}