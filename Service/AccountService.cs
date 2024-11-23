using ApiUCI.Dtos;
using ApiUCI.Dtos.Cuentas;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApiUCI.Dtos.Cuentas;
using MyApiUCI.Interfaces;
using MyApiUCI.Models;

namespace MyApiUCI.Service
{
    public class AccountService : IAcountService
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IFacultadRepository _facuRepo;
        private readonly ICarreraRepository _carreraRepo;
        private readonly IEstudianteRepository _estudianteRepo;
        private readonly ApplicationDbContext _context;
        public AccountService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService,
            IFacultadRepository facuRepo,
            ICarreraRepository carreraRepo,
            IEstudianteRepository estudianteRepo,
            ApplicationDbContext context
            
            )
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _facuRepo = facuRepo;
            _carreraRepo = carreraRepo;
            _estudianteRepo = estudianteRepo;
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
            var user = await  _userManager.Users.FirstOrDefaultAsync(u => u.UserName != null && u.UserName.ToLower() == loginDto.Nombre.ToLower());
            if(user == null) return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if(!result.Succeeded) return null;
            var rol = await _userManager.GetRolesAsync(user);
            if(rol == null) return null;
            var token = await _tokenService.CreateTokenAsync(user);
            // Guarda el token en la base de datos
            await _userManager.SetAuthenticationTokenAsync(user, "JWT", "AccessToken", token);
            return new NewUserDto
                {   
                    id = user.Id,
                    NombreCompleto = user.NombreCompleto,
                    NombreUsuario = user.UserName,
                    Email = user.Email,
                    Rol = rol[0],
                    Token = token
                }
;
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

            // Crear el usuario
            var appUser = new AppUser
            {
                UserName = registerDto.UserName,
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
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(appUser);
                throw new Exception("Error al guardar el encargado: " + ex.Message);
            }
                       // Crear y devolver el DTO
            var newEncargadoDto = new NewEncargadoDto
            {
                UserName = appUser.UserName,
                Email = appUser.Email,
                NombreCompleto = appUser.NombreCompleto,
                Departamento = existeDepartamento.Nombre,
                Token = await _tokenService.CreateTokenAsync(appUser)
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
                UserName = registerDto.UserName,
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
            
            var newEstudianteDto = new NewEstudianteDto
            {
                UserName = appUser.UserName,
                Email = appUser.Email,
                Carrera = existeCarrera.Nombre,
                NombreCompleto = appUser.NombreCompleto,
                Facultad = existeFacultad.Nombre,
                Token = await _tokenService.CreateTokenAsync(appUser)
            };

            return (IdentityResult.Success, newEstudianteDto);
        }
    }
}