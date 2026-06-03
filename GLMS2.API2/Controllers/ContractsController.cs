
using GLMS2.API2.Dtos;
using GLMS2.Enums;
using GLMS2.Interfaces;
using GLMS2.Models;
using GLMS2.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GLMS2.API2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractsController : ControllerBase
    {
        private readonly IContractService _contractService;
        private readonly IWebHostEnvironment _environment;

        public ContractsController(
            IContractService contractService,
            IWebHostEnvironment environment)
        {
            _contractService = contractService;
            _environment = environment;
        }

        /// <summary>
        /// Retrieves contracts with optional filtering by dates and status.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ContractResponseDto>),
            StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ContractResponseDto>>> GetContracts(
            [FromQuery] DateTime? startDateFrom,
            [FromQuery] DateTime? startDateTo,
            [FromQuery] ContractStatus? status)
        {
            var contracts = await _contractService.FilterContractsAsync(
                startDateFrom,
                startDateTo,
                status);

            return Ok(contracts.Select(ToResponseDto));
        }

        /// <summary>
        /// Retrieves a single contract by its identifier.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ContractResponseDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ContractResponseDto>> GetContractById(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);

            if (contract == null)
            {
                return NotFound(new
                {
                    message = $"Contract with ID {id} was not found."
                });
            }

            return Ok(ToResponseDto(contract));
        }

        /// <summary>
        /// Creates a new contract and uploads its signed PDF agreement.
        /// </summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ContractResponseDto),
            StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContractResponseDto>> CreateContract(
            [FromForm] ContractCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var createdContract = await _contractService
                    .CreateContractAsync(model, GetWebRootPath());

                var savedContract = await _contractService
                    .GetContractByIdAsync(createdContract.ContractId);

                var response = ToResponseDto(savedContract ?? createdContract);

                return CreatedAtAction(
                    nameof(GetContractById),
                    new { id = response.ContractId },
                    response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Updates the workflow status of an existing contract.
        /// </summary>
        [HttpPatch("{id:int}/status")]
        [ProducesResponseType(typeof(ContractResponseDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ContractResponseDto>> UpdateContractStatus(
            int id,
            [FromBody] UpdateContractStatusRequest request)
        {
            if (!Enum.IsDefined(typeof(ContractStatus), request.Status))
            {
                return BadRequest(new
                {
                    message = "Invalid contract status supplied."
                });
            }

            var updatedContract = await _contractService
                .UpdateStatusAsync(id, request.Status);

            if (updatedContract == null)
            {
                return NotFound(new
                {
                    message = $"Contract with ID {id} was not found."
                });
            }

            return Ok(ToResponseDto(updatedContract));
        }

        private string GetWebRootPath()
        {
            var webRootPath = _environment.WebRootPath
                ?? Path.Combine(_environment.ContentRootPath, "wwwroot");

            Directory.CreateDirectory(webRootPath);

            return webRootPath;
        }

        private static ContractResponseDto ToResponseDto(Contract contract)
        {
            return new ContractResponseDto
            {
                ContractId = contract.ContractId,
                ClientId = contract.ClientId,
                ClientName = contract.Client?.Name,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                Status = contract.Status,
                ServiceLevel = contract.ServiceLevel,
                ContractType = contract.ContractType,
                SignedAgreementFilePath = contract.SignedAgreementFilePath
            };
        }
    }
}