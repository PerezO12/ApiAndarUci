using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;

namespace MyApiUCI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncargadoController : ControllerBase
    {
        private readonly IEncargadoService _encargadoService;
        public EncargadoController(IEncargadoService encargadoService)
        {
            _encargadoService = encargadoService;
        }

        //[Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObjectEncargado query)
        { 
            if (query.NumeroPagina <= 0)
            {
                return BadRequest("El número de página debe ser mayor que cero.");
            }

            if (query.TamañoPagina <= 0)
            {
                return BadRequest("El tamaño de la página debe ser mayor que cero.");
            }
           var encargados = await _encargadoService.GetAllEncargadosWithDetailsAsync(query);
           
           return Ok(encargados);
        }
    }
}