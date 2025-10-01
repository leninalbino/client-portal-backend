using ClientPortal.Application.DTOs;
using ClientPortal.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientPortal.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        // Servicio para manejar los clientes
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetClients()
        {
            try
            {
                // Obtener todos los clientes
                var clients = await _clientService.GetAllClientsAsync();
                return Ok(clients);
            }
            catch (Exception ex)
            {
                // Si algo sale mal, devolver error
                return StatusCode(500, new { message = "Uy, hubo un problema al traer los clientes", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDto>> GetClient(Guid id)
        {
            try
            {
                // Buscar cliente por ID
                var client = await _clientService.GetClientByIdAsync(id);

                if (client == null)
                    return NotFound(new { message = "No encontré el cliente que buscas" });

                return Ok(client);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Problema al buscar el cliente", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ClientDto>> CreateClient([FromForm] CreateClientDto createClientDto)
        {
            try
            {
                var client = await _clientService.CreateClientAsync(createClientDto);
                return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating client", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ClientDto>> UpdateClient(Guid id, [FromForm] UpdateClientDto updateClientDto)
        {
            try
            {
                var client = await _clientService.UpdateClientAsync(id, updateClientDto);
                return Ok(client);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating client", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteClient(Guid id)
        {
            try
            {
                await _clientService.DeleteClientAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting client", error = ex.Message });
            }
        }

        [HttpGet("export")]
        public async Task<ActionResult> ExportClients()
        {
            try
            {
                var clients = await _clientService.GetAllClientsAsync();
                var csv = GenerateCsv(clients);
                var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
                
                return File(bytes, "text/csv", $"clients_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error exporting clients", error = ex.Message });
            }
        }

        private static string GenerateCsv(IEnumerable<ClientDto> clients)
        {
            var header = "Id,FirstName,LastName,DateOfBirth,DocumentType,DocumentNumber,CurriculumVitaeFileName,PhotoFileName,CreatedAt,UpdatedAt\n";
            var rows = clients.Select(c => 
                $"{c.Id},{c.FirstName},{c.LastName},{c.DateOfBirth:yyyy-MM-dd},{c.DocumentType},{c.DocumentNumber},{c.CurriculumVitaeFileName},{c.PhotoFileName},{c.CreatedAt:yyyy-MM-dd HH:mm:ss},{c.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""}");
            
            return header + string.Join("\n", rows);
        }
    }
}