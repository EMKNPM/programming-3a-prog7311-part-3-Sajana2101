
using GLMS2.API2.Dtos;
using GLMS2.Enums;
using GLMS2.Interfaces;
using GLMS2.Models;
using GLMS2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
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
        [Authorize]
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

        /// <summary>
        /// Updates an existing contract and optionally replaces the signed PDF agreement.
        /// </summary>
        
        [Authorize]
        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ContractResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ContractResponseDto>> UpdateContract(
            int id,
            [FromForm] ContractEditViewModel model)
        {
            if (id != model.ContractId)
            {
                return BadRequest(new
                {
                    message = "The route ID does not match the contract ID."
                });
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var updated = await _contractService
                    .UpdateContractAsync(model, GetWebRootPath());

                if (!updated)
                {
                    return NotFound(new
                    {
                        message = $"Contract with ID {id} was not found."
                    });
                }

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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Deletes an existing contract.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var deleted = await _contractService.DeleteContractAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = $"Contract with ID {id} was not found."
                });
            }

            return NoContent();
        }

       
        /// Downloads the signed agreement PDF for a contract.
      
        [HttpGet("{id:int}/agreement")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);

            if (contract == null || string.IsNullOrWhiteSpace(contract.SignedAgreementFilePath))
            {
                return NotFound(new
                {
                    message = "Signed agreement file was not found for this contract."
                });
            }

            var relativePath = contract.SignedAgreementFilePath
                .Replace("/", Path.DirectorySeparatorChar.ToString());

            var fullPath = Path.Combine(GetWebRootPath(), relativePath);

            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound(new
                {
                    message = "The signed agreement file does not exist on the server."
                });
            }

            var fileName = Path.GetFileName(fullPath);

            return PhysicalFile(fullPath, "application/pdf", fileName);
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