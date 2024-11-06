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
        private readonly UserManager<AppUser> _userManager;
        private readonly IDepartamentoRepository _depaRepo;
        public EncargadoService(
            IEncargadoRepository encargadoRepo,
            UserManager<AppUser> userManager,
            IDepartamentoRepository depaRepo
        )
        {
            _encargadoRepo = encargadoRepo;
            _userManager = userManager;
            _depaRepo = depaRepo;
        }

        public async Task<List<EncargadoDto>> GetAllEncargadosWithDetailsAsync(QueryObjectEncargado query)
        {
            //consulta
            var encargadosModel = await _encargadoRepo.GetAllAsync(query);

            //Ids de las netindades q necesito
            var usuariosIds = encargadosModel.Select(e => e.UsuarioId).ToList();
            var departamentosIds = encargadosModel.Select(e => e.DepartamentoId).ToList();

            //consultas
            var usuariosEncargados = await _userManager.Users
                .Where( u => usuariosIds.Contains(u.Id))
                .ToListAsync();
            
            var departamentos = await _depaRepo.GetAllAsync(new QueryObject {ListaId = departamentosIds});

            var encargadosDatosCombinados = encargadosModel
                .Join(
                    usuariosEncargados,
                    encargados => encargados.UsuarioId,
                    usuario => usuario.Id,
                    (encargado, usuario) => new
                    {
                        Encargado = encargado,
                        Usuario = usuario
                    })
                .Join(
                    departamentos,
                    combinado => combinado.Encargado.DepartamentoId,
                    departamento => departamento.Id,
                    (combinado, departamento) => new EncargadoDto
                    {
                        Id = combinado.Encargado.Id,
                        UsuarioId = combinado.Encargado.UsuarioId,
                        NombreCompleto = combinado.Usuario.NombreCompleto,
                        CarnetIdentidad = combinado.Usuario.CarnetIdentidad,
                        UserName = combinado.Usuario.UserName,
                        Email = combinado.Usuario.Email,
                        NumeroTelefono = combinado.Usuario.PhoneNumber,
                        DepartamentoNombre = departamento.Nombre,
                        DepartamentoId = departamento.Id
                    }
                )
            .ToList();
            //ordenamiento, hacer leugo ************************

            return encargadosDatosCombinados;
        }
    }
}