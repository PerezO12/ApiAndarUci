
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

namespace MyApiUCI.Service
{
    public class EncargadoService : IEncargadoService
    {
        private readonly IEncargadoRepository _encargadoRepo;
        private readonly IDepartamentoRepository _depaRepo;
        private readonly UserManager<AppUser> _userManager;

        public EncargadoService( IEncargadoRepository encargadoRepo, IDepartamentoRepository depRepo, UserManager<AppUser> userManager)
        {
            _encargadoRepo = encargadoRepo;
            _depaRepo = depRepo;
            _userManager = userManager;
        }

        public async Task<EncargadoFirmaDto?> CambiarLlavePublicalAsync(string userId, EncargadoCambiarLlaveDto encargadoDto)
        {
            try{
                byte[] llavePublicaBytes = Convert.FromBase64String(encargadoDto.LlavePublica); // Decodificar la clave pública en base64
                using (var rsa = RSA.Create())
                {
                    rsa.ImportSubjectPublicKeyInfo(llavePublicaBytes, out _);
                }

                var encargado = await _encargadoRepo
                    .UpdateEncargadoByUserIdAsync(userId, new EncargadoUpdateDto{
                        LlavePublica = llavePublicaBytes
                    });
                if(encargado == null) return null;

                return new EncargadoFirmaDto{
                    LlavePublica = encargadoDto.LlavePublica
                };
                
                } catch(FormatException)
                {
                    return null; 
                }
                catch (CryptographicException)
                {
                    return null;
                }
            throw new NotImplementedException();
        }

        public async Task<EncargadoFirmaDto?> GenerarFirmaDigitalAsync(string userId)
        {
            using var rsaGenerado = RSA.Create(2048);
            var llavePublicaByte = rsaGenerado.ExportSubjectPublicKeyInfo();
            //var encargado = await _encargadoRepo.GetEncargadoByUserIdAsync(userId);
            var encargado = await _encargadoRepo
                .UpdateEncargadoByUserIdAsync(userId, new EncargadoUpdateDto{
                    LlavePublica = llavePublicaByte
            });
            if(encargado == null || encargado.LlavePublica == null) return (null); 

            string llavePublicaString = Convert.ToBase64String(encargado.LlavePublica);
            string llavePrivadaString = Convert.ToBase64String(rsaGenerado.ExportPkcs8PrivateKey());

            return (new EncargadoFirmaDto{
                LlavePrivada = llavePrivadaString,
                LlavePublica = llavePublicaString
            });
        }

        public async Task<List<EncargadoDto>> GetAllEncargadosWithDetailsAsync(QueryObjectEncargado query)
        {
            var encargados = await _encargadoRepo.GetAllAsync(query);

            var encargadosDto = encargados.Select(e => new EncargadoDto{
                Id = e.Id,
                UsuarioId = e.UsuarioId,
                DepartamentoNombre = e.Departamento!.Nombre,
                DepartamentoId = e.Departamento.Id,
                NombreCompleto = e.AppUser!.NombreCompleto,
                CarnetIdentidad = e.AppUser.CarnetIdentidad,
                NombreUsuario = e.AppUser.UserName,
                Email = e.AppUser.Email,
                NumeroTelefono = e.AppUser.PhoneNumber
            }).ToList();
            return encargadosDto;
        }

        public async Task<Encargado?> GetEncaradoByUserId(string userId)
        {
            return await _encargadoRepo.GetEncargadoByUserIdAsync(userId);
        }

        public async Task<EncargadoDto?> GetByIdEncargadoWithDetailsAsync(int id)
        {
            var encargado = await _encargadoRepo.GetByIdAsync(id);
            if(encargado == null) return null;

            return new EncargadoDto{
                Id = encargado.Id,
                UsuarioId = encargado.UsuarioId,
                DepartamentoNombre = encargado.Departamento!.Nombre,
                DepartamentoId = encargado.Departamento.Id,
                NombreCompleto = encargado.AppUser!.NombreCompleto,
                CarnetIdentidad = encargado.AppUser.CarnetIdentidad,
                NombreUsuario = encargado.AppUser.UserName,
                Email = encargado.AppUser.Email,
                NumeroTelefono = encargado.AppUser.PhoneNumber
            };
        }

        public async Task<Encargado?> GetEncargadoByDepartamentoIdAsync(int departamentoId)
        {
            return await _encargadoRepo.GetEncargadoByDepartamentoId(departamentoId);
        }

