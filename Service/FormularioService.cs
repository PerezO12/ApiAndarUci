using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ApiUCI.Dtos;
using ApiUCI.Dtos.Formulario;
using ApiUCI.Helpers;
using ApiUCI.Helpers.Querys;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApiUCI.Dtos.Estudiante;
using MyApiUCI.Dtos.Formulario;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Mappers;
using MyApiUCI.Models;
//TODO: Agregar las firmas y los nombres del q la firmo y etc
namespace MyApiUCI.Service
{
    public class FormularioService : IFormularioService
    {
        private readonly IFormularioRepository _formularioRepo;

        private readonly IEstudianteService _estudianteService;

        private readonly IEncargadoService _encargadoService;
        private readonly IDepartamentoRepository _departamentoRepo;
        public FormularioService( 
            IFormularioRepository formularioRepo, 
            IEstudianteService estudianteService,
            IEncargadoService encargadoService,
            IDepartamentoRepository departamentoRepo
            )
        {
            _formularioRepo = formularioRepo;
            _estudianteService = estudianteService;
            _encargadoService = encargadoService;
            _departamentoRepo = departamentoRepo;
        }

        public async Task<ResultadoDto> CreateFormularioAync(string userId, CreateFormularioDto formularioDto)
        {
            //TODO:HACER LAS 2 FUCNIONES DE FOMRA ASYNC
            var departamento = await _departamentoRepo.GetByIdAsync(formularioDto.DepartamentoId);
            var estudiante = await _estudianteService.GetEstudianteByUserId(userId);
            var encargado = await _encargadoService.GetEncargadoByDepartamentoIdAsync(formularioDto.DepartamentoId);
            
            if(departamento == null) {
                return new ResultadoDto{
                    msg = "No existe el departamento",
                    TipoError = "BadRequest",
                    Error = true
                };
            }
            if(encargado == null) {
                return new ResultadoDto{
                    msg = "No existe el encargado",
                    TipoError = "BadRequest",
                    Error = true
                };
            };
            if(estudiante == null) {
                return new ResultadoDto{
                    msg = "No existe el estudiante",
                    TipoError = "BadRequest",
                    Error = true
                };
            };
            if(estudiante.FacultadId == departamento.Id){
                return new ResultadoDto{
                    msg = "El departamento seleccionado no corresponde a la facultad del estudiante.",
                    TipoError = "BadRequest",
                    Error = true
                };
            }
            var existeFormulario = await _formularioRepo.FormByEstudianteDepartamentoAsync(estudiante.Id, formularioDto.DepartamentoId);
            if(existeFormulario != null) {
                 return new ResultadoDto{
                    msg = "El formulario ya existe.",
                    TipoError = "BadRequest",
                    Error = true
                };
            };
            var formulario = await _formularioRepo.CreateAsync(formularioDto.toFormularioFromCreate(estudiante, encargado));
            return new ResultadoDto{
                    msg = "Resultado creado correctamente.",
                    TipoError = "Ok",
                    Error = false
                };;
        }

        public async Task<ResultadoDto> DeleteFormularioAdmin(int formularioId)
        {
            var formulario = await _formularioRepo.GetByIdAsync(formularioId);
                if(formulario == null) {
                    return new ResultadoDto{
                        msg = "El formulario no existe",
                        TipoError = "BadRequest",
                        Error = true
                    };
                }
            await _formularioRepo.DeleteAsync(formularioId);
            return new ResultadoDto{
                msg = "Formulario eliminado exitosamente",
                TipoError = "Ok",
                Error = false
            };
        }

