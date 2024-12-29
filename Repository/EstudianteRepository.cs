using ApiUCI.Dtos.Estudiante;
using Microsoft.EntityFrameworkCore;
using ApiUCI.Helpers;
using ApiUCI.Interfaces;
using ApiUCI.Mappers;
using ApiUCI.Models;

namespace ApiUCI.Repository
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
            try
            {
                await _context.Estudiante.AddAsync(estudianteModel);
                await _context.SaveChangesAsync();
                return estudianteModel;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<Estudiante?> DeleteAsync(int id)
        {
            try
            {
                var estudianteModel = await _context.Estudiante.FindAsync(id);
                if(estudianteModel == null) return null;

                estudianteModel.Activo = false;

                await _context.SaveChangesAsync();
                return estudianteModel;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public async Task<Estudiante?> DeleteByUserIdAsync(string userId)
        {
            try
            {
                var estudianteModel = await _context.Estudiante.FirstOrDefaultAsync( e => e.UsuarioId == userId && e.Activo == true);
                if(estudianteModel == null) return null;

                estudianteModel.Activo = false;

                await _context.SaveChangesAsync();
                return estudianteModel;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        
        }

        public async Task<List<Estudiante>> GetAllAsync(QueryObjectEstudiante query)
        {

            var estudiantes = _context.Estudiante
                .Where(e => e.Activo == true)
                .Include(e => e.AppUser)
                .Include(e => e.Carrera)
                .Include(e => e.Facultad)
                .AsQueryable();
            //BUSQUEDAS
            //busqueda por nombre
            if(query.Nombre != null)
            {
                estudiantes = estudiantes.Where(e => e.AppUser!.NombreCompleto.ToLower().Contains(query.Nombre.ToLower()));
            }
            //busqueda por nombr carrera
            if(query.CarreraNombre != null)
            {
                estudiantes = estudiantes.Where(e => e.Carrera!.Nombre.ToLower().Contains(query.CarreraNombre.ToLower()));
            }
            //busqueda por nombre facultad
            if(query.FacultadNombre != null)
            {
                estudiantes = estudiantes.Where(e => e.Facultad!.Nombre.ToLower().Contains(query.FacultadNombre.ToLower()));
            }
            //busqueda por carnet identidad
            if(query.CarnetIdentidad != null)
            {
                estudiantes = estudiantes.Where(e => e.AppUser!.CarnetIdentidad.Contains(query.CarnetIdentidad));
            }
            //busqueda USUARIO ID
            if(query.UsuarioId != null) 
            {
                estudiantes = estudiantes.Where(e => e.UsuarioId == query.UsuarioId);
                return await estudiantes.ToListAsync();
            }
            //buscar por facultadID
            if(query.FacultadId.HasValue) 
            {
                estudiantes = estudiantes.Where(e => e.FacultadId == query.FacultadId);
            }
            //buscar por carrera Id
            if(query.CarreraId.HasValue)
            {
                estudiantes = estudiantes.Where(e => e.CarreraId == query.CarreraId);
            }
            //Buscar por lsita de estudiantes
            if (query.ListaId.Any())
            {
                estudiantes = estudiantes.Where(e => query.ListaId.Contains(e.Id));
            }
            //buscar por lista de usuario id
            if(query.ListaUserId.Any())
            {
                estudiantes = estudiantes.Where(e => query.ListaUserId.Contains(e.UsuarioId));
            }

            //Ordenamiento
            if(!string.IsNullOrWhiteSpace(query.OrdernarPor))
            {
                if(query.OrdernarPor.Equals("Nombre", StringComparison.OrdinalIgnoreCase))
                {
                    estudiantes = query.Descender ? estudiantes.OrderByDescending(e => e.AppUser!.NombreCompleto) : estudiantes.OrderBy(e => e.AppUser!.NombreCompleto);
                }
                else if(query.OrdernarPor.Equals("Carrera", StringComparison.OrdinalIgnoreCase))
                {
                    estudiantes = query.Descender ? estudiantes.OrderByDescending(e => e.Carrera!.Nombre) : estudiantes.OrderBy(e => e.Carrera!.Nombre);
                }
                else if(query.OrdernarPor.Equals("Facultad", StringComparison.OrdinalIgnoreCase))
                {
                    estudiantes = query.Descender ? estudiantes.OrderByDescending(e => e.Facultad!.Nombre) : estudiantes.OrderBy(e => e.Facultad!.Nombre);
                }
            }
            var skipNumber = ( query.NumeroPagina - 1 ) * query.TamañoPagina;
            
            return await estudiantes.Skip(skipNumber).Take(query.TamañoPagina).ToListAsync();
        }
        
        //GetByID
        public async Task<Estudiante?> GetByIdAsync(int Id)
        {
            return await _context.Estudiante
                .Where(c => c.Id == Id && c.Activo == true)
                .Include(e => e.AppUser)
                .Include(e => e.Carrera)
                .Include(e => e.Facultad)
                .FirstOrDefaultAsync();
        }

        public async Task<Estudiante?> GetEstudianteByUserId(string usuarioId)
        {
            return await _context.Estudiante
                .Where(c => c.UsuarioId == usuarioId && c.Activo == true)
                .Include(e => e.AppUser)
                .Include(e => e.Carrera)
                .Include(e => e.Facultad)
                .FirstOrDefaultAsync();
        
        }

        public async Task<Estudiante?> UpdateAsync(int id, EstudianteUpdateDto estudianteUpdateDto)
        {
            var estudianteExistente = await _context.Estudiante.FindAsync(id);

            if(estudianteExistente == null) return null;

            estudianteExistente.UpdateEstudiante(estudianteUpdateDto);
            await _context.SaveChangesAsync();
            return estudianteExistente;
        }
        public async Task<Estudiante?> UpdateEstudianteByUserIdAsync(string id, EstudianteUpdateDto estudianteUpdateDto)
        {
            var estudianteExistente = await _context.Estudiante.FirstOrDefaultAsync(e => e.UsuarioId == id);
            if(estudianteExistente == null) return null;

            estudianteExistente.UpdateEstudiante(estudianteUpdateDto);
            await _context.SaveChangesAsync();
            return estudianteExistente;
        }
    }
}