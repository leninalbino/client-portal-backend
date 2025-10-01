using ClientPortal.Application.Services;
using ClientPortal.Domain.Interfaces;
using ClientPortal.Infrastructure.Data;
using ClientPortal.Infrastructure.Repositories;
using ClientPortal.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using ClientPortal.Domain.Entities;
using HealthChecks.NpgSql;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =====================================================
// CONFIGURACION BASE DE DATOS
// =====================================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // 1. Primero intenta leer de variable de entorno (ej: Render)
    var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

    // 2. Si no existe, intenta de appsettings.json (local)
    if (string.IsNullOrEmpty(connectionString))
    {
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    }

    if (string.IsNullOrEmpty(connectionString))
    {
        // Si no hay nada, usa InMemory como respaldo (dev/testing)
        options.UseInMemoryDatabase("ClientPortalDb");
        Console.WriteLine("⚠️ Usando base de datos en memoria - no se encontró cadena de conexión");
        Console.WriteLine($"   Variable de entorno DATABASE_URL: {(Environment.GetEnvironmentVariable("DATABASE_URL") ?? "NO DEFINIDA")}");
    }
    else
    {
        // Validar formato básico de la cadena de conexión
        if (!connectionString.StartsWith("Host=") && !connectionString.StartsWith("Server=") && !connectionString.Contains("postgresql://"))
        {
            Console.WriteLine($"❌ Error: La cadena de conexión no tiene el formato correcto: {connectionString.Substring(0, Math.Min(50, connectionString.Length))}...");
            Console.WriteLine("🔄 Usando base de datos en memoria como respaldo");
            options.UseInMemoryDatabase("ClientPortalDb");
        }
        else
        {
            try
            {
                // Intenta configurar PostgreSQL
                options.UseNpgsql(connectionString);

                // Intenta crear una conexión de prueba para verificar que funciona
                using var testConnection = new NpgsqlConnection(connectionString);
                testConnection.Open();

                Console.WriteLine("✅ Base de datos PostgreSQL conectada correctamente");
                Console.WriteLine($"   Cadena de conexión: {connectionString.Substring(0, Math.Min(60, connectionString.Length))}...");
            }
            catch (Exception ex)
            {
                // Si falla la conexión, usa InMemory como respaldo
                Console.WriteLine($"❌ Error conectando a PostgreSQL: {ex.Message}");
                Console.WriteLine($"   Cadena de conexión utilizada: {connectionString.Substring(0, Math.Min(60, connectionString.Length))}...");
                Console.WriteLine("🔄 Usando base de datos en memoria como respaldo");

                options.UseInMemoryDatabase("ClientPortalDb");
            }
        }
    }
});

// =====================================================
// HEALTH CHECKS (para monitoreo en prod)
// =====================================================
builder.Services.AddHealthChecks()
    .AddNpgSql(
        Environment.GetEnvironmentVariable("DATABASE_URL") 
        ?? builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "",
        name: "postgresql",
        failureStatus: HealthStatus.Unhealthy
    );

// =====================================================
// FIN CONFIGURACION POSTGRESQL
// =====================================================

// Configure services and repositories
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IClientService, ClientService>();

// Configure CORS to allow requests from React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        var allowedOrigins = new List<string> { "http://localhost:3000" };

        // Add production frontend URL if environment variable is set
        var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL");
        if (!string.IsNullOrEmpty(frontendUrl))
        {
            allowedOrigins.Add(frontendUrl);
        }

        // For Render deployment, also allow the Render domain
        var renderUrl = Environment.GetEnvironmentVariable("RENDER_EXTERNAL_URL");
        if (!string.IsNullOrEmpty(renderUrl))
        {
            allowedOrigins.Add(renderUrl);
        }

        policy.WithOrigins(allowedOrigins.ToArray())
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Initialize database with test data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Ensure database is created
    await context.Database.EnsureCreatedAsync();
    
    // Add test data if no clients exist
    if (!context.Clients.Any())
    {
        var testClient = new ClientPortal.Domain.Entities.Client
        {
            Id = Guid.NewGuid(),
            FirstName = "Juan",
            LastName = "Pérez",
            DateOfBirth = new DateTime(1990, 5, 15, 0, 0, 0, DateTimeKind.Utc),
            DocumentType = DocumentType.DNI,
            DocumentNumber = "12345678",
            CurriculumVitaeFileName = "cv_test.pdf",
            PhotoFileName = "photo_test.jpg",
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        
        context.Clients.Add(testClient);
        await context.SaveChangesAsync();
        
        Console.WriteLine("✅ Todo listo! Base de datos inicializada con datos de prueba");
        Console.WriteLine($"✅ Cliente de prueba creado: {testClient.FirstName} {testClient.LastName}");
    }
    else
    {
        var clientCount = context.Clients.Count();
        Console.WriteLine($"✅ Base de datos conectada - Clientes encontrados: {clientCount}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReact");
app.UseAuthorization();
app.MapControllers();

app.Run();
