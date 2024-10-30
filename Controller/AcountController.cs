using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyApiUCI.Dtos.Cuentas;
using MyApiUCI.Interfaces;
using MyApiUCI.Models;
using MyApiUCI.Repository;

namespace MyApiUCI.Controller
{
    [Route("api/account")]
    [ApiController]
    public class AcountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AcountController(UserManager<AppUser> userManager,
                                ITokenService tokenService,
                                SignInManager<AppUser> signInManager,
                                ApplicationDbContext context)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpPost("register/estudiante")]
        public async Task<IActionResult> Register([FromBody] RegisterEstudianteDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid) 
                    return BadRequest(ModelState);

                // Verificar si la facultad existe
                var existeFacultad = await _context.facultad.FindAsync(registerDto.FacultadId);
                if (existeFacultad == null) return BadRequest("Facultad no existe");

                // Verificar si la carrera existe
                var existeCarrera = await _context.carrera.FindAsync(registerDto.CarreraId);
                if (existeCarrera == null) return BadRequest("Carrera no existe");

                // Crear el usuario
                var appUser = new AppUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email,
                    NombreCompleto = registerDto.NombreCompleto,
                    CarnetIdentidad = registerDto.CarnetIdentidad
                };

                var createUser = await _userManager.CreateAsync(appUser, registerDto.Password);

                if (createUser.Succeeded)
                {
                    // Asignar rol studinate
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "Estudiante");
                    if (roleResult.Succeeded)
                    {
                        
                        var estudiante = new Estudiante
                        {
                            UsuarioId = appUser.Id,
                            CarreraId = registerDto.CarreraId,
                            FacultadId = registerDto.FacultadId
                        };

                        // Guardar estudnte
                        try
                        {
                            await _context.estudiante.AddAsync(estudiante);
                            await _context.SaveChangesAsync(); // Guardar cambios
                        }
                        catch (Exception e)
                        {
                            return StatusCode(500, new { message = "Error al guardar el estudiante", error = e.Message });
                        }

                        // Retornar el DTO del nuevo estudiante
                        return Ok(new NewEstudianteDto
                        {
                            UserName = appUser.UserName,
                            Email = appUser.Email,
                            Carrera = existeCarrera.Nombre,
                            Facultad = existeFacultad.Nombre,
                            Token = await _tokenService.CreateTokenAsync(appUser)
                        });
                    }
                    else
                    {
                        return StatusCode(500, new { message = "Error al asignar rol", errors = roleResult.Errors });
                    }
                }
                else
                {
                    return StatusCode(500, new { message = "Error al crear usuario", errors = createUser.Errors });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "Error en el servidor", error = e.Message });
            }
        }
        //Crear login de usuario
    }
}
