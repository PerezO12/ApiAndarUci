using ApiUci.Contracts.V1;
using ApiUci.Data;
using ApiUci.Filters;
using ApiUci.Interfaces;
using ApiUci.Middleware;
using ApiUci.Service;
using ApiUci.Utilities;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ApiUci.Models;
using ApiUci.Repository;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using ApiUCI.Interfaces.Services;
using ApiUCI.Service;
using ApiUCI.Middlewares;
using System.Net;

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
//fluent validation
builder.Services
    .AddFluentValidationAutoValidation()
    .AddValidatorsFromAssemblyContaining<Program>();




// Configuración de Identity con validaciones de seguridad de password de los usuarios
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    //configuracioens de password
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    //configuracion del bloquedo por intentos fachidos
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
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
builder.Services.AddScoped<IIpBlockService, IpBlockService>();

//controller
builder.Services.AddControllers(options =>
{
    //options.ModelValidatorProviders.Clear(); // Deshabilita Data Annotations
    //options.Filters.Add<ValidateModelFilter>(); //filtro de validacion
    options.Filters.Add<ExceptionFilter>();//manejo global d expcetiones
    options.Filters.Add<LoggingFilter>();//aplicar logging globalmente
    options.Filters.Add<EstandarResponseFilter>();//estandarizacion de respuestas
}).AddJsonOptions(opt => {
    // Configuración de JsonOptions
    // Usa referencias de objetos en lugar de seguir serializando de forma infinita
    opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;

    // Para ignorar datos en null
    opt.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

// Configuración del servicio para servir archivos estáticos
builder.Services.AddControllersWithViews();

// Registrar el servicio de limpieza en segundo plano
builder.Services.AddHostedService<CleanUpImageService>();

// Habilitar HTTPS en desarrollo y producción
//todo: revizar
builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 443; // Puerta de enlace predeterminada para HTTPS
});
// Configurar el servidor Kestrel para usar el certificado SSL
//todo: estoy usando un certificado autofirmado, para desarrollo.
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Loopback, 5001, listenOptions => 
    {
        listenOptions.UseHttps(
            Path.Combine(Directory.GetCurrentDirectory(), "certificadosAutoFirmados", "server.pfx"),
            builder.Configuration["Certificado:Password"]
        );
    });
});


//configruacion del limite de solicitudes x minuto
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>

        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "UnknownIP",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 3
            }));
});



var app = builder.Build();

//Creacion del usuario admin en la primera ejecución
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

//redirigir el trafigo http a https
app.UseHttpsRedirection();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Middleware de CORS (antes de la autenticación)
app.UseCors("AllowAllOrigins");

app.UseMiddleware<ClientIpMiddleware>(); //para extraer el ip del cliente desde cualquier lugar
//Middleware para ips bloqueados
app.UseMiddleware<IpBlockMiddleware>();

app.UseHttpsRedirection();
app.UseHsts();//Ver si lo dejo aqui

// Middleware de autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Middleware personalizado (asegúrate de colocarlo después de UseAuthentication y antes de MapControllers)
app.UseMiddleware<TokenValidationMiddleware>();


//limitador de solicitudes por tiempo
app.UseRateLimiter();

// Sirve archivos estáticos desde la carpeta "wwwroot" o cualquier otra carpeta
app.UseStaticFiles(); // Para servir archivos desde wwwroot por defecto
// Si quieres servir archivos desde otra carpeta fuera de wwwroot
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "imagenes")),
    RequestPath = "/images"
});

// Mapeo de controladores
app.MapControllers();

app.Run();

public partial class Program { } // Clase parcial para pruebas.
