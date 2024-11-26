using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos.Formulario;
using ApiUCI.Helpers.Querys;
using Microsoft.EntityFrameworkCore;
using MyApiUCI.Dtos.Formulario;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Mappers;
using MyApiUCI.Models;

namespace MyApiUCI.Repository
{
    public class FormularioRepository : IFormularioRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormularioRepository> _logger;

        public FormularioRepository(ApplicationDbContext context, ILogger<FormularioRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Formulario> CreateAsync(Formulario formulario)
        {
            try
            {
                _logger.LogInformation("Iniciando la creación de un nuevo formulario.");
                await _context.Formulario.AddAsync(formulario);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Formulario creado con éxito.");
                return formulario;            
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el formulario: {ex.Message}", ex);
                throw; // Re-lanzar la excepción para que sea manejada a un nivel superior si es necesario
            }
        }

        public async Task<Formulario?> DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Iniciando la eliminación del formulario con Id {Id}.", id);
                var formularioModel = await _context.Formulario.FirstOrDefaultAsync(f => f.Id == id && f.Activo == true);
                
                if (formularioModel == null)
                {
                    _logger.LogWarning("Formulario con Id {Id} no encontrado o ya está desactivado.", id);
                    return null;
                }
                
                formularioModel.Activo = false;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Formulario con Id {Id} desactivado con éxito.", id);
                return formularioModel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el formulario con Id {id}: {ex.Message}", ex);
                throw; // Re-lanzar la excepción
            }
        }

        public async Task<Formulario?> FormByEstudianteDepartamentoAsync(int estudianteId, int DepartamentoId)
        {
            var formulario = await _context.Formulario.FirstOrDefaultAsync(f => f.Activo == true && f.EstudianteId == estudianteId && f.DepartamentoId == DepartamentoId);
        
            return formulario;
        }

        //Este solo sera utilizado x el administrador de la applicacion x lo q no importa  q tenga una gran carga
        public async Task<List<Formulario>> GetAllAsync(QueryObjectFormulario query)
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los formularios con los parámetros proporcionados.");
                var formularios = _context.Formulario
                    .Where(f => f.Activo == true)
                    .Include(f => f.Estudiante)
                        .ThenInclude(e => e!.Carrera)
                    .Include(f => f.Estudiante!.AppUser)
                    .Include(f => f.Departamento)
                    .Include(f => f.Departamento!.Facultad)
                    .Include(f => f.Encargado)
                    .Include(f => f.Encargado!.AppUser)
                    .AsQueryable();

                if (query.UsuarioId != null) //Buscar por un id de usuario
                {
                    formularios = formularios.Where(f => f.Estudiante!.UsuarioId == query.UsuarioId);
                }
                if (query.ListaId.Any()) // buscar por una lista de id de formularios
                {
                    formularios = formularios.Where(f => query.ListaId.Contains(f.Id));
                }
                //todo: Implemetar busqueda por nombre de estudiante y otras
                 if (query.DepartamentoId.HasValue)
                {
                    formularios = formularios.Where(f => f.DepartamentoId == query.DepartamentoId);
                } 
                //TODO:AGREGAR ORDENAMIENTO
                formularios = formularios.OrderBy(f => f.Id);
                
                var skipNumber = (query.NumeroPagina - 1) * query.TamañoPagina;
                var result = await formularios.Skip(skipNumber).Take(query.TamañoPagina).ToListAsync();
                _logger.LogInformation("Se han obtenido {Count} formularios.", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener formularios: {ex.Message}", ex);
                throw; // Re-lanzar la excepción
            }
        }

        public async Task<List<FormularioEncargadoDto>> GetAllFormulariosByEncargado(string userId, QueryObjectFormularioEncargado query)
        {
            var formularios =  _context.Formulario
                .Where(e => e.Activo == true && e.Encargado!.AppUser!.Id == userId)
                .Select(e => new FormularioEncargadoDto{
                    Id = e.Id,
                    NombreCompletoEstudiante = e.Estudiante!.AppUser!.NombreCompleto,
                    Firmado = e.Firmado,
                    NombreCarrera = e.Estudiante!.Carrera!.Nombre,
                    Motivo = e.Motivo,
                    Fechacreacion = e.Fechacreacion
                }).AsQueryable();
            formularios = formularios.Where(f => f.Firmado == query.Firmados);
            if(query.Nombre != null) //Buscar por nombre
            {
                formularios = formularios
                    .Where(f => f.NombreCompletoEstudiante.ToLower().Contains(query.Nombre.ToLower()));
            }
            if(query.Carrera != null) //Buscar por carreara nombre
            {
                formularios = formularios
                    .Where(f => f.NombreCarrera.ToLower().Contains(query.Carrera.ToLower()));
            }
            //Ordenar por
            if(!string.IsNullOrWhiteSpace(query.OrdenarPor))
            {
                if (query.OrdenarPor.Equals("Nombre", StringComparison.OrdinalIgnoreCase))
                {
                    formularios = query.Descender ? formularios.OrderByDescending(f => f.NombreCompletoEstudiante) : formularios.OrderBy(f => f.NombreCompletoEstudiante);
                }
                else if (query.OrdenarPor.Equals("Carrera", StringComparison.OrdinalIgnoreCase))
                {
                    formularios = query.Descender ? formularios.OrderByDescending(f => f.NombreCarrera) : formularios.OrderBy(f => f.NombreCarrera);
                }
                else if (query.OrdenarPor.Equals("Fecha", StringComparison.OrdinalIgnoreCase))
                {
                    formularios = query.Descender ? formularios.OrderByDescending(f => f.Fechacreacion) : formularios.OrderBy(f => f.Fechacreacion);
                }
                
            // Paginación
            var skipNumber = (query.NumeroPagina - 1) * query.TamañoPagina;

            return await formularios.Skip(skipNumber).Take(query.TamañoPagina).ToListAsync();
            }

            return await formularios.ToListAsync();;
        }

        public async Task<List<FormularioEstudianteDto>> GetAllFormulariosByEstudiante(string userId, QueryObjectFormularioEstudiantes query)
        {

             var formularios = _context.Formulario
                .Where(e => e.Activo == true && e.Estudiante!.AppUser!.Id == userId)
                .Select(e => new FormularioEstudianteDto
                {
                    Id = e.Id,
                    NombreEncargado = e.Encargado!.AppUser!.NombreCompleto,
                    NombreDepartamento= e.Departamento!.Nombre,
                    Motivo = e.Motivo,
                    Firmado = e.Firmado,
                    FechaFirmado = e.FechaFirmado,
                    Fechacreacion = e.Fechacreacion
                })
                .AsQueryable();

            if(query.Nombre != null) {
                formularios = formularios.Where(f => f.NombreEncargado.ToLower().Contains(query.Nombre.ToLower()));
            }
            if(query.Departamento != null) {
                formularios = formularios.Where(f => f.NombreDepartamento.ToLower().Contains(query.Departamento.ToLower()));
            }
            return await formularios.ToListAsync();
        }

        public async Task<Formulario?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo formulario con Id {Id}.", id);
                var formulario = await _context.Formulario
                    .Where(f => f.Id == id && f.Activo == true)
                    .Include(f => f.Estudiante)
                        .ThenInclude(e => e!.Carrera)
                    .Include(f => f.Estudiante!.AppUser)
                    .Include(f => f.Departamento)
                    .Include(f => f.Departamento!.Facultad)
                    .Include(f => f.Encargado)
                    .Include(f => f.Encargado!.AppUser)
                    .FirstOrDefaultAsync();
                    
                if (formulario == null)
                {
                    _logger.LogWarning("Formulario con Id {Id} no encontrado.", id);
                }
                return formulario;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener el formulario con Id {id}: {ex.Message}", ex);
                throw; // Re-lanzar la excepción
            }
        }

        public async Task<FormularioEncargadoDto?> GetFormEstudianteByIdForEncargadoAsync(int encargadoId, int formularioId)
        {
            var formulario = await _context.Formulario
                .Where(f => f.Id == formularioId && f.Activo == true)
                .Select(f => new FormularioEncargadoDto
                {
                    Id = f.Id,
                    NombreCompletoEstudiante = f.Estudiante!.AppUser!.NombreCompleto,
                    NombreUsuario = f.Estudiante.AppUser.UserName,
                    Email = f.Estudiante.AppUser.Email,
                    CarnetIdentidad = f.Estudiante.AppUser.CarnetIdentidad,
                    NumeroTelefono = f.Estudiante.AppUser.PhoneNumber,
                    NombreCarrera = f.Estudiante.Carrera!.Nombre,
                    Motivo = f.Motivo,
                    Fechacreacion = f.Fechacreacion
                }).FirstOrDefaultAsync();
                

            return formulario;
        }

        public async Task<Formulario?> UpadatePatchAsync(int id, UpdateFormularioDto formulario)
        {
            try
            {
                _logger.LogInformation("Iniciando la actualización del formulario con Id {Id}.", id);
                var formularioExistente = await _context.Formulario.FirstOrDefaultAsync(f => f.Id == id && f.Activo == true);
                if (formularioExistente == null)
                {
                    _logger.LogWarning("Formulario con Id {Id} no encontrado o ya está desactivado.", id);
                    return null;
                }
                formularioExistente.Motivo = formulario.Motivo ?? formularioExistente.Motivo;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Formulario con Id {Id} actualizado con éxito.", id);
                return formularioExistente;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el formulario con Id {id}: {ex.Message}", ex);
                throw; // Re-lanzar la excepción
            }
        }

        public async Task<Formulario?> UpdateAsync(int id, Formulario formulario)
        {
            var formularioExistente = await _context.Formulario.FindAsync(id);
            if(formularioExistente == null) return null;

            _context.Entry(formularioExistente).CurrentValues.SetValues(formulario); 

            await _context.SaveChangesAsync();
            return formularioExistente;
        }
    }
}
