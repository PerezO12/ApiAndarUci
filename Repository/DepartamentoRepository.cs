using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApiUCI.Interfaces;
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
            await _context.departamento.AddAsync(departamentoModel);
            await _context.SaveChangesAsync();
            return departamentoModel;
        }

        public async Task<Departamento?> DeleteAsync(int id)
        {
            var departamentoModel = await _context.departamento.FindAsync(id);
            
            if(departamentoModel == null)
            {
                return null;
            }
            _context.departamento.Remove(departamentoModel);
            await _context.SaveChangesAsync();
            return departamentoModel;
        }

        public async Task<List<Departamento>> GetAllAsync()
        {
           return await _context.departamento.ToListAsync();
        }

        public async Task<Departamento?> GetByIdAsync(int id)
        {
            return await _context.departamento.FirstOrDefaultAsync( d => d.Id == id);
        }

        public async Task<Departamento?> UpdateAsync(int id, Departamento departamentoModel)
        {
           var existingDepartament = await _context.departamento.FindAsync(id);
           if(existingDepartament == null)
           {
            return null;
           }
           existingDepartament.Nombre = departamentoModel.Nombre;
           existingDepartament.FacultadId = departamentoModel.FacultadId;

            await _context.SaveChangesAsync();
            return existingDepartament;
        }
    }
}