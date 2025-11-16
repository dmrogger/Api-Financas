using ApiFinaças.Src.Application.Interfaces;
using ApiFinaças.Src.Application.Services;
using ApiFinaças.Src.Domain.Interfaces;
using ApiFinaças.Src.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API de Finanças Pessoais",
        Version = "v1",
        Description = "API para gerenciamento de finanças pessoais",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Equipe de Desenvolvimento"
        }
    });

    // Incluir comentários XML no Swagger
    var xmlFile = "ApiFinaças.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Dependency Injection
// Repositories
builder.Services.AddScoped<IMovimentacaoRepository, MovimentacaoRepository>();

// Services
builder.Services.AddScoped<IMovimentacaoService, MovimentacaoService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Finanças Pessoais v1");
        c.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
