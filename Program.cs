using ApiUCI.Contracts.V1;
using ApiUCI.Data;
using ApiUCI.Filters;
using ApiUCI.Interfaces;
using ApiUCI.Middleware;
using ApiUCI.Service;
using ApiUCI.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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
// Add controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Configuración de la base de datos
var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// TODO: CAMBIAR CORS DESPUES
//Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Configuración de Swagger para integrarlo con el JWT
builder.Services.AddSwaggerGen(option =>
{
    //todo: prueba
    //option.SwaggerDoc("v1", new OpenApiInfo { Title = "Api Andar", Version = "v1" });
    option.SwaggerDoc(ApiRoutes.Version, new OpenApiInfo { Title = "Api Andar-Lite", Version = ApiRoutes.Version });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor entre un token válido",
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
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});



// Configuración de Identity con validaciones de seguridad de password de los usuarios
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddErrorDescriber<CustomIdentityErrorDescriber>();

// Configuración del JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]!)
        )
    };
});


// Políticas de seguridad
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOEncargadoPolicy", policy => policy.RequireRole("Admin", "Encargado"));
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("EncargadoPolicy", policy => policy.RequireRole("Encargado"));
    options.AddPolicy("EstudiantePolicy", policy => policy.RequireRole("Estudiante"));
});

// Repositorios
builder.Services.AddScoped<IFacultadRepository, FacultadRepository>();
builder.Services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
builder.Services.AddScoped<ICarreraRepository, CarreraRepository>();
builder.Services.AddScoped<IEstudianteRepository, EstudianteRepository>();
builder.Services.AddScoped<IEncargadoRepository, EncargadoRepository>();
builder.Services.AddScoped<IFormularioRepository, FormularioRepository>();

// Servicios
builder.Services.AddScoped<IEstudianteService, EstudianteService>();
builder.Services.AddScoped<IEncargadoService, EncargadoService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IFormularioService, FormularioService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDepartamentoService, DepartamentoService>();
builder.Services.AddScoped<ICarreraService, CarreraService>();
builder.Services.AddScoped<IFacultadService, FacultadService>();


// Configuración de JsonOptions
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Usa referencias de objetos en lugar de seguir serializando de forma infinita
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    // Para ignorar datos en null
    //options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

//manejo global de modelo no válido
builder.Services.AddControllers(options => 
{
    options.Filters.Add<ValidateModelFilter>();
});

//manejo global d expcetiones
builder.Services.AddControllers(options => 
{
    options.Filters.Add<ExceptionFilter>();
});

//aplicar logging globalmente
builder.Services.AddControllers(option => 
{
    option.Filters.Add<LoggingFilter>();
}); 

// Filtro de estandarizacion de respueastas
builder.Services.AddControllers(options =>
{
    options.Filters.Add<EstandarResponseFilter>();
});

var app = builder.Build();

//Creacion del usuario admin en la primera migration
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var configuration = services.GetRequiredService<IConfiguration>();
    try
    {
        await SeedData.InitializeAsync(services, configuration);
    }
    catch (Exception ex)
    {
        // Manejo de errores (log o salida en consola)
        Console.WriteLine($"Error en la creación del usuario Admin: {ex.Message}");
    }
}



// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware de CORS (antes de la autenticación)
app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

// Middleware de autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Middleware personalizado (asegúrate de colocarlo después de UseAuthentication y antes de MapControllers)
app.UseMiddleware<TokenValidationMiddleware>();

// Mapeo de controladores
app.MapControllers();

app.Run();
