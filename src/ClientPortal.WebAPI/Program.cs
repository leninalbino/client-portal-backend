using ClientPortal.Application.Services;
using ClientPortal.Domain.Interfaces;
using ClientPortal.Infrastructure.Data;
using ClientPortal.Infrastructure.Repositories;
using ClientPortal.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using ClientPortal.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure In-Memory Database for simplicity (for development/testing)
// TODO: Replace with PostgreSQL for production
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("ClientPortalDb"));

// =====================================================
// CONFIGURACION POSTGRESQL (PARA CUANDO QUIERAS USARLA)
// =====================================================
// Cuando quieras cambiar a PostgreSQL, descomenta esta parte
// y comenta la configuración de memoria de arriba

// Agregar soporte para PostgreSQL
/*
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrEmpty(connectionString))
    {
        // Si no hay conexión, usar memoria como respaldo
        options.UseInMemoryDatabase("ClientPortalDb");
        Console.WriteLine("⚠️ Usando base de datos en memoria - no se encontró cadena de conexión");
    }
    else
    {
        // Usar PostgreSQL para producción
        options.UseNpgsql(connectionString);
        Console.WriteLine("✅ Base de datos PostgreSQL conectada correctamente");
    }
});

// Agregar health checks para monitorear la conexión a PostgreSQL
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection") ?? "",
               name: "postgresql",
               failureStatus: HealthStatus.Unhealthy);
*/

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
            DateOfBirth = new DateTime(1990, 5, 15),
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
