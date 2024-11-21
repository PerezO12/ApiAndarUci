using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ApiUCI.Dtos.Encargado;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApiUCI.Dtos.Encargado;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Migrations;
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

        public async Task<EncargadoFirmaDto?> CambiarLlavePublicalAsync(string userId, EncargadoCambiarLlaveDto encargadoDto)
        {
            var encargadosTask = _encargadoRepo.GetAllAsync(new QueryObjectEncargado { UsuarioId = userId });
            var encargados = await encargadosTask;
            var encargado = encargados.FirstOrDefault();

            if(encargado == null) return null;

            try{
                    // Decodificar la llave pública en base64
                    byte[] llavePublicaBytes = Convert.FromBase64String(encargadoDto.LlavePublica);

                    using (var rsa = RSA.Create())
                    {
                        rsa.ImportSubjectPublicKeyInfo(llavePublicaBytes, out _);
                    }
                    encargado.LlavePublica = llavePublicaBytes;
                    await _encargadoRepo.UpdateAsync(encargado.Id, encargado);
                    return new EncargadoFirmaDto{
                        LlavePublica = encargadoDto.LlavePublica
                    };
                } catch(FormatException)
                {
                    return null; 
                }
                catch (CryptographicException)
                {
                    return null;
                }
            throw new NotImplementedException();
        }

        public async Task<EncargadoFirmaDto?> GenerarFirmaDigitalAsync(string userId)
        {
            var encargadosTask = _encargadoRepo.GetAllAsync(new QueryObjectEncargado { UsuarioId = userId });
            var encargados = await encargadosTask;
            var encargado = encargados.FirstOrDefault();

            if(encargado == null) return null; 

            //TODO:si viene alguna firma digital publica la comproabmos y guardamos, falta hacer
            using var rsaGenerado = RSA.Create(2048);
            byte[] llavePublica = rsaGenerado.ExportSubjectPublicKeyInfo();
            string llavePrivada = Convert.ToBase64String(rsaGenerado.ExportPkcs8PrivateKey());

            encargado.LlavePublica = llavePublica;
            await _encargadoRepo.UpdateAsync(encargado.Id, encargado);
            return new EncargadoFirmaDto{
                LlavePrivada = llavePrivada,
                LlavePublica = Convert.ToBase64String(llavePublica)
            };
        }

        public async Task<List<EncargadoDto>> GetAllEncargadosWithDetailsAsync(QueryObjectEncargado query)
        {
            var encargados = await _encargadoRepo.GetAllAsync(query);

            var encargadosDto = encargados.Select(e => new EncargadoDto{
                Id = e.Id,
                UsuarioId = e.UsuarioId,
                DepartamentoNombre = e.Departamento!.Nombre,
                DepartamentoId = e.Departamento.Id,
                NombreCompleto = e.AppUser!.NombreCompleto,
                CarnetIdentidad = e.AppUser.CarnetIdentidad,
                NombreUsuario = e.AppUser.UserName,
                Email = e.AppUser.Email,
                NumeroTelefono = e.AppUser.PhoneNumber
            }).ToList();
            return encargadosDto;
        }

        public async Task<Encargado?> GetEncaradoByUserId(string userId)
        {
            return await _encargadoRepo.GetEncargadoByUserIdAsync(userId);
        }

        public async Task<EncargadoDto?> GetByIdEncargadoWithDetailsAsync(int id)
        {
            var encargado = await _encargadoRepo.GetByIdAsync(id);
            if(encargado == null) return null;

            return new EncargadoDto{
                Id = encargado.Id,
                UsuarioId = encargado.UsuarioId,
                DepartamentoNombre = encargado.Departamento!.Nombre,
                DepartamentoId = encargado.Departamento.Id,
                NombreCompleto = encargado.AppUser!.NombreCompleto,
                CarnetIdentidad = encargado.AppUser.CarnetIdentidad,
                NombreUsuario = encargado.AppUser.UserName,
                Email = encargado.AppUser.Email,
                NumeroTelefono = encargado.AppUser.PhoneNumber
            };
        }

        public async Task<Encargado?> GetEncargadoByDepartamentoId(int departamentoId)
        {
            return await _encargadoRepo.GetEncargadoByDepartamentoId(departamentoId);
        }
    }
}