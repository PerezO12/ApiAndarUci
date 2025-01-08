using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ApiUci.Dtos.Carrera;
using ApiUci.Helpers;
using ApiUci.Interfaces;
using ApiUci.Mappers;
using ApiUci.Models;

namespace ApiUci.Repository
{
    public class CarreraRepository : ICarreraRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CarreraRepository> _logger;
        public CarreraRepository(ApplicationDbContext context, ILogger<CarreraRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Carrera> CreateAsync(Carrera carreraModel)
        {
            try
            {
                await _context.Carrera.AddAsync(carreraModel);
                await _context.SaveChangesAsync();
                await _context.Entry(carreraModel)
                    .Reference(d => d.Facultad)
                    .LoadAsync();
                return carreraModel;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al crear la carrera: {ex.Message}", ex);
                throw;
            }
            
        }

        public async Task<Carrera?> DeleteAsync(int id)
        {
            try
            {
                var carreraExist = await _context.Carrera.FirstOrDefaultAsync(c => c.Id == id && c.Activo == true);
                
                if(carreraExist == null)
                {
                    return null;
                }
                carreraExist.Activo = false;

                await _context.SaveChangesAsync();

                return carreraExist;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al borrar la tarea {id}: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<bool> ExisteCarrera(int id)
        {
            try{       
                return await _context.Carrera.AnyAsync(c => c.Id == id && c.Activo == true);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al verificar si la carrera existe {id}: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<List<Carrera>> GetAllAsync(QueryObjectCarrera query)
        {
            try
            {
                var carreras = _context.Carrera
                    .Where(c => c.Activo == true)
                    .Include(c => c.Facultad)
                    .AsQueryable();
                
                //Validacion de busquedas
                if(!string.IsNullOrWhiteSpace(query.Nombre))
                    carreras = carreras.Where( c => c.Nombre.ToLower().Contains(query.Nombre.ToLower()) );

                if(!string.IsNullOrWhiteSpace(query.Facultad))
                    carreras = carreras.Where( c => c.Facultad!.Nombre.ToLower().Contains(query.Facultad.ToLower()));

                if( query.FacultadId.HasValue && query.FacultadId > 0)
                    carreras = carreras.Where( c => c.FacultadId == query.FacultadId);
            
                //Orndea
                if(!string.IsNullOrWhiteSpace(query.OrdernarPor))
                {
                    if(query.OrdernarPor.Equals("Nombre", StringComparison.OrdinalIgnoreCase))
                    {
                        carreras = query.Descender ? carreras.OrderByDescending(d => d.Nombre) : carreras.OrderBy(d => d.Nombre);
                    }
                    else if(query.OrdernarPor.Equals("Facultad", StringComparison.OrdinalIgnoreCase))
                    {
                        carreras = query.Descender ? carreras.OrderByDescending(d => d.Facultad!.Nombre) : carreras.OrderBy( d => d.Facultad!.Nombre);
                    }
                    else if(query.OrdernarPor.Equals("Fecha", StringComparison.OrdinalIgnoreCase))
                    {
                        carreras = query.Descender ? carreras.OrderByDescending(d => d.Fechacreacion) : carreras.OrderBy( d => d.Fechacreacion);
                    }
                }
                //Paginacion
                var skipNumber = ( query.NumeroPagina - 1) * query.TamañoPagina;
                
                return await carreras.Skip(skipNumber).Take(query.TamañoPagina).ToListAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al obtener las carreras: {ex.Message}", ex);
                throw;
            }

        }

        public async Task<Carrera?> GetByIdAsync(int id)
        {
            try
            {
            return await _context.Carrera
                .Include(f => f.Facultad)
                .FirstOrDefaultAsync(c => c.Id == id && c.Activo == true);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al obtener la carrera con id {id}: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<Carrera?> PatchAsync(int id, PatchCarreraDto carreraDto)
        {
            try
            {

                var carreraModel = await _context.Carrera
                                    .FirstOrDefaultAsync( c => c.Id == id && c.Activo == true);
                if(carreraModel == null)
                {
                    return null;
                }
                carreraModel.toPatchingCarrera(carreraDto);
                
                await _context.SaveChangesAsync();
                await _context.Entry(carreraModel)
                    .Reference(d => d.Facultad)
                    .LoadAsync();
                return carreraModel;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al actualizar la carrera {id}: {ex.Message}", ex);
                throw;
            }

        }

        public async Task<Carrera?> UpdateAsync(int id, Carrera carreraModel)
        {
            try
            {
                var carreraExist = await _context.Carrera
                    .Include(c => c.Facultad)
                    .FirstOrDefaultAsync( c => c.Id == id && c.Activo == true);
                    
                if(carreraExist == null)
                {
                    return null;
                }
                carreraExist.FacultadId = carreraModel.FacultadId;
                carreraExist.Nombre = carreraModel.Nombre;
                await _context.Entry(carreraExist)
                    .Reference(d => d.Facultad)
                    .LoadAsync();

                await _context.SaveChangesAsync();
                return carreraExist;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al actualizar la carrera {id}: {ex.Message}", ex);
                throw;
            }
            
        }
    }
}