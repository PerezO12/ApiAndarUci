using ApiUci.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ApiUci.Models;

namespace ApiUci.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            // Servicios necssarios
            using var scope = serviceProvider.CreateScope();
            var usuarioService = scope.ServiceProvider.GetRequiredService<IUsuarioService>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            
            var adminEmail = configuration["AdminSettings:AdminEmail"];
            var adminUsername = configuration["AdminSettings:AdminUsername"];
            var adminPassword = configuration["AdminSettings:AdminPassword"];
            if(adminPassword == null || adminUsername ==null || adminEmail== null){
                Console.WriteLine("Variables de entorno no configuradas correctamente.");
                return;
            }
            var existingAdmin = await userManager.FindByEmailAsync("admin@admin.com") ?? await userManager.Users.FirstOrDefaultAsync(u => u.UserName != null && u.UserName.ToLower() == adminUsername.ToLower());;
            
            if (existingAdmin != null)
            {
                Console.WriteLine("El usuario administrador ya existe.");
                return;
            }
            // Datos del usuario administrador por defecto
            var admin = new AppUser
            {
                NombreCompleto = "Admin Web Api",
                CarnetIdentidad = "00000000000",
                UserName = adminUsername,
                Email = adminEmail,
            };

            try
            {
                var createUserResult = await userManager.CreateAsync(admin, adminPassword);
                if(!createUserResult.Succeeded){
                    Console.WriteLine($"Error al crear el usuario {createUserResult.Errors.ToArray()}");
                    return;
                }
                var roleResult = await userManager.AddToRoleAsync(admin, "Admin");
                if(!roleResult.Succeeded)
                {
                    Console.WriteLine($"Error al asignar rol al usuario {roleResult.Errors.ToArray()}");
                }
                Console.WriteLine("Admin creado exitosamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error al crear el usuario administrador: {ex.Message}");
            }
        }
    }
}
