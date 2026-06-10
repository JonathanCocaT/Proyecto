using Microsoft.Data.SqlClient;
using NuitUrbaine.API.Data;

var builder = WebApplication.CreateBuilder(args);

// ── Servicios ──
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// CORS: permite que el HTML se comunique con la API desde cualquier origen local
builder.Services.AddCors(options =>
{
    options.AddPolicy("NuitPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Inyección de la cadena de conexión
var connString = builder.Configuration.GetConnectionString("NuitDB")
    ?? throw new InvalidOperationException("Connection string 'NuitDB' not found.");

builder.Services.AddScoped<NuitRepository>(_ => new NuitRepository(connString));

var app = builder.Build();

app.UseCors("NuitPolicy");
app.UseAuthorization();
app.MapControllers();

// Prueba rápida de conexión al iniciar
try
{
    using var conn = new SqlConnection(connString);
    conn.Open();
    Console.WriteLine("✅  Conexión a SQL Server exitosa — Base de datos: Nuitt");
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️  No se pudo conectar a SQL Server: {ex.Message}");
    Console.WriteLine("   → Verifica que SQL Server esté corriendo y el nombre de BD sea correcto en appsettings.json");
}

app.Run("http://localhost:5050");
