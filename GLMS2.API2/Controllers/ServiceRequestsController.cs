using GLMS2.API2.Dtos;
using GLMS2.Interfaces;
using GLMS2.Models;
using GLMS2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GLMS2.API2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestService _serviceRequestService;
        private readonly ICurrencyService _currencyService;

        public ServiceRequestsController(
            IServiceRequestService serviceRequestService,
            ICurrencyService currencyService)
        {
            _serviceRequestService = serviceRequestService;
            _currencyService = currencyService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ServiceRequestResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ServiceRequestResponseDto>>> GetServiceRequests()
        {
            var serviceRequests =
                await _serviceRequestService.GetAllServiceRequestsAsync();

            return Ok(serviceRequests.Select(ToResponseDto));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ServiceRequestResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ServiceRequestResponseDto>> GetServiceRequestById(int id)
        {
            var serviceRequest =
                await _serviceRequestService.GetServiceRequestByIdAsync(id);

            if (serviceRequest == null)
            {
                return NotFound(new
                {
                    message = $"Service request with ID {id} was not found."
                });
            }

            return Ok(ToResponseDto(serviceRequest));
        }

        [HttpGet("exchange-rate")]
        [ProducesResponseType(typeof(ExchangeRateResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<ExchangeRateResponseDto>> GetExchangeRate()
        {
            try
            {
                var rate = await _currencyService.GetUsdToZarRateAsync();

                return Ok(new ExchangeRateResponseDto
                {
                    Rate = rate
                });
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    new
                    {
                        message = "Exchange rate could not be loaded."
                    });
            }
        }
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ServiceRequestResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ServiceRequestResponseDto>> CreateServiceRequest(
            [FromBody] ServiceRequestCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var createdRequest =
                    await _serviceRequestService.CreateServiceRequestAsync(model);

                var savedRequest =
                    await _serviceRequestService.GetServiceRequestByIdAsync(
                        createdRequest.ServiceRequestId);

                var response = ToResponseDto(savedRequest ?? createdRequest);

                return CreatedAtAction(
                    nameof(GetServiceRequestById),
                    new { id = response.ServiceRequestId },
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
        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ServiceRequestResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ServiceRequestResponseDto>> UpdateServiceRequest(
            int id,
            [FromBody] ServiceRequestEditViewModel model)
        {
            if (id != model.ServiceRequestId)
            {
                return BadRequest(new
                {
                    message = "The route ID does not match the service request ID."
                });
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var updated =
                    await _serviceRequestService.UpdateServiceRequestAsync(model);

                if (!updated)
                {
                    return NotFound(new
                    {
                        message = $"Service request with ID {id} was not found."
                    });
                }

                var serviceRequest =
                    await _serviceRequestService.GetServiceRequestByIdAsync(id);

                if (serviceRequest == null)
                {
                    return NotFound(new
                    {
                        message = $"Service request with ID {id} was not found."
                    });
                }

                return Ok(ToResponseDto(serviceRequest));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteServiceRequest(int id)
        {
            var deleted =
                await _serviceRequestService.DeleteServiceRequestAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = $"Service request with ID {id} was not found."
                });
            }

            return NoContent();
        }

        private static ServiceRequestResponseDto ToResponseDto(
            ServiceRequest serviceRequest)
        {
            return new ServiceRequestResponseDto
            {
                ServiceRequestId = serviceRequest.ServiceRequestId,
                ContractId = serviceRequest.ContractId,
                ClientName = serviceRequest.Contract?.Client?.Name,
                ContractStatus = serviceRequest.Contract?.Status,
                Description = serviceRequest.Description,
                CostUSD = serviceRequest.CostUSD,
                CostZAR = serviceRequest.CostZAR,
                Status = serviceRequest.Status,
                CreatedDate = serviceRequest.CreatedDate
            };
        }
    }
}