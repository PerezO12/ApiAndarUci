using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiUCI.Interfaces.Services
{
    public interface IIpBlockService
    {
        Task<bool> IsBlockedAsync(string ipAddress);
        Task RegisterFailedAttemptAsync(string ipAddress);
    }
}