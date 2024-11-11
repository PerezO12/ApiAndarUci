
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
        public EstudianteService(IEstudianteRepository estudianteRepository)
        {
            _estudianteRepo = estudianteRepository;
        }

        public async Task<EstudianteDto?> GetByIdWithDetailsAsync(int id)
        {
            var estudiante = await _estudianteRepo.GetByIdAsync(id);
            if(estudiante == null) return null;
            return new EstudianteDto{
                Id = estudiante.Id,
                UsuarioId = estudiante.UsuarioId,
                NombreCompleto = estudiante.AppUser.NombreCompleto,
                CarnetIdentidad = estudiante.AppUser.CarnetIdentidad,
                NombreUsuario = estudiante.AppUser.UserName,
                Email = estudiante.AppUser.Email,
                NumeroTelefono = estudiante.AppUser.PhoneNumber,
                NombreCarrera = estudiante.Carrera.Nombre,
                NombreFacultad = estudiante.Facultad.Nombre
            };
        }

        public async Task<List<EstudianteDto>> GetEstudiantesWithDetailsAsync(QueryObjectEstudiante query)
        {
            //consulta basic
            var estudiantes = await _estudianteRepo.GetAllAsync(query);
            var estudiantesDto = estudiantes.Select( e => new EstudianteDto{
                Id = e.Id,
                UsuarioId = e.UsuarioId,
                NombreCompleto = e.AppUser.NombreCompleto,
                CarnetIdentidad = e.AppUser.CarnetIdentidad,
                NombreUsuario = e.AppUser.UserName,
                Email = e.AppUser.Email,
                NumeroTelefono = e.AppUser.PhoneNumber,
                NombreCarrera = e.Carrera.Nombre,
                NombreFacultad = e.Facultad.Nombre
            }).ToList();
            
            return estudiantesDto;
        }   
    }
}