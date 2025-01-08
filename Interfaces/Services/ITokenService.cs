using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUci.Models;

namespace ApiUci.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(AppUser user);
    }
}