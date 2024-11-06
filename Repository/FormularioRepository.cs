using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Models;

namespace MyApiUCI.Repository
{
    public class FormularioRepository : IFormularioRepository
    {
        private readonly ApplicationDbContext _contex;

        public FormularioRepository(ApplicationDbContext context)
        {
            _contex = context;
        }

        public async Task<Formulario> CreateAsync(Formulario formulario)
        {
            await _contex.Formulario.AddAsync(formulario);
            await _contex.SaveChangesAsync();
            return formulario;            
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Formulario>> GetAllAsync(QueryObjectFormulario query)
        {
            var formularios = _contex.Formulario.Where( f => f.Activo == true).AsQueryable();

            if(query.UsuarioId != null)
            {
                formularios = formularios.Where(f => f.UsuarioId == query.UsuarioId);
            }
            if(query.ListaId.Any())
            {
                formularios = formularios.Where(f => query.ListaId.Contains(f.Id));
            }
            if(query.DepartamentoId.HasValue)
            {
                formularios = formularios.Where(f => f.DepartamentoId == query.DepartamentoId);
            } //El ordenamiento sera en el servicio

            var skipNumber = ( query.NumeroPagina - 1 ) * query.TamañoPagina;
            return await formularios.Skip(skipNumber).Take(query.TamañoPagina).ToListAsync();
        }

        public async Task<Formulario?> GetByIdAsync(int id)
        {
            return await _contex.Formulario.FindAsync(id);//poner solo activos
        }

        public Task<bool> UpdateAsync(Formulario formulario)
        {
            throw new NotImplementedException();
        }
    }
}