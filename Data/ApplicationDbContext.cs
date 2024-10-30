using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyApiUCI.Models;


public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions dbContextOptions)
    : base(dbContextOptions)
    {

    }   

    public DbSet<Facultad> facultad { get; set; } 
    public DbSet<Departamento> departamento { get; set; }   
    public DbSet<Carrera> carrera { get; set; } 
    public DbSet<Estudiante> estudiante {get; set;}
    

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Definir la restricción de unicidad para EstudianteId y DepartamentoId en la tabla Formulario
        builder.Entity<Formulario>()
            .HasIndex(f => new { f.EstudianteId, f.DepartamentoId })
            .IsUnique()
            .HasDatabaseName("IX_Formulario_Estudiante_Departamento");

        List<IdentityRole> roles = new List<IdentityRole>
        {
            new IdentityRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole
            {
                Name = "Estudiante",
                NormalizedName = "ESTUDIANTE"
            },
            new IdentityRole
            {
                Name = "Encargado",
                NormalizedName = "ENCARGADO"
            },
        };
        builder.Entity<IdentityRole>().HasData(roles);
    }
}
