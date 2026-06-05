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