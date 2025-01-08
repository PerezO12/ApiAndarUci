using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUci.Contracts.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiUci.Controller
{
    [Route(ApiRoutes.Rol.RutaGenaral)]
    [ApiController]
    public class RolController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return Ok(roles);
        }
    }
}