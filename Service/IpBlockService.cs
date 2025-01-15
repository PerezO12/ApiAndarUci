using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUCI.Interfaces.Services;
using ApiUCI.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiUCI.Service
{
    public class IpBlockService : IIpBlockService
    {
        private readonly ApplicationDbContext _context;
        private readonly int _maxAttempts;
        private readonly TimeSpan _blockDuration;

        public IpBlockService( ApplicationDbContext context, int maxAttempts = 7, TimeSpan? blockDuration = null)
        {
            _context = context;
            _maxAttempts = maxAttempts;
            _blockDuration = blockDuration ?? TimeSpan.FromMinutes(30);
        }

        //verifica si esta bloqueado, y si ya paso la fecha de bloqueo
        public async Task<bool> IsBlockedAsync(string ipAddress)
        {
            var registro = await _context.FailedLoginAttempt.FirstOrDefaultAsync(r => r.IPAddress == ipAddress);
            if (registro == null || registro.LockoutEnd == null)
                return false;

            if (registro.LockoutEnd > DateTime.UtcNow)
                return true;
            //si el bloqueo ya expiro restrablecemos el contador
            registro.AttemptCount = 0;
            registro.LockoutEnd = null;
            await _context.SaveChangesAsync();

            return false;
        }
        //registramos el intento fallido
        public async Task RegisterFailedAttemptAsync(string ipAddress)
        {
            var record = await _context.FailedLoginAttempt.FirstOrDefaultAsync(r => r.IPAddress == ipAddress);
            //si no tenia ninguno lo registramos
            if (record == null)
            {
                record = new FailedLoginAttempt
                {
                    IPAddress = ipAddress,
                    AttemptCount = 1,
                    LastAttempt = DateTime.UtcNow
                };
                _context.FailedLoginAttempt.Add(record);
            }
            else
            {//si ya tenia uno le agregamos otro intento
                record.AttemptCount++;
                record.LastAttempt = DateTime.UtcNow;

                if (record.AttemptCount >= _maxAttempts)
                {
                    record.LockoutEnd = DateTime.UtcNow.Add(_blockDuration);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}   
