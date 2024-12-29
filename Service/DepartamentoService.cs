using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Dtos;
using ApiUCI.Dtos.Cuentas;
using ApiUCI.Helpers.Querys;
using ApiUCI.Interfaces;
using MyApiUCI.Dtos.Departamento;
using MyApiUCI.Interfaces;
using MyApiUCI.Mappers;
using MyApiUCI.Models;

namespace ApiUCI.Service
{
    public class DepartamentoService : IDepartamentoService
    {
        private readonly IDepartamentoRepository _departamentoRepo;
        private readonly IEncargadoService _encargadoService;
        private readonly IFacultadRepository _facultadRepo;
        private readonly IEstudianteRepository _estudianteRepo;
        public DepartamentoService(
            IDepartamentoRepository departamentoRepo,
            IFacultadRepository facultadRepo,
            IEstudianteRepository estudianteRepo,    
            IEncargadoService encargadoService
        )
        {
            _departamentoRepo = departamentoRepo;
            _encargadoService = encargadoService;
            _facultadRepo = facultadRepo;
            _estudianteRepo = estudianteRepo;
        }

        public async Task<RespuestasGenerales<DepartamentoDto>> CreateAsync(CreateDepartamentoDto departamentoDto)
        {
            try
            {
                var facultad = await _facultadRepo.GetByIdAsync(departamentoDto.FacultadId);
                if(facultad == null)
                    return RespuestasGenerales<DepartamentoDto>.ErrorResponseService("Facultad", "La facultad no existe"); 

                var departamento = await _departamentoRepo.CreateAsync(departamentoDto.toDepartamentoFromCreate());
                departamento.Facultad = facultad;
                return RespuestasGenerales<DepartamentoDto>.SuccessResponse(departamento.toDepartamentDto(), "Departamento creado exitosamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<RespuestasGenerales<DepartamentoDto>> DeleteAsync(int departamentoId)
        {
            try
            {  
                var departamento = await _departamentoRepo.DeleteAsync(departamentoId);
                if(departamento == null)
                    return RespuestasGenerales<DepartamentoDto>.ErrorResponseService("Departamento", "El departamento no existe", "NotFound");
                //todo:borrar el encargado ver si funciona bien
                var encargado = await _encargadoService.DeleteEncargadoByDepartamentoIdAsync(departamento.Id);
                return  RespuestasGenerales<DepartamentoDto>.SuccessResponse(departamento.toDepartamentDto(), "El departamento fue borrado exitosamente.");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<RespuestasGenerales<IEnumerable<DepartamentoDto>>> GetAllAsync(QueryObjectDepartamentos query)
        {
            try
            {
                var departamentos =  await _departamentoRepo.GetAllAsync(query);
                var departamentosDto = departamentos.Select(d => d.toDepartamentDto());

                return RespuestasGenerales<IEnumerable<DepartamentoDto>>.SuccessResponse(departamentosDto, "Operacion realizada exitosamente.");
            }
            catch(Exception ex)
            {
               Console.WriteLine(ex.Message);
                throw; 
            }
        }

        public async Task<RespuestasGenerales<DepartamentoDto>> GetByIdAsync(int id)
        {
            try
            {
                var departamento = await _departamentoRepo.GetByIdAsync(id);
    
                if(departamento == null)
                    return RespuestasGenerales<DepartamentoDto>.ErrorResponseService("Departamento", "El departamento no existe", "NotFound");

                return RespuestasGenerales<DepartamentoDto>.SuccessResponse(departamento.toDepartamentDto(), "Operacion realizada exitosamente.");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw; 
            }
        }

        public async Task<RespuestasGenerales<DepartamentoDto>> UpdateAsync(int departamentoId, UpdateDepartamentoDto departamentoDto)
        {
            try
            {
                if(!await _facultadRepo.FacultyExists(departamentoDto.FacultadId))
                    return RespuestasGenerales<DepartamentoDto>.ErrorResponseService("Facultad", "La facultad no existe"); 

                var departamento = await _departamentoRepo.UpdateAsync(departamentoId, departamentoDto.toDepartamentoFromUpdate());
                if(departamento == null)
                    return RespuestasGenerales<DepartamentoDto>.ErrorResponseService("Departamento", "El departamento no existe", "NotFound");

                return RespuestasGenerales<DepartamentoDto>.SuccessResponse(departamento.toDepartamentDto(), "Departamento actualizado exitosamente");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw; 
            }
        }
        public async Task<RespuestasGenerales<DepartamentoDto>> PatchAsync(int id, PatchDepartamentoDto departamentoDto)
        {
            try
            {
                if(departamentoDto.FacultadId != null && !await _facultadRepo.FacultyExists((int)departamentoDto.FacultadId))
                    return RespuestasGenerales<DepartamentoDto>.ErrorResponseService("Facultad", "La facultad no existe");
                
                var departamento = await _departamentoRepo.PatchAsync(id, departamentoDto);
                
                if(departamento == null)
                    return RespuestasGenerales<DepartamentoDto>.ErrorResponseService("Departamento", "El departamento no existe");
                    
                return RespuestasGenerales<DepartamentoDto>.SuccessResponse(departamento.toDepartamentDto(), "Operacion realizada exitosamente."); 
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw; 
            }
        }
        public async Task<RespuestasGenerales<IEnumerable<DepartamentoDto>>> DeleteAllDepartamentosByFacultad(int facultadId)
        {
            try
            {
                var departamentos = await _departamentoRepo.DeleteAllDepartamentosByFacultad(facultadId);
                foreach (var departamento in departamentos)
                {
                    await _encargadoService.DeleteEncargadoByDepartamentoIdAsync(departamento.Id);//todo: ver si se maneja asi
                }
                var departamentosDto = departamentos.Select(d => d.toDepartamentDto());
                return RespuestasGenerales<IEnumerable<DepartamentoDto>>.SuccessResponse(departamentosDto, "Los departamentos fueron borrados exitosamente.");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<RespuestasGenerales<IEnumerable<DepartamentoDto>>> GetAllDepartamentoByEstudiante(string userId)
        {
            var estudiante = await _estudianteRepo.GetEstudianteByUserId(userId);
            if(estudiante == null)
                return RespuestasGenerales<IEnumerable<DepartamentoDto>>.ErrorResponseService("Estudiante", "El estudiante no existe.", "BadRequest");
            
            var departamentos = await _departamentoRepo.GetAllDepartamentosByFacultadId(estudiante.FacultadId);
            if(departamentos == null)
                return RespuestasGenerales<IEnumerable<DepartamentoDto>>.ErrorResponseService("Departamentos", "No hay departamentos correspondientes.", "NotFound");
            
            var departamentosDto = departamentos.Select(d => d.toDepartamentDto());
            return RespuestasGenerales<IEnumerable<DepartamentoDto>>.SuccessResponse(departamentosDto, "Operacion realizada exitosamente."); 
        }
    }
}