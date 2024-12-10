using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApiUCI.Models;

namespace ApiUCI.Interfaces
{   //todo: ir moviendo de a poco la logica para aqui
    public interface IDepartamentoService
    {
        Task<List<Departamento>> DeleteAllDepartamentosByFacultad(int facultadId);
    }
}