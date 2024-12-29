
using System.Security.Cryptography;

using ApiUCI.Dtos.Encargado;

using MyApiUCI.Dtos.Encargado;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.IO.Compression;
using MyApiUCI.Models;
using ApiUCI.Helpers;
using MyApiUCI.Dtos.Departamento;
using Microsoft.AspNetCore.Identity;
using ApiUCI.Dtos;
using MyApiUCI.Dtos.Cuentas;
using ApiUCI.Extensions;
using MyApiUCI.Mappers;

namespace MyApiUCI.Service
{
    public class EncargadoService : IEncargadoService
    {
        private readonly IEncargadoRepository _encargadoRepo;
        private readonly IDepartamentoRepository _depaRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<EncargadoService> _logger;

        public EncargadoService
        (
            IEncargadoRepository encargadoRepo,
            IDepartamentoRepository depRepo,
            UserManager<AppUser> userManager,
            ILogger<EncargadoService> logger,
            SignInManager<AppUser> signInManager            
        )
        {
            _encargadoRepo = encargadoRepo;
            _depaRepo = depRepo;
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
        }

        public async Task<RespuestasGenerales<EncargadoFirmaDto?>> CambiarLlavePublicalAsync(string userId, EncargadoCambiarLlaveDto encargadoDto)
        {
            try
            {
                var usuario = await _userManager.FindByIdAsync(userId); 
                var resultadoPassword = await _signInManager.CheckPasswordSignInAsync(usuario!, encargadoDto.Password, false);
                if(resultadoPassword.Succeeded)
                    return RespuestasGenerales<EncargadoFirmaDto?>.ErrorResponseService("Password", "La contraseña es incorrecta.");

                // Decodificmamos la llave pública
                byte[] llavePublicaBytes = Convert.FromBase64String(encargadoDto.LlavePublica);

                // La válidamos
                using (var rsa = RSA.Create())
                {
                    rsa.ImportSubjectPublicKeyInfo(llavePublicaBytes, out _);
                }

                var encargado = await _encargadoRepo.UpdateEncargadoByUserIdAsync(userId, new EncargadoUpdateDto
                {
                    LlavePublica = llavePublicaBytes
                });

                if (encargado == null)
                    return RespuestasGenerales<EncargadoFirmaDto?>.ErrorResponseService("Encargado", "El encargado no existe.");

                // Construir la respuesta de éxito
                var nuevaFirmaEncargado = new EncargadoFirmaDto
                {
                    LlavePublica = encargadoDto.LlavePublica
                };
                return RespuestasGenerales<EncargadoFirmaDto?>.SuccessResponse(nuevaFirmaEncargado, "La clave pública fue cambiada exitosamente.");
            }
            catch (FormatException ex)
            {
                _logger.LogError($"Error al cambiar la llave publica del encargado {userId}", ex);
                return RespuestasGenerales<EncargadoFirmaDto?>.ErrorResponseService("LlavePublica", "La clave pública tiene un formato no válido.");
            }
            catch (CryptographicException ex)
            {
                _logger.LogError($"Error al cambiar la llave publica del encargado {userId}", ex);
                return RespuestasGenerales<EncargadoFirmaDto?>.ErrorResponseService("LlavePublica", "La clave pública tiene un formato no válido.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al cambiar la llave publica del encargado {userId}, exception: {ex.Message}");
                throw;
            }
        }


        public async Task<RespuestasGenerales<EncargadoFirmaDto?>> GenerarFirmaDigitalAsync(string userId, EncargadoGenerarLLaveDto encargadoDto)
        {
            try
            {
                var usuario = await _userManager.FindByIdAsync(userId); 
                var resultadoPassword = await _signInManager.CheckPasswordSignInAsync(usuario!, encargadoDto.Password, false);
                if(resultadoPassword.Succeeded)
                    return RespuestasGenerales<EncargadoFirmaDto?>.ErrorResponseService("Password", "La contraseña es incorrecta.");


                using var rsaGenerado = RSA.Create(2048);
                var llavePublicaByte = rsaGenerado.ExportSubjectPublicKeyInfo();

                var encargado = await _encargadoRepo
                    .UpdateEncargadoByUserIdAsync(userId, new EncargadoUpdateDto
                    {
                        LlavePublica = llavePublicaByte
                    });

                if (encargado == null || encargado.LlavePublica == null)
                    return RespuestasGenerales<EncargadoFirmaDto?>.ErrorResponseService("Encargado", "El encargado no existe.");

                string llavePublicaString = Convert.ToBase64String(encargado.LlavePublica);
                string llavePrivadaString = Convert.ToBase64String(rsaGenerado.ExportPkcs8PrivateKey());

                var nuevaFirmaEncargado = new EncargadoFirmaDto
                {
                    LlavePrivada = llavePrivadaString,
                    LlavePublica = llavePublicaString
                };
                return RespuestasGenerales<EncargadoFirmaDto?>.SuccessResponse(nuevaFirmaEncargado, "La firma se generó exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al generar la firma del encargado{userId}, exception: {ex.Message}");
                throw;
            }
        }