        public async Task<EncargadoDto?> GetByUserIdWithUserId(string id)
        {
            var encargado = await _encargadoRepo.GetByUserIdAsync(id);
            if(encargado == null) return null;

            return new EncargadoDto{
                Id = encargado.Id,
                UsuarioId = encargado.UsuarioId,
                DepartamentoNombre = encargado.Departamento!.Nombre,
                DepartamentoId = encargado.Departamento.Id,
                FacultadNombre = encargado.Departamento.Facultad?.Nombre,
                NombreCompleto = encargado.AppUser!.NombreCompleto,
                CarnetIdentidad = encargado.AppUser.CarnetIdentidad,
                NombreUsuario = encargado.AppUser.UserName,
                Email = encargado.AppUser.Email,
                NumeroTelefono = encargado.AppUser.PhoneNumber
            };
        }

        public async Task<bool> ExisteEncargadoByDepartamentoIdAsync(int departamentoId)
        {
            return await _encargadoRepo.ExisteEncargadoByDepartamentoIdAsync(departamentoId);
        }

        public async Task<Encargado?> DeleteEncargadoByDepartamentoIdAsync(int departamentoId, bool borrarDepartamento)
        {
            var encargado = await _encargadoRepo.DeleteByDepartamentoIdAsync(departamentoId);
            //todo: verificar si funciona
            //si se elimina el encargdalo lo logico es eliminar la referencia del departamento hacia el
            if(encargado == null) return null;

            await _userManager.RemoveFromRoleAsync(encargado!.AppUser!, "Encargado");
            await _userManager.AddToRoleAsync(encargado!.AppUser!, "Profesor");
            if(borrarDepartamento)
            {
                await _depaRepo.CambiarEncargado(departamentoId);
            }
            return encargado;
        }

        public async Task<Encargado?> DeleteByUserIdAsync(string userId)
        {
            //todo:VEr si fucniona
            var encargado = await _encargadoRepo.DeleteByUserIdAsync(userId);
            if(encargado == null) return null;
            await _depaRepo.CambiarEncargado(encargado.DepartamentoId);
            return encargado;
        }

        public async Task<Encargado?> DeleteAsync(int id)
        {
            var encargado = await _encargadoRepo.DeleteAsync(id);
            if(encargado == null) return null;
            await _depaRepo.CambiarEncargado(id);
            return encargado;
        }

        public async Task<Encargado> CreateAsync(Encargado encargadoModel)
        {
            try{
                var encagado = await _encargadoRepo.CreateAsync(encargadoModel);
                await _depaRepo.CambiarEncargado(encagado.DepartamentoId, encagado.Id); 
                return encagado;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public async Task<RespuestasServicios<NewEncargadoDto>> RegisterEncargadoAsync(RegisterEncargadoDto registerDto)
        {
            // Verificar si el departamento existe
            if (!await _depaRepo.ExistDepartamento(registerDto.DepartamentoId))
            {
                var errors = ErrorBuilder.Build("DepartamentoId", "El departamento no existe");
                return RespuestasServicios<NewEncargadoDto>.ErrorResponse(errors);
            }

            // Verificar si ya existe un encargado en el departamento
            if (await _depaRepo.TieneEncargado(registerDto.DepartamentoId))
            {
                var errors = ErrorBuilder.Build("DepartamentoId", "Ya existe un encargado en el departamento");
                return RespuestasServicios<NewEncargadoDto>.ErrorResponse(errors);
            }

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
                return RespuestasServicios<NewEncargadoDto>.ErrorResponse(errores);
            }

            // Asignar el rol de encargado
            var roleResult = await _userManager.AddToRoleAsync(appUser, "Encargado");
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(appUser); // Revertir la creación del usuario si falla la asignación de rol
                var errores = ErrorBuilder.ParseIdentityErrors(roleResult.Errors);
                return RespuestasServicios<NewEncargadoDto>.ErrorResponse(errores);
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

            return RespuestasServicios<NewEncargadoDto>.SuccessResponse(newEncargadoDto, "Encargado creado exitosamente");
        }

        public async Task<Encargado?> UpdateEncargadoByUserIdAsync(string id, EncargadoUpdateDto encargadoDto)
        {
            try
            {
                var encargado = await _encargadoRepo.UpdateEncargadoByUserIdAsync(id, encargadoDto);
                if(encargado == null) return null;
                if(encargadoDto.DepartamentoId != null && encargadoDto.DepartamentoId > 0)
                {
                    await _depaRepo.DeleteEncargadoByEncargadoID(encargado.Id);
                    await _depaRepo.CambiarEncargado(encargado.DepartamentoId, encargado.Id);
                }
                return encargado;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<Encargado?> UpdateAsync(int id, EncargadoUpdateDto encargadoDto)
        {
            try
            {
                var encargado = await _encargadoRepo.UpdateAsync(id, encargadoDto);
                if(encargado == null) return null;
                if(encargadoDto.DepartamentoId != null && encargadoDto.DepartamentoId > 0)
                {
                    await _depaRepo.CambiarEncargado(encargado.Id, encargadoDto.DepartamentoId);
                }
                return encargado;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}