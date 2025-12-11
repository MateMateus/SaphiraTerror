//desenvolvido na fase 1

//using SaphiraTerror.Infrastructure;
//using SaphiraTerror.Infrastructure.Persistence.Seed;

//var builder = WebApplication.CreateBuilder(args);

//// Infra (DbContext + Identity)
//builder.Services.AddInfrastructure(builder.Configuration);

//// Controllers (usaremos na Fase 3)
//builder.Services.AddControllers();

//// CORS aberto (fecharemos na Fase 3)
//builder.Services.AddCors(o => o.AddPolicy("Default", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

//var app = builder.Build();

//app.UseCors("Default");

//// Health na raiz
//app.MapGet("/", () => Results.Ok(new { name = "SaphiraTerror.Api", status = "ok" }));

//app.MapControllers();

//// ---- SEED (migrations + dados iniciais)
//await DatabaseSeeder.EnsureSeededAsync(app.Services);

//app.Run();



// refatorado fase 03                                            // Tag de histórico da fase (não afeta o build).
using SaphiraTerror.Infrastructure;                              // Extensões de DI da camada de Infraestrutura.
using SaphiraTerror.Infrastructure.Persistence.Seed;             // Seeder para migrações e dados iniciais.

var builder = WebApplication.CreateBuilder(args);                // Cria o builder do host Web (config, DI, logging, etc).

// Infra (DbContext + Identity + Repos/Services)
builder.Services.AddInfrastructure(builder.Configuration);       // Registra DbContext, Identity, Repositórios e Services via métodos de extensão.

// Controllers + Swagger
builder.Services.AddControllers();                               // Adiciona suporte a Controllers (API).
builder.Services.AddEndpointsApiExplorer();                      // Explora endpoints para geração de documentação.
builder.Services.AddSwaggerGen();                                // Gera o documento OpenAPI/Swagger (UI ativada no dev).

// CORS por configuração (dev: localhost do Web MVC)
var allowed = builder.Configuration                               // Recupera a lista de origens permitidas do appsettings.
    .GetSection("Cors:AllowedOrigins")                            // Seção: "Cors:AllowedOrigins": [ "http://...", "https://..." ]
    .Get<string[]>()                                              // Mapeia para array de strings.
    ?? new[] { "https://localhost:5001", "http://localhost:5000" }; // Fallback sensato para desenvolvimento (MVC local).

builder.Services.AddCors(o =>                                     // Registra política CORS nomeada.
    o.AddPolicy("Default", p => p.WithOrigins(allowed)             // Restringe às origens configuradas (prevenção de CSRF/abuso).
                                 .AllowAnyHeader()                 // Permite qualquer header (útil em APIs públicas de dev).
                                 .AllowAnyMethod()));              // Permite GET/POST/PUT/DELETE/etc.

// Constrói o app (pipeline HTTP + container DI final).
var app = builder.Build();

app.UseCors("Default");                                           // Aplica a política CORS antes dos endpoints.

// Ativa Swagger somente em Desenvolvimento (evita expor em produção).
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();                                             // Middleware para servir o JSON do OpenAPI.
    app.UseSwaggerUI();                                           // UI do Swagger (interativa).
}

// Health na raiz
app.MapGet("/", () => Results.Ok(new { name = "SaphiraTerror.Api", status = "ok" })); // Endpoint simples de health-check (200 OK).

app.MapControllers();                                             // Mapeia controllers baseados em atributos (rotas da API).

// Migrate + Seed
await DatabaseSeeder.EnsureSeededAsync(app.Services);             // Aplica migrações e faz seed de dados iniciais (idempotente).

app.Run();                                                        // Inicia o servidor (bloqueante até shutdown).
