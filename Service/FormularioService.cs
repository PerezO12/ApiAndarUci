using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApiUCI.Dtos.Estudiante;
using MyApiUCI.Dtos.Formulario;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Mappers;
using MyApiUCI.Models;

namespace MyApiUCI.Service
{
    public class FormularioService : IFormularioService
    {
        private readonly IFormularioRepository _formularioRepo;
        private readonly IEstudianteService _estudianteService;
     //   private readonly IEstudianteRepository _estudianteRepo;
        private readonly IDepartamentoRepository _departamentoRepo;
       // private readonly IFacultadRepository _facuRepo;
        private readonly IDepartamentoRepository _depaRepo;
  //      private readonly UserManager<AppUser> _userManager;
        private readonly IEncargadoService _encargadoService;
        public FormularioService( 
            IFormularioRepository formularioRepo, 
            IEstudianteRepository estudianteRepo,
            IDepartamentoRepository departamentoRepo,
  //          IFacultadRepository facuRepo,
            IDepartamentoRepository depaRepo,
            IEstudianteService estudianteService,
            IEncargadoService encargadoService,
            UserManager<AppUser> userManager
            )
        {

            _formularioRepo = formularioRepo;
            _estudianteService = estudianteService;
      //      _estudianteRepo = estudianteRepo;
            _departamentoRepo = departamentoRepo;
       //     _userManager = userManager;
    //        _facuRepo = facuRepo;
            _depaRepo = depaRepo;
            _encargadoService = encargadoService;
        }



         public async Task<List<FormularioDto>> GetAllFormulariosWhithDetailsAsync(QueryObjectFormulario query)
        {
            var formulariosModel = await _formularioRepo.GetAllAsync(query);

            //Ids de las entidades a buscar
            var estudiantesIds = formulariosModel.Select(f => f.UsuarioId).ToList();
            var departamentosIds = formulariosModel.Select(f => f.DepartamentoId).ToList();

            //consultas
            var departamentos = await _depaRepo.GetAllAsync(new QueryObject { ListaId = departamentosIds});
            var estudiantes = await _estudianteService.GetEstudiantesWithDetailsAsync(
                new QueryObjectEstudiante{ ListaUserId = estudiantesIds });
            var encargados = await _encargadoService.GetAllEncargadosWithDetailsAsync(
                new QueryObjectEncargado{ ListaDepartamentoId = departamentosIds });

            var formulariosDatosCombinados = formulariosModel
                .Join(
                    estudiantes,
                    formulario => formulario.UsuarioId,
                    estudiante => estudiante.UsuarioId,
                    (formulario, estudiante) => new
                    {
                        Formulario = formulario,
                        Estudiante = estudiante
                    })
                .Join(
                    encargados,
                    combinado => combinado.Formulario.DepartamentoId,
                    encargado => encargado.DepartamentoId,
                    (combinado, encargado) => new FormularioDto
                    {
                            NombreCompleto = combinado.Estudiante.NombreCompleto,              
                            NombreUsuario = combinado.Estudiante.NombreUsuario,      
                            CarnetIdentidad = combinado.Estudiante.CarnetIdentidad,       
                            Email = combinado.Estudiante.Email,             
                            NumeroTelefono = combinado.Estudiante.NumeroTelefono, 
                            NombreCarrera = combinado.Estudiante.NombreCarrera,             
                            NombreFacultad = combinado.Estudiante.NombreFacultad,          
                            firmado = combinado.Formulario.Firmado,
                            NombreDepartamento = encargado.DepartamentoNombre,         
                            NombreEncargado = encargado.NombreCompleto,          
                            FechaFirmado = combinado.Formulario.FechaFirmado,  
                            Fechacreacion = combinado.Formulario.Fechacreacion,       
                            Motivo = combinado.Formulario.Motivo, 
                    })
                .ToList();
                Console.WriteLine($"Cantidad de formularios después de la combinación: {formulariosDatosCombinados.Count}");

            //ordenamiento hacer despuest************************
            return formulariosDatosCombinados;

     }   

        //Cambiar esto**************************************************
        public async Task<FormularioDto?> GetFormularioWithDetailsAsync(int id)
        {
            var formulario = await _formularioRepo.GetByIdAsync(id);
            if(formulario == null) return null;

            var estudiantes = await _estudianteService.GetEstudiantesWithDetailsAsync(
                new QueryObjectEstudiante{UsuarioId = formulario.UsuarioId});
                
            var estudiante = estudiantes.FirstOrDefault();//es una lista q solod evuelve un elemento
            if(!estudiantes.Any()) return null;
            if(estudiante == null ) return null;
            
            var departamento = await _departamentoRepo.GetByIdAsync(formulario.DepartamentoId);

            string nombreDepartamento = departamento?.Nombre ?? "Departamento no encontrado";

            var formularioDto =  new FormularioDto{
                NombreCompleto = estudiante.NombreCompleto,
                NombreUsuario = estudiante.NombreUsuario,
                CarnetIdentidad = estudiante.CarnetIdentidad,
                Email = estudiante.Email,
                NombreCarrera = estudiante.NombreCarrera,
                NombreFacultad = estudiante.NombreFacultad,
                NombreDepartamento = nombreDepartamento,
                Fechacreacion = formulario.Fechacreacion
            };
            return formularioDto;
        }
    }
}