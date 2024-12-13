using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos.Encargado;
using Microsoft.EntityFrameworkCore;
using MyApiUCI.Dtos.Encargado;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Mappers;
using MyApiUCI.Models;

namespace MyApiUCI.Repository
{
    public class EncargadoRepository : IEncargadoRepository
    {
        private readonly ApplicationDbContext _context;
        public EncargadoRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Encargado> CreateAsync(Encargado encargadoModel)
        {
            await _context.Encargado.AddAsync(encargadoModel);
            await _context.SaveChangesAsync();
            return encargadoModel;
        }

        public async Task<Encargado?> DeleteAsync(int id)
        {
            var encargadoModel = await _context.Encargado.FindAsync(id);
            if(encargadoModel == null) return null;

            encargadoModel.Activo = false;

            await _context.SaveChangesAsync();
            return encargadoModel;
        }

        public async Task<Encargado?> DeleteByUserIdAsync(string userId)
        {
            var encargadoModel = await _context.Encargado.FirstOrDefaultAsync(e => e.UsuarioId == userId && e.Activo == true);
            if(encargadoModel == null) return null;

            encargadoModel.Activo = false;

            await _context.SaveChangesAsync();
            return encargadoModel;
        }

        public async Task<List<Encargado>> GetAllAsync(QueryObjectEncargado query)
        {
            var encargados = _context.Encargado
                .Where(e => e.Activo == true)
                .Include(e => e.AppUser)
                .Include(e => e.Departamento)
                .AsQueryable();
            //Busquedas
            if(query.Nombre != null)//BUSCAR POR NOMBRE Encargado
            {
                encargados = encargados.Where(e => e.AppUser!.NombreCompleto.ToLower().Contains(query.Nombre.ToLower()));
            }
            if(query.DepartamentoNombre != null)//buscar Nombre departamento
            {
                encargados = encargados.Where(e => e.Departamento!.Nombre.ToLower().Contains(query.DepartamentoNombre.ToLower()));
            }
            if(query.CarnetIdentidad != null)//Carnet Identidad
            {
                encargados = encargados.Where(e => e.AppUser!.CarnetIdentidad.Contains(query.CarnetIdentidad));
            }
            if(query.UsuarioId != null)//Usuario Id
            {
                encargados = encargados.Where(e => e.UsuarioId == query.UsuarioId);
                return await encargados.ToListAsync();
            } 
            if (query.ListaId.Any())//buscar por lista de id de encargados
            {
                encargados = encargados.Where(e => query.ListaId.Contains(e.Id));
            }
            if (query.DepartamentoId != null)//buscar por lista de id de encargados
            {
                encargados = encargados.Where(e => e.Departamento!.Id.Equals(query.DepartamentoId));
            }
            if(query.ListaDepartamentoId.Any())//Buscar por id de departamento
            {
                encargados = encargados.Where(e => query.ListaDepartamentoId.Contains(e.DepartamentoId));
            }
            if(query.ListaUserId.Any())//buscar por lista de id de usuarios
            {
                encargados = encargados.Where(e => query.ListaUserId.Contains(e.UsuarioId));
            }

            //Ordenamiento
            if(!string.IsNullOrWhiteSpace(query.OrdernarPor))
            {
                if(query.OrdernarPor.Equals("Nombre", StringComparison.OrdinalIgnoreCase))
                {
                    encargados = query.Descender ? encargados.OrderByDescending(e => e.AppUser!.NombreCompleto) : encargados.OrderBy(e => e.AppUser!.NombreCompleto);
                }
                else if(query.OrdernarPor.Equals("Departamento", StringComparison.OrdinalIgnoreCase))
                {
                    encargados = query.Descender ? encargados.OrderByDescending(e => e.Departamento!.Nombre) : encargados.OrderBy(e => e.Departamento!.Nombre);
                }
            } 
            var skipNumber = ( query.NumeroPagina - 1 ) * query.TamañoPagina;    
            return await encargados.Skip(skipNumber).Take(query.TamañoPagina).ToListAsync();
        }

        public async Task<Encargado?> GetByIdAsync(int Id)
        {
            return await _context.Encargado
                .Where(c => c.Id == Id && c.Activo == true)
                .Include(e => e.AppUser)
                .Include(e => e.Departamento)
                .FirstOrDefaultAsync();
        }

        public async Task<Encargado?> GetByUserIdAsync(string id)
        {
            return await _context.Encargado
                .Where(c => c.UsuarioId == id && c.Activo == true)
                .Include(e => e.AppUser)
                .Include(e => e.Departamento)
                    .ThenInclude(f => f!.Facultad)
                .FirstOrDefaultAsync();
        }

        public async Task<Encargado?> GetEncargadoByDepartamentoId(int departamentoId)
        {
            return await _context.Encargado
            .Include(e => e.AppUser)
            .FirstOrDefaultAsync(e => e.Activo && e.DepartamentoId == departamentoId);
        }
        public async Task<bool> ExisteEncargadoByDepartamentoIdAsync(int departamentoId)
        {
           return await _context.Encargado.AnyAsync(e => e.DepartamentoId == departamentoId && e.Activo == true);
        }
 
        public async Task<Encargado?> GetEncargadoByUserIdAsync(string usuarioId)
        {
            return await _context.Encargado.FirstOrDefaultAsync(e => e.Activo == true && e.UsuarioId == usuarioId);
        }

        public async Task<Encargado?> UpdateAsync(int id, EncargadoUpdateDto encargadoUpdateDto)
        {
            var encargadoExiste = await _context.Encargado.FindAsync(id);

            if(encargadoExiste == null) return null;

            encargadoExiste.UpdateEncargado(encargadoUpdateDto);
            await _context.SaveChangesAsync();

            return encargadoExiste;
        }
        public async Task<Encargado?> UpdateEncargadoByUserIdAsync(string usuarioId, EncargadoUpdateDto encargadoUpdateDto)
        {
            var encargadoExiste = await _context.Encargado.FirstOrDefaultAsync(e => e.UsuarioId == usuarioId);

            if(encargadoExiste == null) return null;

            encargadoExiste.UpdateEncargado(encargadoUpdateDto);
            await _context.SaveChangesAsync();

            return encargadoExiste;
        }

        public async Task<Encargado?> DeleteByDepartamentoIdAsync(int departamentoId)
        {
            var encargado = await _context.Encargado
                .Include(d => d.AppUser)
                .FirstOrDefaultAsync(e => e.DepartamentoId == departamentoId);
            if(encargado == null) return null;

            encargado.Activo = false;
            await _context.SaveChangesAsync();
            return encargado;
        }
    }
}