using Microsoft.EntityFrameworkCore;
using MyApiUCI.Models;


public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions dbContextOptions)
    : base(dbContextOptions)
    {

    }   

    public DbSet<Facultad> facultad { get; set; } 
    public DbSet<Departamento> departamento { get; set; }   
    public DbSet<Carrera> carrera { get; set; } 
}