        public async Task<RespuestasGenerales<List<EncargadoDto>>> GetAllEncargadosWithDetailsAsync(QueryObjectEncargado query)
        {
            try
            {
                var encargados = await _encargadoRepo.GetAllAsync(query);

                if (encargados == null || !encargados.Any())
                    return RespuestasGenerales<List<EncargadoDto>>.ErrorResponseService("Encargados", "No se encontraron encargados.");

                var encargadosDto = encargados.Select(e => e.toEncargadoDtoFromEncargado()).ToList();

                return RespuestasGenerales<List<EncargadoDto>>.SuccessResponse(encargadosDto, "Encargados obtenidos exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener encargados: {ex.Message}");
                return RespuestasGenerales<List<EncargadoDto>>.ErrorResponseService("encargados", "Hubo un error al obtener los encargados.");
            }
        }

        //se puede borrar
        public async Task<Encargado?> GetEncaradoByUserId(string userId)
        {
            return await _encargadoRepo.GetEncargadoByUserIdAsync(userId);
        }

        public async Task<RespuestasGenerales<EncargadoDto?>> GetByIdEncargadoWithDetailsAsync(int id)
        {
            try
            {
                var encargado = await _encargadoRepo.GetByIdAsync(id);
                if (encargado == null)
                    return RespuestasGenerales<EncargadoDto?>.ErrorResponseService("Encargado", "El encargado no existe.");

                return RespuestasGenerales<EncargadoDto?>.SuccessResponse(encargado.toEncargadoDtoFromEncargado(), "Encargado obtenido exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener encargado con ID {id}: {ex.Message}");
                return RespuestasGenerales<EncargadoDto?>.ErrorResponseService("Encargado", "Hubo un error al obtener el encargado."); ;
            }
        }

        //borrar
        public async Task<Encargado?> GetEncargadoByDepartamentoIdAsync(int departamentoId)
        {
            return await _encargadoRepo.GetEncargadoByDepartamentoId(departamentoId);
        }

        public async Task<RespuestasGenerales<EncargadoDto?>> GetByUserIdWithUserId(string id)
        {
            try
            {
                var encargado = await _encargadoRepo.GetByUserIdAsync(id);
                if (encargado == null)
                    return RespuestasGenerales<EncargadoDto?>.ErrorResponseService("Encargado", "No se encontró el encargado.");

                return RespuestasGenerales<EncargadoDto?>.SuccessResponse(encargado.toEncargadoDtoFromEncargado(), "Encargado obtenido exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener encargado con ID {id}: {ex.Message}");
                throw;
            }
        }   
        //borrar
        public async Task<bool> ExisteEncargadoByDepartamentoIdAsync(int departamentoId)
        {
            return await _encargadoRepo.ExisteEncargadoByDepartamentoIdAsync(departamentoId);
        }

        public async Task<Encargado?> DeleteEncargadoByDepartamentoIdAsync(int departamentoId, bool borrarDepartamento)
        {
            try
            {
                var encargado = await _encargadoRepo.DeleteByDepartamentoIdAsync(departamentoId);

                if (encargado == null) return null;

                await _userManager.RemoveFromRoleAsync(encargado.AppUser!, "Encargado");
                await _userManager.AddToRoleAsync(encargado.AppUser!, "Profesor");//arreglar esto luego 
                
                if (borrarDepartamento)
                {
                    await _depaRepo.CambiarEncargado(departamentoId);
                }
                return encargado;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al borrar el encargado del departamento: {departamentoId}: {ex.Message}");
                throw;
            }
        }

        public async Task<Encargado?> DeleteByUserIdAsync(string userId)
        {
            try
            {
                var encargado = await _encargadoRepo.DeleteByUserIdAsync(userId);
                if (encargado == null) return null;
                await _depaRepo.CambiarEncargado(encargado.DepartamentoId);
                return encargado;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al borrar el encargado con ID {userId}: {ex.Message}");
                throw;
            }
        }

