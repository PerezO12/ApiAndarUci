using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
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
            await _context.Estudiante.AddAsync(estudianteModel);
            await _context.SaveChangesAsync();
            return estudianteModel;
        }

        public async Task<Estudiante?> DeleteAsync(int id)
        {
            var estudianteModel = await _context.Estudiante.FindAsync(id);
            if(estudianteModel == null) return null;

            estudianteModel.Activo = false;

            await _context.SaveChangesAsync();
            return estudianteModel;
        }
        //Falta terminar, unirlos con el usuario
        /*  */
        public async Task<List<Estudiante>> GetAllAsync(QueryObjectEstudiante query)
        {

            var estudiantes = _context.Estudiante.Where(e => e.Activo == true).AsQueryable();
            
            if(query.UsuarioId != null)
            {
                estudiantes = estudiantes.Where(e => e.UsuarioId == query.UsuarioId);
                return await estudiantes.ToListAsync();
            }

            if (query.ListaId.Any())
            {
                estudiantes = estudiantes.Where(e => query.ListaId.Contains(e.Id));
            }
            if(query.ListaUserId.Any())
            {
                estudiantes = estudiantes.Where(e => query.ListaUserId.Contains(e.UsuarioId));
            }

            if(query.FacultadId.HasValue)
            {
                estudiantes = estudiantes.Where(e => e.FacultadId == query.FacultadId);
            }
            if(query.CarreraId.HasValue)
            {
                estudiantes = estudiantes.Where(e => e.CarreraId == query.CarreraId);
            }
            //Ordenamiento
            if(!string.IsNullOrWhiteSpace(query.OrdernarPor))
            {
                if(query.OrdernarPor.Equals("Carrera", StringComparison.OrdinalIgnoreCase))
                {
                    estudiantes = query.Descender ? estudiantes.OrderByDescending(d => d.CarreraId) : estudiantes.OrderBy(d => d.CarreraId);
                }
                else if(query.OrdernarPor.Equals("Facultad", StringComparison.OrdinalIgnoreCase))
                {
                    estudiantes = query.Descender ? estudiantes.OrderByDescending(d => d.FacultadId) : estudiantes.OrderBy( d => d.FacultadId);
                }
            }
            var skipNumber = ( query.NumeroPagina - 1 ) * query.TamañoPagina;
            
            return await estudiantes.Skip(skipNumber).Take(query.TamañoPagina).ToListAsync();
        }
        
        //GetByID
        public async Task<Estudiante?> GetByIdAsync(int Id)
        {
            return await _context.Estudiante.FirstOrDefaultAsync(c => c.Id == Id &&c.Activo == true);
        }

        public async Task<Estudiante?> UpdateAsync(int id, Estudiante estudianteModel)
        {
            var estudianteExistente = await _context.Estudiante.FindAsync(id);

            if(estudianteExistente == null) return null;

            estudianteExistente.UpdateEstudiante(estudianteModel);
            await _context.SaveChangesAsync();
            return estudianteExistente;
        }
    }
}