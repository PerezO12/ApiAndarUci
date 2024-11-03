using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyApiUCI.Interfaces;
using MyApiUCI.Models;
using MyApiUCI.Repository;
using MyApiUCI.Service;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//add controladores
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//esot es temporal cambiar y arreglar despues 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5173",
        policy => policy.WithOrigins("http://localhost:5173")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

//******************************

//Configuracion del swagger para integrarlo con el JWT
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
//aqui termina*************************



//conf a la basedatos
var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);

//Configuracion de identity, con validaciones de seguridad de  password d los usuarios
builder.Services.AddIdentity<AppUser, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 7;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

//Schemnas del JWT con Bear
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer =  builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
        )
    };
});

//Repositorios
builder.Services.AddScoped<IFacultadRepository, FacultadRepository>();
builder.Services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
builder.Services.AddScoped<ICarreraRepository, CarreraRepository>();
builder.Services.AddScoped<IEstudianteRepository, EstudianteRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
//Servicios
builder.Services.AddScoped<IEstudianteService, EstudianteService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAcountService, AcountService>();

var app = builder.Build();

//Cambiar codigo, momentaneo
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
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
        Console.WriteLine($"Error al conectar a la base de datos: {ex.Message}");
    }
}



// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

//Cambiar luego
app.UseCors("AllowLocalhost5173");


app.UseHttpsRedirection();

//mapeo controladores
app.MapControllers();

app.Run();

