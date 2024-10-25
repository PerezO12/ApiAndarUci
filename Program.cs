using Microsoft.EntityFrameworkCore;
using MyApiUCI.Interfaces;
using MyApiUCI.Repository;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//add controladores
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//conf a la basedatos
var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);

builder.Services.AddScoped<IFacultadRepository, FacultadRepository>();
builder.Services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();

var app = builder.Build();

//Cambiar codigo, momentaneo
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        // Verifica si la base de datos es accesible
        if (await dbContext.Database.CanConnectAsync())
        {
            Console.WriteLine("Conexión exitosa a la base de datos.");
        }
        else
        {
            Console.WriteLine("No se pudo conectar a la base de datos.");
        }
    }
    catch (Exception ex)
    {
        // Maneja el error y muestra el mensaje en la consola
        Console.WriteLine($"Error al conectar a la base de datos: {ex.Message}");
    }
}






// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

//mapeo controladores
app.MapControllers();

app.Run();

