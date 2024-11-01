using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Mappers;
using MyApiUCI.Models;

namespace MyApiUCI.Repository
{
    public class EstudianteRepository : IEstudianteRepository
    {
        private readonly ApplicationDbContext _context;
        
        public EstudianteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Estudiante> CreateAsync(Estudiante estudianteModel)
        {
            await _context.estudiante.AddAsync(estudianteModel);
            await _context.SaveChangesAsync();
            return estudianteModel;
        }

        public async Task<Estudiante?> DeleteAsync(int id)
        {
            var estudianteModel = await _context.estudiante.FindAsync(id);
            if(estudianteModel == null) return null;

            estudianteModel.Activo = false;

            await _context.SaveChangesAsync();
            return estudianteModel;
        }
        //Falta terminar, unirlos con el usuario
        public async Task<List<Estudiante>> GetAllAsync(QueryObject query)
        {

            var estudiantes = _context.estudiante.Where(e => e.Activo == true).AsQueryable();
        
            if(query.FacultadId.HasValue)
            {
                estudiantes = estudiantes.Where(e => e.FacultadId == query.FacultadId);
            }
            if(query.CarreraId.HasValue)
            {
                estudiantes = estudiantes.Where(e => e.CarreraId == query.CarreraId);
            }
            //Ordenamiento
            if(!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if(query.SortBy.Equals("Nombre", StringComparison.OrdinalIgnoreCase))
                {
                    estudiantes = query.IsDescending ? estudiantes.OrderByDescending(d => d.CarreraId) : estudiantes.OrderBy(d => d.CarreraId);
                }
                else if(query.SortBy.Equals("Facultad", StringComparison.OrdinalIgnoreCase))
                {
                    estudiantes = query.IsDescending ? estudiantes.OrderByDescending(d => d.FacultadId) : estudiantes.OrderBy( d => d.FacultadId);
                }
            }
            var skipNumber = ( query.PageNumber - 1 ) * query.PageSize;
            
            return await estudiantes.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<Estudiante?> GetByIdAsync(int Id)
        {
            return await _context.estudiante.FirstOrDefaultAsync(c => c.Id == Id &&c.Activo == true);
        }

        public async Task<Estudiante?> UpdateAsync(int id, Estudiante estudianteModel)
        {
            var estudianteExistente = await _context.estudiante.FindAsync(id);

            if(estudianteExistente == null) return null;

            estudianteExistente.UpdateEstudiante(estudianteModel);
            await _context.SaveChangesAsync();
            return estudianteExistente;
        }
    }
}