using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApiUCI.Dtos.Estudiante;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Models;

namespace MyApiUCI.Service
{
    public class EstudianteService : IEstudianteService
    {
        private readonly IEstudianteRepository _estudianteRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICarreraRepository _carreraRepo;
        private readonly IFacultadRepository _facultadRepo;

        public EstudianteService(
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

        public async Task<List<EstudianteDto>> GetEstudiantesWithDetailsAsync(QueryObject query)
        {
            //consulta basic
            var estudiantesModel = await _estudianteRepo.GetAllAsync(query);

            //IDS de las entidades 
            var estudiantesIds = estudiantesModel.Select( e => e.UsuarioId).ToList();
            var facultadesIds = estudiantesModel.Select( e => e.FacultadId).Distinct().ToList();
            var carreraIds = estudiantesModel.Select( e => e.CarreraId).Distinct().ToList();

            //consultas
            var usuariosEstudiantes = await _userManager.Users
                .Where(u => estudiantesIds.Contains(u.Id))
                .ToListAsync();
            
            var carreras = await _carreraRepo.GetAllAsync(new QueryObject { ListaId = carreraIds});
            var facultades = await _facultadRepo.GetAllAsync(new QueryObject { ListaId = carreraIds});
    //*******************************************
            Console.WriteLine("IDs de estudiantes: " + string.Join(", ", estudiantesIds));
            Console.WriteLine("IDs de facultades: " + string.Join(", ", facultadesIds));
            Console.WriteLine("IDs de carreras: " + string.Join(", ", carreraIds));

            foreach (var usuario in usuariosEstudiantes)
                Console.WriteLine($"Usuario obtenido: {usuario.Id} - {usuario.NombreCompleto}");

            foreach (var carrera in carreras)
                Console.WriteLine($"Carrera obtenida: {carrera.Id} - {carrera.Nombre}");

            foreach (var facultad in facultades)
                Console.WriteLine($"Facultad obtenida: {facultad.Id} - {facultad.Nombre}");

    //*******************************************
            //combinacion datos
            var estudiantesDatosCombinados  = estudiantesModel
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
                        Id = combinado.Estudiante.Id,
                        NombreCompleto = combinado.Usuario.NombreCompleto,
                        CarnetIdentidad = combinado.Usuario.CarnetIdentidad,
                        UserName = combinado.Usuario.UserName,
                        Email = combinado.Usuario.Email,
                        NumeroTelefono = combinado.Usuario.PhoneNumber,
                        Carrera = combinado.Carrera.Nombre,
                        Facultad = facultad.Nombre
                    })
                .ToList();
            
            if(query.Nombre != null)
            {
                estudiantesDatosCombinados = estudiantesDatosCombinados
                    .Where( e => e.NombreCompleto != null && e.NombreCompleto.Contains(query.Nombre, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            
            
            return estudiantesDatosCombinados;
        }   
    }
}