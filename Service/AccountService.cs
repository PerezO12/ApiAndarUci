using ApiUCI.Dtos;
using ApiUCI.Dtos.Cuentas;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApiUCI.Dtos.Cuentas;
using MyApiUCI.Interfaces;
using MyApiUCI.Models;

namespace MyApiUCI.Service
{
    public class AccountService : IAccountService
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IFacultadRepository _facuRepo;
        private readonly ICarreraRepository _carreraRepo;
        private readonly IEstudianteRepository _estudianteRepo;
        private readonly IEncargadoService _encargadoService;
        private readonly ApplicationDbContext _context;
        public AccountService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService,
            IFacultadRepository facuRepo,
            ICarreraRepository carreraRepo,
            IEstudianteRepository estudianteRepo,
            IEncargadoService encargadoService,
            ApplicationDbContext context
            )
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _facuRepo = facuRepo;
            _carreraRepo = carreraRepo;
            _estudianteRepo = estudianteRepo;
            _encargadoService = encargadoService;
            _context = context;
        }

        public async Task<IdentityResult> CambiarPasswordAsync(string userId, CambiarPasswordDto cuentaDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return (IdentityResult.Failed(new IdentityError { Description = "El usuario no existe"}));
            }

            var result = await _userManager.ChangePasswordAsync(user, cuentaDto.PasswordActual, cuentaDto.PasswordNueva );
            
            if(result.Succeeded)
            {
                return (IdentityResult.Success);
            }
            return (IdentityResult.Failed(result.Errors.ToArray()));
        }

        public async Task<NewUserDto?> Login(LoginDto loginDto)
        {
            try{

            var user = await  _userManager.Users
                .FirstOrDefaultAsync(u => u.UserName != null && u.UserName.ToLower() == loginDto.Nombre.ToLower());
            if(user == null) return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if(!result.Succeeded) return null;

            var roles = await _userManager.GetRolesAsync(user);
            if(roles == null || !roles.Any())
            {
                throw new Exception("El usuario no tiene roles asignados.");
            }
            var token = await _tokenService.CreateTokenAsync(user);
            // Guarda el token en la base de datos
            await _userManager.SetAuthenticationTokenAsync(user, "JWT", "AccessToken", token);
            return new NewUserDto
                {   
                    id = user.Id,
                    NombreCompleto = user.NombreCompleto,
                    NombreUsuario = user.UserName,
                    Email = user.Email,
                    Rol = roles[0],
                    Token = token
                };
            }
            catch(Exception ex)
            {
                Console.Write(ex);
                throw;
            }
        }

        public async Task<UserPerfilDto?> ObtenerPerfilAsync(string id)
        {
            var usuarioModel = await _userManager.FindByIdAsync(id);
            if(usuarioModel == null) return null; 
            var rol = await _userManager.GetRolesAsync(usuarioModel);
            if(rol == null) return null;    
            return new UserPerfilDto {
                id = id,
                NombreCompleto = usuarioModel.NombreCompleto,
                NombreUsuario = usuarioModel.UserName,
                Email = usuarioModel.Email,
                Rol = rol[0]
            };

        }

        public async Task<(IdentityResult, NewEncargadoDto?)> RegisterEncargadoAsync(RegisterEncargadoDto registerDto)
        {

            var existeDepartamento = await _context.Departamento.FindAsync(registerDto.DepartamentoId);
            if(existeDepartamento == null) return (IdentityResult.Failed(new IdentityError { Description = "Departamento no existe"}), null);
            //comprobar q  no haya un encargado en el departamento ya q solo puede haber un encargado
            var existeEncargadoEnDepartmaneto = await _encargadoService.ExisteEncargadoByDepartamentoIdAsync(registerDto.DepartamentoId);
            if(existeEncargadoEnDepartmaneto) return (IdentityResult.Failed(new IdentityError { Description = "Ya existe un encargado en el departamento"}), null);
            // Crear el usuario
            var appUser = new AppUser
            {
                UserName = registerDto.nombreUsuario,
                Email = registerDto.Email,
                NombreCompleto = registerDto.NombreCompleto,
                CarnetIdentidad = registerDto.CarnetIdentidad
            };
            var createUserResult = await _userManager.CreateAsync(appUser, registerDto.Password);
            //validacion d la creacion
            if (!createUserResult.Succeeded)
            {
                return (IdentityResult.Failed(createUserResult.Errors.ToArray()), null);
            }
            var roleResult = await _userManager.AddToRoleAsync(appUser, "Encargado");

            if (!roleResult.Succeeded)
            {
                return (IdentityResult.Failed(roleResult.Errors.ToArray()), null);
            } 

            var encargado = new Encargado
            {
                UsuarioId = appUser.Id,
                DepartamentoId = registerDto.DepartamentoId
            };
            try{
                await _context.Encargado.AddAsync(encargado);
                await _context.SaveChangesAsync();
                existeDepartamento.EncargadoId = encargado.Id;
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(appUser);
                throw new Exception("Error al guardar el encargado: " + ex.Message);
            }
            var roles =  new List<string>();
            roles.Add("Encargado");
            var newEncargadoDto = new NewEncargadoDto
            {
                Id = appUser.Id,
                Activo = appUser.Activo,
                CarnetIdentidad = appUser.CarnetIdentidad,
                NombreUsuario = appUser.UserName,
                Email = appUser.Email,
                NombreCompleto = appUser.NombreCompleto,
                Departamento = existeDepartamento.Nombre,
                Roles = roles
            };

            return (IdentityResult.Success, newEncargadoDto);
        }

        //Estudiantes
        public async Task<(IdentityResult, NewEstudianteDto?)> RegisterEstudianteAsync(RegisterEstudianteDto registerDto)
        {
            // Verificar si la facultad existe
            var existeFacultad = await _facuRepo.GetByIdAsync(registerDto.FacultadId);
            if (existeFacultad == null) return (IdentityResult.Failed(new IdentityError { Description = "Facultad no existe" }), null);

            // Verificar si la carrera existe
            var existeCarrera = await _carreraRepo.GetByIdAsync(registerDto.CarreraId);
            if (existeCarrera == null) return (IdentityResult.Failed(new IdentityError { Description = "Facultad no existe" }), null);
            
            // Crear el usuario
            var appUser = new AppUser
            {
                UserName = registerDto.nombreUsuario,
                Email = registerDto.Email,
                NombreCompleto = registerDto.NombreCompleto,
                CarnetIdentidad = registerDto.CarnetIdentidad
            };

            var createUserResult = await _userManager.CreateAsync(appUser, registerDto.Password);
            //validacion d la creacion
            if (!createUserResult.Succeeded)
            {
                return (IdentityResult.Failed(createUserResult.Errors.ToArray()), null);
            }
            
            var roleResult = await _userManager.AddToRoleAsync(appUser, "Estudiante");

            if (!roleResult.Succeeded)
            {
                return (IdentityResult.Failed(roleResult.Errors.ToArray()), null);
            } 
            
            var estudiante = new Estudiante
            {
                UsuarioId = appUser.Id,
                CarreraId = registerDto.CarreraId,
                FacultadId = registerDto.FacultadId
            };

            try
            {
                await _estudianteRepo.CreateAsync(estudiante);
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(appUser);
                throw new Exception("Error al guardar el estudiante: " + ex.Message);
            }
            var roles =  new List<string>();
            roles.Add("Estudiante");

            var newEstudianteDto = new NewEstudianteDto
            {
                Id = appUser.Id,
                Activo = appUser.Activo,
                CarnetIdentidad = appUser.CarnetIdentidad,
                NombreUsuario = appUser.UserName!,
                Email = appUser.Email,
                NombreCompleto = appUser.NombreCompleto,
                Roles = roles
            };

            return (IdentityResult.Success, newEstudianteDto);
        }

        public async Task<(IdentityResult, NewAdminDto?)> RegistrarAdministradorAsync(RegistroAdministradorDto registroDto)
        {
            var appUser = new AppUser
            {
                UserName = registroDto.NombreUsuario,
                Email = registroDto.Email,
                NombreCompleto = registroDto.NombreCompleto,
                CarnetIdentidad = registroDto.CarnetIdentidad
            };
            var createUserResult = await _userManager.CreateAsync(appUser, registroDto.Password);
            //validacion d la creacion
            if (!createUserResult.Succeeded)
            {
                return (IdentityResult.Failed(createUserResult.Errors.ToArray()), null);
            }
            
            var roleResult = await _userManager.AddToRoleAsync(appUser, "Admin");

            if (!roleResult.Succeeded)
            {
                return (IdentityResult.Failed(roleResult.Errors.ToArray()), null);
            } 
            var roles =  new List<string>();
            roles.Add("Admin");

            var newAdminDto = new NewAdminDto
            {
                Id = appUser.Id,
                Activo = appUser.Activo,
                CarnetIdentidad = appUser.CarnetIdentidad,
                NombreUsuario = appUser.UserName!,
                Email = appUser.Email,
                NombreCompleto = appUser.NombreCompleto,
                Roles = roles 
            };

            return (IdentityResult.Success, newAdminDto);
        }
    }
}