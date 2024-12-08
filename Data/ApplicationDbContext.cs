using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
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

        builder.Entity<Encargado>()
        .HasIndex(e => e.DepartamentoId)
        .IsUnique()
        .HasFilter("\"Activo\" = true");

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
            new IdentityRole
            {
                Name = "Profesor",
                NormalizedName = "PROFESOR"
            },
        };
        builder.Entity<IdentityRole>().HasData(roles);

    }

}