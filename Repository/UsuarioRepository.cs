using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApiUCI.Helpers;
using MyApiUCI.Interfaces;
using MyApiUCI.Models;

namespace MyApiUCI.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly UserManager<AppUser> _userManager;

        public UsuarioRepository(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public Task<AppUser> CreateAsync(AppUser appUser)
        {
            throw new NotImplementedException();
        }

        public Task<AppUser?> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<AppUser>> GetAllAsync(QueryObjectUsuario query)
        {
            var usuarios = _userManager.Users.Where(e => e.Activo == true).AsQueryable();

            //FILTROS
            if(query.CarnetIdentidad != null)
            {
                usuarios = usuarios.Where( u => 
                    u.CarnetIdentidad.ToLower().Contains(query.CarnetIdentidad.ToLower()));
            }
          
            if(query.Nombre != null)
            {
                usuarios = usuarios.Where( u => u.NombreCompleto != null && 
                    u.NombreCompleto.ToLower().Contains(query.Nombre.ToLower()));
            }
            if(query.Email != null)
            {
                usuarios = usuarios.Where( u => u.Email != null && 
                    u.Email.ToLower().Contains(query.Email.ToLower()));
            }
            if(query.NombreUsuario != null)
            {
                usuarios = usuarios.Where( u => u.UserName != null &&
                    u.UserName.ToLower().Contains(query.NombreUsuario.ToLower()));
            }
            //Ordernar
            if(!string.IsNullOrWhiteSpace(query.OrdernarPor))
            {
                if(query.OrdernarPor.Equals("Nombre"))
                {
                    usuarios = query.Descender 
                        ? usuarios.OrderByDescending( u => u.NombreCompleto) 
                        : usuarios.OrderBy( u => u.NombreCompleto);
                }
                if(query.OrdernarPor.Equals("Email"))
                {
                    usuarios = query.Descender
                        ? usuarios.OrderByDescending( u => u.Email)
                        : usuarios.OrderBy( u=> u.Email );
                }
                if(query.OrdernarPor.Equals("Usuario"))
                {
                    usuarios = query.Descender
                        ? usuarios.OrderByDescending( u => u.UserName)
                        : usuarios.OrderBy (u => u.UserName);
                }
                if(query.OrdernarPor.Equals("Carnet"))
                {
                    usuarios = query.Descender
                        ? usuarios.OrderByDescending( u => u.CarnetIdentidad)
                        : usuarios.OrderBy (u => u.CarnetIdentidad);
                }
            }
            //paginas
            var skipNumber = ( query.NumeroPagina - 1 ) * query.TamañoPagina;
            
            return await usuarios.Skip(skipNumber).Take(query.TamañoPagina).ToListAsync();
        }

        public Task<AppUser?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<AppUser?> UpdateAsync(int id, AppUser appUser)
        {
            throw new NotImplementedException();
        }
    }
}