        public async Task<Encargado?> DeleteAsync(int id)
        {
            try
            {
                var encargado = await _encargadoRepo.DeleteAsync(id);
                if (encargado == null) return null;

                await _depaRepo.CambiarEncargado(id);
                return encargado;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al borrar el encargado con ID {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<Encargado> CreateAsync(Encargado encargadoModel)
        {
            try
            {
                var encagado = await _encargadoRepo.CreateAsync(encargadoModel);
                await _depaRepo.CambiarEncargado(encagado.DepartamentoId, encagado.Id);
                return encagado;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el encargado: {ex.Message}");
                throw;
            }
        }
        public async Task<RespuestasGenerales<NewEncargadoDto>> RegisterEncargadoAsync(RegisterEncargadoDto registerDto)
        {
            if (!await _depaRepo.ExistDepartamento(registerDto.DepartamentoId))
                return RespuestasGenerales<NewEncargadoDto>.ErrorResponseService("Departamento", "El departamento no existe.");
            if (await _depaRepo.TieneEncargado(registerDto.DepartamentoId))
                return RespuestasGenerales<NewEncargadoDto>.ErrorResponseService("Departamento", "Ya existe un encargado en el departamento");
           
            // Crear el usuario
            var appUser = new AppUser
            {
                UserName = registerDto.NombreUsuario,
                Email = registerDto.Email,
                NombreCompleto = registerDto.NombreCompleto,
                CarnetIdentidad = registerDto.CarnetIdentidad
            };

            // Crear el usuario en la base de datos
            var createUserResult = await _userManager.CreateAsync(appUser, registerDto.Password);
            if (!createUserResult.Succeeded)
            {
                var errores = ErrorBuilder.ParseIdentityErrors(createUserResult.Errors);
                return RespuestasGenerales<NewEncargadoDto>.ErrorResponseController(errores);
            }

            // Asignar el rol de encargado
            var roleResult = await _userManager.AddToRoleAsync(appUser, "Encargado");
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(appUser); // Revertir la creación del usuario si falla la asignación de rol
                var errores = ErrorBuilder.ParseIdentityErrors(roleResult.Errors);
                return RespuestasGenerales<NewEncargadoDto>.ErrorResponseController(errores);
            }

            try
            {
                var encagado = await _encargadoRepo.CreateAsync((new Encargado
                {
                    UsuarioId = appUser.Id,
                    DepartamentoId = registerDto.DepartamentoId
                }));
                await _depaRepo.CambiarEncargado(encagado.DepartamentoId, encagado.Id);
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(appUser);
                await _encargadoRepo.DeleteByUserIdAsync(appUser.Id);
                Console.WriteLine(ex.Message);
                throw;
            }

            // Crear el DTO de encargado
            var newEncargadoDto = new NewEncargadoDto
            {
                Id = appUser.Id,
                Activo = appUser.Activo,
                CarnetIdentidad = appUser.CarnetIdentidad,
                NombreUsuario = appUser.UserName!,
                Email = appUser.Email,
                NombreCompleto = appUser.NombreCompleto,
                Roles = new List<string> { "Encargado" }
            };

            return RespuestasGenerales<NewEncargadoDto>.SuccessResponse(newEncargadoDto, "Encargado creado exitosamente");
        }

        public async Task<Encargado?> UpdateEncargadoByUserIdAsync(string id, EncargadoUpdateDto encargadoDto)
        {
            try
            {
                var encargado = await _encargadoRepo.UpdateEncargadoByUserIdAsync(id, encargadoDto);
                if (encargado == null) return null;
                if (encargadoDto.DepartamentoId != null && encargadoDto.DepartamentoId > 0)
                {
                    await _depaRepo.DeleteEncargadoByEncargadoID(encargado.Id);
                    await _depaRepo.CambiarEncargado(encargado.DepartamentoId, encargado.Id);
                }
                return encargado;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el encargado {id} : {ex.Message}");
                throw;
            }
        }

        public async Task<Encargado?> UpdateAsync(int id, EncargadoUpdateDto encargadoDto)
        {
            try
            {
                var encargado = await _encargadoRepo.UpdateAsync(id, encargadoDto);
                if (encargado == null) return null;
                if (encargadoDto.DepartamentoId != null && encargadoDto.DepartamentoId > 0)
                {
                    await _depaRepo.CambiarEncargado(encargado.Id, encargadoDto.DepartamentoId);
                }
                return encargado;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el encargado {id}: {ex.Message}");
                throw;
            }
        }
    }
}