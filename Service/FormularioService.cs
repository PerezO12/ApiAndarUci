using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ApiUci.Dtos;
using ApiUci.Dtos.Formulario;
using ApiUci.Helpers;
using ApiUci.Helpers.Querys;
using ApiUci.Interfaces;
using ApiUci.Mappers;


namespace ApiUci.Service
{
    public class FormularioService : IFormularioService
    {
        private readonly IFormularioRepository _formularioRepo;

        private readonly IEstudianteRepository _estudianteRepo;

        private readonly IEncargadoService _encargadoService;
        private readonly IEstudianteService _estudianteService;
        private readonly IDepartamentoRepository _departamentoRepo;
        public FormularioService( 
            IFormularioRepository formularioRepo, 
            IEstudianteRepository estudianteRepo,
            IEncargadoService encargadoService,
            IDepartamentoRepository departamentoRepo,
            IEstudianteService estudianteService
            )
        {
            _formularioRepo = formularioRepo;
            _estudianteRepo = estudianteRepo;
            _encargadoService = encargadoService;
            _departamentoRepo = departamentoRepo;
            _estudianteService = estudianteService;
        }

        public async Task<RespuestasGenerales<FormularioEstudianteDto>> CreateFormularioAsync(string userId, CreateFormularioDto formularioDto)
        {
            var departamento = await _departamentoRepo.GetByIdAsync(formularioDto.DepartamentoId);
            if (departamento == null)
                return RespuestasGenerales<FormularioEstudianteDto>.ErrorResponseService("Departamento","No existe el departamento.");

            var estudiante = await _estudianteRepo.GetEstudianteByUserId(userId);
            if (estudiante == null)
                return RespuestasGenerales<FormularioEstudianteDto>.ErrorResponseService("Estudiante", "No existe el estudiante.");

            var encargado = await _encargadoService.GetEncargadoByDepartamentoIdAsync(formularioDto.DepartamentoId);
            if (encargado == null)
                return RespuestasGenerales<FormularioEstudianteDto>.ErrorResponseService("Encargado", "No existe el encargado.");

            if (estudiante.FacultadId != departamento.FacultadId)
                return RespuestasGenerales<FormularioEstudianteDto>.ErrorResponseService("Departamento", "El departamento seleccionado no corresponde a la facultad del estudiante.");

            var existeFormulario = await _formularioRepo.FormByEstudianteDepartamentoAsync(estudiante.Id, formularioDto.DepartamentoId);
            if (existeFormulario != null)
                return RespuestasGenerales<FormularioEstudianteDto>.ErrorResponseService("Formulario", "El formulario ya existe.");

            var formulario = await _formularioRepo.CreateAsync(formularioDto.toFormularioFromCreate(estudiante, encargado));

            return RespuestasGenerales<FormularioEstudianteDto>.SuccessResponse(formulario.toFormularioEstudianteDto(), "Formulario creado correctamente.");
        }

        public async Task<RespuestasGenerales<FormularioDto?>> DeleteFormularioAdmin(int formularioId)
        {
            var formularioBorrado = await _formularioRepo.DeleteAsync(formularioId);
            if (formularioBorrado == null)
                return RespuestasGenerales<FormularioDto?>.ErrorResponseService("Formulario", "El formulario no existe.");

            return RespuestasGenerales<FormularioDto?>.SuccessResponse(formularioBorrado.toFormularioDtoWithDetails(), "Formulario eliminado exitosamente.");
        }

