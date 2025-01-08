using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ApiUci.Models;


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

    builder.Entity<Departamento>()
        .HasOne(d => d.Encargado)
        .WithOne(e => e.Departamento)
        .HasForeignKey<Departamento>(d => d.EncargadoId)  // 'EncargadoId' es la clave foránea en Departamento
        .OnDelete(DeleteBehavior.SetNull);  // Si se elimina el Encargado, se establece a null en Departamento

    builder.Entity<Encargado>()
        .HasIndex(e => e.DepartamentoId)
        .IsUnique(); 

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