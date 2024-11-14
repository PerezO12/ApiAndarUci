using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos.Formulario;
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

        public async Task<IEnumerable<Formulario>> GetAllAsync(QueryObjectFormulario query)
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los formularios con los parámetros proporcionados.");
                var formularios = _context.Formulario.Where(f => f.Activo == true).AsQueryable();

                if (query.UsuarioId != null)
                {
                    formularios = formularios.Where(f => f.UsuarioId == query.UsuarioId);
                }
                if (query.ListaId.Any())
                {
                    formularios = formularios.Where(f => query.ListaId.Contains(f.Id));
                }
                if (query.DepartamentoId.HasValue)
                {
                    formularios = formularios.Where(f => f.DepartamentoId == query.DepartamentoId);
                }

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

        public async Task<Formulario?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo formulario con Id {Id}.", id);
                var formulario = await _context.Formulario.FindAsync(id); //poner solo activos
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

        public async Task<Formulario?> UpadatePatchAsync(UpdateFormularioDto formulario, int id)
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

                //mapeo
                formularioExistente.toPatchFormulario(formulario);
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
    }
}
