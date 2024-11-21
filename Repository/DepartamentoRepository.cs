using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApiUCI.Dtos.Departamento;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Mappers;
using MyApiUCI.Models;

namespace MyApiUCI.Repository
{
    public class DepartamentoRepository : IDepartamentoRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartamentoRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Departamento> CreateAsync(Departamento departamentoModel)
        {
            await _context.Departamento.AddAsync(departamentoModel);
            await _context.SaveChangesAsync();
            return departamentoModel;
        }

        public async Task<Departamento?> DeleteAsync(int id)
        {
            var departamentoModel = await _context.Departamento.FirstOrDefaultAsync(d => d.Id == id && d.Activo == true);
            
            if(departamentoModel == null)
            {
                return null;
            }
            departamentoModel.Activo = false; //No lo eliminare lo pasare a falso
            await _context.SaveChangesAsync();
            return departamentoModel;
        }

        public async Task<List<Departamento>> GetAllAsync( QueryObject query)
        {
           /* return await _context.departamento
                .Where(d => d.Activo == true)
                .ToListAsync(); //solo mostrara los activos */

            var departamentos = _context.Departamento.Where(d => d.Activo == true).AsQueryable();
            //validaciones de busquesdas
            if(query.ListaId.Any())
            {
                departamentos = departamentos.Where(d => query.ListaId.Contains(d.Id));                
            }
            if(!string.IsNullOrWhiteSpace(query.Nombre))
            {
                departamentos = departamentos.Where(d => d.Nombre.Contains(query.Nombre, StringComparison.OrdinalIgnoreCase));
            }
            if(query.FacultadId.HasValue)
            {
                departamentos = departamentos.Where(d => d.FacultadId == query.FacultadId);
            }

            //Ordenamiento
            if(!string.IsNullOrWhiteSpace(query.OrdernarPor))
            {
                if(query.OrdernarPor.Equals("Nombre", StringComparison.OrdinalIgnoreCase))
                {
                    departamentos = query.Descender ? departamentos.OrderByDescending(d => d.Nombre) : departamentos.OrderBy(d => d.Nombre);
                }
                else if(query.OrdernarPor.Equals("Facultad", StringComparison.OrdinalIgnoreCase))
                {
                    departamentos = query.Descender ? departamentos.OrderByDescending(d => d.FacultadId) : departamentos.OrderBy( d => d.FacultadId);
                }
            }
            //Paginacion
            var skipNumber = ( query.NumeroPagina - 1) * query.TamañoPagina;
            
            return await departamentos.Skip(skipNumber).Take(query.TamañoPagina).ToListAsync();
        }

        public async Task<List<Departamento>> GetAllByFacultadId(int id)
        {
            return await _context.Departamento.Where(d => d.Activo && d.FacultadId == id).ToListAsync();
        }

        public async Task<Departamento?> GetByIdAsync(int id)
        {
            return await _context.Departamento
                .FirstOrDefaultAsync( d => d.Id == id && d.Activo == true); //lo mismo
        }

        public async Task<Departamento?> PatchAsync(int id, PatchDepartamentoDto departamentoDto)
        {
            var departamentModel = await _context.Departamento
                                    .FirstOrDefaultAsync(d => d.Id == id && d.Activo == true);
            if(departamentModel == null)
            {
                return null;
            }
            departamentModel.toPatchingDepartamento(departamentoDto);
            await _context.SaveChangesAsync();
            return departamentModel;
        }

        public async Task<Departamento?> UpdateAsync(int id, Departamento departamentoModel)
        {
           var existeDepartamento = await _context.Departamento.FirstOrDefaultAsync(d => d.Id == id && d.Activo == true);
           if(existeDepartamento == null)
           {
            return null;
           }
           existeDepartamento.Nombre = departamentoModel.Nombre;
           existeDepartamento.FacultadId = departamentoModel.FacultadId;

            await _context.SaveChangesAsync();
            return existeDepartamento;
        }
        
    }
}