        public async Task<ResultadoDto> DeleteFormularioEstudianteAsync(string userId, int formularioId)
        {
            try
            {
                var estudiante = await  _estudianteService.GetEstudianteByUserId(userId);
                if(estudiante == null) {
                    return new ResultadoDto{
                        msg = "El estudiante no existe",
                        TipoError = "BadRequest",
                        Error = true
                    };
                }
                var formulario = await _formularioRepo.GetByIdAsync(formularioId);
                if(formulario == null) {
                    return new ResultadoDto{
                        msg = "El formulario no existe",
                        TipoError = "BadRequest",
                        Error = true
                    };
                }
                if(formulario.EstudianteId != estudiante.Id)
                {
                    return new ResultadoDto{
                        msg = "Acción no valida",
                        TipoError = "Unauthorized",
                        Error = true
                    };
                }
                await _formularioRepo.DeleteAsync(formularioId);
                return new ResultadoDto{
                        msg = "Formulario eliminado exitosamente",
                        TipoError = "Ok",
                        Error = false
                    };

            }            
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ResultadoDto{
                        msg = ex.Message,
                        TipoError = "StatusCode500",
                        Error = true
                    }; 
            }
        }

        public async Task<ResultadoDto> FirmarFormularioAsync(string userIdEncargado, int id, FormularioFirmarDto formularioDto)
        {
            try
            {
                //Validar la llave privada, si no lanza una exepction es valida
                var llavePrivadaBytes = Convert.FromBase64String(formularioDto.LlavePrivada);
                using var rsa = RSA.Create();
                rsa.ImportPkcs8PrivateKey(llavePrivadaBytes, out _);

                var formulario = await _formularioRepo.GetByIdAsync(id);
                var encargado = await _encargadoService.GetEncaradoByUserId(userIdEncargado);
                if(formulario == null){
                    return new ResultadoDto {
                        msg = "El formulario no existe",
                        TipoError = "NotFound",
                        Error = true
                    };
                }
                if(encargado == null) {
                    return new ResultadoDto {
                        msg = "El encargado no existe",
                        TipoError = "NotFound",
                        Error = true
                    };
                }
                if(encargado.LlavePublica == null) {
                    return new ResultadoDto {
                        msg = "No tiene llave publica registrada",
                        TipoError = "NotFound",
                        Error = true
                    };
                }
                if(formulario.EncargadoId != encargado.Id) {
                    return new ResultadoDto {
                        msg = "No Authorizado",
                        TipoError = "Unauthorized",
                        Error = true
                    };
                }
                //procedimiento para firmar
                var contenidoFirmar = new FormularioFirmadoDto{
                    FormularioId = formulario.Id,
                    EstudianteId = formulario.EstudianteId,
                    EncargadoId = formulario.EncargadoId,
                    NombreEstudiante = formulario.Estudiante!.AppUser!.NombreCompleto,
                    NombreEncargado = formulario.Encargado!.AppUser!.NombreCompleto,
                    NombreDepartamento = formulario!.Departamento!.Nombre,
                    Fechacreacion = formulario.Fechacreacion,
                    Motivo = formulario.Motivo
                };
                // Serializar el contenido a JSON
                var contenidoJson = JsonSerializer.Serialize(contenidoFirmar);
                var contenidoBytes = Encoding.UTF8.GetBytes(contenidoJson);

                // Generar el hash del contenido
                var hasDocumento = SHA256.HashData(contenidoBytes);

                // Firmar el hash con la llave privada
                var documentoFirmado = rsa.SignHash(hasDocumento, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                
                var verificador = new FirmaDigital();
                //comprobamos si su firma coincide con la correspondiente publica
                bool esCorrespondiente = verificador.VerificarFirmaFormulario(contenidoJson, documentoFirmado, hasDocumento, encargado.LlavePublica );

                if(!esCorrespondiente) {
                    return new ResultadoDto {
                        msg = "La llave privada proporcionada no coincide con la clave pública asociada.",
                        TipoError = "BadRequest",
                        Error = false
                    };
                }
                formulario.FirmaEncargado = documentoFirmado;
                formulario.HashDocumento = hasDocumento;
                formulario.Firmado = true;
                formulario.FechaFirmado = DateTime.UtcNow;

                await _formularioRepo.UpdateAsync(formulario.Id, formulario);
                return new ResultadoDto {
                        msg = "Formulario firmado",
                        TipoError = "Ok",
                        Error = false
                    };
            }
            catch (FormatException)
            {
                return new ResultadoDto {
                    msg = "La llave no es valida",
                    TipoError = "BadRequest",
                    Error = true
                };
            }
            catch (CryptographicException)
            {
                return new ResultadoDto {
                    msg = "La llave no es valida",
                    TipoError = "BadRequest",
                    Error = true
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;   
            }
   
        }
 

        public async Task<List<FormularioEncargadoDto>> GetAllFormulariosEncargadosAsync(string usuarioId, QueryObjectFormularioEncargado query)
        {
            var formulariosEncargados = await _formularioRepo.GetAllFormulariosByEncargado(usuarioId, query);

            return formulariosEncargados;
        }

        public async Task<List<FormularioEstudianteDto>> GetAllFormulariosEstudiantesAsync(string usuarioId, QueryObjectFormularioEstudiantes query)
        {
            var formulariosEstudiante = await _formularioRepo.GetAllFormulariosByEstudiante(usuarioId, query);
/*             var formulariosDto = formulariosEstudiante.Select(e => new FormularioEstudianteDto{
                NombreEncargado = e.Encargado.AppUser.NombreCompleto,
                NombreDepartamento= e.Departamento.Nombre,
                Motivo = e.Motivo,
                Firmado = e.Firmado,
                FechaFirmado = e.FechaFirmado,
                Fechacreacion = e.Fechacreacion
            }).ToList(); */
            return formulariosEstudiante;
        }

        public async Task<List<FormularioDto>> GetAllFormulariosWhithDetailsAsync(QueryObjectFormulario query)
        {
             var formularios = await _formularioRepo.GetAllAsync(query);

            Console.WriteLine($"Cantidad de formularios después de la combinación: {formularios.Count}");

            return formularios;

     }

        public async Task<FormularioEncargadoDto?> GetFormEstudianteByIdForEncargadoAsync(string userId, int formularioId)
        {
            var encargado = await _encargadoService.GetEncaradoByUserId(userId);
            if(encargado == null) return null;

            var formulario = await _formularioRepo.GetFormEstudianteByIdForEncargadoAsync(encargado.Id, formularioId);
            if(formulario == null) return null;
            return formulario;
        }

        public async Task<FormularioDto?> GetFormularioWithDetailsAsync(int id)
        {
            var formularioModel = await _formularioRepo.GetByIdAsync(id);
            if(formularioModel == null) return null;
            //TODO:Arreglar como arriba
            return new FormularioDto {
                id = formularioModel.Id,
                NombreEstudiante = formularioModel.Estudiante!.AppUser!.NombreCompleto,
                NombreUsuarioEstudiante = formularioModel.Estudiante.AppUser.UserName,
                CarnetIdentidadEstudiante = formularioModel.Estudiante.AppUser.CarnetIdentidad,
                EmailEstudiante = formularioModel.Estudiante.AppUser.Email,
                NombreCarrera = formularioModel.Estudiante.Carrera!.Nombre,
                NombreDepartamento = formularioModel.Departamento!.Nombre,
                NombreFacultad = formularioModel.Departamento.Facultad!.Nombre,
                Motivo = formularioModel.Motivo,
                NombreEncargado = formularioModel.Encargado!.AppUser!.NombreCompleto,
                Firmado = formularioModel.Firmado,
                FechaFirmado = formularioModel.FechaFirmado,
                Fechacreacion = formularioModel.Fechacreacion
            };
        }

        public async Task<ResultadoDto> UpdatePatchFormularioAsync(string usuarioId, int id, UpdateFormularioDto formularioDto)
        {
            var formulario = await _formularioRepo.GetByIdAsync(id);
            if(formulario == null) {
                return new ResultadoDto {
                    msg = "No existe el formulario",
                    TipoError = "NotFound",
                    Error = true
                };;
            };
            if(formulario.Estudiante!.UsuarioId != usuarioId){
                return new ResultadoDto {
                    msg = "No Authorizado",
                    TipoError = "Unauthorized",
                    Error = true
                };
            }

            var formularioActualizdo = await _formularioRepo.UpadatePatchAsync(id, formularioDto);
            
            return new ResultadoDto {
                    msg = "Formulario actualizado",
                    TipoError = "Ok",
                    Error = false
                };
        }
    }
}