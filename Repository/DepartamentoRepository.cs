using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Helpers.Querys;
using Microsoft.EntityFrameworkCore;
using ApiUCI.Controller;
using ApiUCI.Dtos.Departamento;
using ApiUCI.Helpers;
using ApiUCI.Interfaces;
using ApiUCI.Mappers;
using ApiUCI.Models;

namespace ApiUCI.Repository
{
    public class DepartamentoRepository : IDepartamentoRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormularioRepository> _logger;

        public DepartamentoRepository(ApplicationDbContext context, ILogger<FormularioRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<Departamento> CreateAsync(Departamento departamentoModel)
        {
            try
            {
                var departamento = await _context.Departamento
                    .AddAsync(departamentoModel);
                await _context.SaveChangesAsync();

                return departamentoModel;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al crear el departamento: {ex.Message}");
                throw;
            }
        }

        public async Task<Departamento?> DeleteAsync(int id)
        {
            try
            {
                var departamentoModel = await _context.Departamento.FirstOrDefaultAsync(d => d.Id == id && d.Activo == true);
                
                if(departamentoModel == null)
                    return null;

                departamentoModel.Activo = false; //No lo eliminare lo pasare a falso
                await _context.SaveChangesAsync();
                return departamentoModel;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al borrar el departamento id {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Departamento>> GetAllAsync( QueryObjectDepartamentos query)
        {
            try
            {

                var departamentos = _context.Departamento
                    .Where(d => d.Activo == true)
                    .Include(d => d.Facultad)
                    .Include(d => d.Encargado)
                        .ThenInclude(e => e!.AppUser)
                    .AsQueryable();
                //validaciones de busquesdas

                if(!string.IsNullOrWhiteSpace(query.Departamento))
                {
                    departamentos = departamentos.Where(d => d.Nombre.Contains(query.Departamento, StringComparison.OrdinalIgnoreCase));
                }
                if(!string.IsNullOrWhiteSpace(query.Facultad))
                {
                    departamentos = departamentos.Where(d => d.Facultad!.Nombre.Contains(query.Facultad, StringComparison.OrdinalIgnoreCase));
                }
                if(query.FacultadId.HasValue && query.FacultadId > 0)
                {
                    departamentos = departamentos.Where(d => d.FacultadId == query.FacultadId);
                }
                if(query.EncargadoId.HasValue && query.FacultadId > 0)
                {
                    departamentos = departamentos.Where(d => d.EncargadoId == query.EncargadoId);
                }
                //Ordenamiento
                if(!string.IsNullOrWhiteSpace(query.OrdenarPor))
                {
                    if(query.OrdenarPor.Equals("Nombre", StringComparison.OrdinalIgnoreCase))
                    {
                        departamentos = query.Descender ? departamentos.OrderByDescending(d => d.Nombre) : departamentos.OrderBy(d => d.Nombre);
                    }
                    else if(query.OrdenarPor.Equals("Facultad", StringComparison.OrdinalIgnoreCase))
                    {
                        departamentos = query.Descender ? departamentos.OrderByDescending(d => d.Facultad!.Nombre) : departamentos.OrderBy( d => d.Facultad!.Nombre);
                    }
                    else if(query.OrdenarPor.Equals("Fecha", StringComparison.OrdinalIgnoreCase))
                    {
                        departamentos = query.Descender ? departamentos.OrderByDescending(d => d.Fechacreacion) : departamentos.OrderBy( d => d.Fechacreacion);
                    }
                }
                //Paginacion
                var skipNumber = ( query.NumeroPagina - 1) * query.TamañoPagina;
                
                return await departamentos.Skip(skipNumber).Take(query.TamañoPagina).ToListAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al obtener los departamentos: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Departamento>> GetAllDepartamentosByFacultadId(int id)
        {
            try
            {
                return await _context.Departamento.Where(d => d.Activo && d.FacultadId == id).ToListAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al obtener los departamentos por facultades: {ex.Message}");
                throw;
            }
        }

        public async Task<Departamento?> GetByIdAsync(int id)
        {
            try
            {
                return  await _context.Departamento
                    .Include( d => d.Facultad)
                    .Include(d => d.Encargado)
                        .ThenInclude(e => e!.AppUser)
                    .FirstOrDefaultAsync( d => d.Id == id && d.Activo == true);

            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al obtener el departamento{id}: {ex.Message}");
                throw;
            }
        }

        public async Task<Departamento?> PatchAsync(int id, PatchDepartamentoDto departamentoDto)
        {
            try
            {
                var departamentModel = await _context.Departamento
                    .Include(d => d.Facultad)
                    .FirstOrDefaultAsync(d => d.Id == id && d.Activo == true);
                if(departamentModel == null)
                    return null;
                    
                departamentModel.toPatchingDepartamento(departamentoDto);

                await _context.SaveChangesAsync();

                return departamentModel;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al actualizar el departamento: {ex.Message}");
                throw;
            }
        }

        public async Task<Departamento?> CambiarEncargado(int departamentoId, int? nuevoEncargado = null)
        {
            var departamento = await _context.Departamento.FindAsync(departamentoId);
            if(departamento == null) return null;
            departamento.EncargadoId = nuevoEncargado;
            await _context.SaveChangesAsync();
            return departamento;
        }
        public async Task<Departamento?> UpdateAsync(int id, Departamento departamentoModel)
        {
            try
            {
                var existeDepartamento = await _context.Departamento
                    .Include(d => d.Facultad)
                    .Include(d => d.Encargado)
                    .FirstOrDefaultAsync(d => d.Id == id && d.Activo == true);
                if(existeDepartamento == null)
                    return null;

                existeDepartamento.Nombre = departamentoModel.Nombre;
                existeDepartamento.FacultadId = departamentoModel.FacultadId;
                existeDepartamento.EncargadoId = departamentoModel.EncargadoId;

                await _context.SaveChangesAsync();

                return existeDepartamento;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al actualizar el departamento{id}: {ex.Message}");
                throw;
            }
        }

        //cuidado!!!!
        public async Task<List<Departamento>> DeleteAllDepartamentosByFacultad(int facultadId)
        {
            try
            {
                var encargados = await _context.Departamento
                    .Where(e => e.FacultadId == facultadId && e.Activo == true)
                    .ToListAsync();
                foreach (var encargado in encargados)
                {
                    encargado.Activo = false;
                }
                await _context.SaveChangesAsync();

                return encargados;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<bool> ExistDepartamento(int id)
        {
            try
            {
                return await _context.Departamento.AnyAsync(d => d.Id == id && d.Activo == true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar el departamento: {ex.Message}");//todo: no se si retornar false o relan la exception
                return false;
            }
        }
        public async Task<bool> TieneEncargado(int id)
        {
            try
            {
                return await _context.Departamento.AnyAsync(d => d.Id == id && d.Activo == true && d.EncargadoId != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar el departamento: {ex.Message}");//todo: no se si retornar false o relan la exception
                return false;
            }
        }

        public async Task<Departamento?> DeleteEncargadoByEncargadoID(int encargadoId)
        {
            try
            {
                var departamentoModel = await _context.Departamento.FirstOrDefaultAsync(d => d.EncargadoId == encargadoId && d.Activo == true);
                
                if(departamentoModel == null) return null;

                departamentoModel.EncargadoId = null;
                await _context.SaveChangesAsync();
                return departamentoModel;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al borrar el encargado con {encargadoId}del departamento id : {ex.Message}");
                throw;
            }
        }
    }
}