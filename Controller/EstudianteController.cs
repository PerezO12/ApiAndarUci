using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Route("api/[controller]")]
    [ApiController]
    public class EstudianteController : ControllerBase
    {
        public readonly IEstudianteService _estudianteService;

        public EstudianteController(IEstudianteService estudianteService)
        {
            _estudianteService = estudianteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        { 
            if (query.PageNumber <= 0)
            {
                return BadRequest("El número de página debe ser mayor que cero.");
            }

            if (query.PageSize <= 0)
            {
                return BadRequest("El tamaño de la página debe ser mayor que cero.");
            }
           var estudiantes = await _estudianteService.GetEstudiantesWithDetailsAsync(query);
           return Ok(estudiantes);
        }
    }
}
