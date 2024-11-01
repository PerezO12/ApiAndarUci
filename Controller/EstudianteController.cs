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
        private readonly IEstudianteRepository _estudianteRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICarreraRepository _carreraRepo;
        private readonly IFacultadRepository _facultadRepo;

        public EstudianteController(
            IEstudianteRepository estudianteRepository,
            UserManager<AppUser> user,
            ICarreraRepository carreraRepo,
            IFacultadRepository facultadRepo
            )
        {
            _estudianteRepo = estudianteRepository;
            _userManager = user;
            _carreraRepo  = carreraRepo;
            _facultadRepo = facultadRepo;
        }

        //FALTA, y arreglar  y completar **************************************+
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        { 
            var estudiantesModel = await _estudianteRepo.GetAllAsync(query);

            var estudiantesIds = estudiantesModel.Select(e => e.UsuarioId).ToList();
            var carreraIds = estudiantesModel.Select(e => e.CarreraId).Distinct().ToList();
            var facultadIds = estudiantesModel.Select(e => e.FacultadId).Distinct().ToList();


            var usuariosEstudiantes = await _userManager.Users.Where(u => estudiantesIds.Contains(u.Id))
                                            .ToListAsync();
            
            var carreras = await _carreraRepo.GetAllAsync(new QueryObject{ListaId = carreraIds}); // Suponiendo que tu repositorio acepta lista de IDs
            var facultades = await _facultadRepo.GetAllAsync(new QueryObject{ListaId = facultadIds});


            var estudiantesDatosCombinados = estudiantesModel
                .Join(
                    usuariosEstudiantes,
                    estudiante => estudiante.UsuarioId,
                    usuario => usuario.Id,
                    (estudiante, usuario) => new
                    {
                        Estudiante = estudiante,
                        Usuario = usuario
                    })
                .Join(
                    carreras,
                    combinado => combinado.Estudiante.CarreraId,
                    carrera => carrera.Id,
                    (combinado, carrera) => new 
                    {
                        combinado.Estudiante,
                        combinado.Usuario,
                        Carrera = carrera
                    })
                .Join(
                    facultades,
                    combinado => combinado.Estudiante.FacultadId,
                    facultad => facultad.Id,
                    (combinado, facultad) => new EstudianteDto
                    {
                        NombreCompleto = combinado.Usuario.NombreCompleto,
                        CarnetIdentidad = combinado.Usuario.CarnetIdentidad,
                        UserName = combinado.Usuario.UserName,
                        Email = combinado.Usuario.Email,
                        NumeroTelefono = combinado.Usuario.PhoneNumber,
                        Carrera = combinado.Carrera.Nombre,
                        Facultad = facultad.Nombre 
                    })
                .ToList();
            return Ok(estudiantesDatosCombinados);
        }
    }
}
