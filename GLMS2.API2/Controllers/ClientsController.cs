using GLMS2.API2.Dtos;
using GLMS2.Interfaces;
using GLMS2.Models;
using Microsoft.AspNetCore.Mvc;

namespace GLMS2.API2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ClientResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClientResponseDto>>> GetClients()
        {
            var clients = await _clientRepository.GetAllAsync();

            return Ok(clients.Select(ToResponseDto));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ClientResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClientResponseDto>> GetClientById(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            if (client == null)
            {
                return NotFound(new
                {
                    message = $"Client with ID {id} was not found."
                });
            }

            return Ok(ToResponseDto(client));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ClientResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ClientResponseDto>> CreateClient(
            [FromBody] CreateClientRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var client = new Client
            {
                Name = request.Name,
                Region = request.Region,
                Email = request.Email
            };

            await _clientRepository.AddAsync(client);
            await _clientRepository.SaveChangesAsync();

            var response = ToResponseDto(client);

            return CreatedAtAction(
                nameof(GetClientById),
                new { id = response.ClientId },
                response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ClientResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClientResponseDto>> UpdateClient(
            int id,
            [FromBody] UpdateClientRequestDto request)
        {
            if (id != request.ClientId)
            {
                return BadRequest(new
                {
                    message = "The route ID does not match the client ID."
                });
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var client = await _clientRepository.GetByIdAsync(id);

            if (client == null)
            {
                return NotFound(new
                {
                    message = $"Client with ID {id} was not found."
                });
            }

            client.Name = request.Name;
            client.Region = request.Region;
            client.Email = request.Email;

            _clientRepository.Update(client);
            await _clientRepository.SaveChangesAsync();

            return Ok(ToResponseDto(client));
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            if (client == null)
            {
                return NotFound(new
                {
                    message = $"Client with ID {id} was not found."
                });
            }

            _clientRepository.Remove(client);
            await _clientRepository.SaveChangesAsync();

            return NoContent();
        }

        private static ClientResponseDto ToResponseDto(Client client)
        {
            return new ClientResponseDto
            {
                ClientId = client.ClientId,
                Name = client.Name,
                Region = client.Region,
                Email = client.Email
            };
        }
    }
}