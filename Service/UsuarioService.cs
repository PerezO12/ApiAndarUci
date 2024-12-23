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


namespace ApiUCI.Service
{
    public class UsuarioService : IUsuarioService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IEstudianteRepository _estudianteRepo;
        private readonly IEncargadoService _encargadoService;
        public UsuarioService(
            UserManager<AppUser> usuarioManager,
            ApplicationDbContext context,
            IEstudianteRepository estudianteRepo,
            IEncargadoService encargadoService
            )
        {
            _userManager = usuarioManager;
            _context = context;
            _estudianteRepo = estudianteRepo;
            _encargadoService = encargadoService;
            
        }

        public async Task<ResultadoDto> DeleteUserYRolAsync(string usuarioId, string adminId)
        {
            try
            {

                var adminUser = await _userManager.FindByIdAsync(adminId);
                if(adminUser == null){
                    return new ResultadoDto{
                        msg = "No Autorizado",
                        TipoError = "Unauthorized",
                        Error = true
                    }; 
                }
                var usuarioBorrar = await _userManager.FindByIdAsync(usuarioId);
                if(usuarioBorrar == null || usuarioBorrar.Activo == false)
                {
                    return new ResultadoDto{
                        msg = "El usuario no existe",
                        TipoError = "BadRequest",
                        Error = true
                    };
                }
                var rol = await _userManager.GetRolesAsync(usuarioBorrar);
                if(rol == null){
                    return new ResultadoDto{
                        msg = "El usuario no tiene rol",
                        TipoError = "StatusCode500",
                        Error = true
                    };
                }
                usuarioBorrar.Activo = false;
                var resultado = await _userManager.UpdateAsync(usuarioBorrar);
                if(!resultado.Succeeded) 
                {
                    return new ResultadoDto{
                        msg = "Error al eliminar el usuario",
                        TipoError = "StatusCode500",
                        Error = true
                    };
                }
                if(rol.Contains("Estudiante")) {
                    var result = await _estudianteRepo.DeleteByUserIdAsync(usuarioId);
                    if(result == null) 
                    {
                        return new ResultadoDto{
                            msg = "Error al eliminar el estudiante",
                            TipoError = "StatusCode500",
                            Error = true
                        };
                    } 
                }
                if(rol.Contains("Encargado")) {
                    var result = await _encargadoService.DeleteByUserIdAsync(usuarioId);
                    if(result == null) 
                    {
                        return new ResultadoDto{
                            msg = "Error al eliminar el encargado",
                            TipoError = "StatusCode500",
                            Error = true
                        };
                    } 
                }
                return new ResultadoDto {
                    msg = "Usuario eliminado exitosamente",
                    TipoError = "Ok",
                    Error = false
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<IdentityResult> UpdateAsync(string userId, UsuarioWhiteRolUpdateDto usuarioUpdateDto)
        {
            try
            {
                var usuario = await _userManager.FindByIdAsync(userId);
                if(usuario == null) return IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado"});
                var rolesActual = await _userManager.GetRolesAsync(usuario);
                //comprobar q si va a cambiar el nomre de usuario no exista otro con el mismo nombre
                if( (usuarioUpdateDto.NombreUsuario != null ) && (usuario.UserName != usuarioUpdateDto.NombreUsuario)){
                    var existeUsername = await  _userManager.Users
                        .FirstOrDefaultAsync(
                            u => u.UserName != null 
                            && u.UserName.ToLower() == usuarioUpdateDto.NombreUsuario.ToLower() 
                            && u.Id != usuario.Id);
                    if(existeUsername != null) return IdentityResult.Failed(new IdentityError { Description = "Nombre de usuario no disponible"});
                }
                // Mapear los valores manualmente desde el DTO
                usuario.NombreCompleto = usuarioUpdateDto.NombreCompleto ?? usuario.NombreCompleto;
                usuario.Activo = usuarioUpdateDto.Activo ?? usuario.Activo;
                usuario.CarnetIdentidad = usuarioUpdateDto.CarnetIdentidad ?? usuario.CarnetIdentidad;
                usuario.UserName = usuarioUpdateDto.NombreUsuario ?? usuario.UserName;
                usuario.Email = usuarioUpdateDto.Email ?? usuario.Email;
                usuario.PhoneNumber = usuarioUpdateDto.NumeroTelefono ?? usuario.PhoneNumber;
       
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
                        if(estudiante == null) return IdentityResult.Failed(new IdentityError { Description = "Estudiante no encontrado"});
                    }
                    else if(usuarioUpdateDto.Roles.Contains("Encargado"))
                    {
                        /* var encargado = await _encargadoService
                        .UpdateEncargadoByUserIdAsync(userId, new EncargadoUpdateDto {
                            DepartamentoId = usuarioUpdateDto.DepartamentoId,
                            Activo = usuarioUpdateDto.Activo
                        }); */
                        var encargado = await _encargadoService
                        .UpdateEncargadoByUserIdAsync(userId, new EncargadoUpdateDto {
                            DepartamentoId = usuarioUpdateDto.DepartamentoId,
                            Activo = usuarioUpdateDto.Activo
                        });
                        if(encargado == null) return IdentityResult.Failed(new IdentityError { Description = "Encargado no encontrado"});
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
                        {
                            return IdentityResult.Failed(new IdentityError { Description = "Ya existe un encargado en el departamento"});
                        }
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
                if(!result.Succeeded) return IdentityResult.Failed(new IdentityError { Description = "Error al actualizar el usuario"});
                //ver si el password viene para cambiarlo
                if(!string.IsNullOrWhiteSpace(usuarioUpdateDto.Password))
                {
                    var removePasswordResult = await _userManager.RemovePasswordAsync(usuario);
                    if(!removePasswordResult.Succeeded) return IdentityResult.Failed(new IdentityError { Description = "Error al eliminar la contraseña actual"});
                    
                    var addPasswordResult = await _userManager.AddPasswordAsync(usuario, usuarioUpdateDto.Password);
                    if(!addPasswordResult.Succeeded) return IdentityResult.Failed(new IdentityError { Description = "Error al guardar la contraseña"});
                }

                return IdentityResult.Success;
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
        public async Task<AppUser?> DeleteAsync(string id)
        {
            try
            {
                var usuario = await _userManager.FindByIdAsync(id);
                if(usuario == null) return null;
                usuario.Activo = false;
                
                var result = await _userManager.UpdateAsync(usuario);
                if(result.Succeeded) return usuario;

                return null;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<List<UsuarioDto>> GetAllAsync(QueryObjectUsuario query)
        {
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

            return usuariosDto;
        }

        public async Task<UsuarioDto?> GetByIdAsync(string id)
        {
            var user = await _userManager
                .FindByIdAsync(id);
            if (user == null) return null;
            var roles  = await _userManager.GetRolesAsync(user);
            
           return new UsuarioDto {
                Id = user.Id,
                Activo = user.Activo,
                NombreUsuario = user.UserName!,
                NombreCompleto = user.NombreCompleto,
                Email = user.Email,
                CarnetIdentidad = user.CarnetIdentidad,
                NumeroTelefono = user.PhoneNumber,
                Roles = roles.ToList()
           };
        }

        public async Task<RespuestasServicios<NewAdminDto>> RegistrarAdministradorAsync(RegistroAdministradorDto registroDto)
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
                return RespuestasServicios<NewAdminDto>.ErrorResponse(errores);
            }

            // Asignar rol de administrador
            var roleResult = await _userManager.AddToRoleAsync(appUser, "Admin");
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(appUser); // Revertir creación del usuario si falla la asignación del rol
                var errores = ErrorBuilder.ParseIdentityErrors(roleResult.Errors);
                return RespuestasServicios<NewAdminDto>.ErrorResponse(errores);
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

            // Devolver éxito con el DTO
            return RespuestasServicios<NewAdminDto>.SuccessResponse(newAdminDto, "Administrador creado exitosamente");
        }
    }
}