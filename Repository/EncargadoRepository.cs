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

        public async Task<List<Encargado>> GetAllAsync(QueryObjectEncargado query)
        {
            var encargados = _context.Encargado.Where(e => e.Activo == true).AsQueryable();
            
            if(query.UsuarioId != null)
            {
                encargados = encargados.Where(e => e.UsuarioId == query.UsuarioId);
                return await encargados.ToListAsync();
            }

            if (query.ListaId.Any())
            {
                encargados = encargados.Where(e => query.ListaId.Contains(e.Id));
            }
            if(query.ListaDepartamentoId.Any())
            {
                encargados = encargados.Where(e => query.ListaDepartamentoId.Contains(e.DepartamentoId));
            }
            if(query.ListaUserId.Any())
            {
                encargados = encargados.Where(e => query.ListaUserId.Contains(e.UsuarioId));
            }

            /* if(query.FacultadId.HasValue)
            {
                encargados = encargados.Where(e => e.FacultadId == query.FacultadId);
            } */
            if(query.DepartamentoId.HasValue)
            {
                encargados = encargados.Where(e => e.DepartamentoId == query.DepartamentoId);
            }
            //Ordenamiento
/*             if(!string.IsNullOrWhiteSpace(query.OrdernarPor))
            {
                if(query.OrdernarPor.Equals("Carrera", StringComparison.OrdinalIgnoreCase))
                {
                    encargados = query.Descender ? encargados.OrderByDescending(d => d.CarreraId) : encargados.OrderBy(d => d.CarreraId);
                }
                else if(query.OrdernarPor.Equals("Facultad", StringComparison.OrdinalIgnoreCase))
                {
                    encargados = query.Descender ? encargados.OrderByDescending(d => d.FacultadId) : encargados.OrderBy( d => d.FacultadId);
                }
            } */
            var skipNumber = ( query.NumeroPagina - 1 ) * query.TamañoPagina;
            
            return await encargados.Skip(skipNumber).Take(query.TamañoPagina).ToListAsync();
        }

        public async Task<Encargado?> GetByIdAsync(int id)
        {
            return await _context.Encargado.FirstOrDefaultAsync(c => c.Id == id && c.Activo == true);
        }

        public async Task<Encargado?> UpdateAsync(int id, Encargado encargadoModel)
        {
            var encargadoExiste = await _context.Encargado.FindAsync(id);

            if(encargadoExiste == null) return null;

            encargadoExiste.UpdateEncargado(encargadoModel);
            await _context.SaveChangesAsync();

            return encargadoExiste;
        }
    }
}