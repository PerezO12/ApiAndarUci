using Microsoft.EntityFrameworkCore;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Models;

namespace MyApiUCI.Repository
{

    public class FacultadRepository : IFacultadRepository
    
    {
        private readonly ApplicationDbContext _context;

        public FacultadRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Facultad> CreateAsync(Facultad facultadModel)
        {
            await _context.facultad.AddAsync(facultadModel);
            await _context.SaveChangesAsync();
            return facultadModel;
        }

        public async Task<Facultad?> DeleteAsync(int id)
        {
            var facultadModel = await _context.facultad.FirstOrDefaultAsync(f => f.Id == id && f.Activo == true);
            
            if(facultadModel == null)
            {
                return null;
            }
            facultadModel.Activo = false;
            await _context.SaveChangesAsync();
            return facultadModel;
        }

        public async Task<bool> FacultyExists(int id)
        {
            return await _context.facultad.AnyAsync(f => f.Id == id && f.Activo == true);
        }

        public async Task<List<Facultad>> GetAllAsync(QueryObject query)
        {
            var facultades = _context.facultad.Where(f => f.Activo == true).AsQueryable();

            if (query.ListaId.Any())
            {
                facultades = facultades.Where(f => query.ListaId.Contains(f.Id));
            }

            if (!string.IsNullOrWhiteSpace(query.Nombre))
            {
                facultades = facultades.Where(f => f.Nombre.ToLower() == query.Nombre.ToLower());
            }

            // Ordenar
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Nombre", StringComparison.OrdinalIgnoreCase))
                {
                    facultades = query.IsDescending ? facultades.OrderByDescending(f => f.Nombre) : facultades.OrderBy(f => f.Nombre);
                }
                else if (query.SortBy.Equals("Id", StringComparison.OrdinalIgnoreCase))
                {
                    facultades = query.IsDescending ? facultades.OrderByDescending(f => f.Id) : facultades.OrderBy(f => f.Id);
                }
            }

            // Paginación
            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await facultades.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<Facultad?> GetByIdAsync(int id)
        {
            return await _context.facultad.FirstOrDefaultAsync(f => f.Id == id && f.Activo == true);
            //return await _context.facultad.FindAsync(id);
        }

        public async Task<Facultad?> UpdateAsync(int id, Facultad facultadModel)
        {
            var existingFacultad = await _context.facultad.FindAsync(id);

            if(existingFacultad == null)
            {
                return null;
            }
            existingFacultad.Nombre = facultadModel.Nombre;
            await _context.SaveChangesAsync();
            return existingFacultad;
        }
    }
}