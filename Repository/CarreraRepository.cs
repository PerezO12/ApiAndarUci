using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApiUCI.Dtos.Carrera;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Mappers;
using MyApiUCI.Models;

namespace MyApiUCI.Repository
{
    public class CarreraRepository : ICarreraRepository
    {
        private readonly ApplicationDbContext _context;
        public CarreraRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Carrera> CreateAsync(Carrera carreraModel)
        {
            await _context.carrera.AddAsync(carreraModel);
            await _context.SaveChangesAsync();
            return carreraModel;
        }

        public async Task<Carrera?> DeleteAsync(int id)
        {
            var carreraExist = await _context.carrera.FirstOrDefaultAsync(c => c.Id == id && c.Activo == true);
            
            if(carreraExist == null)
            {
                return null;
            }
            carreraExist.Activo = false;

            await _context.SaveChangesAsync();

            return carreraExist;
        }

        public async Task<bool> ExisteCarrera(int id)
        {
            return await _context.carrera.AnyAsync(c => c.Id == id && c.Activo == true);
        }

        public async Task<List<Carrera>> GetAllAsync(QueryObject query)
        {
            /* return await _context.carrera
                .Where(c => c.Activo == true)
                .ToListAsync(); */
            var carreras = _context.carrera.Where(c => c.Activo == true).AsQueryable();
            
            //Validacion de busquedas
            if(!string.IsNullOrWhiteSpace(query.Nombre))
            {
                carreras = carreras.Where( c => c.Nombre.ToLower() == query.Nombre.ToLower() );
            }
            if(query.FacultadId.HasValue)
            {
                carreras = carreras.Where( c => c.FacultadId == query.FacultadId);
            }
            //Orndea
            if(!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if(query.SortBy.Equals("Nombre", StringComparison.OrdinalIgnoreCase))
                {
                    carreras = query.IsDescending ? carreras.OrderByDescending(d => d.Nombre) : carreras.OrderBy(d => d.Nombre);
                }
                else if(query.SortBy.Equals("Facultad", StringComparison.OrdinalIgnoreCase))
                {
                    carreras = query.IsDescending ? carreras.OrderByDescending(d => d.FacultadId) : carreras.OrderBy( d => d.FacultadId);
                }
            }
            //Paginacion
            var skipNumber = ( query.PageNumber - 1) * query.PageSize;
            
            return await carreras.Skip(skipNumber).Take(query.PageSize).ToListAsync();

        }

        public async Task<Carrera?> GetByIdAsync(int id)
        {
            return await _context.carrera
                .FirstOrDefaultAsync(c => c.Id == id && c.Activo == true);
        }

        public async Task<Carrera?> PatchAsync(int id, PatchCarreraDto carreraDto)
        {
            var carreraModel = await _context.carrera
                                .FirstOrDefaultAsync( c => c.Id == id && c.Activo == true);
            if(carreraModel == null)
            {
                return null;
            }
            carreraModel.toPatchingCarrera(carreraDto);
            
            await _context.SaveChangesAsync();
            return carreraModel;

        }

        public async Task<Carrera?> UpdateAsync(int id, Carrera carreraModel)
        {
            var carreraExist = await _context.carrera
                                .FirstOrDefaultAsync( c => c.Id == id && c.Activo == true);
            if(carreraExist == null)
            {
                return null;
            }
            carreraExist.FacultadId = carreraModel.FacultadId;
            carreraExist.Nombre = carreraModel.Nombre;

            await _context.SaveChangesAsync();
            
            return carreraExist;
        }
    }
}