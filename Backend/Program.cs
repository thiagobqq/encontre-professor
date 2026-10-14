using Backend.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IProfessorRepository>(_ =>
    new JsonProfessorRepository(Path.Combine(AppContext.BaseDirectory, "professores.json")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoint mínimo só para validar que a aplicação sobe.
// Substituir/remover ao implementar POST /api/professores/busca.
app.MapGet("/", () => Results.Ok(new { servico = "desafio-tecnico", status = "ok" }));

app.Run();
