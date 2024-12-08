
using System.Security.Cryptography;

using ApiUCI.Dtos.Encargado;

using MyApiUCI.Dtos.Encargado;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.IO.Compression;
using MyApiUCI.Models;
using ApiUCI.Helpers;

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
            try{
                byte[] llavePublicaBytes = Convert.FromBase64String(encargadoDto.LlavePublica); // Decodificar la llave pública en base64
                using (var rsa = RSA.Create())
                {
                    rsa.ImportSubjectPublicKeyInfo(llavePublicaBytes, out _);
                }

                var encargado = await _encargadoRepo
                    .UpdateEncargadoByUserIdAsync(userId, new EncargadoUpdateDto{
                        LlavePublica = llavePublicaBytes
                    });
                if(encargado == null) return null;

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
            using var rsaGenerado = RSA.Create(2048);
            var llavePublicaByte = rsaGenerado.ExportSubjectPublicKeyInfo();
            //var encargado = await _encargadoRepo.GetEncargadoByUserIdAsync(userId);
            var encargado = await _encargadoRepo
                .UpdateEncargadoByUserIdAsync(userId, new EncargadoUpdateDto{
                    LlavePublica = llavePublicaByte
            });
            if(encargado == null || encargado.LlavePublica == null) return (null); 

            string llavePublicaString = Convert.ToBase64String(encargado.LlavePublica);
            string llavePrivadaString = Convert.ToBase64String(rsaGenerado.ExportPkcs8PrivateKey());

            return (new EncargadoFirmaDto{
                LlavePrivada = llavePrivadaString,
                LlavePublica = llavePublicaString
            });
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

        public async Task<EncargadoDto?> GetByUserIdWithUserId(string id)
        {
            var encargado = await _encargadoRepo.GetByUserIdAsync(id);
            if(encargado == null) return null;

            return new EncargadoDto{
                Id = encargado.Id,
                UsuarioId = encargado.UsuarioId,
                DepartamentoNombre = encargado.Departamento!.Nombre,
                DepartamentoId = encargado.Departamento.Id,
                FacultadNombre = encargado.Departamento.Facultad?.Nombre,
                NombreCompleto = encargado.AppUser!.NombreCompleto,
                CarnetIdentidad = encargado.AppUser.CarnetIdentidad,
                NombreUsuario = encargado.AppUser.UserName,
                Email = encargado.AppUser.Email,
                NumeroTelefono = encargado.AppUser.PhoneNumber
            };
        }

        public async Task<bool> ExisteEncargadoByDepartamentoIdAsync(int departamentoId)
        {
            return await _encargadoRepo.ExisteEncargadoByDepartamentoIdAsync(departamentoId);
        }
    }
}