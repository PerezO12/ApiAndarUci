using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApiUCI.Dtos.Encargado;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Models;

namespace MyApiUCI.Service
{
    public class EncargadoService : IEncargadoService
    {
        private readonly IEncargadoRepository _encargadoRepo;
        public EncargadoService( IEncargadoRepository encargadoRepo)
        {
            _encargadoRepo = encargadoRepo;
        }

        public async Task<List<EncargadoDto>> GetAllEncargadosWithDetailsAsync(QueryObjectEncargado query)
        {
            var encargados = await _encargadoRepo.GetAllAsync(query);

            var encargadosDto = encargados.Select(e => new EncargadoDto{
                Id = e.Id,
                UsuarioId = e.UsuarioId,
                DepartamentoNombre = e.Departamento.Nombre,
                DepartamentoId = e.Departamento.Id,
                NombreCompleto = e.AppUser.NombreCompleto,
                CarnetIdentidad = e.AppUser.CarnetIdentidad,
                NombreUsuario = e.AppUser.UserName,
                Email = e.AppUser.Email,
                NumeroTelefono = e.AppUser.PhoneNumber
            }).ToList();
            return encargadosDto;
        }

        public async Task<EncargadoDto?> GetByIdEncargadoWithDetailsAsync(int id)
        {
            var encargado = await _encargadoRepo.GetByIdAsync(id);
            if(encargado == null) return null;

            return new EncargadoDto{
                Id = encargado.Id,
                UsuarioId = encargado.UsuarioId,
                DepartamentoNombre = encargado.Departamento.Nombre,
                DepartamentoId = encargado.Departamento.Id,
                NombreCompleto = encargado.AppUser.NombreCompleto,
                CarnetIdentidad = encargado.AppUser.CarnetIdentidad,
                NombreUsuario = encargado.AppUser.UserName,
                Email = encargado.AppUser.Email,
                NumeroTelefono = encargado.AppUser.PhoneNumber
            };
        }
    }
}