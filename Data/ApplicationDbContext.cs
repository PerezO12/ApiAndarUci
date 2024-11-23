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

    public DbSet<Facultad> Facultad { get; set; } 
    public DbSet<Carrera> Carrera { get; set; } 
    public DbSet<Estudiante> Estudiante { get; set; }
    public DbSet<Encargado> Encargado { get; set; }
    public DbSet<Departamento> Departamento { get; set; }   
    public DbSet<Formulario> Formulario { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Definir la restricción de unicidad para UsuarioId y DepartamentoId en la tabla Formulario
        builder.Entity<Formulario>()
            .HasIndex(f => new { f.EstudianteId, f.DepartamentoId })
            .IsUnique()
            .HasDatabaseName("IX_Formulario_Estudiante_Departamento_Activo");

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