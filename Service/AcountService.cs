using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MyApiUCI.Dtos.Cuentas;
using MyApiUCI.Interfaces;
using MyApiUCI.Models;

namespace MyApiUCI.Service
{
    public class AcountService : IAcountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        public AcountService(UserManager<AppUser> userManager,
                ITokenService tokenService,
                SignInManager<AppUser> signInManager,
                ApplicationDbContext context)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<(IdentityResult, NewEncargadoDto?)> RegisterEncargado(RegisterEncargadoDto registerDto)
        {
            // Generar el hash SHA-256 de la firma digital cambiar esto mas adelante********
            byte[] firmaDigitalHash;
            using (var sha256 = SHA256.Create())
            {
                firmaDigitalHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(registerDto.FirmaDigital));
            }


            var existeDepartamento = await _context.departamento.FindAsync(registerDto.DepartamentoId);
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
                DepartamentoId = registerDto.DepartamentoId,
                FirmaDigital = firmaDigitalHash
            };
            try{
                await _context.encargado.AddAsync(encargado);
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

        public async Task<(IdentityResult, NewEstudianteDto?)> RegisterEstudiante(RegisterEstudianteDto registerDto)
        {
            //Mejorar esto *****************************************************
            // Verificar si la facultad existe
            var existeFacultad = await _context.facultad.FindAsync(registerDto.FacultadId);
            if (existeFacultad == null) return (IdentityResult.Failed(new IdentityError { Description = "Facultad no existe" }), null);

            // Verificar si la carrera existe
            var existeCarrera = await _context.carrera.FindAsync(registerDto.CarreraId);
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
                //Mejorar
            try
            {
                await _context.estudiante.AddAsync(estudiante);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(appUser); //pra no dejar usuarios sin estudintes asignados
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