        public async Task<RespuestasGenerales<FormularioEstudianteDto>> DeleteFormularioEstudianteAsync(string userId, int formularioId)
        {
            try
            {
                var estudiante = await _estudianteRepo.GetEstudianteByUserId(userId);
                if (estudiante == null)
                    return RespuestasGenerales<FormularioEstudianteDto>.ErrorResponseService("Estudiante","El estudiante no existe");

                var formulario = await _formularioRepo.GetByIdAsync(formularioId);
                if (formulario == null)
                    return RespuestasGenerales<FormularioEstudianteDto>.ErrorResponseService("Formulario", "El formulario no existe");

                if (formulario.EstudianteId != estudiante.Id)
                    return RespuestasGenerales<FormularioEstudianteDto>.ErrorResponseService("Acción no válida.", "Unauthorized");

                var formularioBorrado = await _formularioRepo.DeleteAsync(formularioId);

                return RespuestasGenerales<FormularioEstudianteDto>.SuccessResponse(formularioBorrado!.toFormularioEstudianteDto(), "Formulario eliminado exitosamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<RespuestasGenerales<FormularioEncargadoDto>> FirmarFormularioAsync(string userIdEncargado, int id, FormularioFirmarDto formularioDto)
        {   //todo: Mejorar esto
            try
            {
                // Validar la llave privada, si no lanza una excepción es válida
                var llavePrivadaBytes = Convert.FromBase64String(formularioDto.LlavePrivada);
                using var rsa = RSA.Create();
                rsa.ImportPkcs8PrivateKey(llavePrivadaBytes, out _);

                var formulario = await _formularioRepo.GetByIdAsync(id);
                var encargado = await _encargadoService.GetEncaradoByUserId(userIdEncargado);

                if (formulario == null)
                    return RespuestasGenerales<FormularioEncargadoDto>.ErrorResponseService("Formulario","El formulario no existe");

                if (encargado == null)
                    return RespuestasGenerales<FormularioEncargadoDto>.ErrorResponseService("Encargado", "El encargado no existe");

                if (encargado.LlavePublica == null)
                    return RespuestasGenerales<FormularioEncargadoDto>.ErrorResponseService("LlavePublica", "No tiene clave pública registrada");

                if (formulario.EncargadoId != encargado.Id)
                    return RespuestasGenerales<FormularioEncargadoDto>.ErrorResponseService("No autorizado", "Unauthorized");

                // Procedimiento para firmar
                var contenidoFirmar = new FormularioFirmadoDto
                {
                    FormularioId = formulario.Id,
                    EstudianteId = formulario.EstudianteId,
                    EncargadoId = formulario.EncargadoId,
                    NombreEstudiante = formulario.Estudiante!.AppUser!.NombreCompleto,
                    NombreEncargado = formulario.Encargado!.Usuario!.NombreCompleto,
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
                // Comprobamos si su firma coincide con la correspondiente pública
                bool esCorrespondiente = verificador.VerificarFirmaFormulario(contenidoJson, documentoFirmado, hasDocumento, encargado.LlavePublica);

                if (!esCorrespondiente)
                    return RespuestasGenerales<FormularioEncargadoDto>.ErrorResponseService("LlavePrivada", "La llave privada proporcionada no coincide con la clave pública asociada.");

                formulario.FirmaEncargado = documentoFirmado;
                formulario.HashDocumento = hasDocumento;
                formulario.Firmado = true;
                formulario.FechaFirmado = DateTime.UtcNow;

                var formularioFirmado = await _formularioRepo.UpdateAsync(formulario.Id, formulario);
                //cada vez q se firme el formulario se llama la funcion de comprobar si se le da de baja al estudiante
                await _estudianteService.ComprobarBajaEstudiante(formulario.EstudianteId);
                return RespuestasGenerales<FormularioEncargadoDto>.SuccessResponse(formularioFirmado!.toFormularioEncargadoDto(), "Formulario firmado exitosamente.");
            }
            catch (FormatException)
            {
                return RespuestasGenerales<FormularioEncargadoDto>.ErrorResponseService("LlavePublica", "La llave no es válida.");
            }
            catch (CryptographicException)
            {
                return RespuestasGenerales<FormularioEncargadoDto>.ErrorResponseService("LlavePrivada", "La llave no es válida.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        } 

        public async Task<RespuestasGenerales<List<FormularioEncargadoDto>>> GetAllFormulariosEncargadosAsync(string usuarioId, QueryObjectFormularioEncargado query)
        {
            try
            {
                var formulariosEncargados = await _formularioRepo.GetAllFormulariosByEncargado(usuarioId, query);

                return RespuestasGenerales<List<FormularioEncargadoDto>>.SuccessResponse(formulariosEncargados, "Operación realizada exitosamente.");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<RespuestasGenerales<List<FormularioEstudianteDto>>> GetAllFormulariosEstudiantesAsync(string usuarioId, QueryObjectFormularioEstudiantes query)
        {
            try
            {
                var formulariosEstudiante = await _formularioRepo.GetAllFormulariosByEstudiante(usuarioId, query);
                return RespuestasGenerales<List<FormularioEstudianteDto>>.SuccessResponse(formulariosEstudiante, "Operación realizada exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<RespuestasGenerales<List<FormularioDto>>> GetAllFormulariosWhithDetailsAsync(QueryObjectFormulario query)
        {
            try
            {
                var formularios = await _formularioRepo.GetAllAsync(query);
                return RespuestasGenerales<List<FormularioDto>>.SuccessResponse(formularios, "Operación realizada exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<RespuestasGenerales<FormularioEncargadoDto?>> GetFormEstudianteByIdForEncargadoAsync(string userId, int formularioId)
        {
            try
            {
                var encargado = await _encargadoService.GetEncaradoByUserId(userId);
                if(encargado == null) 
                    return RespuestasGenerales<FormularioEncargadoDto?>.ErrorResponseService("Encargado", "El encargado no existe.", "Unauthorized");

                var formulario = await _formularioRepo.GetFormEstudianteByIdForEncargadoAsync(encargado.Id, formularioId);
                if(formulario == null) 
                    return RespuestasGenerales<FormularioEncargadoDto?>.ErrorResponseService("Formulario", "El formulario no existe.");
                
                return RespuestasGenerales<FormularioEncargadoDto?>.SuccessResponse(formulario, "Operación realizada exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<RespuestasGenerales<FormularioDto?>> GetFormularioWithDetailsAsync(int id)
        {
            try
            {
                var formularioModel = await _formularioRepo.GetByIdAsync(id);
                if(formularioModel == null) 
                    return RespuestasGenerales<FormularioDto?>.ErrorResponseService("Formulario", "El formulario no existe.");
                
                return RespuestasGenerales<FormularioDto?>.SuccessResponse(formularioModel.toFormularioDtoWithDetails(), "Operación realizada exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<RespuestasGenerales<FormularioEstudianteDto>> UpdatePatchFormularioAsync(string usuarioId, int id, UpdateFormularioDto formularioDto)
        {
            try
            {
                var formulario = await _formularioRepo.GetByIdAsync(id);

                if (formulario == null)
                    return RespuestasGenerales<FormularioEstudianteDto>.ErrorResponseService("Formulario", "No existe el formulario");

                if (formulario.Estudiante!.UsuarioId != usuarioId)
                    return RespuestasGenerales<FormularioEstudianteDto>.ErrorResponseService("Usuario", "No autorizado", "Unauthorized");

                var formularioActualizado = await _formularioRepo.UpadatePatchAsync(id, formularioDto);


                var formularioDtoUpdate = formularioActualizado?.toFormularioEstudianteDto();

                return RespuestasGenerales<FormularioEstudianteDto>.SuccessResponse(formularioDtoUpdate!, "Formulario actualizado.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

    }
}