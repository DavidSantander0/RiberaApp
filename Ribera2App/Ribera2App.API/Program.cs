using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ribera2App.API.BackgroundServices;
using Ribera2App.API.Data; 
using System.Text;
using Microsoft.Extensions.FileProviders; // Para servir archivos
var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


// 2. Registrar el ApplicationDbContext como un servicio
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
// --- INICIO DE CONFIGURACIÓN JWT ---

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Valida que la llave de la firma sea la correcta
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration.GetSection("Jwt:Key").Value!)),

            // Por simplicidad, no validamos el "Issuer" ni el "Audience"
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// --- FIN DE CONFIGURACIÓN JWT ---
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHostedService<GeneradorAlicuotasService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// --- Configuración de Swagger para que entienda JWT ---
builder.Services.AddSwaggerGen(options =>
{
    // 1. Definir la seguridad (Bearer token)
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingresa el token JWT así: Bearer [TU_TOKEN]"
    });

    // 2. Hacer que los endpoints requieran el token
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
// --- AÑADIR ESTE BLOQUE PARA CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          // Aquí le dices que confíe en tu app de Angular
                          policy.WithOrigins("http://localhost:4200", "http://localhost:4201")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);


// --- SERVIR ARCHIVOS ESTÁTICOS ---
// Esto permite que el admin vea las imágenes subidas.
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads")),
    RequestPath = "/uploads" // La URL será https://.../uploads/nombre-archivo.jpg
});
// --- FIN DE SERVIR ARCHIVOS ---

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
