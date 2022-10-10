using API.Data.Models;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API.Controller
{
    [EnableCors("CORS")]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        // Constructor
        public ClientsController(ApplicationDbContext context, IMapper mapper
                                )
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("")]
        public IActionResult GetClients()
        {
            return Ok(_context.Clients.Where(x => x.Activo));
        }
        [HttpGet("Inactivos")]
        public IActionResult GetClientsInactivos()
        {
            return Ok(_context.Clients.Where(x => !x.Activo));
        }

        [HttpPost("")]
        public async Task<IActionResult> PostClient(CreateClientDto client)
        {
            var tiendamodel = _mapper.Map<Clients>(client);
            _context.Clients.Add(tiendamodel);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("")]
        public async Task<IActionResult> PutClient(UpdateClientDto client)
        {
            var clientes = _context.Clients.Where(x => x.id == client.id).FirstOrDefault();

            _mapper.Map(client, clientes);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var clientes = _context.Clients.Where(x => x.id == id).FirstOrDefault();
            clientes.Activo = false;
            await _context.SaveChangesAsync();
            return Ok();
        }

    }
    public class CreateClientDto
    {
        public string Nombre { get; set; }

        public string Codigo { get; set; }
        public string NombreExtranjero { get; set; }
        public string Grupo { get; set; }
        public string RFC { get; set; }
        public string Calle { get; set; }
        public string Colonia { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }

        public bool Activo = true;
    }

    public class UpdateClientDto
    {
        public int id { get; set; }
        public string Nombre { get; set; }

        public string Codigo { get; set; }
        public string NombreExtranjero { get; set; }
        public string Grupo { get; set; }
        public string RFC { get; set; }
        public string Calle { get; set; }
        public string Colonia { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }

    }
}
