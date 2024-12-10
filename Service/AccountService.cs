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
        private readonly UserManager<AppUser> _userManager;
        private readonly IFacultadRepository _facuRepo;
        private readonly ICarreraRepository _carreraRepo;
        private readonly IEstudianteRepository _estudianteRepo;
        private readonly IEncargadoService _encargadoService;
        private readonly ApplicationDbContext _context;
        public AccountService(          
            UserManager<AppUser> userManager,
            IFacultadRepository facuRepo,
            ICarreraRepository carreraRepo,
            IEstudianteRepository estudianteRepo,
            IEncargadoService encargadoService,
            ApplicationDbContext context
            )
        {
            _userManager = userManager;
            _facuRepo = facuRepo;
            _carreraRepo = carreraRepo;
            _estudianteRepo = estudianteRepo;
            _encargadoService = encargadoService;
            _context = context;
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