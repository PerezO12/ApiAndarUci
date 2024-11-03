using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyApiUCI.Helpers;
using MyApiUCI.Models;

namespace MyApiUCI.Interfaces
{
    public interface IUsuarioRepository
    {
        public Task<List<AppUser>> GetAllAsync(QueryObjectUsuario query);
        public Task<AppUser?> GetByIdAsync(int id);
        public Task<AppUser> CreateAsync(AppUser appUser);
        public Task<AppUser?> UpdateAsync(int id, AppUser appUser);
        public Task<AppUser?> DeleteAsync(int id);
        //public Task<AppUser?> PatchAsync(int id, )
    }
}