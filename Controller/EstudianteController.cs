using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;

namespace MyApiUCI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstudianteController : ControllerBase
    {
        private readonly IEstudianteRepository _estudianteRepo;

        public EstudianteController(IEstudianteRepository estudianteRepository)
        {
            _estudianteRepo = estudianteRepository;
        }

        //FALTA, y arreglar  y completar **************************************+
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        { 
            var estudiantesModel = await _estudianteRepo.GetAllAsync(query);
            
            return Ok(estudiantesModel);
;
        }
    }
}