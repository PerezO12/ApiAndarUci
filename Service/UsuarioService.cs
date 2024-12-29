using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApiUCI.Dtos.Usuarios;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Models;
using ApiUCI.Dtos.Usuarios;
using ApiUCI.Interfaces;
using ApiUCI.Dtos;
using ApiUCI.Dtos.Estudiante;
using ApiUCI.Dtos.Encargado;
using ApiUCI.Dtos.Cuentas;
using ApiUCI.Extensions;
using MyApiUCI.Mappers;


namespace ApiUCI.Service
{
    public class UsuarioService : IUsuarioService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEstudianteRepository _estudianteRepo;
        private readonly IEncargadoService _encargadoService;
        private readonly ApplicationDbContext _context;
        public UsuarioService(
            UserManager<AppUser> usuarioManager,
            IEstudianteRepository estudianteRepo,
            IEncargadoService encargadoService,
            ApplicationDbContext context
            )
        {
            _userManager = usuarioManager;
            _estudianteRepo = estudianteRepo;
            _encargadoService = encargadoService; 
            _context = context;  
        }

        public async Task<RespuestasGenerales<UsuarioDto>> DeleteUserYRolAsync(string usuarioId)
        {
            try
            {
                var usuarioBorrar = await _userManager.FindByIdAsync(usuarioId);
                if (usuarioBorrar == null || usuarioBorrar.Activo == false)
                    return RespuestasGenerales<UsuarioDto>.ErrorResponseService("Usuario","El usuario no existe");

                var rol = await _userManager.GetRolesAsync(usuarioBorrar);
                if (rol == null || rol.Any())
                    return RespuestasGenerales<UsuarioDto>.ErrorResponseService("Rol", "El usuario no tiene rol", "StatusCode500");

                usuarioBorrar.Activo = false;
                var resultado = await _userManager.UpdateAsync(usuarioBorrar);
                if (!resultado.Succeeded)
                    return RespuestasGenerales<UsuarioDto>.ErrorResponseService("Error", "Error al eliminar el usuario", "StatusCode500");

                if (rol.Contains("Estudiante"))
                {
                    var result = await _estudianteRepo.DeleteByUserIdAsync(usuarioId);
                    if (result == null)
                        return RespuestasGenerales<UsuarioDto>.ErrorResponseService("Estudiante", "Error al eliminar el estudiante", "StatusCode500");
                }

                if (rol.Contains("Encargado"))
                {
                    var result = await _encargadoService.DeleteByUserIdAsync(usuarioId);
                    if (result == null)
                        return RespuestasGenerales<UsuarioDto>.ErrorResponseService("Estudiante","Error al eliminar el encargado", "StatusCode500");
                }

                return RespuestasGenerales<UsuarioDto>.SuccessResponse(usuarioBorrar.toUsuarioDtoBorrar(), "Usuario eliminado exitosamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<RespuestasGenerales<UsuarioDto>> UpdateAsync(string userId, UsuarioWhiteRolUpdateDto usuarioUpdateDto)
        {   //todo: arreglar esto, separarlo para q sea mas mantenible
            try
            {
                var usuario = await _userManager.FindByIdAsync(userId);
                if(usuario == null) 
                    return RespuestasGenerales<UsuarioDto>.ErrorResponseService("Usuario", "El usuario no existe.");
                
                var rolesActual = await _userManager.GetRolesAsync(usuario);
                //comprobar q si va a cambiar el nomre de usuario no exista otro con el mismo nombre
                if( (usuarioUpdateDto.NombreUsuario != null ) && (usuario.UserName != usuarioUpdateDto.NombreUsuario)){
                    var existeUsername = await _userManager.Users
                        .FirstOrDefaultAsync(
                            u => u.UserName != null 
                            && u.UserName.ToLower() == usuarioUpdateDto.NombreUsuario.ToLower() 
                            && u.Id != usuario.Id);
                    if(existeUsername != null) 
                        return RespuestasGenerales<UsuarioDto>.ErrorResponseService("UserName", "El nombre de usuario no disponible.");
                }
                // Mapear los valores manualmente desde el DTO
                usuario.updateAppUserFromUsuarioWhiteRole(usuarioUpdateDto);
       
                //verifica si no ocurrió un cambio de rol            
                if(rolesActual.ToHashSet().SetEquals(usuarioUpdateDto.Roles))
                {
                    if(usuarioUpdateDto.Roles.Contains("Estudiante"))
                    {
                        var estudiante = await _estudianteRepo
                        .UpdateEstudianteByUserIdAsync(userId, new EstudianteUpdateDto{
                            CarreraId = usuarioUpdateDto.CarreraId,
                            FacultadId = usuarioUpdateDto.FacultadId,
                            Activo = usuarioUpdateDto.Activo    
                        });
                        if(estudiante == null) 
                            return RespuestasGenerales<UsuarioDto>.ErrorResponseService("Estudiante", "El estudiante no existe.");
                    }
                    else if(usuarioUpdateDto.Roles.Contains("Encargado"))
                    {
                        var encargado = await _encargadoService
                        .UpdateEncargadoByUserIdAsync(userId, new EncargadoUpdateDto {
                            DepartamentoId = usuarioUpdateDto.DepartamentoId,
                            Activo = usuarioUpdateDto.Activo
                        });
                        if(encargado == null) 
                            return RespuestasGenerales<UsuarioDto>.ErrorResponseService("Encargado", "El encargado no existe.");
                    }
                }//si hay un cambio de rol
                else               
                {
                    //ver  q rol se le asignara para crearlo
                    if(usuarioUpdateDto.Roles.Contains("Estudiante"))//rol estudiante
                    {
                        await _estudianteRepo
                            .CreateAsync(new Estudiante {
                                            UsuarioId = userId,
                                            CarreraId = usuarioUpdateDto.CarreraId,
                                            FacultadId = usuarioUpdateDto.FacultadId
                                        });
                        await _userManager.AddToRoleAsync(usuario, "Estudiante");
                    }
                    else if(usuarioUpdateDto.Roles.Contains("Encargado"))//encargado
                    {   //si vamos a crear un encargado verificamos q no existe un encargado en el departamento
                        var existeEncargadoDepartamento = await _encargadoService.ExisteEncargadoByDepartamentoIdAsync(usuarioUpdateDto.DepartamentoId);
                        if(existeEncargadoDepartamento)
                            return RespuestasGenerales<UsuarioDto>.ErrorResponseService("Departamento", "Ya existe un encargado en el departamento.");

                        await _encargadoService
                            .CreateAsync( new Encargado {
                                            UsuarioId = userId,
                                            DepartamentoId = usuarioUpdateDto.DepartamentoId
                                        });
                        await _userManager.AddToRoleAsync(usuario, "Encargado");
                    }
                    else if(usuarioUpdateDto.Roles.Contains("Admin"))//admin
                    {
                        await _userManager.AddToRoleAsync(usuario, "Admin");
                    }
                    //ver rol antiguo para borrarlo
                    if(rolesActual.Contains("Estudiante"))
                    {
                        await _estudianteRepo.DeleteByUserIdAsync(userId);
                        await _userManager.RemoveFromRoleAsync(usuario, "Estudiante");
                    }
                    else if(rolesActual.Contains("Encargado"))
                    {
                        await _encargadoService.DeleteByUserIdAsync(userId);
                        await _userManager.RemoveFromRoleAsync(usuario, "Encargado");
                    }
                    else if(rolesActual.Contains("Admin"))
                    {
                        await _userManager.RemoveFromRoleAsync(usuario, "Admin");
                    }
                    
                }

                // Actualizar el usuario
                var result = await _userManager.UpdateAsync(usuario);
                if(!result.Succeeded) 
                    return RespuestasGenerales<UsuarioDto>.ErrorResponseService("Error", "Error al actualizar el usuario.", "StatusCode500");
                //ver si el password viene para cambiarlo
                if(!string.IsNullOrWhiteSpace(usuarioUpdateDto.Password))
                {
                    var removePasswordResult = await _userManager.RemovePasswordAsync(usuario);
                    if(!removePasswordResult.Succeeded) 
                        return RespuestasGenerales<UsuarioDto>.ErrorResponseService("Error", "Error al eliminar la contraseña.", "StatusCode500");
                    
                    var addPasswordResult = await _userManager.AddPasswordAsync(usuario, usuarioUpdateDto.Password);
                    if(!addPasswordResult.Succeeded) 
                        return RespuestasGenerales<UsuarioDto>.ErrorResponseService("Error", "Error al guardar la contraseña.", "StatusCode500");
                }
                var roles = await _userManager.GetRolesAsync(usuario);

                return RespuestasGenerales<UsuarioDto>.ErrorResponseService(usuario.toUsuarioDto(roles) ,"Usuario actualizado exitosamente.");
            }
            catch(Exception ex)
            {
                
                if(usuarioUpdateDto.Roles.Contains("Estudiante"))
                {
                    await _estudianteRepo.DeleteByUserIdAsync(userId);
                }   
                else if(usuarioUpdateDto.Roles.Contains("Encargado"))
                {
                    await _encargadoService.DeleteByUserIdAsync(userId);
                }
                Console.WriteLine(ex);
                throw;
            }
        }
        public async Task<RespuestasGenerales<UsuarioDto?>> DeleteAsync(string id)
        {
            try
            {
                var usuario = await _userManager.FindByIdAsync(id);
                if(usuario == null) 
                    return RespuestasGenerales<UsuarioDto?>.ErrorResponseService("Usuario", "El usuario no existe.");
                usuario.Activo = false;
                
                var result = await _userManager.UpdateAsync(usuario);
                if(!result.Succeeded)
                    return RespuestasGenerales<UsuarioDto?>.ErrorResponseService("Error", "Error al borrar el usuario.");

                return RespuestasGenerales<UsuarioDto?>.SuccessResponse(usuario.toUsuarioDtoBorrar(), "Usuario borrado.");;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<RespuestasGenerales<List<UsuarioDto>>> GetAllAsync(QueryObjectUsuario query)
        {
            //todo: esto hay q arreglarlo para hacerlo ams eficiente  y seguir buenas practicas !!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            var usuariosQuery = _userManager.Users
                .Where(user => user.NombreCompleto != "Admin Web Api")
                .Select(user => new
                {
                    User = user,
                    Roles = _context.UserRoles
                        .Where(ur => ur.UserId == user.Id)
                        .Join(_context.Roles,
                            ur => ur.RoleId,
                            role => role.Id,
                            (ur, role) => role.Name)
                        .ToList()
                });

            // Filtros
            if (query.SoloActivos)
            {
                usuariosQuery = usuariosQuery.Where(u => u.User.Activo == true);
            }
            if (!string.IsNullOrWhiteSpace(query.CarnetIdentidad))
            {
                usuariosQuery = usuariosQuery.Where(u => u.User.CarnetIdentidad.ToLower().Contains(query.CarnetIdentidad.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(query.Nombre))
            {
                usuariosQuery = usuariosQuery.Where(u => u.User.NombreCompleto != null &&
                                                        u.User.NombreCompleto.ToLower().Contains(query.Nombre.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(query.Email))
            {
                usuariosQuery = usuariosQuery.Where(u => u.User.Email != null &&
                                                        u.User.Email.ToLower().Contains(query.Email.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(query.NombreUsuario))
            {
                usuariosQuery = usuariosQuery.Where(u => u.User.UserName != null &&
                                                        u.User.UserName.ToLower().Contains(query.NombreUsuario.ToLower()));
            }

            // Ordenar
            if (!string.IsNullOrWhiteSpace(query.OrdenarPor))
            {
                usuariosQuery = query.OrdenarPor.ToLower() switch
                {
                    "nombre" => query.Descender
                        ? usuariosQuery.OrderByDescending(u => u.User.NombreCompleto)
                        : usuariosQuery.OrderBy(u => u.User.NombreCompleto),
                    "email" => query.Descender
                        ? usuariosQuery.OrderByDescending(u => u.User.Email)
                        : usuariosQuery.OrderBy(u => u.User.Email),
                    "usuario" => query.Descender
                        ? usuariosQuery.OrderByDescending(u => u.User.UserName)
                        : usuariosQuery.OrderBy(u => u.User.UserName),
                    "carnet" => query.Descender
                        ? usuariosQuery.OrderByDescending(u => u.User.CarnetIdentidad)
                        : usuariosQuery.OrderBy(u => u.User.CarnetIdentidad),
                    _ => usuariosQuery
                };
            }

            // Paginación
            var skipNumber = (query.NumeroPagina - 1) * query.TamañoPagina;

            // Ejecutar la consulta asincrónica y proyectar a UsuarioDto en memoria
            var usuarios = await usuariosQuery
                .Skip(skipNumber)
                .Take(query.TamañoPagina)
                .ToListAsync();

            // Proyección a UsuarioDto
            var usuariosDto = usuarios.Select(u => new UsuarioDto
            {
                Id = u.User.Id,
                Activo = u.User.Activo,
                NombreUsuario = u.User.UserName!,
                NombreCompleto = u.User.NombreCompleto,
                Email = u.User.Email,
                CarnetIdentidad = u.User.CarnetIdentidad,
                NumeroTelefono = u.User.PhoneNumber,
                Roles = u.Roles!
            }).ToList();

            return RespuestasGenerales<List<UsuarioDto>>.SuccessResponse(usuariosDto, "Operación relaizada exitosamente.");
        }

        public async Task<RespuestasGenerales<UsuarioDto?>> GetByIdAsync(string id)
        {
            var user = await _userManager
                .FindByIdAsync(id);
            if (user == null) 
                return RespuestasGenerales<UsuarioDto?>.ErrorResponseService("Usuario", "El usuario no existe.");
            
            var roles  = await _userManager.GetRolesAsync(user);
            
           return RespuestasGenerales<UsuarioDto?>.SuccessResponse(user.toUsuarioDto(roles), "Operación realizada exitosamente.");
        }

        public async Task<RespuestasGenerales<NewAdminDto>> RegistrarAdministradorAsync(RegistroAdministradorDto registroDto)
        {
            // Crear el usuario
            var appUser = new AppUser
            {
                UserName = registroDto.NombreUsuario,
                Email = registroDto.Email,
                NombreCompleto = registroDto.NombreCompleto,
                CarnetIdentidad = registroDto.CarnetIdentidad
            };

            // Crear el usuario en la base de datos
            var createUserResult = await _userManager.CreateAsync(appUser, registroDto.Password);
            if (!createUserResult.Succeeded)
            {
                var errores = ErrorBuilder.ParseIdentityErrors(createUserResult.Errors);
                return RespuestasGenerales<NewAdminDto>.ErrorResponseController(errores);
            }

            // Asignar rol de administrador
            var roleResult = await _userManager.AddToRoleAsync(appUser, "Admin");
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(appUser); // Revertir creación del usuario si falla la asignación del rol
                var errores = ErrorBuilder.ParseIdentityErrors(roleResult.Errors);
                return RespuestasGenerales<NewAdminDto>.ErrorResponseController(errores);
            }

            // Crear el DTO para el administrador
            var newAdminDto = new NewAdminDto
            {
                Id = appUser.Id,
                Activo = appUser.Activo,
                CarnetIdentidad = appUser.CarnetIdentidad,
                NombreUsuario = appUser.UserName!,
                Email = appUser.Email,
                NombreCompleto = appUser.NombreCompleto,
                Roles = new List<string> { "Admin" }
            };
            IList<string> roles = new List<string> { "Admin"};
            // Devolver éxito con el DTO
            return RespuestasGenerales<NewAdminDto>.SuccessResponse(appUser.toAdminDto(roles), "Administrador creado exitosamente");
        }


    }
}