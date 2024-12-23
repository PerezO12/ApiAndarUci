using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Contracts.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApiUCI.Dtos.Estudiante;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Models;
using MyApiUCI.Repository;

namespace MyApiUCI.Controller
{
    [ApiController]
    [Route(ApiRoutes.Estudiante.RutaGenaral)]
    public class EstudianteController : ControllerBase
    {
        public readonly IEstudianteService _estudianteService;

        public EstudianteController(IEstudianteService estudianteService)
        {
            _estudianteService = estudianteService;
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObjectEstudiante query)
        { 
            if (query.NumeroPagina <= 0)
            {
                return BadRequest("El número de página debe ser mayor que cero.");
            }

            if (query.TamañoPagina <= 0)
            {
                return BadRequest("El tamaño de la página debe ser mayor que cero.");
            }
           var estudiantes = await _estudianteService.GetEstudiantesWithDetailsAsync(query);
           return Ok(estudiantes);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute]int id) {
            var estudiante = await _estudianteService.GetByIdWithDetailsAsync(id);
            if(estudiante == null) return NotFound("El estudiante no existe");
            
            return Ok(estudiante);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet(ApiRoutes.Estudiante.GetByUserId)]
        public async Task<IActionResult> GetByUserId([FromRoute] string id)
        {
            var estudiante = await _estudianteService.GetEstudianteWithByUserId(id);
            if(estudiante == null) return NotFound(new {msg = "El estudiante no existe"});

            return Ok(estudiante);
        }
    }
}
