using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Interfaces;
using MyApiUCI.Interfaces;
using MyApiUCI.Models;

namespace ApiUCI.Service
{
    public class DepartamentoService : IDepartamentoService
    {
        private readonly IDepartamentoRepository _departamentoRepo;
        private readonly IEncargadoService _encargadoService;
        public DepartamentoService(
            IDepartamentoRepository departamentoRepo,
            IEncargadoService encargadoService    
        )
        {
            _departamentoRepo = departamentoRepo;
            _encargadoService = encargadoService;
        }

        public async Task<List<Departamento>> DeleteAllDepartamentosByFacultad(int facultadId)
        {
            try
            {
                var departamentos = await _departamentoRepo.DeleteAllDepartamentosByFacultad(facultadId);
                foreach (var departamento in departamentos)
                {
                    await _encargadoService.DeleteEncargadoByDepartamentoIdAsync(departamento.Id);
                }
                return departamentos;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}