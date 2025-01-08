using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ApiUci.Dtos.Usuarios;
using ApiUci.Helpers;
using ApiUci.Interfaces;
using ApiUci.Models;
using ApiUci.Dtos;
using ApiUci.Dtos.Estudiante;
using ApiUci.Dtos.Encargado;
using ApiUci.Dtos.Cuentas;
using ApiUci.Extensions;
using ApiUci.Mappers;


namespace ApiUci.Service
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
                usuariosQuery = usuariosQuery.Where(u => u.User.Activo == true);

            if (!string.IsNullOrWhiteSpace(query.CarnetIdentidad))
                usuariosQuery = usuariosQuery.Where(u => u.User.CarnetIdentidad.ToLower().Contains(query.CarnetIdentidad.ToLower()));
            if (!string.IsNullOrWhiteSpace(query.Nombre))
                usuariosQuery = usuariosQuery.Where(u => u.User.NombreCompleto != null &&
                                                        u.User.NombreCompleto.ToLower().Contains(query.Nombre.ToLower()));
            
            if (!string.IsNullOrWhiteSpace(query.Email))
                usuariosQuery = usuariosQuery.Where(u => u.User.Email != null &&
                                                        u.User.Email.ToLower().Contains(query.Email.ToLower()));
            
            if (!string.IsNullOrWhiteSpace(query.Usuario))
                usuariosQuery = usuariosQuery.Where(u => u.User.UserName != null &&
                                                        u.User.UserName.ToLower().Contains(query.Usuario.ToLower()));

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
                UserName = u.User.UserName!,
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
                UserName = registroDto.UserName,
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
                UserName = appUser.UserName!,
                Email = appUser.Email,
                NombreCompleto = appUser.NombreCompleto,
                Roles = new List<string> { "Admin" }
            };
            IList<string> roles = new List<string> { "Admin"};
            // Devolver éxito con el DTO
            return RespuestasGenerales<NewAdminDto>.SuccessResponse(appUser.toAdminDto(roles), "Administrador creado exitosamente");
        }

        public async Task<RespuestasGenerales<UsuarioDto>> UpdateAsync(string userId, UsuarioWhiteRolUpdateDto usuarioUpdateDto)
        {
            try
            {   //obtenemos el usuario y validamos si existe
                var usuario = await _userManager.FindByIdAsync(userId);
                if (usuario == null)
                    return RespuestasGenerales<UsuarioDto>.ErrorResponseService("Usuario", "El usuario no existe.");

                //se  mapean los datos
                usuario.updateAppUserFromUsuarioWhiteRole(usuarioUpdateDto);

                var result = await _userManager.UpdateAsync(usuario);
                if (!result.Succeeded)
                    return RespuestasGenerales<UsuarioDto>.ErrorResponseController(ErrorBuilder.ParseIdentityErrors(result.Errors));

                //se verifica s hay cambio de contrasena
                if (!string.IsNullOrWhiteSpace(usuarioUpdateDto.Password))
                {
                    var cambioPasswordResponse = await CambiarPasswordUsuario(usuario, usuarioUpdateDto.Password);
                    if (!cambioPasswordResponse.Success)
                        return RespuestasGenerales<UsuarioDto>.ErrorResponseService("Password", cambioPasswordResponse.Message ?? "Error al cambiar la contraseña.");
                }

                //si verifica si hay cambio de roles si no hay o lo q sea y toma dec segun eso
                var resultRolesNuevos = await CambiarRolesYremoverEntidades(usuario, usuarioUpdateDto.Roles);
                if(!resultRolesNuevos.Success)
                    return RespuestasGenerales<UsuarioDto>.ErrorResponseController(resultRolesNuevos.Errors!, "Error al cambiar los roles.");
                
                //se crean las entidades relacionadas
                await UpdateOrCreateRelatedEntities(usuario, usuarioUpdateDto);


                return RespuestasGenerales<UsuarioDto>.SuccessResponse(usuario.toUsuarioDto(resultRolesNuevos.Data), "Usuario actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /* Metodos auxiliares a update user, tmb pueden tener otros propositos */

        public async Task<RespuestasGenerales<bool>> CambiarPasswordUsuario(AppUser usuario, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                return RespuestasGenerales<bool>.ErrorResponseService("Password", "La nueva contraseña no puede estar vacía.");
            
            var removePasswordResult = await _userManager.RemovePasswordAsync(usuario);
            if (!removePasswordResult.Succeeded)
                return RespuestasGenerales<bool>.ErrorResponseController(ErrorBuilder.ParseIdentityErrors(removePasswordResult.Errors));

            var addPasswordResult = await _userManager.AddPasswordAsync(usuario, newPassword);
            if (!addPasswordResult.Succeeded)
            {
                var errores = ErrorBuilder.ParseIdentityErrors(addPasswordResult.Errors);
                return RespuestasGenerales<bool>.ErrorResponseController(errores);
            }

            return RespuestasGenerales<bool>.SuccessResponse(true, "Contraseña actualizada exitosamente.");
        }

        
        /* Manejo de camabios de rol */
        private async Task<RespuestasGenerales<IEnumerable<string>>> CambiarRolesYremoverEntidades(AppUser usuario, List<string> rolesNuevos)
        {
            try
            {
                var rolesRemover =  await _userManager.GetRolesAsync(usuario);

                // Si no hay cambios en los roles, actualiza las entidades relacionadas
                if (rolesRemover.ToHashSet().SetEquals(rolesNuevos.ToHashSet()))
                    return RespuestasGenerales<IEnumerable<string>>.SuccessResponse(rolesRemover, "No hay cambios en los roles.");

                // Si hay cambios de roles
                var addResult = await _userManager.AddToRolesAsync(usuario, rolesNuevos);
                if(!addResult.Succeeded)
                    throw new Exception("Error añadiendo roles.");

                var removeResult = await _userManager.RemoveFromRolesAsync(usuario, rolesRemover);
                if(!removeResult.Succeeded)
                throw new Exception("Error removiendo roles.");

                //remover las entidades relacionadas
                await RemoveEntidades(usuario, rolesRemover);//remueve las entidades viejas
                return RespuestasGenerales<IEnumerable<string>>.SuccessResponse(rolesNuevos, "Roles actualizados exitosamente.");
            }
            catch (Exception ex)
            {
                await _userManager.RemoveFromRolesAsync(usuario, rolesNuevos);//si falla intenta remover los roles nuevos
                Console.WriteLine(ex);
                throw;
            }
        }
        /* Update entidades relacionadas */
        private async Task<RespuestasGenerales<UsuarioDto>> UpdateOrCreateRelatedEntities(AppUser usuario, UsuarioWhiteRolUpdateDto usuarioUpdateDto)
        {
            try
            {
                if (usuarioUpdateDto.Roles.Contains("Estudiante"))
                {
                    //actualiza el estudiante si existe
                    var estudiante = await _estudianteRepo.UpdateEstudianteByUserIdAsync(usuario.Id, new EstudianteUpdateDto
                    {
                        CarreraId = usuarioUpdateDto.CarreraId,
                        FacultadId = usuarioUpdateDto.FacultadId,
                        Activo = usuarioUpdateDto.Activo
                    });
                    //si no existe crealo
                    if (estudiante == null)
                    {
                        await _estudianteRepo.CreateAsync(new Estudiante
                        {
                            UsuarioId = usuario.Id,
                            CarreraId = usuarioUpdateDto.CarreraId,
                            FacultadId = usuarioUpdateDto.FacultadId
                        });
                    }
                }
                else if (usuarioUpdateDto.Roles.Contains("Encargado"))
                {   //actualiza el encargado
                    var encargado = await _encargadoService.UpdateEncargadoByUserIdAsync(usuario.Id, new EncargadoUpdateDto
                    {
                        DepartamentoId = usuarioUpdateDto.DepartamentoId,
                        Activo = usuarioUpdateDto.Activo
                    });
                    //si no existe se crea
                    if (encargado == null)
                    {
                        await _encargadoService.CreateAsync(new Encargado
                        {
                            UsuarioId = usuario.Id,
                            DepartamentoId = usuarioUpdateDto.DepartamentoId
                        });
                    }
                }

                return RespuestasGenerales<UsuarioDto>.SuccessResponse(usuario.toUsuarioDto());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        private async Task RemoveEntidades(AppUser usuario, IEnumerable<string> entidadesRemover)
        {
            foreach (var rol in entidadesRemover)
            {
                if (rol == "Estudiante")
                {
                    await _estudianteRepo.DeleteByUserIdAsync(usuario.Id);
                }
                else if (rol == "Encargado")
                {
                    await _encargadoService.DeleteByUserIdAsync(usuario.Id);
                }
            }
        }
    }
}