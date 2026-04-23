using Microsoft.AspNetCore.Identity;
using ProyectoFinalAlvaradoMoraMauricio.Models;

namespace ProyectoFinalAlvaradoMoraMauricio.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Administrador", "Estudiante", "Docente", "Registro" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            string adminEmail = "admin@matricula.com";
            string adminPassword = "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Cedula = "000000000",
                    Nombre = "Administrador",
                    Apellidos = "General",
                    Direccion = "Sistema",
                    Activo = true,
                    FechaIngreso = DateTime.Now,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);

                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrador");
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(adminUser, "Administrador"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrador");
                }
            }
        }
    }
}